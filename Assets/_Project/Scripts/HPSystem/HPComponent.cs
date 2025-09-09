using EventBus;
using UnityEngine;
using UnityEngine.Events;

namespace HP
{
    public class HPComponent : MonoBehaviour
    {
        [field: SerializeField] public int MaxHealth { get; set; }
        protected float currentHealth;
        public float CurrentHealth
        {
            get
            {
                return currentHealth;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxHealth);
                if (value != currentHealth)
                {
                    currentHealth = value;
                    OnDamageTaken?.Invoke(currentHealth);
                }
            }
        }
        /// <summary>
        /// Fires when this entity's health value changes. Has as parameter the 
        /// entity's current health.
        /// </summary>
        public UnityEvent<float> OnDamageTaken;
        /// <summary>
        /// Fires when this entity dies.
        /// </summary>
        public UnityEvent OnDeath;
        protected virtual void Awake()
        {
            //add damage binding for this entity
            EventBus<TakeDamage>.AddActions(transform.root.GetInstanceID(), TakeDamage);
        }
        protected void OnEnable()
        {
            CurrentHealth = MaxHealth;
        }
        /// <summary>
        /// Attempt to deal damage to an object.
        /// </summary>
        /// <param name="transform">The object whose root that we are trying to damage.</param>
        /// <param name="dmg">The damage Event.</param>
        /// <returns>True if successfully dealt damage to the object's root.</returns>
        public static bool TakeDamage(Transform transform, TakeDamage dmg)
        {
            return EventBus<TakeDamage>.Raise(transform.root.GetInstanceID(), dmg);
        }
        /// <summary>
        /// Take damage. Invoke OnDeath when HP reaches 0. 
        /// </summary>
        /// <param name="dmg">Damage Event.</param>
        public void TakeDamage(TakeDamage dmg)
        {
            CurrentHealth -= CalculateDamage(dmg);
            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
                return;
            }
        }
        protected virtual float CalculateDamage(TakeDamage dmg)
        {
            return dmg.Damage;
        }
        protected void OnDestroy()
        {
            //clear this binding from the Event bus.
            EventBus<TakeDamage>.RemoveActions(transform.GetInstanceID(), TakeDamage);
        }
    }
}