using EventBus;
using System.Collections.Generic;
using System.Linq;
using Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Player
{
    public class ObjectDisplay : MonoBehaviour
    {
        #region Fields
        #region Parameters
        [SerializeField] Image iconPrefab;
        [SerializeField] float minTextScale, maxTextScale;
        #endregion
        #region Implementation
        public static ObjectDisplay Instance { get; private set; }
        protected readonly struct ObjectUIData
        {
            public readonly RectTransform RectTransform;
            public readonly Image BoundingBox;
            public readonly Unit Object;
            public readonly TextMeshProUGUI DistanceText;
            public readonly GameObject GameObject { get => RectTransform.gameObject; }
            public ObjectUIData(Image image, Unit @object, TextMeshProUGUI text)
            {
                RectTransform = image.rectTransform;
                BoundingBox = image;
                Object = @object;
                DistanceText = text;
            }
        }
        [field: SerializeField] public Unit CurrentTarget { get; protected set; }
        protected Camera cam;
        [SerializeField] protected RectTransform reticleParent;
        protected readonly Dictionary<Unit, ObjectUIData> toTrack = new();
        protected readonly Stack<(Image, TextMeshProUGUI)> iconPool = new();
        protected readonly CountdownTimer reorderTimer = new(GlobalSettings.UIUpdateCooldown);
        #endregion
        #endregion
        #region Setup
        protected void Awake()
        {
            Instance = this;
            cam = GetComponentInChildren<Camera>();
            EventBus<SpecialObjectAdded>.AddActions(AddIdentified);
            GameManager.Teams[0].OnMemberAdded += AddIdentified;
            GameManager.Teams[0].OnMemberRemoved += RemoveObject;
            GameManager.Teams[0].OnIdentifiedTargetAdded += AddIdentified;
            GameManager.Teams[0].OnTrackedTargetAdded += AddTracked;
            GameManager.Teams[0].OnTargetRemoved += RemoveObject;
        }
        private void OnEnable()
        {
            reorderTimer.OnTimerStop += () =>
            {
                ReorderBoundingBoxes();
                reorderTimer.Start();
            };
            reorderTimer.Start();
        }
        private void OnDisable()
        {
            EventBus<SpecialObjectAdded>.RemoveActions(AddIdentified);
            reorderTimer.OnTimerStop = null;
            reorderTimer.Stop();
        }
        private void OnDestroy()
        {
            reorderTimer.Dispose();
        }
        #endregion
        #region Object management
        /// <summary>
        /// Does nothing for the player object.
        /// Add an object to be tracked and displayed on the UI. Disables identified renderer and 
        /// enables tracked renderer.
        /// Disables button and raycast target.
        /// </summary>
        /// <param name="obj">The object to keep track of.</param>
        public void AddTracked(Unit obj)
        {
            if (obj == GameManager.Player) return;
            obj.IdentifiedRenderer.enabled = false;
            obj.TrackedRenderer.enabled = true;
            if (obj.Icon == null) return;
            if (!toTrack.TryGetValue(obj, out var data))
            {
                obj.OnDespawn += RemoveObject;
                Image img; TextMeshProUGUI txt;
                (img, txt) = GetIcon(obj.Icon);
                img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.Transform.position);
                img.raycastTarget = false;
                data = new(img, obj, txt);
                toTrack.Add(obj, data);
            }
            data.BoundingBox.material = obj.TrackedRenderer.material;
        }
        public void AddIdentified(SpecialObjectAdded objectAdded)
        {
            AddIdentified(objectAdded.SpecialObject);
        }
        /// <summary>
        /// Does nothing for the player object.
        /// Add an object to be tracked and displayed on the UI. Enables identified renderer and disables tracked renderer.
        /// Disables button and raycast target if the object is not selectable.
        /// </summary>
        /// <param name="obj"></param>
        public void AddIdentified(Unit obj)
        {
            if (obj.ID == ObjectType.Player) return;
            obj.IdentifiedRenderer.enabled = true;
            obj.TrackedRenderer.enabled = false;
            if (obj.Icon == null) return;
            if (!toTrack.TryGetValue(obj, out var data))
            {
                Image img; TextMeshProUGUI txt;
                (img, txt) = GetIcon(obj.Icon);
                obj.OnDespawn += RemoveObject;
                img.raycastTarget = obj.Selectable;
                img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.Transform.position);
                data = new(img, obj, txt);
                toTrack.Add(obj, data);
            }
            data.BoundingBox.material = obj.Material;
        }
        /// <summary>
        /// Does nothing for the player object.
        /// Removes an object from the UI. Disables both identified and tracked renderers. Pushes the 
        /// image and button to the pool.
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObject(Unit obj)
        {
            if (obj.ID == ObjectType.Player) return;
            obj.OnDespawn -= RemoveObject;
            obj.IdentifiedRenderer.enabled = obj.TrackedRenderer.enabled = false;
            if (CurrentTarget == obj) ClearTarget();
            if (toTrack.TryGetValue(obj, out var data))
            {
                iconPool.Push((data.BoundingBox, data.DistanceText));
                data.RectTransform.gameObject.SetActive(false);
                toTrack.Remove(obj);
            }
        }
        protected (Image, TextMeshProUGUI) GetIcon(Sprite sprite)
        {
            Image icon;
            TextMeshProUGUI txt;
            if (iconPool.Count == 0)
            {
                icon = Instantiate(iconPrefab, reticleParent.transform);
                txt = icon.GetComponentInChildren<TextMeshProUGUI>();
                icon.sprite = sprite;
                icon.fillCenter = false;
            }
            else
            {
                (icon, txt) = iconPool.Pop();
                icon.sprite = sprite;
                icon.gameObject.SetActive(true);
            }
            txt.text = "";
            return (icon, txt);
        }
        #endregion
        #region UI
        private void LateUpdate()
        {
            HandleDisplay();
        }
        void HandleDisplay()
        {
            foreach (var a in toTrack)
            {
                if (a.Key == null) continue;

                Vector3 worldPos = a.Value.Object.Transform.position;
                Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

                Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);
                if (viewportPos.z <= 0)
                {
                    a.Value.GameObject.SetActive(false);
                    continue;
                }
                a.Value.GameObject.SetActive(true);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(reticleParent, screenPos, null, out var localPos);
                a.Value.RectTransform.localPosition = localPos;

                //scale based on distance
                float dist = Vector3.Distance(cam.transform.position, worldPos);
                var size = Mathf.Clamp(a.Value.Object.IconSizeCoefficient / (dist + 0.00001f),
                    minTextScale, maxTextScale);
                a.Value.RectTransform.sizeDelta = new Vector2(20, 20) * size;
                dist = Vector3.Distance(cam.transform.root.position, worldPos);
                a.Value.DistanceText.text = dist.ToString("0.0");
            }
        }
        void ReorderBoundingBoxes()
        {
            var list = toTrack.Values.ToList();
            list.Sort((a, b) =>
            {
                float distA = Vector3.Distance(Camera.main.transform.position, a.Object.transform.position);
                float distB = Vector3.Distance(Camera.main.transform.position, b.Object.transform.position);
                return distB.CompareTo(distA);
            });
            for (int i = 0; i < list.Count; i++)
            {
                list[i].RectTransform.SetSiblingIndex(i);
            }
        }
        #endregion
        void TargetSelected(Unit obj)
        {
            CurrentTarget = obj;
        }
        public void ClearTarget()
        {
            CurrentTarget = null;
        }
    }
}