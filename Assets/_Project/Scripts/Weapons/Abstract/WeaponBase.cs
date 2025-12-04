using Timers;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected float shotCooldown = 1;
        protected CountdownTimer shotTimer;
        [field: SerializeField] public float Damage { get; protected set; } = 1;
        public bool CanFire => !shotTimer.IsRunning;
        protected virtual void Awake()
        {
            shotTimer = new(shotCooldown);
        }
        public void Fire(Object @object)
        {
            ActuallyShoot(@object);
            shotTimer.Start();
        }
        protected abstract void ActuallyShoot(Object target);
        protected virtual void OnDestroy() => shotTimer.Dispose();
    }
}