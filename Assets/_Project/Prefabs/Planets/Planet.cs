using EventBus;
using System.Collections.Generic;
public class Planet : Object
{
    public static HashSet<Planet> Planets { get; } = new();
    protected override void OnEnable()
    {
        base.OnEnable();
        if (Planets.Add(this)) EventBus<SpecialObjectAdded>.Raise(new SpecialObjectAdded(this));
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Planets.Remove(this);
    }
}
