using System.Collections.Generic;
using UnityEngine;
namespace Spawning.Pooling
{
    public class MultiManager<ID> : Manager
    {
        protected Dictionary<ID, Stack<IDPoolable<ID>>> multiPool = new();
        public override void ReturnToPool(Poolable poolable)
        {
            var p = poolable as IDPoolable<ID>;
            if (p != null)
            {
                if (!multiPool.TryGetValue(p.ID, out Stack<IDPoolable<ID>> pool))
                {
                    pool = new();
                    multiPool.Add(p.ID, pool);
                }
                pool.Push(p);
            }
            else
            {
                Debug.LogError($"{this} received invalid IDPoolable {poolable}!");
            }
        }
        public override Poolable GetFromPool(SpawnableData objectData)
        {
            var p = objectData as IDPoolableData<ID>;
            if (p != null)
            {
                if (multiPool.TryGetValue(p.ID, out Stack<IDPoolable<ID>> pool))
                {
                    if (pool.TryPop(out IDPoolable<ID> iDPoolable))
                    {
                        return iDPoolable;
                    }
                }
            }
            else
            {
                Debug.LogError($"{this} received invalid IDPoolableData {objectData}!");
            }
            return null;
        }
    }
}