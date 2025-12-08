using UnityEngine;
namespace Utilities
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            return component;
        }
    }
}