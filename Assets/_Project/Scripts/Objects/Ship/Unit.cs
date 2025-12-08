using HP;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using Weapons;
[RequireComponent(typeof(HullComponent))]
public class Unit : Object
{
    #region Fields
    [field: SerializeField] public float ScanRange { get; protected set; }
    public Team Team { get; set; }
    [field: SerializeField] public List<Turret> Turrets { get; protected set; } = new();
    public bool TurretsHaveTarget;
    protected CountdownTimer tickTimer;
    #endregion
    #region Setup
    protected override void Awake()
    {
        base.Awake();
        tickTimer = new CountdownTimer(GlobalSettings.AITickCooldown);
        tickTimer.OnTimerStop += () =>
        {
            tickTimer.Start();
            Tick(GlobalSettings.AITickCooldown);
        };
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        tickTimer.Start();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        tickTimer.Stop();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        tickTimer.Dispose();
    }
    #endregion
    protected virtual void Tick(float deltaTime)
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
        for (int i = 0; i < Turrets.Count; i++)
        {
            Turrets[i].Fire();
        }
    }
    protected virtual void ConsiderTarget(Object @object)
    {
        for (int i = 0; i < Turrets.Count; i++)
        {
            if (Turrets[i].ConsiderTarget(@object)) TurretsHaveTarget = true;
        }
    }
}
