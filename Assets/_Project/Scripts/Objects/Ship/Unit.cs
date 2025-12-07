using HP;
using System.Collections.Generic;
using UnityEngine;
using Weapons;
[RequireComponent(typeof(HullComponent))]
public class Unit : Object
{
    #region Fields
    [field: SerializeField] public float ScanRange { get; protected set; }
    public Team Team { get; set; }
    [field: SerializeField] public List<Turret> turrets { get; protected set; } = new();
    public bool TurretsHaveTarget;
    #endregion
    #region Setup
    protected override void OnDisable()
    {
        base.OnDisable();
        Team?.RemoveMember(this);
        Team = null;
    }
    #endregion
    public virtual void Tick(float deltaTime)
    {
        TurretsHaveTarget = false;
        foreach (var target in Team.IdentifiedTargets)
        {
            ConsiderTarget(target);
        }
        foreach (var target in Team.TrackedTargets)
        {
            ConsiderTarget(target);
        }
        for (int i = 0; i < turrets.Count; i++)
        {
            turrets[i].Fire();
        }
    }
    protected virtual void ConsiderTarget(Object @object)
    {
        for (int i = 0; i < turrets.Count; i++)
        {
            if (turrets[i].ConsiderTarget(@object)) TurretsHaveTarget = true;
        }
    }
}
