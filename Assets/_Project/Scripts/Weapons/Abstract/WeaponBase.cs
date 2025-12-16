using Timers;
using UnityEngine;
using UnityEngine.Events;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        #region Fields
        #region Parameters
        [SerializeField] protected float shotCooldown = 1;
        [SerializeField] protected float rateOfFireRampUp = 0.1f;
        [SerializeField] protected float rateOfFireRampDown = 0.2f;
        [SerializeField] protected float minShotCooldown = 0.1f;
        [field: SerializeField] public float Damage { get; protected set; } = 1;
        [field: SerializeField] public float SignatureIncreaseOnFire { get; protected set; } = 1;
        [Tooltip("By how much should the signature decrease every tick the weapon is not firing.")]
        [field: SerializeField] public float SignatureDecrease { get; protected set; } = .5f;
        [SerializeField] protected DamageType damageType = DamageType.Energy;
        #endregion
        protected float currentShotCooldown;
        protected CountdownTimer shotTimer;
        protected TakeDamage takeDamage;
        public bool CanFire => Charge >= 1;
        public float Charge => shotTimer.IsRunning ? shotTimer.Progress : 1;
        [SerializeField] protected UnityEvent onFire;
        #endregion
        protected virtual void Awake()
        {
            shotTimer = new(shotCooldown);
        }
        protected virtual void OnEnable()
        {
            currentShotCooldown = shotCooldown;
            shotTimer.Reset(currentShotCooldown);
        }
        public void Fire(Unit @object)
        {
            ActuallyShoot(@object);
            onFire?.Invoke();
            currentShotCooldown = Mathf.Max(minShotCooldown, currentShotCooldown - rateOfFireRampUp);
            shotTimer.Reset(currentShotCooldown);
            shotTimer.Start();
        }
        public void DecreaseRateOfFire()
        {
            currentShotCooldown = Mathf.Min(shotCooldown, currentShotCooldown + rateOfFireRampDown);
            shotTimer.Reset(currentShotCooldown);
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