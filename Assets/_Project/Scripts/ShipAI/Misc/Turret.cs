using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    [RequireComponent(typeof(WeaponBase))]
    public class Turret : MonoBehaviour
    {
        TargetStrategy targetStrategy;
        WeaponBase weapon;
        [Tooltip("What targeting strategy should this turret employ. Defaults to closest target.")]
        [SerializeField] TargetStrategyType targetStrategyType;
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
        public bool ConsiderTarget(Object @object)
        {
            return targetStrategy.ConsiderTarget(@object);
        }
        public void Fire()
        {
            if (targetStrategy.CurrentTarget == null) return;
            var @object = targetStrategy.CurrentTarget;
            targetStrategy.Reset();

            //check cooldown
            if (!weapon.CanFire) return;

            //check range
            if (Vector3.Distance(transform.position, @object.Transform.position) > Range) return;

            //check angle
            if (Angle < 360)
            {
                Vector3 directionToTarget = (@object.Transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                if (angleToTarget > Angle / 2) return;
            }

            weapon.Fire(@object);
        }
    }
}