using EventBus;
using Global;
using System.Collections.Generic;
using System.Linq;
using Timers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Player.UI
{
    [RequireComponent(typeof(PlayerShip))]
    public class ObjectDisplay : MonoBehaviour
    {
        public static Material HullDamageMaterial { get; private set; }
        public static Material ShieldDamageMaterial { get; private set; }
        #region Fields
        #region Parameters
        [SerializeField] Image iconPrefab;
        [SerializeField] float minTextScale, maxTextScale;
        [Tooltip("IconIdentifiedMaterial to use for the selected target.")]
        [SerializeField] Material selectedMaterial, hullDamageMaterial, shieldDamageMaterial;
        #endregion
        #region Implementation
        protected readonly struct ObjectUIData
        {
            public readonly RectTransform RectTransform;
            public Image BoundingBox { get; }
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
        protected Camera cam;
        [Tooltip("The object that will be the parent of all bounding boxes.")]
        [SerializeField] protected RectTransform boundingBoxParent;
        /// <summary>
        /// All objects being tracked and their corresponding UI data.
        /// </summary>
        protected Dictionary<Unit, ObjectUIData> toTrack { get; } = new();
        /// <summary>
        /// The counterpart dictionary to quickly find UI data based on the image GameObject.
        /// </summary>
        protected Dictionary<GameObject, ObjectUIData> images { get; } = new();
        protected readonly Stack<(Image, TextMeshProUGUI)> iconPool = new();
        /// <summary>
        /// This timer determines how often the bounding boxes are reordered.
        /// </summary>
        protected readonly CountdownTimer reorderTimer = new(GlobalSettings.UIUpdateCooldown);
        protected GraphicRaycaster raycaster;
        protected EventSystem eventSystem;
        public Unit CurrentTarget { get; protected set; }
        #endregion
        #endregion
        #region Setup
        protected void Awake()
        {
            HullDamageMaterial = hullDamageMaterial;
            ShieldDamageMaterial = shieldDamageMaterial;
            cam = GetComponentInChildren<Camera>();
            EventBus<SpecialObjectAdded>.AddActions(AddIdentified);
            GameManager.Teams[0].OnMemberAdded += AddIdentified;
            GameManager.Teams[0].OnMemberRemoved += RemoveObject;
            raycaster = GetComponentInChildren<GraphicRaycaster>();
        }
        private void OnEnable()
        {
            eventSystem ??= EventSystem.current;
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
        /// Disables raycast target.
        /// </summary>
        /// <param name="obj">The object to keep track of.</param>
        public void AddTracked(Unit obj)
        {
            if (obj == GameManager.Player) return;
            obj.EnableTrackedRenderer();
            if (obj.Icon == null) return;
            if (!toTrack.TryGetValue(obj, out var data))
            {
                obj.OnDespawn += RemoveObject;
                Image img; TextMeshProUGUI txt;
                (img, txt) = GetIcon(obj.Icon);
                img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.Transform.position);
                data = new(img, obj, txt);
                toTrack.Add(obj, data);
                images.Add(img.gameObject, data);
            }
            data.BoundingBox.raycastTarget = false;
            if (CurrentTarget == obj) ClearTarget();
            data.BoundingBox.material = obj.IconTrackedMaterial;
        }
        public void AddIdentified(SpecialObjectAdded objectAdded) => AddIdentified(objectAdded.SpecialObject);
        /// <summary>
        /// Does nothing for the player object.
        /// Add an object to be tracked and displayed on the UI. Enables identified renderer and disables tracked renderer.
        /// Disables raycast target if the object is not selectable.
        /// </summary>
        /// <param name="obj"></param>
        public void AddIdentified(Unit obj)
        {
            if (obj.ID == ObjectType.Player) return;
            obj.EnableIdentifiedRenderer();
            if (obj.Icon == null) return;
            if (!toTrack.TryGetValue(obj, out var data))
            {
                Image img; TextMeshProUGUI txt;
                (img, txt) = GetIcon(obj.Icon);
                obj.OnDespawn += RemoveObject;

                img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.Transform.position);
                data = new(img, obj, txt);
                toTrack.Add(obj, data);
                images.Add(img.gameObject, data);
            }
            data.BoundingBox.raycastTarget = obj.Selectable;
            if (obj != CurrentTarget) data.BoundingBox.material = obj.IconIdentifiedMaterial;
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
            if (CurrentTarget == obj) ClearTarget();
            if (toTrack.TryGetValue(obj, out var data))
            {
                try
                {
                    iconPool.Push((data.BoundingBox, data.DistanceText));
                    data.RectTransform.gameObject.SetActive(false);
                    toTrack.Remove(obj);
                    images.Remove(data.BoundingBox.gameObject);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error removing object {obj.ID} from ObjectDisplay: {e.Message}");
                }
            }
        }
        /// <summary>
        /// Get an icon prefab from the pool and set the sprite.
        /// </summary>
        /// <param name="sprite">What sprite should the new icon have.</param>
        /// <returns>An instance of the icon prefab with empty text and corresponding sprite.</returns>
        protected (Image, TextMeshProUGUI) GetIcon(Sprite sprite)
        {
            Image icon;
            TextMeshProUGUI txt;
            if (iconPool.Count == 0)
            {
                icon = Instantiate(iconPrefab, boundingBoxParent.transform);
                txt = icon.GetComponentInChildren<TextMeshProUGUI>();
                icon.sprite = sprite;
                //icon.fillCenter = false;
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
        private void Update()
        {
            HandleDisplay();
        }
        /// <summary>
        /// Repositions and rescales all tracked object icons based on their world position.
        /// </summary>
        void HandleDisplay()
        {
            foreach (var a in toTrack)
            {
                if (a.Key == null) continue;

                Vector3 worldPos = a.Value.Object.Transform.position;

                Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);
                if (viewportPos.z <= 0)
                {
                    a.Value.GameObject.SetActive(false);
                    continue;
                }

                Vector2 screenPos = new Vector2(
                    viewportPos.x * Screen.width,
                    viewportPos.y * Screen.height
                );

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    boundingBoxParent,
                    screenPos,
                    cam,
                    out Vector2 localPos
                );

                a.Value.RectTransform.localPosition = localPos;

                #region Scaling
                //scale based on distance
                float dist = Vector3.Distance(cam.transform.position, worldPos);
                var size = Mathf.Clamp(a.Value.Object.IconSizeCoefficient / (dist + 0.00001f),
                    minTextScale, maxTextScale);
                a.Value.RectTransform.sizeDelta = new Vector2(20, 20) * size;
                dist = Vector3.Distance(cam.transform.root.position, worldPos);

                #endregion

                if (a.Key.PlayerIsTarget)
                {
                    a.Value.DistanceText.text = $"<color=red>{dist:0.0}";
                }
                else a.Value.DistanceText.text = $"<color=blue>{dist:0.0}";

                a.Value.GameObject.SetActive(true);
            }
        }
        /// <summary>
        /// Reorders all tracked objects in the UI hierarchy based on their distance to the camera.
        /// </summary>
        void ReorderBoundingBoxes()
        {
            var list = toTrack.Values.ToList();
            list.Sort((a, b) =>
            {
                float distA = Vector3.Distance(cam.transform.position, a.Object.transform.position);
                float distB = Vector3.Distance(cam.transform.position, b.Object.transform.position);
                return distB.CompareTo(distA);
            });
            for (int i = 0; i < list.Count; i++)
            {
                list[i].RectTransform.SetSiblingIndex(i);
            }
        }
        #endregion
        #region Target selection
        /// <summary>
        /// Cast a ray to find an image in front of the crosshair. If found, select the corresponding object 
        /// as the current target.
        /// </summary>
        public void SelectTarget()
        {
            var pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);
            if (results.Count == 0)
            {
                ClearTarget();
                return;
            }
            foreach (RaycastResult result in results)
            {
                if (images.TryGetValue(result.gameObject, out var data))
                {
                    TargetSelected(data);
                    break;
                }
            }
        }
        /// <summary>
        /// Sets the current target and changes its bounding box material. Resets the material of the previous target.
        /// </summary>
        /// <param name="data">The data of the target object.</param>
        void TargetSelected(ObjectUIData data)
        {
            if (GameManager.IsPaused) return;
            if (CurrentTarget != null && toTrack.TryGetValue(CurrentTarget, out var previousData))
            {
                if (previousData.Object.Identified) previousData.BoundingBox.material = previousData.Object.IconIdentifiedMaterial;
            }
            CurrentTarget = data.Object;
            data.BoundingBox.material = selectedMaterial;
        }
        /// <summary>
        /// Clears current target and resets its bounding box material.
        /// </summary>
        public void ClearTarget()
        {
            if (CurrentTarget == null) return;
            if (toTrack.TryGetValue(CurrentTarget, out var data))
            {
                if (data.Object.Identified) data.BoundingBox.material = data.Object.IconIdentifiedMaterial;
            }
            CurrentTarget = null;
        }
        #endregion
    }
}