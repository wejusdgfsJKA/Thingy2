namespace Resources
{
    public class StorageCell
    {
        public readonly int MaxAmount;
        public int Amount { get; set; }
        public int CapacityLeft
        {
            get => MaxAmount - Amount;
        }
        public StorageCell(int maxAmount, int amount = 0)
        {
            MaxAmount = maxAmount;
            Amount = amount;
        }
    }
}