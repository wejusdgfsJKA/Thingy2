using UnityEngine;
using UnityEngine.Events;
namespace Spawning.Pooling
{
    public class Poolable : Spawnable
    {
        [Header("On Reset")]//these exist bc unity has a bug with protected serialized fields
        [SerializeField] protected UnityEvent reset;
        public Manager Manager { get; set; }
        public void ResetObject()
        {
            reset?.Invoke();
        }
        protected virtual void OnDisable()
        {
            Manager?.ReturnToPool(this);
        }
    }
}