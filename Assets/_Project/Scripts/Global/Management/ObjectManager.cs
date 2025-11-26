using Spawning.Pooling;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
public enum Teams : byte
{
    Player,
    Enemy
}
public class ObjectManager : MultiManager<ObjectType>
{
    [Tooltip("Min. nr. of asteroids to be active at any given time.")][SerializeField] int asteroidIntendedCount = 2;
    public HashSet<Object> Objects { get; } = new();
    static Dictionary<ObjectType, ObjectData> assets = new();
    static List<string> addresses = new()
    {
        "Asteroid",
        "Player",
        "FriendStation",
        "Planet"
    };
    public static ObjectManager Instance { get; protected set; }
    public readonly Team PlayerTeam = new(), EnemyTeam = new();
    #region Technical
    readonly float checkInterval = .2f;
    WaitForSeconds wait;
    readonly Stack<Object> toRemove = new();
    Coroutine coroutine;
    #endregion
    #region Setup
    public async static Task LoadAssets()
    {
        if (assets.Count > 0) return;
        foreach (var address in addresses)
        {
            var data = await Addressables.LoadAssetAsync<ObjectData>(address).Task;
            assets.Add(data.ID, data);
        }
        Debug.Log("assets loaded");
    }
    private async void Awake()
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
    async Task SpawnAsteroid()
    {
        //pick a position
        float dist = GameManager.Player.ScanRange + 1; //+ (GlobalSettings.UpdateRange + GlobalSettings.PlayerTrackingRange) / 2;
        Vector3 pos = GameManager.Player.Transform.position + Random.onUnitSphere * dist;
        //pick an inertia
        float inertiaMagnitude = Random.Range(GlobalSettings.AsteroidInertia.Item1,
            GlobalSettings.AsteroidInertia.Item2);
        Vector3 inertia = Random.onUnitSphere * inertiaMagnitude;
        var asteroid = Spawn(await GetObjectData(ObjectType.Asteroid), pos) as Asteroid;
        if (asteroid == null)
        {
            Debug.LogError($"Unable to convert Spawnable to InertTrackable when attempting to spawn asteroid at {System.DateTime.Now}.");
            return;
        }
        asteroid.Inertia = inertia;

        ActiveEntityCounts[ObjectType.Asteroid]++;
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
    public void HandleAsteroids()
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
            //UpdateExisting();
            //HandleAsteroids();
            PlayerTeam.Update();
            EnemyTeam.Update();
        }
    }
    #endregion
    public async Task<ObjectData> GetObjectData(ObjectType type)
    {
        if (!assets.TryGetValue(type, out var data))
        {
            data = await Addressables.LoadAssetAsync<ObjectData>(type.ToString()).Task;
            assets[type] = data;
        }
        return data;
    }
    public async Task SpawnObject(ObjectType type, Teams team, Vector3 position)
    {
        Ship ship = Spawn(await GetObjectData(type), position) as Ship;
        if (team == Teams.Player)
        {
            PlayerTeam.AddMember(ship);
        }
        else if (team == Teams.Enemy)
        {
            EnemyTeam.AddMember(ship);
        }
    }
    public async void SpawnPlayer()
    {
        SpawnObject(ObjectType.Player, Teams.Player, Vector3.zero);
    }
    public void SpawnPlanet(Vector3 position)
    {

    }
}
