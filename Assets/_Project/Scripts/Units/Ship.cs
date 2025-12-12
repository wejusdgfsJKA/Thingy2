using HybridBT.Template;
using UnityEngine;
[RequireComponent(typeof(ShipBT))]
public class Ship : Unit
{
    [SerializeField] protected float speed, rotationSpeed;
    [SerializeField] protected TargetSelectionStrategy.Type targetSelectionType = TargetSelectionStrategy.Type.Nearest;
    [field: SerializeField] public float MaxTargetDistance { get; protected set; } = GlobalSettings.MaxTargetDistance;
    public Unit CurrentTarget => targetSelectionStrategy.CurrentTarget;
    public Navigation Navigation { get; protected set; }
    protected ShipBT behaviourTree;
    protected TargetSelectionStrategy targetSelectionStrategy;
    protected override void Awake()
    {
        base.Awake();
        behaviourTree = GetComponent<ShipBT>();
        Navigation = new(transform, speed, rotationSpeed);
        targetSelectionStrategy = TargetSelectionStrategy.Create(targetSelectionType, this);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Navigation.Destination = null;
    }
    protected override void Tick(float deltaTime)
    {
        targetSelectionStrategy.Clear();
        base.Tick(deltaTime);
        behaviourTree.SetValue(ShipAIKeys.Target, targetSelectionStrategy.CurrentTarget);
        behaviourTree.Tick();
        Navigation?.Update(deltaTime);
    }
    protected override void OnTarget(Unit target, DetectionState detectionState = DetectionState.Identified)
    {
        Debug.Log(targetSelectionStrategy.ConsiderTarget(target, detectionState));

    }
}
