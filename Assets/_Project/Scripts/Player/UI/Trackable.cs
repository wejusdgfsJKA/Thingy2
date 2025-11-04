using Spawning.Pooling;
using UnityEngine;

public enum DetectionState
{
    Hidden,
    Tracked,
    Identified
}

public class Trackable : IDPoolable<ObjectType>
{

    [SerializeField] protected DetectionState detectionState;
    public DetectionState DetectionState
    {
        get => detectionState;
        set
        {
            if (detectionState != value)
            {
                switch (value)
                {
                    case DetectionState.Hidden:
                        if (mainRenderer.enabled) mainRenderer.enabled = false;
                        if (dotRenderer.enabled) dotRenderer.enabled = false;
                        break;
                    case DetectionState.Tracked:
                        if (mainRenderer.enabled) mainRenderer.enabled = false;
                        dotRenderer.enabled = true;
                        break;
                    default:
                        dotRenderer.enabled = false;
                        mainRenderer.enabled = true;
                        break;
                }
                detectionState = value;
            }
        }
    }
    [SerializeField] MeshRenderer mainRenderer, dotRenderer;
    [field: SerializeField] public float Signature { get; protected set; } = 0;
    public override void ResetObject()
    {
        base.ResetObject();
        detectionState = DetectionState.Hidden;
        mainRenderer.enabled = false;
        dotRenderer.enabled = false;
    }
}
