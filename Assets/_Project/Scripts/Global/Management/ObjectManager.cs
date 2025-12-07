using Player;
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
    #region Fields
    static readonly Dictionary<ObjectType, ObjectData> assets = new();
    static readonly List<string> addresses = new()
    {
        "Player",
        "FriendStation",
        "Enemy"
    };
    public static ObjectManager Instance { get; protected set; }
    #region Technical
    WaitForSeconds aiWait;
    readonly List<Coroutine> coroutines = new();
    #endregion
    #endregion
    #region Setup
    public async static Task LoadAssets()
    {
        if (assets.Count > 0) return;
        Task<ObjectData>[] tasks = new Task<ObjectData>[addresses.Count];
        for (int i = 0; i < addresses.Count; i++)
        {
            tasks[i] = Addressables.LoadAssetAsync<ObjectData>(addresses[i]).Task;
        }
        await Task.WhenAll(tasks);
        for (int i = 0; i < tasks.Length; i++)
        {
            var result = tasks[i].Result;
            assets.Add(result.ID, result);
        }
        Debug.Log("Assets loaded successfully.");
    }
    private void Awake()
    {
        aiWait = new WaitForSeconds(GlobalSettings.AITickCooldown);
        Instance = this;
    }
    private void OnEnable()
    {
        coroutines.Add(StartCoroutine(AICoroutine()));
    }
    private void OnDisable()
    {
        for (int i = 0; i < coroutines.Count; i++) if (coroutines[i] != null) StopCoroutine(coroutines[i]);
    }
    #endregion
    #region AI
    IEnumerator AICoroutine()
    {
        yield return new WaitUntil(() => GameManager.Teams[0] != null && GameManager.Teams[1] != null);
        while (true)
        {
            yield return aiWait;
            GameManager.Teams[0]?.Tick(GlobalSettings.AITickCooldown);
            GameManager.Teams[1]?.Tick(GlobalSettings.AITickCooldown);
        }
    }
    #endregion
    #region Object updates

    #endregion
    #region Spawning
    public Object SpawnShip(ObjectType type, Teams team, Vector3 position, Quaternion rotation)
    {
        if (!assets.TryGetValue(type, out var data))
        {
            Debug.LogError($"No asset found for object type {type}.");
            return null;
        }
        Unit ship = Spawn(data, position, rotation) as Unit;
        if (team == Teams.Player)
        {
            GameManager.Teams[0].AddMember(ship);
        }
        else if (team == Teams.Enemy)
        {
            GameManager.Teams[1].AddMember(ship);
        }
        return ship;
    }
    public Object SpawnShip(ObjectType type, Teams team, Vector3 position)
    {
        return SpawnShip(type, team, position, Quaternion.identity);
    }
    public PlayerShip SpawnPlayer()
    {
        return (PlayerShip)SpawnShip(ObjectType.Player, Teams.Player, Vector3.zero);
    }
    public Object SpawnPlanet(Vector3 position)
    {
        return (Object)Spawn(assets[ObjectType.Planet], position);
    }
    #endregion
}