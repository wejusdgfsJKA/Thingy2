using HybridBT.Template;
using UnityEngine;

[RequireComponent(typeof(ShipBT))]
public class Ship : Unit
{
    [SerializeField] protected float speed, rotationSpeed;
    public Navigation Navigation { get; protected set; }
    protected ShipBT behaviourTree;
    protected override void Awake()
    {
        base.Awake();
        behaviourTree = GetComponent<ShipBT>();
        Navigation = new(transform, speed, rotationSpeed);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Navigation.Destination = null;
    }
    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        behaviourTree.Tick();
        Navigation?.Update(deltaTime);
    }
}
