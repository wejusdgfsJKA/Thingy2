using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Global
{
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
        #region Save file stuff
        public static string GetSaveFilePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }
        public static string GetSaveFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "save.dat");
        }
        #endregion
        public static readonly float PlayerWinThreshold = 50, PlayerLoseThreshold = -50;
        static readonly Dictionary<(DamageType, TargetType), float> modifiers = new()
        {
            { (DamageType.Energy,TargetType.Hull),1.5f },
            { (DamageType.Kinetic,TargetType.Shield),1.75f }
        };
        public static float GetDamageModifier(DamageType damage, TargetType target)
        {
            return modifiers.GetValueOrDefault((damage, target), 1);
        }
        static readonly Dictionary<ObjectType, float> weights = new() { };
        /// <summary>
        /// Get the weight for the given ObjectType in mission score.
        /// </summary>
        /// <param name="type">The type of SpecialObject to determine the weight for.</param>
        /// <returns>Weight for the given ObjectType. Default 1.</returns>
        public static float GetWeight(ObjectType type)
        {
            return weights.GetValueOrDefault(type, 1);
        }
        public static readonly string MainSceneAddress = "MainScene", IntermediateSceneAddress = "Intermediate", EndSceneAddress = "End";
        public static readonly float AITickCooldown = 0.05f, DamageRenderTime = 0.2f, UIUpdateCooldown = 1f, BeamRenderTime = 0.2f;
        public static readonly int MaxTargetDistance = 1000;
    }
}