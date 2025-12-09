using EventBus;

public struct SpecialObjectAdded : IEvent
{
    public Unit SpecialObject;
    public SpecialObjectAdded(Unit obj) => SpecialObject = obj;
}
