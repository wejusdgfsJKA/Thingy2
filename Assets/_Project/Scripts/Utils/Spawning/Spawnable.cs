using UnityEngine;
namespace Spawning
{
    /// <summary>
    /// Represents something that can be spawned.
    /// </summary>
    public class Spawnable : MonoBehaviour
    {
        public virtual void Initialize(SpawnableData objectData)
        {

        }
    }
}