using HP;
using Timers;
using UnityEngine;
using UnityEngine.Events;

public class ShieldComponent : MonoBehaviour
{
    [field: SerializeField] public float MaxShieldPoints { get; protected set; }
    [SerializeField] protected float shieldPoints;
    public float ShieldPoints
    {
        get => shieldPoints;
        protected set
        {
            var newValue = Mathf.Clamp(value, 0, MaxShieldPoints);
            if (shieldPoints != newValue)
            {
                shieldPoints = newValue;
                shieldValueChanged?.Invoke(shieldPoints / MaxShieldPoints);
            }
        }
    }
    [Tooltip("Fires when the value of the shield changes. Has as paramater the percentage.")]
    [SerializeField] protected UnityEvent<float> shieldValueChanged;
    [Tooltip("Shield regeneration amount.")]
    [field: SerializeField] public float ShieldRegen { get; protected set; }
    [Tooltip("Time that needs to pass without taking damage for shields to begin regenerating.")]
    [field: SerializeField] public float ShieldRegenCD { get; protected set; }
    [Tooltip("Time between shield value regenerating.")]
    [field: SerializeField] public float ShieldRegenInterval { get; protected set; }
    protected IntervalTimer regenTimer;
    protected CountdownTimer shieldCDTimer;
    protected virtual void Awake()
    {
        regenTimer = new IntervalTimer(Mathf.Infinity, ShieldRegenInterval);
        regenTimer.OnInterval += () =>
        {
            ShieldPoints += ShieldRegen;
            if (ShieldPoints >= MaxShieldPoints)
            {
                regenTimer.Stop();
            }
        };

        shieldCDTimer = new CountdownTimer(ShieldRegenCD);
        shieldCDTimer.OnTimerStop += () => regenTimer.Start();
    }
    private void OnEnable()
    {
        ShieldPoints = MaxShieldPoints;
    }
    public void TakeDamage(TakeDamage takeDamage)
    {
        ShieldPoints -= takeDamage.Damage * GlobalSettings.GetDamageModifier(takeDamage.DamageType, TargetType.Shield);
        regenTimer.Stop();
        shieldCDTimer.Start();
    }
    private void OnDestroy()
    {
        regenTimer.Dispose();
        shieldCDTimer.Dispose();
    }
}
