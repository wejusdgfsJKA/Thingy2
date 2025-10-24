using UnityEngine;
namespace Spawning.Pooling
{
    public class IDPoolable<Id> : Poolable
    {
        [field: SerializeField] public Id ID { get; protected set; }
        public override void Initialize(SpawnableData data)
        {
            base.Initialize(data);
            var d = data as IDPoolableData<Id>;
            if (d != null)
            {
                ID = d.ID;
            }
            else
            {
                Debug.LogError($"IDPoolable {this} received invalid IDPoolableData {data}!");
            }
        }
    }
}