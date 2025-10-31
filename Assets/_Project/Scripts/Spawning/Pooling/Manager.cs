namespace Spawning.Pooling
{
    /// <summary>
    /// A spawner with a built-in pool.
    /// </summary>
    public abstract class Manager : Spawner
    {
        /// <summary>
        /// Return an object to this manager's pool.
        /// </summary>
        /// <param name="poolable">Object to return to the pool.</param>
        public abstract void ReturnToPool(Poolable poolable);
        /// <summary>
        /// With a manager, we first try to get an available object from the pool. If that fails, 
        /// fallback to base version.
        /// </summary>
        /// <param name="objectData">Data which willl be used to either retrieve an existing object from
        /// the pool or create a new one.</param>
        /// <returns>The object.</returns>
        protected override Spawnable Obtain(SpawnableData objectData)
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
        /// <summary>
        /// Obtain an object from the pool based on its data.
        /// </summary>
        /// <param name="objectData">The data which will be used to get an object from the pool.</param>
        /// <returns>An available object, or null if none is found.</returns>
        protected abstract Poolable GetFromPool(SpawnableData objectData);
    }
}