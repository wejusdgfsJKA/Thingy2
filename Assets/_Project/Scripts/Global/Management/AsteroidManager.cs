using Spawning;
using Spawning.Pooling;
using UnityEngine;

public class AsteroidManager : SoloManager
{
    public SpawnableData data;
    public void SpawnAsteroid(Vector3 pos)
    {
        Spawn(data, pos);
    }
}
