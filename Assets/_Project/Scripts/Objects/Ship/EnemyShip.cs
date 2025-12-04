using HP;
using System.Collections.Generic;
using UnityEngine;
public class EnemyShip : Unit
{
    [SerializeField] float assaultRange = 2f;
    TakeDamage assault;
    Navigation navigation;
    Queue<Object> planets = new();
    [SerializeField] List<ObjectType> ignore = new();
    bool assaulting;
    Object closestTarget;
    float closestTargetDist;
    protected override void Awake()
    {
        base.Awake();
        assault = new TakeDamage { Damage = 1f, DamageType = DamageType.Kinetic, Source = transform };
        navigation = new(transform, 5);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        navigation.Destination = null;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        planets.Clear();
    }
    public override void Tick()
    {
        closestTarget = null;
        closestTargetDist = float.PositiveInfinity;
        assaulting = false;
        base.Tick();
        while (planets.Count > 0)
        {
            planets.Dequeue().TakeDamage(assault);
        }
        navigation.Update();
        if (closestTarget == null)
        {
            foreach (var @object in Team.TrackedHostiles)
            {
                float dist = Vector3.Distance(Transform.position, @object.Transform.position);
                if (dist < closestTargetDist)
                {
                    closestTarget = @object;
                    closestTargetDist = dist;
                }
            }
            if (closestTarget != null)
            {
                navigation.Destination = closestTarget.Transform.position;
            }
            else navigation.Destination = Vector3.zero;
        }
    }
    protected override void ConsiderTarget(Object @object)
    {
        base.ConsiderTarget(@object);
        if (@object.ID == ObjectType.Planet)
        {
            if (!assaulting)
            {
                assaulting = true;
                if (Vector3.Distance(transform.position, @object.transform.position) <= assaultRange)
                {
                    planets.Enqueue(@object);
                }
                else navigation.Destination = @object.Transform.position;
            }
        }
        else
        {
            if (!assaulting)
            {
                float dist = Vector3.Distance(Transform.position, @object.Transform.position);
                if (dist < closestTargetDist)
                {
                    closestTargetDist = dist;
                    closestTarget = @object;
                }
            }
        }
    }
}
