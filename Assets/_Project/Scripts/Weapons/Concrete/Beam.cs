using Spawning.Pooling;
using Timers;
using UnityEngine;
namespace Weapons
{
    public enum RandomShit
    {
        Beam
    }
    [RequireComponent(typeof(LineRenderer))]
    public class Beam : IDPoolable<RandomShit>
    {
        [field: SerializeField] public LineRenderer LineRenderer { get; protected set; }
        CountdownTimer timer = new CountdownTimer(.1f);
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