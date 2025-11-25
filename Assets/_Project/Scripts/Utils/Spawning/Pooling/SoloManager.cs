using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spawning.Pooling
{
    /// <summary>
    /// A manager which can only handle one type of object.
    /// </summary>
    public class SoloManager : Manager
    {
        public int ActiveEntityCount { get; protected set; } = 0;
        /// <summary>
        /// Holds all the pooled objects.
        /// </summary>
        protected Stack<Poolable> pool = new();
        public override Spawnable Spawn(SpawnableData objectData, Vector3 position, Quaternion rotation, Action<Spawnable> executeBeforeSpawn = null)
        {
            var s = base.Spawn(objectData, position, rotation, executeBeforeSpawn);
            if (s) ActiveEntityCount++;
            return s;
        }
        public override void ReturnToPool(Poolable poolable)
        {
            if (poolable)
            {
                ActiveEntityCount--;
                pool.Push(poolable);
            }
            else Debug.LogError($"Solo manager {this} was supplied a null poolable.");
        }
        protected override Poolable GetFromPool(SpawnableData objectData)
        {
            if (pool.TryPop(out Poolable result))
            {
                return result;
            }
            return null;
        }
    }
}