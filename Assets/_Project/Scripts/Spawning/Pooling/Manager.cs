namespace Spawning.Pooling
{
    public abstract class Manager : Spawner
    {
        public abstract void ReturnToPool(Poolable poolable);
        public override Spawnable Obtain(SpawnableData objectData)
        {
            var s = GetFromPool(objectData);
            if (s != null)
            {
                s.ResetObject();
            }
            else
            {
                s = (Poolable)base.Obtain(objectData);
                s.Manager = this;
            }
            return s;
        }
        public abstract Poolable GetFromPool(SpawnableData objectData);
    }
}