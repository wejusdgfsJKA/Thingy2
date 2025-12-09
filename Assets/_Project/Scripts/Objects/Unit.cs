using HP;
using Spawning.Pooling;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using Weapons;
[RequireComponent(typeof(HullComponent))]
[RequireComponent(typeof(SignatureComponent))]
public class Unit : IDPoolable<ObjectType>
{
    #region Fields
    #region UI stuff
    [field: SerializeField] public bool Selectable { get; set; } = true;
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public Material Material { get; protected set; }
    [field: SerializeField] public float IconSizeCoefficient { get; protected set; } = 5;
    [field: SerializeField] public MeshRenderer IdentifiedRenderer { get; protected set; }
    [field: SerializeField] public MeshRenderer TrackedRenderer { get; protected set; }
    #endregion
    #region Parameters
    public float Signature => signatureComponent.Signature;
    [field: SerializeField] public float ScanRange { get; protected set; }
    [field: SerializeField] public List<Turret> Turrets { get; protected set; } = new();
    #endregion
    public event System.Action<Unit> OnDespawn;
    public Transform Transform { get; protected set; }
    public Vector3 Position => Transform.position;
    protected HullComponent hullComponent;
    protected SignatureComponent signatureComponent;
    protected ShieldComponent shieldComponent;
    public float HullPoints => hullComponent.CurrentHullPoints;
    public float ShieldPoints => shieldComponent.ShieldPoints;
    public Team Team { get; set; }
    public bool TurretsHaveTarget { get; protected set; }
    protected CountdownTimer tickTimer;
    #endregion
    #region Setup
    protected virtual void Awake()
    {
        Transform = transform;
        hullComponent = GetComponent<HullComponent>();
        shieldComponent = GetComponent<ShieldComponent>();
        signatureComponent = GetComponent<SignatureComponent>();

        #region UI stuff
        if (IdentifiedRenderer == null) IdentifiedRenderer = GetComponent<MeshRenderer>();
        if (TrackedRenderer == null && Transform.childCount > 0)
        {
            TrackedRenderer = Transform.GetChild(0).GetComponent<MeshRenderer>();
        }
        #endregion

        #region Timers
        tickTimer = new CountdownTimer(GlobalSettings.AITickCooldown);
        tickTimer.OnTimerStop += () =>
        {
            tickTimer.Start();
            Tick(GlobalSettings.AITickCooldown);
        };
        #endregion
    }
    protected virtual void OnEnable()
    {
        if (ID != ObjectType.Player)
        {
            IdentifiedRenderer.enabled = false;
            if (TrackedRenderer != null) TrackedRenderer.enabled = false;
        }
        tickTimer.Start();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        OnDespawn?.Invoke(this);
        tickTimer.Stop();
    }
    protected virtual void OnDestroy()
    {
        OnDespawn = null;
        tickTimer.Dispose();
    }
    #endregion
    public void TakeDamage(TakeDamage takeDamage)
    {
        if (shieldComponent != null)
        {
            float newDamage = takeDamage.Damage - ShieldPoints / GlobalSettings.GetDamageModifier(takeDamage.DamageType, TargetType.Shield);
            shieldComponent.TakeDamage(takeDamage);
            takeDamage.Damage = newDamage;
        }
        if (takeDamage.Damage > 0)
        {
            hullComponent.TakeDamage(takeDamage);
        }
    }
    protected virtual void Tick(float deltaTime)
    {
        if (Team == null) return;
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
    protected virtual void ConsiderTarget(Unit @object)
    {
        for (int i = 0; i < Turrets.Count; i++)
        {
            if (Turrets[i].ConsiderTarget(@object)) TurretsHaveTarget = true;
        }
    }
}
