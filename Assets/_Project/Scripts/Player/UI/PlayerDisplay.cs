using System.Collections.Generic;
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
            public readonly Image Image;
            public readonly Button Button;
            public readonly Object Object;
            public readonly GameObject GameObject { get => RectTransform.gameObject; }
            public ObjectUIData(Image image, Object @object, Button button)
            {
                RectTransform = image.rectTransform;
                Image = image;
                Object = @object;
                Button = button;
            }
        }
        [field: SerializeField] public Object CurrentTarget { get; protected set; }
        protected Camera cam;
        protected Canvas canvas;
        protected readonly Dictionary<Object, ObjectUIData> toTrack = new();
        protected readonly Stack<(Image, Button)> iconPool = new();
        #endregion
        #endregion
        #region Setup
        protected void Awake()
        {
            Instance = this;
            cam = GetComponentInChildren<Camera>();
            canvas = cam.transform.GetComponentInChildren<Canvas>();
            ObjectManager.Instance.PlayerTeam.OnMemberAdded += AddIdentified;
            ObjectManager.Instance.PlayerTeam.OnMemberRemoved += RemoveObject;
            ObjectManager.Instance.PlayerTeam.OnIdentifiedTargetAdded += AddIdentified;
            ObjectManager.Instance.PlayerTeam.OnTrackedTargetAdded += AddTracked;
            ObjectManager.Instance.PlayerTeam.OnTargetRemoved += RemoveObject;
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
        public void AddTracked(Object obj)
        {
            if (obj == GameManager.Player) return;
            obj.IdentifiedRenderer.enabled = false;
            obj.TrackedRenderer.enabled = true;
            if (obj.Icon == null) return;
            if (!toTrack.TryGetValue(obj, out var data))
            {
                Image img; Button btn;
                (img, btn) = GetIcon(obj.Icon);
                if (obj.Selectable) btn.onClick.AddListener(() => TargetSelected(obj));
                img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.Transform.position);
                data = new(img, obj, btn);
                toTrack.Add(obj, data);
            }
            data.Image.material = obj.TrackedRenderer.material;
            data.Button.enabled = data.Image.raycastTarget = false;
        }
        /// <summary>
        /// Does nothing for the player object.
        /// Add an object to be tracked and displayed on the UI. Enables identified renderer and disables tracked renderer.
        /// Disables button and raycast target if the object is not selectable.
        /// </summary>
        /// <param name="obj"></param>
        public void AddIdentified(Object obj)
        {
            if (obj.ID == ObjectType.Player) return;
            obj.IdentifiedRenderer.enabled = true;
            obj.TrackedRenderer.enabled = false;
            if (obj.Icon == null) return;
            if (!toTrack.TryGetValue(obj, out var data))
            {
                Image img; Button btn;
                (img, btn) = GetIcon(obj.Icon);
                if (obj.Selectable) btn.onClick.AddListener(() => TargetSelected(obj));
                img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.Transform.position);
                data = new(img, obj, btn);
                toTrack.Add(obj, data);
            }
            data.Image.material = obj.Material;
            data.Button.enabled = data.Image.raycastTarget = obj.Selectable;
        }
        /// <summary>
        /// Does nothing for the player object.
        /// Removes an object from the UI. Disables both identified and tracked renderers. Pushes the 
        /// image and button to the pool.
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObject(Object obj)
        {
            if (obj.ID == ObjectType.Player) return;
            obj.IdentifiedRenderer.enabled = obj.TrackedRenderer.enabled = false;
            if (CurrentTarget == obj) ClearTarget();
            if (toTrack.TryGetValue(obj, out var data))
            {
                data.Button.onClick.RemoveAllListeners();
                iconPool.Push((data.Image, data.Button));
                data.RectTransform.gameObject.SetActive(false);
                toTrack.Remove(obj);
            }
        }
        protected (Image, Button) GetIcon(Sprite sprite)
        {
            Image icon; Button btn;
            if (iconPool.Count == 0)
            {
                icon = Instantiate(iconPrefab, canvas.transform);
                btn = icon.GetComponent<Button>();
                icon.sprite = sprite;
                icon.fillCenter = false;
            }
            else
            {
                (icon, btn) = iconPool.Pop();
                icon.sprite = sprite;
                icon.gameObject.SetActive(true);
            }
            return (icon, btn);
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

                Vector3 worldPos = a.Key.Transform.position;

                // Convert world position to viewport coordinates (0..1)
                Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);

                // If behind the camera, hide the icon
                if (viewportPos.z <= 0)
                {
                    a.Value.GameObject.SetActive(false);
                    continue;
                }

                a.Value.GameObject.SetActive(true);

                // Map viewport (0..1) to canvas local position
                RectTransform canvasRect = a.Value.RectTransform.parent as RectTransform;
                Vector2 canvasPos = new Vector2(
                    (viewportPos.x - 0.5f) * canvasRect.sizeDelta.x,
                    (viewportPos.y - 0.5f) * canvasRect.sizeDelta.y
                );

                a.Value.RectTransform.anchoredPosition = canvasPos;

                //scale based on distance
                float dist = Vector3.Distance(cam.transform.position, worldPos);
                var size = Mathf.Clamp(a.Value.Object.IconSizeCoefficient / (dist + 0.00001f),
                    minTextScale, maxTextScale);
                a.Value.RectTransform.sizeDelta = new Vector2(20, 20) * size;
            }
        }
        #endregion
        void TargetSelected(Object obj)
        {
            CurrentTarget = obj;
        }
        public void ClearTarget()
        {
            CurrentTarget = null;
        }
    }
}