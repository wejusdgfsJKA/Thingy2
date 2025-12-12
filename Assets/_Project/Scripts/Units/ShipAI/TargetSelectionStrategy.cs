using UnityEngine;

public abstract class TargetSelectionStrategy
{
    public enum Type
    {
        Nearest
    }
    public Unit CurrentTarget { get; protected set; } = null;
    protected float currentTargetScore = 0;
    protected Transform self;
    public TargetSelectionStrategy(Ship ship)
    {
        self = ship.Transform;
    }
    public static TargetSelectionStrategy Create(Type type, Ship ship)
    {
        return type switch
        {
            Type.Nearest => new NearestTargetSelectionStrategy(ship)
        };
    }
    public void Clear()
    {
        CurrentTarget = null;
        currentTargetScore = 0;
    }
    public bool ConsiderTarget(Unit potentialTarget, DetectionState detectionState = DetectionState.Identified)
    {
        float score = ScoreTarget(potentialTarget, detectionState);
        if (score > currentTargetScore)
        {
            currentTargetScore = score;
            CurrentTarget = potentialTarget;
            return true;
        }
        return false;
    }
    public abstract float ScoreTarget(Unit potentialTarget, DetectionState detectionState = DetectionState.Identified);
}
public class NearestTargetSelectionStrategy : TargetSelectionStrategy
{
    readonly float maxDistance;
    public NearestTargetSelectionStrategy(Ship ship) : base(ship)
    {
        maxDistance = GlobalSettings.MaxTargetDistance;
    }
    public override float ScoreTarget(Unit potentialTarget, DetectionState detectionState = DetectionState.Identified)
    {
        return maxDistance - Vector3.Distance(self.position, potentialTarget.Transform.position);
    }
}
