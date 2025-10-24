using UnityEngine;
namespace Spawning
{
    [CreateAssetMenu(menuName ="ScriptableObjects/SpawnableData")]
    public class SpawnableData : ScriptableObject
    {
        public Spawnable Prefab;
    }
}