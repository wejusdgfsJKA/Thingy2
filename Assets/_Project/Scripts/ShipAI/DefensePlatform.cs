using System.Collections.Generic;
using UnityEngine;
using Weapons;

/// <summary>
/// Attempts to fire on the closest target every tick with all weapons.
/// </summary>
public class DefensePlatform : Ship
{
    [SerializeField] protected List<WeaponBase> weapons = new();
    [SerializeField] protected List<ObjectType> ignore = new() { ObjectType.Asteroid, ObjectType.Planet };
    public override void Tick()
    {
        //get the closest target
        Object closestTarget = null;
        float bestDist = float.PositiveInfinity;
        foreach (var target in Team.Targets[DetectionState.Identified])
        {
            if (ignore.Contains(target.ID)) continue;
            float dist = Vector3.Distance(Transform.position, target.Transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                closestTarget = target;
            }
        }
        if (closestTarget != null)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].Fire(closestTarget);
            }
        }
    }
}
