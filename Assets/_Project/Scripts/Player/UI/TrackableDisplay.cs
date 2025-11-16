using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TrackableDisplay : MonoBehaviour
{
    #region Fields
    public static TrackableDisplay Instance { get; private set; }
    protected readonly struct TrackableData
    {
        public readonly RectTransform RectTransform;
        public readonly Image Image;
        public readonly Button Button;
        public readonly Trackable Trackable;
        public readonly Vector3 ScreenPosition
        {
            get => RectTransform.position;
            set => RectTransform.position = value;
        }
        public readonly GameObject GameObject { get => RectTransform.gameObject; }
        public TrackableData(Image image, Trackable trackable, Button button)
        {
            RectTransform = image.rectTransform;
            Image = image;
            Trackable = trackable;
            Button = button;
        }
    }
    #region Parameters
    [SerializeField] Image iconPrefab;
    [SerializeField] float minTextScale, maxTextScale;
    #endregion
    #region Implementation
    [field: SerializeField] public Trackable CurrentTarget { get; protected set; }
    protected Camera cam;
    protected Canvas canvas;
    protected readonly Dictionary<Transform, TrackableData> toTrack = new();
    protected readonly Stack<(Image, Button)> iconPool = new();
    #endregion
    #endregion
    #region Setup
    protected void Awake()
    {
        Instance = this;
        cam = GetComponentInChildren<Camera>();
        canvas = cam.transform.GetComponentInChildren<Canvas>();
    }
    #endregion
    #region Collection management
    public void AddTrackable(Trackable obj)
    {
        if (obj.Icon == null) return;
        if (!toTrack.ContainsKey(obj.transform))
        {
            Image img; Button btn;
            (img, btn) = GetIcon(obj.Icon);
            btn.onClick.AddListener(() => CurrentTarget = obj);
            img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.transform.position);
            toTrack.Add(obj.transform, new(img, obj, btn));
        }
    }
    public void RemoveTrackable(Trackable obj)
    {
        if (toTrack.TryGetValue(obj.transform, out var data))
        {
            data.Button.onClick.RemoveAllListeners();
            iconPool.Push((data.Image, data.Button));
            data.RectTransform.gameObject.SetActive(false);
            toTrack.Remove(obj.transform);
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

            Vector3 worldPos = a.Key.position;

            // Convert world position to viewport coordinates (0..1)
            Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);

            // If behind the camera, hide the icon
            if (viewportPos.z <= 0)
            {
                a.Value.Image.gameObject.SetActive(false);
                continue;
            }

            a.Value.Image.gameObject.SetActive(true);

            // Map viewport (0..1) to canvas local position
            RectTransform canvasRect = a.Value.RectTransform.parent as RectTransform;
            Vector2 canvasPos = new Vector2(
                (viewportPos.x - 0.5f) * canvasRect.sizeDelta.x,
                (viewportPos.y - 0.5f) * canvasRect.sizeDelta.y
            );

            a.Value.RectTransform.anchoredPosition = canvasPos;

            //scale based on distance
            float dist = Vector3.Distance(cam.transform.position, worldPos);
            var size = Mathf.Clamp(a.Value.Trackable.IconSizeCoefficient / (dist + 0.00001f),
                minTextScale, maxTextScale);
            a.Value.RectTransform.sizeDelta = new Vector2(20, 20) * size;
        }
    }
    public void ClearTarget() => CurrentTarget = null;
    #endregion
}
