using System;
using System.Collections.Generic;
using UnityEngine;
namespace Spawning.Pooling
{
    /// <summary>
    /// A manager which can handle different types of objects.
    /// </summary>
    /// <typeparam name="ID">The type of the variable which will be used to categorize objects.</typeparam>
    public class MultiManager<ID> : Manager
    {
        /// <summary>
        /// Holds all the pooled objects.
        /// </summary>
        protected Dictionary<ID, Stack<IDPoolable<ID>>> multiPool = new();
        public Dictionary<ID, int> ActiveEntityCounts = new();
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
                if (ActiveEntityCounts.TryGetValue(p.ID, out int count))
                {
                    if (count > 0) ActiveEntityCounts[p.ID]--;
                }
                pool.Push(p);
            }
            else
            {
                Debug.LogError($"{this} received invalid IDPoolable {poolable}!");
            }
        }
        public override Spawnable Spawn(SpawnableData objectData, Vector3 position, Quaternion rotation, Action<Spawnable> executeBeforeSpawn = null)
        {
            var s = base.Spawn(objectData, position, rotation, executeBeforeSpawn);
            if (s)
            {
                var p = objectData as IDPoolableData<ID>;
                if (p != null)
                {
                    var id = p.ID;
                    if (!ActiveEntityCounts.ContainsKey(id)) ActiveEntityCounts[id] = 0;
                    ActiveEntityCounts[id]++;
                }
                else Debug.LogWarning($"MultiManager {this} received object data {objectData} which it could not convert to IDPoolable.");
            }
            return s;
        }
        protected override Poolable GetFromPool(SpawnableData objectData)
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