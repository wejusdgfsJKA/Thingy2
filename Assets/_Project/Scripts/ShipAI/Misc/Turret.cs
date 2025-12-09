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
        TargetStrategy targetStrategy;
        WeaponBase weapon;
        [Tooltip("What targeting strategy should this turret employ. Defaults to closest target.")]
        [SerializeField] TargetStrategyType targetStrategyType;
        [field: SerializeField] public AngleType TypeOfAngle { get; protected set; } = AngleType.ForwardArc;
        [field: SerializeField] public float Angle { get; protected set; } = 360;
        [field: SerializeField] public float Range { get; protected set; }
        [field: SerializeField] public List<TargetPriority> TargetPriorities { get; protected set; } = new();
        [field: SerializeField] public bool RequiresLock { get; protected set; } = true;
        public bool HasTarget => targetStrategy.CurrentTarget != null;
        private void Awake()
        {
            weapon = GetComponent<WeaponBase>();
            switch (targetStrategyType)
            {
                default:
                    targetStrategy = new ClosestTargetStrategy(this);
                    break;
            }
        }
        private void OnEnable()
        {
            targetStrategy?.Reset();
        }
        public bool ConsiderTarget(Unit @object)
        {
            if (@object == null) throw new System.ArgumentNullException($"{this} received null target for evaluation.");
            return targetStrategy.ConsiderTarget(@object);
        }
        public void Fire()
        {
            if (targetStrategy.CurrentTarget == null) return;
            var @object = targetStrategy.CurrentTarget;
            targetStrategy.Reset();

            //check cooldown
            if (weapon.Charge < 1) return;
            //check range
            if (Vector3.Distance(transform.position, @object.Transform.position) > Range) return;

            //check angle
            if (!IsInAngle(@object)) return;

            weapon.Fire(@object);
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