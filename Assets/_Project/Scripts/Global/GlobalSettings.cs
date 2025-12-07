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
        { (DamageType.Energy,TargetType.Hull),1.25f }
    };
    public static float GetModifier(DamageType damage, TargetType target)
    {
        return modifiers.GetValueOrDefault((damage, target), 1);
    }
    public static readonly string MainSceneAddress = "MainScene", IntermediateSceneAddress = "Intermediate";
    public static float AITickCooldown = 0.05f;
}
