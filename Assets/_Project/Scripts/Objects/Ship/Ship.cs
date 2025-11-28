using HP;
using UnityEngine;
[RequireComponent(typeof(HullComponent))]
public abstract class Ship : Object
{
    #region Fields
    [field: SerializeField] public float ScanRange { get; protected set; }
    public Team Team { get; set; }
    #endregion
    #region Setup
    protected override void OnDisable()
    {
        base.OnDisable();
        Team?.RemoveMember(this);
        Team = null;
    }
    #endregion
    public abstract void Tick();
}
