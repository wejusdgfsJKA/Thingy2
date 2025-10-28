using Environment;
using System.Collections.Generic;

public static class GlobalSettings
{
    public static readonly int UpdateRange = 100;
    public static readonly Dictionary<Resource, string> ResourceColors = new() {
        { Resource.Metal, "green" },{ Resource.Fuel, "orange" }};
}
