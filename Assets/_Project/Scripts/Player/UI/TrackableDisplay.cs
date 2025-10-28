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
    [SerializeField] LayerMask trackableMask = 1 << 6;
    #endregion 
    protected Camera cam;
    protected Canvas canvas;
    protected readonly Dictionary<Transform, TrackableData> toTrack = new();
    protected readonly Stack<TextMeshProUGUI> textPool = new();
    protected static Collider[] nonAllocBuffer = new Collider[30];
    #endregion
    #region Setup
    protected void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        canvas = cam.transform.GetComponentInChildren<Canvas>();
    }
    private void OnEnable()
    {
        ComponentManager<Trackable>.OnDeRegister += RemoveTrackable;
    }
    private void OnDisable()
    {
        ComponentManager<Trackable>.OnDeRegister -= RemoveTrackable;
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
    private void OnTriggerEnter(Collider other)
    {
        var t = ComponentManager<Trackable>.Get(other.transform);
        if (t) AddTrackable(t);
    }
    private void OnTriggerExit(Collider other)
    {
        var t = ComponentManager<Trackable>.Get(other.transform);
        if (t) RemoveTrackable(t);
    }
    protected void AddTrackable(Trackable obj)
    {
        if (!toTrack.ContainsKey(obj.transform))
        {
            obj.MeshRenderer.enabled = true;
            obj.UpdateString += UpdateString;
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
            obj.MeshRenderer.enabled = false;
            obj.UpdateString -= UpdateString;
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
        text.rectTransform.anchorMax = text.rectTransform.anchorMin = Vector2.zero;
        return text;
    }
    #endregion
    #region UI
    private void Update()
    {
        CheckForTrackables();
        HandleBoundingBoxes();
    }
    void CheckForTrackables()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.root.position, trackingRange, nonAllocBuffer, trackableMask);
        for (int i = 0; i < count; i++)
        {
            var t = ComponentManager<Trackable>.Get(nonAllocBuffer[i].transform);
            if (t) AddTrackable(t);
        }
    }
    void HandleBoundingBoxes()
    {
        Stack<Trackable> toRemove = new();
        foreach (var a in toTrack)
        {
            if (a.Key == null) continue;

            if (Vector3.Distance(transform.root.position, a.Key.position) > trackingRange)
            {
                toRemove.Push(a.Value.Trackable);
                continue;
            }
            var dist = Vector3.Distance(cam.transform.position, a.Key.position);
            a.Value.RectTransform.anchoredPosition = cam.WorldToScreenPoint(a.Key.position);
            a.Value.RectTransform.localScale = (a.Value.Trackable.TextSizeCoefficient /
                (dist + 0.00001f)) * Vector3.one;
        }
        while (toRemove.TryPop(out var tr))
        {
            RemoveTrackable(tr);
        }
    }
    #endregion
}
