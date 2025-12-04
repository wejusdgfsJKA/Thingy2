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
    [SerializeField] protected List<Turret> turrets = new();
    #endregion
    #region Setup
    protected override void OnDisable()
    {
        base.OnDisable();
        Team?.RemoveMember(this);
        Team = null;
    }
    #endregion
    public virtual void Tick()
    {
        foreach (var target in Team.IdentifiedTargets)
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
            turrets[i].ConsiderTarget(@object);
        }
    }
}
