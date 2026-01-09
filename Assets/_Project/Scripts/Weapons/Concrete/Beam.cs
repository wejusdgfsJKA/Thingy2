using Global;
using Spawning.Pooling;
using Timers;
using UnityEngine;
namespace Weapons
{
    public enum WeaponType
    {
        Beam,
        Torpedo
    }
    [RequireComponent(typeof(LineRenderer))]
    public class Beam : IDPoolable<WeaponType>
    {
        [field: SerializeField] public LineRenderer LineRenderer { get; protected set; }
        readonly CountdownTimer timer = new(GlobalSettings.BeamRenderTime);
        private void Awake()
        {
            timer.OnTimerStop += () => gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            timer.Start();
        }
        private void OnDestroy()
        {
            timer.Dispose();
        }
    }
}