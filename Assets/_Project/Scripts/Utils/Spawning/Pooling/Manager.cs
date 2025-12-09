
using UnityEngine;

namespace Spawning.Pooling
{
    /// <summary>
    /// A spawner with a built-in pool.
    /// </summary>
    public abstract class Manager : Spawner
    {
        /// <summary>
        /// Return an SpecialObject to this manager's pool.
        /// </summary>
        /// <param name="poolable">Object to return to the pool.</param>
        public abstract void ReturnToPool(Poolable poolable);
        /// <summary>
        /// With a manager, we first try to get an available SpecialObject from the pool. If that fails, 
        /// fallback to base version.
        /// </summary>
        /// <param name="objectData">Data which willl be used to either retrieve an existing SpecialObject from
        /// the pool or create a new one.</param>
        /// <returns>The SpecialObject.</returns>
        protected override Spawnable Obtain(SpawnableData objectData)
        {
            var s = GetFromPool(objectData);
            if (s != null)
            {
                s.ResetObject();
                return s;
            }
            var s2 = base.Obtain(objectData);
            s = s2 as Poolable;
            if (s) s.Manager = this;
            else Debug.LogError($"Manager {this} received spawnable data {objectData} which it could not convert to a poolable.");
            return s2;
        }
        /// <summary>
        /// Obtain an SpecialObject from the pool based on its data.
        /// </summary>
        /// <param name="objectData">The data which will be used to get an SpecialObject from the pool.</param>
        /// <returns>An available SpecialObject, or null if none is found.</returns>
        protected abstract Poolable GetFromPool(SpawnableData objectData);
    }
}