using UnityEngine;
namespace Spawning
{
    public class Spawner : MonoBehaviour
    {
        public Spawnable Spawn(SpawnableData objectData, Transform spawnPoint, System.Action<Spawnable> executeBeforeSpawn = null)
        {
            return Spawn(objectData, spawnPoint.position, spawnPoint.rotation, executeBeforeSpawn);
        }
        public Spawnable Spawn(SpawnableData objectData, Vector3 position, System.Action<Spawnable> executeBeforeSpawn = null)
        {
            return Spawn(objectData, position, Quaternion.identity, executeBeforeSpawn);
        }
        public Spawnable Spawn(SpawnableData objectData, Vector3 position, Quaternion rotation, System.Action<Spawnable> executeBeforeSpawn = null)
        {
            var s = Obtain(objectData);
            executeBeforeSpawn?.Invoke(s);
            s.transform.SetPositionAndRotation(position, rotation);
            s.gameObject.SetActive(true);
            return s;
        }
        public virtual Spawnable Obtain(SpawnableData objectData)
        {
            var s = Instantiate(objectData.Prefab);
            s.Initialize(objectData);
            return s;
        }
    }
}