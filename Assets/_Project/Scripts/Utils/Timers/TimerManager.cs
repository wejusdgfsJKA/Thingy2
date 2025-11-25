namespace Timers
{
    internal static class TimerManager
    {
        static event System.Action updateTimers;
        public static void UpdateTimers() => updateTimers?.Invoke();
        public static void RegisterTimer(Timer timer) => updateTimers += timer.Tick;
        public static void DeregisterTimer(Timer timer) => updateTimers -= timer.Tick;
        public static void Clear() => updateTimers = null;
    }
}