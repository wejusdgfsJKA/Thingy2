using HP;
using Player;
using Spawning.Pooling;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using Weapons;
[RequireComponent(typeof(HullComponent))]
public abstract class Unit : IDPoolable<ObjectType>
{
    #region Fields
    #region UI stuff
    [field: SerializeField] public bool Selectable { get; set; } = true;
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public Material IconIdentifiedMaterial { get; protected set; }
    [field: SerializeField] public Material IconTrackedMaterial { get; protected set; }
    [field: SerializeField] public float IconSizeCoefficient { get; protected set; } = 5;
    [field: SerializeField] public MeshRenderer IdentifiedRenderer { get; protected set; }
    [field: SerializeField] public MeshRenderer TrackedRenderer { get; protected set; }
    protected Material defaultIdentifiedRendererMaterial;
    #endregion
    #region Parameters
    [SerializeField] protected float defaultSignature = 10;
    [field: SerializeField] public float ScanRange { get; protected set; }
    [field: SerializeField] public List<Turret> Turrets { get; protected set; } = new();
    #endregion

    #region Helpers
    /// <summary>
    /// Unit transform.
    /// </summary>
    public Transform Transform { get; protected set; }
    /// <summary>
    /// Transform.position.
    /// </summary>
    public Vector3 Position => Transform.position;
    public float HullPoints => hullComponent.CurrentHullPoints;
    public float ShieldPoints => shieldComponent.ShieldPoints;
    public float Signature { get; protected set; }
    #endregion   
    protected HullComponent hullComponent;
    protected ShieldComponent shieldComponent;
    /// <summary>
    /// Index of this ship's team in GameManager.Teams.
    /// </summary>
    public int Team { get; set; }
    /// <summary>
    /// True if any turret has a target this tick.
    /// </summary>
    public bool TurretsHaveTarget { get; protected set; }
    protected CountdownTimer tickTimer, damageRenderTimer;
    public readonly Dictionary<DetectionState, HashSet<Unit>> Targets = new() {
        {DetectionState.Identified, new HashSet<Unit>() },
        {DetectionState.Tracked, new HashSet<Unit>() }
    };
    /// <summary>
    /// Fires when the unit is deactivated. Including when destroyed.
    /// </summary>
    public event System.Action<Unit> OnDespawn;
    public abstract Unit CurrentTarget { get; }
    #endregion
    #region Setup
    protected virtual void Awake()
    {
        Transform = transform;
        hullComponent = GetComponent<HullComponent>();
        shieldComponent = GetComponent<ShieldComponent>();

        #region UI stuff
        if (!IdentifiedRenderer) IdentifiedRenderer = GetComponent<MeshRenderer>();
        defaultIdentifiedRendererMaterial = IdentifiedRenderer.material;
        if (!IconIdentifiedMaterial && IdentifiedRenderer)
        {
            IconIdentifiedMaterial = IdentifiedRenderer.material;
        }
        if (!TrackedRenderer && Transform.childCount > 0)
        {
            TrackedRenderer = Transform.GetChild(0).GetComponent<MeshRenderer>();
        }
        if (!IconTrackedMaterial && TrackedRenderer)
        {
            IconTrackedMaterial = TrackedRenderer.material;
        }
        #endregion

        #region Timers
        tickTimer = new CountdownTimer(GlobalSettings.AITickCooldown);
        tickTimer.OnTimerStop += () =>
        {
            tickTimer.Start();
            Tick(GlobalSettings.AITickCooldown);
        };
        damageRenderTimer = new CountdownTimer(GlobalSettings.DamageRenderTime);
        damageRenderTimer.OnTimerStop += ResetMaterials;
        #endregion
    }
    protected virtual void OnEnable()
    {
        ResetMaterials();
        Signature = defaultSignature;
        Targets[DetectionState.Identified].Clear();
        Targets[DetectionState.Tracked].Clear();
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
        Targets.Clear();
        OnDespawn = null;
        tickTimer.Dispose();
    }
    #endregion
    #region Functionality
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
            IdentifiedRenderer.material = ObjectDisplay.HullDamageMaterial;
        }
        else IdentifiedRenderer.material = ObjectDisplay.ShieldDamageMaterial;
        damageRenderTimer.Start();
    }
    protected void ResetMaterials()
    {
        IdentifiedRenderer.material = defaultIdentifiedRendererMaterial;
    }
    protected virtual void Tick(float deltaTime)
    {
        TurretsHaveTarget = false;
        IterateOverTargets();

        foreach (var target in Targets[DetectionState.Identified])
        {
            ConsiderTargetForTurrets(target);
        }
        foreach (var target in Targets[DetectionState.Tracked])
        {
            ConsiderTargetForTurrets(target, DetectionState.Tracked);
        }
        for (int i = 0; i < Turrets.Count; i++)
        {
            Turrets[i].Tick();
        }

        RecalculateSignature();
    }
    protected void RecalculateSignature()
    {
        Signature = defaultSignature;
        for (int i = 0; i < Turrets.Count; i++)
        {
            var turret = Turrets[i];
            if (turret != null)
            {
                Signature += turret.Signature;
            }
        }
    }
    #region Targeting
    protected virtual void IterateOverTargets()
    {
        var enemyTeam = Team == 0 ? GameManager.Teams[1] : GameManager.Teams[0];
        foreach (var obj in enemyTeam.Members)
        {
            if (!Targets[DetectionState.Identified].Contains(obj) && !Targets[DetectionState.Tracked].Contains(obj))
            {
                obj.OnDespawn += RemoveTarget;
            }
            float dist = Vector3.Distance(obj.Position, Position) - obj.Signature;
            if (dist <= ScanRange)
            {
                Targets[DetectionState.Tracked].Remove(obj);
                Targets[DetectionState.Identified].Add(obj);
                OnTarget(obj);
            }
            else
            {
                Targets[DetectionState.Identified].Remove(obj);
                Targets[DetectionState.Tracked].Add(obj);
                OnTarget(obj, DetectionState.Tracked);
            }
        }
    }
    protected void RemoveTarget(Unit unit)
    {
        unit.OnDespawn -= RemoveTarget;
        if (Targets[DetectionState.Identified].Remove(unit) || Targets[DetectionState.Tracked].Remove(unit))
        {

        }
    }
    protected virtual void OnTarget(Unit target, DetectionState detectionState = DetectionState.Identified)
    {

    }
    protected virtual void ConsiderTargetForTurrets(Unit @object, DetectionState detectionState = DetectionState.Identified)
    {
        for (int i = 0; i < Turrets.Count; i++)
        {
            if (Turrets[i].ConsiderTarget(@object, detectionState)) TurretsHaveTarget = true;
        }
    }
    #endregion
    #endregion
}
