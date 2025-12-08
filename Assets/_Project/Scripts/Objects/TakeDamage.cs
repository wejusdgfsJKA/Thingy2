using EventBus;
using UnityEngine;
namespace HP
{
    /// <summary>
    /// Use this to deal heavyDamage to an entity.
    /// </summary>
    public struct TakeDamage : IEvent
    {
        /// <summary>
        /// How much heavyDamage this attack has dealt.
        /// </summary>
        public float Damage { get; set; }
        public DamageType DamageType;
        /// <summary>
        /// The source of the heavyDamage.
        /// </summary>
        public Transform Source { get; set; }
        public TakeDamage(float damage, Transform source, DamageType damageType)
        {
            Damage = damage;
            Source = source;
            DamageType = damageType;
        }
    }
}