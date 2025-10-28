using UnityEngine;

namespace Environment
{
    public enum Resource : byte //one byte should be more than enough for this thing
    {
        Metal,
        Fuel
    }
    [System.Serializable]
    public struct ResourceGen
    {
        [Tooltip("The odds that the container will have this resource.")] public float Probability;
        public Resource Resource;
        [Tooltip("Minimum amount of the resource if generated.")] public int Min;
        [Tooltip("Minimum amount of the resource if generated.")] public int Max;
    }
}