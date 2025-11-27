using Timers;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected float shotCooldown;
        protected CountdownTimer shotTimer;
        protected bool firing;
        public bool Firing
        {
            get => firing;
            set
            {
                if (firing == value) return;
                if (value)
                {
                    Fire();
                    shotTimer.OnTimerStop += Fire;
                }
                else shotTimer.OnTimerStop -= Fire;
            }
        }
        protected virtual void Awake()
        {
            shotTimer = new(shotCooldown);
        }
        public void Fire()
        {
            if (shotTimer.IsRunning) return;
            Shoot();
            shotTimer.Start();
        }
        protected abstract void Shoot();
    }
}