using Player;
using Spawning;
using Spawning.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Teams : byte
{
    Player,
    Enemy
}
public class ObjectManager : MultiManager<ObjectType>
{
    [Tooltip("Min. nr. of asteroids to be active at any given time.")][SerializeField] int asteroidIntendedCount = 2;
    public HashSet<Object> Objects { get; } = new();
    [SerializeField] ObjectData asteroidData;
    [SerializeField] ObjectData planetData;
    [SerializeField] SpawnableData playerData;
    [SerializeField] ObjectData friend1;
    public static ObjectManager Instance { get; protected set; }
    public readonly Team PlayerTeam = new(), EnemyTeam = new();
    #region Technical
    readonly float checkInterval = .2f;
    WaitForSeconds wait;
    readonly Stack<Object> toRemove = new();
    Coroutine coroutine;
    #endregion
    #region Setup
    private void Awake()
    {
        wait = new WaitForSeconds(checkInterval);
        Instance = this;
        var objectTypes = (ObjectType[])ObjectType.GetValues(typeof(ObjectType));
        foreach (var objectType in objectTypes)
        {
            ActiveEntityCounts[objectType] = 0;
        }
    }
    private void OnEnable()
    {
        coroutine = StartCoroutine(UpdateCoroutine());
    }
    private void OnDisable()
    {
        if (coroutine != null) StopCoroutine(coroutine);
    }
    #endregion
    #region Object updates
    Asteroid SpawnAsteroid()
    {
        //pick a position
        float dist = GameManager.Player.ScanRange + 1; //+ (GlobalSettings.UpdateRange + GlobalSettings.PlayerTrackingRange) / 2;
        Vector3 pos = GameManager.Player.Transform.position + Random.onUnitSphere * dist;
        //pick an inertia
        float inertiaMagnitude = Random.Range(GlobalSettings.AsteroidInertia.Item1,
            GlobalSettings.AsteroidInertia.Item2);
        Vector3 inertia = Random.onUnitSphere * inertiaMagnitude;
        var asteroid = Spawn(asteroidData, pos) as Asteroid;
        if (asteroid == null)
        {
            Debug.LogError($"Unable to convert Spawnable to InertTrackable when attempting to spawn asteroid at {System.DateTime.Now}.");
            return null;
        }
        asteroid.Inertia = inertia;

        ActiveEntityCounts[ObjectType.Asteroid]++;

        return asteroid;
    }
    void DeactivateObject(Object obj)
    {
        obj.gameObject.SetActive(false);
        ActiveEntityCounts[obj.ID]--;
        Objects.Remove(obj);
    }
    public void UpdateExisting()
    {
        foreach (var obj in Objects)
        {
            if (obj.Persistent) continue;

            var dist = Mathf.Max(Vector3.Distance(GameManager.Player.Transform.position,
                obj.Transform.position) - obj.Signature, 0.1f);

            if (dist > GlobalSettings.UpdateRange)
            {
                //remove this object
                toRemove.Push(obj);
            }
        }
        while (toRemove.Count > 0) DeactivateObject(toRemove.Pop());
    }
    public void HandleSpawning()
    {
        ActiveEntityCounts.TryGetValue(ObjectType.Asteroid, out int count);
        count = asteroidIntendedCount - count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++) SpawnAsteroid();
        }
    }
    public IEnumerator UpdateCoroutine()
    {
        yield return new WaitUntil(() => GameManager.Player != null);
        while (true)
        {
            yield return wait;
            UpdateExisting();
            HandleSpawning();
            PlayerTeam.Update();
            EnemyTeam.Update();
        }
    }
    #endregion
    public void SpawnShip(ObjectType type, Teams team)
    {
        Ship ship = null;
        if (team == Teams.Player)
        {
            PlayerTeam.AddMember(ship);
        }
        else if (team == Teams.Enemy)
        {
            EnemyTeam.AddMember(ship);
        }
    }
    public void SpawnPlayer()
    {
        //var s = Instantiate(playerData.Prefab, Vector3.zero, Quaternion.identity);
        //s.Initialize(playerData);
        PlayerTeam.AddMember(Spawn(playerData, Vector3.zero) as PlayerShip);
    }
    public void SpawnPlanet(Vector3 position)
    {
        //var s = Instantiate(planetData.Prefab, position, Quaternion.identity);
        //s.Initialize(planetData);
        Spawn(planetData, position);
    }
}
