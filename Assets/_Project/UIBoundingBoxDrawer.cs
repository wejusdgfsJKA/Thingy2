using UnityEngine;

public class UIBoundingBoxDrawer : MonoBehaviour
{
    public Trackable target;
    public RectTransform image;
    public Camera cam;
    void Update()
    {
        image.position = cam.WorldToScreenPoint(target.transform.position);
        image.localScale = Vector3.one * target.boxSize / Vector3.Distance(target.transform.position, transform.position);
    }
}
