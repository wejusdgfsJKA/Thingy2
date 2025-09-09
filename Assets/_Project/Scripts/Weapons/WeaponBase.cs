using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected float shotCooldown;
        /// <summary>
        /// If true, the weapon will fire every frame it is off cooldown.
        /// </summary>
        [field: SerializeField] public bool Firing { get; set; }
        /// <summary>
        /// When did we last shoot? Used for cooldown system.
        /// </summary>
        protected float timeLastShot;
        protected void Update()
        {
            if (Firing)
            {
                if (Time.time - timeLastShot >= shotCooldown)
                {
                    timeLastShot = Time.time;
                    Fire();
                }
            }
        }
        protected virtual void OnEnable()
        {
            timeLastShot = -1;
            Firing = false;
        }
        protected abstract void Fire();
    }
}