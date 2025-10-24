namespace Environment
{
    public enum Resource : byte
    {
        Metal,
        Fuel
    }
    [System.Serializable]
    public struct ResourceValue
    {
        public float Probability;
        public Resource Resource;
        public int Min, Max;
    }
}