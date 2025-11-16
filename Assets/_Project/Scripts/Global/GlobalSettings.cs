using Resources;
using System.Collections.Generic;

public static class GlobalSettings
{
    public static readonly int UpdateRange = 20, PlayerSpottingRange = 10, PlayerTrackingRange = 15;
    public static readonly Dictionary<Resource, string> ResourceColors = new() {
        { Resource.Metal, "green" },{ Resource.Fuel, "orange" }};
    public static (float, float) AsteroidInertia { get; } = (0, 0);
}
