using Player;
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
                            RemoveFromDisplay();
                        }
                        if (dotRenderer.enabled) dotRenderer.enabled = false;
                        break;
                    case DetectionState.Tracked:
                        if (detectionState == DetectionState.Identified)
                        {
                            mainRenderer.enabled = false;
                            RemoveFromDisplay();
                        }
                        dotRenderer.enabled = true;
                        break;
                    case DetectionState.Identified:
                        AddToDisplay();
                        dotRenderer.enabled = false;
                        mainRenderer.enabled = true;
                        break;
                }
                detectionState = value;
            }
        }
    }
    [SerializeField] protected MeshRenderer mainRenderer, dotRenderer;
    /// <summary>
    /// This gets subtracted from the distance to a radar when determining detection. Default 0.
    /// </summary>
    [field: SerializeField] public float Signature { get; protected set; } = 0;
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public float IconSizeCoefficient { get; protected set; } = 200;
    /// <summary>
    /// If true, this object will not despawn when exiting update range.
    /// </summary>
    [field: SerializeField] public bool Persistent { get; set; } = false;
    public override void ResetObject()
    {
        base.ResetObject();
        detectionState = DetectionState.Hidden;
        mainRenderer.enabled = false;
        dotRenderer.enabled = false;
    }
    protected void AddToDisplay()
    {
        TrackableDisplay.Instance.AddTrackable(this);
    }
    protected void RemoveFromDisplay()
    {
        TrackableDisplay.Instance.RemoveTrackable(this);
    }
    protected override void OnDisable()
    {
        RemoveFromDisplay();
        base.OnDisable();
    }
}
