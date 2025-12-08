using HP;
using Spawning.Pooling;
using UnityEngine;

public class Object : IDPoolable<ObjectType>
{
    #region Fields
    [field: SerializeField] public float Signature { get; set; } = 0;
    #region UI/Detection stuff
    [field: SerializeField] public bool Selectable { get; set; } = true;
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public Material Material { get; protected set; }
    [field: SerializeField] public float IconSizeCoefficient { get; protected set; } = 300;
    [field: SerializeField] public MeshRenderer IdentifiedRenderer { get; protected set; }
    [field: SerializeField] public MeshRenderer TrackedRenderer { get; protected set; }
    #endregion
    public event System.Action<Object> OnDespawn;
    public Transform Transform { get; protected set; }
    public Vector3 Position => Transform.position;
    protected HullComponent hullComponent;
    protected ShieldComponent shieldComponent;
    public float HullPoints => hullComponent.CurrentHullPoints;
    public float ShieldPoints => shieldComponent.ShieldPoints;
    #endregion
    #region Setup
    protected virtual void Awake()
    {
        Transform = transform;
        if (IdentifiedRenderer == null) IdentifiedRenderer = GetComponent<MeshRenderer>();
        if (TrackedRenderer == null && Transform.childCount > 0)
        {
            TrackedRenderer = Transform.GetChild(0).GetComponent<MeshRenderer>();
        }
        hullComponent = GetComponent<HullComponent>();
        shieldComponent = GetComponent<ShieldComponent>();
    }
    protected virtual void OnEnable()
    {
        if (ID != ObjectType.Player)
        {
            IdentifiedRenderer.enabled = false;
            if (TrackedRenderer != null) TrackedRenderer.enabled = false;
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        OnDespawn?.Invoke(this);
    }
    protected virtual void OnDestroy()
    {
        OnDespawn = null;
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
}
