using UnityEngine;
using UnityEngine.Events;

namespace HP
{
    public class HullComponent : MonoBehaviour
    {
        [field: SerializeField] public int MaxHullPoints { get; set; } = 5;
        protected float currentHullPoints;
        public float CurrentHullPoints
        {
            get
            {
                return currentHullPoints;
            }
            set
            {
                value = Mathf.Clamp(value, 0, MaxHullPoints);
                if (value != currentHullPoints)
                {
                    currentHullPoints = value;
                    OnHullChanged?.Invoke(currentHullPoints);
                }
            }
        }
        /// <summary>
        /// Fires when this entity's health value changes. Has as parameter the 
        /// entity's current health.
        /// </summary>
        public UnityEvent<float> OnHullChanged;
        protected virtual void OnEnable()
        {
            CurrentHullPoints = MaxHullPoints;
        }

        public void TakeDamage(TakeDamage dmg)
        {
            CurrentHullPoints -= dmg.Damage * GlobalSettings.GetModifier(dmg.DamageType, TargetType.Hull);
            if (CurrentHullPoints <= 0) gameObject.SetActive(false);
        }
    }
}