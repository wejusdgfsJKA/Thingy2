using EventBus;

public readonly struct MissionOverEvent : IEvent
{
    public readonly float Score;
    public MissionOverEvent(float score) => Score = score;
}
