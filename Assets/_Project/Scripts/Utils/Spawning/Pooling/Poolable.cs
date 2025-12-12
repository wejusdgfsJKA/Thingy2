using UnityEngine;
namespace Spawning.Pooling
{
    public class Poolable : Spawnable
    {
        [Header("On Clear")]//these exist bc unity has a bug with protected serialized fields
        public Manager Manager { get; set; }
        public virtual void ResetObject()
        {

        }
        protected virtual void OnDisable()
        {
            Manager?.ReturnToPool(this);
        }
    }
}