using UnityEngine;

public class Trackable : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    private void OnEnable()
    {
        BoundingBoxManager.Instance?.Register(transform, sprite);
    }
    private void OnDisable()
    {
        BoundingBoxManager.Instance?.DeRegister(transform);
    }
}
