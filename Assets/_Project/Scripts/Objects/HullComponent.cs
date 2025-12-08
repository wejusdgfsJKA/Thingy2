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
                    OnHullChanged?.Invoke(currentHullPoints / MaxHullPoints);
                }
            }
        }
        [Tooltip("Fires when this entity's health value changes. Has as parameter the entity's current health percentage.")]
        public UnityEvent<float> OnHullChanged;
        public UnityEvent<float> OnDamageTaken;
        protected virtual void OnEnable()
        {
            CurrentHullPoints = MaxHullPoints;
        }

        public void TakeDamage(TakeDamage dmg)
        {
            float finalDmg = dmg.Damage * GlobalSettings.GetDamageModifier(dmg.DamageType, TargetType.Hull);
            CurrentHullPoints -= finalDmg;
            OnDamageTaken.Invoke(finalDmg);
            if (CurrentHullPoints <= 0) gameObject.SetActive(false);
        }
    }
}