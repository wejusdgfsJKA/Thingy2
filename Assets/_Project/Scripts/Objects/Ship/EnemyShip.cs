using HP;
using System.Collections.Generic;
using UnityEngine;
using Weapons;
public class EnemyShip : Ship
{
    [SerializeField] float assaultRange = 2f;
    TakeDamage assault;
    [SerializeField] List<ObjectType> ignore = new() { ObjectType.Asteroid };
    WeaponBase weapon;
    Navigation navigation;
    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<WeaponBase>();
        assault = new TakeDamage { Damage = 1f, DamageType = DamageType.Kinetic, Source = transform };
        navigation = new(transform, 5);
    }
    public override void Tick()
    {
        bool assaulting = false;
        Object closestTarget = null;
        float bestDist = float.PositiveInfinity;
        Queue<Object> planets = new();
        //commence assault on a planet if in range
        foreach (var target in Team.Targets[DetectionState.Identified])
        {
            if (ignore.Contains(target.ID)) continue;
            if (target.ID == ObjectType.Planet)
            {
                if (!assaulting)
                {
                    assaulting = true;
                    float distance = Vector3.Distance(Transform.position, target.Transform.position);
                    if (distance <= assaultRange)
                    {
                        planets.Enqueue(target);
                    }
                    else navigation.Destination = target.Transform.position + (assaultRange / 2) * Random.onUnitSphere;
                }
                continue;
            }
            float dist = Vector3.Distance(Transform.position, target.Transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                closestTarget = target;
            }
        }
        if (closestTarget != null)
        {
            if (!assaulting) navigation.Destination = closestTarget.Transform.position;
            weapon.Fire(closestTarget);
        }
        while (planets.Count > 0)
        {
            planets.Dequeue().TakeDamage(assault);
        }
        navigation.Update();
    }
}
