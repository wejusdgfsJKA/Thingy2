using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TrackableDisplay : MonoBehaviour
{
    #region Fields
    protected struct TrackableData
    {
        public RectTransform RectTransform;
        public TextMeshProUGUI Text;
        public Trackable Trackable;
        public TrackableData(TextMeshProUGUI text, Trackable trackable)
        {
            RectTransform = text.rectTransform;
            Text = text;
            Trackable = trackable;
        }
    }
    #region Parameters
    [SerializeField] TextMeshProUGUI textPrefab;
    [SerializeField] float trackingRange = 10;
    #endregion 
    protected Camera cam;
    protected Canvas canvas;
    protected SphereCollider triggerCollider;
    protected readonly Dictionary<Transform, TrackableData> toTrack = new();
    protected readonly Stack<TextMeshProUGUI> textPool = new();
    #endregion
    #region Setup
    protected void Awake()
    {
        triggerCollider = GetComponentInChildren<SphereCollider>();
        triggerCollider.radius = trackingRange;
        triggerCollider.isTrigger = true;
        cam = GetComponentInChildren<Camera>();
        canvas = cam.transform.GetComponentInChildren<Canvas>();
    }
    #endregion
    #region Misc
    protected void UpdateString(Trackable obj)
    {
        if (toTrack.TryGetValue(obj.transform, out var value))
        {
            value.Text.text = obj.ToString();
        }
    }
    #endregion
    #region Collection management
    protected void AddTrackable(Trackable obj)
    {
        if (!toTrack.ContainsKey(obj.transform))
        {
            //obj.MeshRenderer.enabled = true;
            //obj.UpdateString += UpdateString;
            var text = GetTextObject();
            text.text = obj.ToString();
            text.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.transform.position);
            toTrack.Add(obj.transform, new(text, obj));
        }
    }
    protected void RemoveTrackable(Trackable obj)
    {
        if (toTrack.TryGetValue(obj.transform, out var data))
        {
            //obj.MeshRenderer.enabled = false;
            //obj.UpdateString -= UpdateString;
            textPool.Push(data.Text);
            data.RectTransform.gameObject.SetActive(false);
            toTrack.Remove(obj.transform);
        }
    }
    protected TextMeshProUGUI GetTextObject()
    {
        TextMeshProUGUI text;
        if (textPool.Count == 0)
        {
            text = Instantiate(textPrefab, canvas.transform);
        }
        else
        {
            text = textPool.Pop();
            text.gameObject.SetActive(true);
        }
        return text;
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

            // If behind the camera, hide the label
            if (viewportPos.z <= 0)
            {
                a.Value.Text.gameObject.SetActive(false);
                return;
            }

            // Set active
            a.Value.Text.gameObject.SetActive(true);

            // Map viewport (0..1) to canvas local position
            RectTransform canvasRect = a.Value.RectTransform.parent as RectTransform;
            Vector2 canvasPos = new Vector2(
                (viewportPos.x - 0.5f) * canvasRect.sizeDelta.x,
                (viewportPos.y - 0.5f) * canvasRect.sizeDelta.y
            );

            // Assign the position
            a.Value.RectTransform.anchoredPosition = canvasPos;

            // Optional: scale based on distance
            float dist = Vector3.Distance(cam.transform.position, worldPos);
            //var textSize = Mathf.Clamp(a.Value.Trackable.TextSizeCoefficient / (dist + 0.00001f),
            //    minTextScale, maxTextScale);
            //a.Value.RectTransform.sizeDelta = new Vector2(20, 20) * textSize;
            //a.Value.Text.fontSize = textSize * 5;
        }
    }
    #endregion
}
