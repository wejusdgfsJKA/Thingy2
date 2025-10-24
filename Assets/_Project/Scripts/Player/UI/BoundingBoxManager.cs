using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoundingBoxManager : MonoBehaviour
{
    [SerializeField] Image imagePrefab;
    [SerializeField] Camera cam;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Canvas canvas;
    readonly Dictionary<Transform, (RectTransform, Image)> toTrack = new();
    readonly Stack<Image> imagePool = new();
    public static BoundingBoxManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    public bool Register(Transform obj)
    {
        return Register(obj, defaultSprite);
    }
    public bool Register(Transform obj, Sprite sprite)
    {
        if (obj == null || sprite == null)
        {
            Debug.LogError("Cannot register empty object!");
            return false;
        }
        if (!toTrack.ContainsKey(obj))
        {
            var img = GetSprite(sprite);
            img.rectTransform.anchoredPosition = cam.ScreenToWorldPoint(obj.position);
            toTrack.Add(obj, (img.rectTransform, img));
            return true;
        }
        return false;
    }
    public bool DeRegister(Transform obj)
    {
        if (toTrack.TryGetValue(obj, out var img))
        {
            img.Item1.gameObject.SetActive(false);
            imagePool.Push(img.Item2);
            toTrack.Remove(obj);
            return true;
        }
        return false;
    }
    protected Image GetSprite(Sprite sprite)
    {
        Image img;
        if (imagePool.Count == 0)
        {
            img = Instantiate(imagePrefab, canvas.transform);
        }
        else
        {
            img = imagePool.Pop();
            img.gameObject.SetActive(true);
        }
        img.rectTransform.anchorMax = img.rectTransform.anchorMin = Vector2.zero;
        img.sprite = sprite;
        img.fillCenter = false;
        return img;
    }
    private void Update()
    {
        foreach (var a in toTrack)
        {
            a.Value.Item1.anchoredPosition = cam.WorldToScreenPoint(a.Key.position);
        }
    }
}
