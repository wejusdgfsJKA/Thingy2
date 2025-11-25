using HP;
using UnityEngine;

[RequireComponent(typeof(HullComponent))]
public class Ship : Object
{
    #region Fields
    [field: SerializeField] public float ScanRange { get; protected set; }
    [SerializeField] protected HullComponent hullComponent;
    [SerializeField] protected ShieldComponent shieldComponent;
    public float HullPoints => hullComponent.CurrentHullPoints;
    public float ShieldPoints => shieldComponent.ShieldPoints;
    #endregion
    #region Setup
    protected override void Awake()
    {
        base.Awake();
        hullComponent = GetComponent<HullComponent>();
        shieldComponent = GetComponent<ShieldComponent>();
    }
    #endregion
    public void TakeDamage(TakeDamage takeDamage)
    {
        if (shieldComponent != null)
        {
            takeDamage.Damage -= ShieldPoints / GlobalSettings.GetModifier(takeDamage.DamageType, TargetType.Shield);
            shieldComponent.TakeDamage(takeDamage);
        }
        if (takeDamage.Damage > 0)
        {
            hullComponent.TakeDamage(takeDamage);
        }
    }
}
