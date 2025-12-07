using EventBus;

public struct SpecialObjectAdded : IEvent
{
    public Object @object;
    public SpecialObjectAdded(Object planet) => @object = planet;
}
