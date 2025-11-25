using Timers;
using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected float shotCooldown;
        protected IntervalTimer shotTimer;
        public bool Firing
        {
            get => shotTimer.IsRunning;
            set
            {
                if (value)
                {
                    if (!shotTimer.IsRunning) shotTimer.Start();
                }
                else shotTimer.Stop();
            }
        }
        protected virtual void Awake()
        {
            shotTimer = new(float.PositiveInfinity, shotCooldown);
            shotTimer.OnInterval += Fire;
        }
        protected abstract void Fire();
    }
}