using UnityEngine;
namespace Utilities
{
    public enum Directions
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
            return component;
        }
        public static Directions GetDirection(this GameObject gameObject, Vector3 point)
        {
            Vector3 localDirection = gameObject.transform.InverseTransformDirection(point - gameObject.transform.position);
            if (Mathf.Abs(localDirection.x) > Mathf.Abs(localDirection.y) && Mathf.Abs(localDirection.x) > Mathf.Abs(localDirection.z))
            {
                return localDirection.x > 0 ? Directions.Right : Directions.Left;
            }
            else if (Mathf.Abs(localDirection.y) > Mathf.Abs(localDirection.x) && Mathf.Abs(localDirection.y) > Mathf.Abs(localDirection.z))
            {
                return localDirection.y > 0 ? Directions.Up : Directions.Down;
            }
            else
            {
                return localDirection.z > 0 ? Directions.Forward : Directions.Backward;
            }
        }
    }
}