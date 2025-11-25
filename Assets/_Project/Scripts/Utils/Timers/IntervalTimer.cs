using System;
using UnityEngine;

namespace Timers
{
    public class IntervalTimer : Timer
    {
        readonly float interval;
        float lastInterval;

        public Action OnInterval;

        public IntervalTimer(float totalTime, float intervalSeconds) : base(totalTime)
        {
            interval = intervalSeconds;
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime <= initialTime)
            {
                CurrentTime += Time.deltaTime;

                while (CurrentTime - lastInterval >= interval)
                {
                    lastInterval += interval;
                    OnInterval?.Invoke();
                }
            }

            if (IsRunning && CurrentTime <= 0)
            {
                CurrentTime = 0;
                Stop();
            }
        }
        public override void Reset() => CurrentTime = 0;
        public override bool IsFinished => CurrentTime >= initialTime;
    }
}