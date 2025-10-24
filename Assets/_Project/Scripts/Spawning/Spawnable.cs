using UnityEngine;
using UnityEngine.Events;
namespace Spawning
{
    public class Spawnable : MonoBehaviour
    {
        [Header("On Init")]//these exist bc unity has a bug with protected serialized fields
        [SerializeField] protected UnityEvent init;
        public virtual void Initialize(SpawnableData data)
        {
            init?.Invoke();
        }
    }
}