using HP;
using Timers;
using UnityEngine;

public class ShieldComponent : MonoBehaviour
{
    [field: SerializeField] public float MaxShieldPoints { get; protected set; }
    protected float shieldPoints;
    public float ShieldPoints
    {
        get => shieldPoints;
        protected set => shieldPoints = Mathf.Clamp(value, 0, MaxShieldPoints);
    }
    [field: SerializeField] public float ShieldRegen { get; protected set; }
    [field: SerializeField] public float ShieldRegenCD { get; protected set; }
    [field: SerializeField] public float ShieldRegenInterval { get; protected set; }
    protected IntervalTimer regenTimer;
    protected CountdownTimer shieldCDTimer;
    protected virtual void Awake()
    {
        regenTimer = new IntervalTimer(ShieldRegen, ShieldRegenInterval);
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
        ShieldPoints -= takeDamage.Damage * GlobalSettings.GetModifier(takeDamage.DamageType, TargetType.Shield);
        shieldCDTimer.Start();
    }
    private void OnDestroy()
    {
        regenTimer.Dispose();
        shieldCDTimer.Dispose();
    }
}
