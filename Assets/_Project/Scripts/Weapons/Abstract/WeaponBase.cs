using Timers;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected float shotCooldown = 1;
        protected CountdownTimer shotTimer;
        protected TakeDamage takeDamage;
        [field: SerializeField] public float Damage { get; protected set; } = 1;
        public float Charge => shotTimer.IsRunning ? shotTimer.Progress : 1;
        [SerializeField] protected DamageType damageType = DamageType.Energy;
        [SerializeField] protected UnityEvent onFire;
        [field: SerializeField] public float SignatureOnFire { get; protected set; } = 1;
        [field: SerializeField] public float SignatureDecrease { get; protected set; } = .5f;
        protected virtual void Awake()
        {
            shotTimer = new(shotCooldown);
        }
        protected virtual void OnEnable()
        {
            shotTimer.Reset();
        }
        public void Fire(Unit @object)
        {
            ActuallyShoot(@object);
            onFire?.Invoke();
            shotTimer.Start();
        }
        protected abstract void ActuallyShoot(Unit target);
        protected virtual void OnDisable()
        {
            onFire.RemoveAllListeners();
        }
        protected virtual void OnDestroy()
        {
            shotTimer.Dispose();
        }
    }
}