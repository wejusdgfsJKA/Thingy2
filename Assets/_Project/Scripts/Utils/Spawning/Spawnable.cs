using UnityEngine;
namespace Spawning
{
    /// <summary>
    /// Represents something that can be spawned. Has a Initialize method.
    /// </summary>
    public class Spawnable : MonoBehaviour
    {
        /// <summary>
        /// Fires right after instantiation.
        /// </summary>
        /// <param name="data">Data to initialize with.</param>
        public virtual void Initialize(SpawnableData data)
        {

        }
    }
}