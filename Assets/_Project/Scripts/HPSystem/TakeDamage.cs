using EventBus;
using UnityEngine;
namespace HP
{
    /// <summary>
    /// Use this to deal damage to an entity.
    /// </summary>
    public struct TakeDamage : IEvent
    {
        /// <summary>
        /// How much damage this attack has dealt.
        /// </summary>
        public int Damage { get; set; }
        /// <summary>
        /// The source of the damage.
        /// </summary>
        public Transform Source { get; set; }
        /// <summary>
        /// The collider we hit.
        /// </summary>
        public int? ColliderID { get; set; }
        public TakeDamage(int damage, Transform source, Collider collider)
        {
            Damage = damage;
            Source = source;
            if (collider != null)
            {
                ColliderID = collider.GetInstanceID();
            }
            else ColliderID = null;
        }
    }
}