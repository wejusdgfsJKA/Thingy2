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
                        if (detectionState == DetectionState.Identified)
                        {
                            mainRenderer.enabled = false;
                            DeRegisterIconFromDisplay();
                        }
                        if (dotRenderer.enabled) dotRenderer.enabled = false;
                        break;
                    case DetectionState.Tracked:
                        if (detectionState == DetectionState.Identified)
                        {
                            mainRenderer.enabled = false;
                            DeRegisterIconFromDisplay();
                        }
                        dotRenderer.enabled = true;
                        break;
                    case DetectionState.Identified:
                        RegisterIconToDisplay();
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
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public float IconSizeCoefficient { get; protected set; } = 200;
    [field: SerializeField] public bool Persistent { get; set; } = false;
    public override void ResetObject()
    {
        base.ResetObject();
        detectionState = DetectionState.Hidden;
        mainRenderer.enabled = false;
        dotRenderer.enabled = false;
    }
    protected void RegisterIconToDisplay()
    {
        TrackableDisplay.Instance.AddTrackable(this);
    }
    protected void DeRegisterIconFromDisplay()
    {
        TrackableDisplay.Instance.RemoveTrackable(this);
    }
    protected override void OnDisable()
    {
        DeRegisterIconFromDisplay();
        base.OnDisable();
    }
}
