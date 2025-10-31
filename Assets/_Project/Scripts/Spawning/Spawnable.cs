using UnityEngine;
using UnityEngine.Events;
namespace Spawning
{
    /// <summary>
    /// Represents something that can be spawned. Has a Initialize method.
    /// </summary>
    public class Spawnable : MonoBehaviour
    {
        [Header("On Init")]//these exist bc unity has a bug with protected serialized fields
        [SerializeField] protected UnityEvent init;
        /// <summary>
        /// Fires right after instantiation.
        /// </summary>
        /// <param name="data">Data to initialize with.</param>
        public virtual void Initialize(SpawnableData data)
        {
            init?.Invoke();
        }
    }
}