using Global;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

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
        [field: SerializeField] public List<ArmorModifier> ArmorModifiers { get; private set; } = new();
        protected virtual void OnEnable()
        {
            CurrentHullPoints = MaxHullPoints;
            GameManager.ClearKill(transform.root);
        }

        public void TakeDamage(TakeDamage dmg)
        {
            float finalDmg = CalculateDamage(dmg);
            if (finalDmg <= 0) return;
            CurrentHullPoints -= finalDmg;
            OnDamageTaken.Invoke(finalDmg);
            if (CurrentHullPoints <= 0)
            {
                if (dmg.Source == GameManager.Player.Transform) GameManager.AddPlayerKill(transform.root);
                gameObject.SetActive(false);
            }
        }
        public float CalculateDamage(TakeDamage dmg)
        {
            float armorModifier = 1;
            var direction = gameObject.GetDirection(dmg.Source.position);
            foreach (var armor in ArmorModifiers)
            {
                if (armor.Direction == direction)
                {
                    armorModifier = armor.Modifier;
                    break;
                }
            }
            return armorModifier * dmg.Damage * GlobalSettings.GetDamageModifier(dmg.DamageType, TargetType.Hull);
        }
    }
}