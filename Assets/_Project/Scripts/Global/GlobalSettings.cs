using System.Collections.Generic;

public enum DamageType
{
    Kinetic,
    Energy
}
public enum TargetType
{
    Hull,
    Shield
}
public static class GlobalSettings
{
    static readonly Dictionary<(DamageType, TargetType), float> modifiers = new()
    {
        { (DamageType.Energy,TargetType.Hull),1.5f },
        { (DamageType.Kinetic,TargetType.Shield),1.75f }
    };
    static readonly Dictionary<ObjectType, float> weights = new() { };
    public static float GetDamageModifier(DamageType damage, TargetType target)
    {
        return modifiers.GetValueOrDefault((damage, target), 1);
    }
    /// <summary>
    /// Get the weight for the given ObjectType in mission score.
    /// </summary>
    /// <param name="type">The type of object to determine the weight for.</param>
    /// <returns>Weight for the given ObjectType. Default 1.</returns>
    public static float GetWeight(ObjectType type)
    {
        return weights.GetValueOrDefault(type, 1);
    }
    public static readonly string MainSceneAddress = "MainScene", IntermediateSceneAddress = "Intermediate";
    public static float AITickCooldown = 0.05f;
}
