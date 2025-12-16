using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public class Turret : MonoBehaviour
    {
        public enum AngleType
        {
            ForwardArc,
            Side
        }
        TurretTargetStrategy targetStrategy;
        WeaponBase weapon;
        [Tooltip("What targeting strategy should this turret employ. Defaults to closest target.")]
        [SerializeField] TargetStrategyType targetStrategyType;
        [Tooltip("What type of angle should the turret have. Side is the opposite of forward arc, takes everything except forward and backward.")]
        [field: SerializeField] public AngleType TypeOfAngle { get; protected set; } = AngleType.ForwardArc;
        [field: SerializeField] public float Angle { get; protected set; } = 360;
        [Tooltip("The range at which this weapon can fire.")]
        [field: SerializeField] public float Range { get; protected set; }
        [field: SerializeField] public List<TargetPriority> TargetPriorities { get; protected set; } = new();
        [Tooltip("If true, this turret will only fire at identified targets.")]
        [field: SerializeField] public bool RequiresLock { get; protected set; } = true;
        [Tooltip("Target score modifier applied to the target that is also the target of the ship itself.")]
        [field: SerializeField] public float CurrentTargetModifier { get; protected set; } = 2f;
        public bool HasTarget => targetStrategy.CurrentTarget != null;
        public float Signature { get; protected set; }
        private void Awake()
        {
            weapon = GetComponent<WeaponBase>();
            targetStrategy = TurretTargetStrategy.Create(targetStrategyType, this);
        }
        private void OnEnable()
        {
            Signature = 0;
            targetStrategy?.Clear();
        }
        public bool ConsiderTarget(Unit @object, DetectionState detectionState = DetectionState.Identified)
        {
            if (@object == null) throw new System.ArgumentNullException($"{this} received null target for evaluation.");
            return targetStrategy.ConsiderTarget(@object, detectionState);
        }
        public bool Tick()
        {
            var @object = CanFire();
            if (!@object)
            {
                Signature = Mathf.Max(0, Signature - weapon.SignatureDecrease * GlobalSettings.AITickCooldown);
                if (weapon.CanFire) weapon.DecreaseRateOfFire();
            }
            else
            {
                weapon.Fire(@object);
                Signature = weapon.SignatureIncreaseOnFire;
            }
            targetStrategy.Clear();
            return @object != null;
        }
        public Unit CanFire()
        {
            if (targetStrategy.CurrentTarget != null)
            {
                Unit target = targetStrategy.CurrentTarget;
                //check cooldown
                if (!weapon.CanFire) return null;
                //check range
                if (Vector3.Distance(transform.position, target.Transform.position) > Range) return null;
                //check angle
                if (!IsInAngle(target)) return null;
                return target;
            }
            return null;
        }
        public bool IsInAngle(Unit target)
        {
            switch (TypeOfAngle)
            {
                case AngleType.ForwardArc:
                    {
                        if (Angle >= 360) return true;
                        Vector3 directionToTarget = (target.Transform.position - transform.position).normalized;
                        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                        return angleToTarget <= Angle / 2;
                    }
                case AngleType.Side:
                    {
                        if (Angle >= 180) return true;
                        Vector3 directionToTarget = (target.Transform.position - transform.position).normalized;
                        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                        return 180 - angleToTarget <= Angle;
                    }
            }
            return false;
        }
    }
}