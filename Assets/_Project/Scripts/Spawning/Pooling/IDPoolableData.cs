using UnityEngine;
namespace Spawning.Pooling
{
    [CreateAssetMenu(menuName = "ScriptableObjects/IDPoolableData")]
    public class IDPoolableData<Id> : SpawnableData
    {
        public Id ID;
    }
}