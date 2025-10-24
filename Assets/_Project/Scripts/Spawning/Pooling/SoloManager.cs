using System.Collections.Generic;
namespace Spawning.Pooling
{
    public class SoloManager : Manager
    {
        protected Stack<Poolable> pool = new();
        public override void ReturnToPool(Poolable poolable)
        {
            pool.Push(poolable);
        }
        public override Poolable GetFromPool(SpawnableData objectData)
        {
            if (pool.TryPop(out Poolable result))
            {
                return result;
            }
            return null;
        }
    }
}