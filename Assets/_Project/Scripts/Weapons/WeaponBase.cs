using Timers;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected float shotCooldown;
        protected CountdownTimer shotTimer;
        [field: SerializeField] public float Range { get; protected set; }
        [field: SerializeField] public float Damage { get; protected set; } = 1;
        [field: SerializeField] public float Angle { get; protected set; } = 360;
        public bool CanFire => !shotTimer.IsRunning;
        protected virtual void Awake()
        {
            shotTimer = new(shotCooldown);
        }
        public void Fire(Object @object)
        {
            //check cooldown
            if (!CanFire) return;
            //check range
            if (Vector3.Distance(transform.position, @object.Transform.position) > Range) return;
            //check angle
            if (Angle < 360)
            {
                Vector3 directionToTarget = (@object.Transform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                if (angleToTarget > Angle / 2) return;
            }
            ActuallyShoot(@object);
            shotTimer.Start();
        }
        protected abstract void ActuallyShoot(Object target);
        protected virtual void OnDestroy() => shotTimer.Dispose();
    }
}