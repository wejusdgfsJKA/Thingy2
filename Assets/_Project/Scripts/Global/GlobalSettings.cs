using Resources;
using System.Collections.Generic;

public static class GlobalSettings
{
    public static readonly int UpdateRange = 20, PlayerSpottingRange = 5, PlayerTrackingRange = 15;
    public static readonly Dictionary<Resource, string> ResourceColors = new() {
        { Resource.Metal, "green" },{ Resource.Fuel, "orange" }};
    public static readonly (float, float) AsteroidInertia = (0, 2);
}
