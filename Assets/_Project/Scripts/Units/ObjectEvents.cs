using EventBus;
using Global;
using UnityEngine;
public struct SignatureIncreaseEvent : IEvent
{
    public float Amount;
    public float Duration;
    public SignatureIncreaseEvent(float amount, float duration)
    {
        Amount = amount;
        Duration = duration;
    }
}
/// <summary>
/// Use this to deal damage to an entity.
/// </summary>
public struct TakeDamage : IEvent
{
    /// <summary>
    /// How much damage this attack has dealt.
    /// </summary>
    public float Damage { get; set; }
    public DamageType DamageType;
    /// <summary>
    /// The source of the damage.
    /// </summary>
    public Transform Source { get; set; }
    public TakeDamage(float damage, Transform source, DamageType damageType)
    {
        Damage = damage;
        Source = source;
        DamageType = damageType;
    }
}