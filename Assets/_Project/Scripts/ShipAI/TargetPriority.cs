namespace Weapons
{
    [System.Serializable]
    public struct TargetPriority
    {
        public ObjectType Type;
        public float Weight;
        public TargetPriority(ObjectType type, float weight = 1)
        {
            Type = type;
            Weight = weight;
        }
    }
}