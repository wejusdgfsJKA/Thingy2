using Player;
using Spawning.Pooling;
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
    static readonly Dictionary<ObjectType, UnitData> assets = new();
    public static ObjectManager Instance { get; protected set; }
    static readonly string shipGroupLabel = "Ships";
    #endregion
    #region Setup
    public async static Task LoadAssets()
    {
        if (assets.Count > 0) return;
        await Addressables.LoadAssetsAsync<UnitData>(shipGroupLabel, (data) => assets.Add(data.ID, data)).Task;
        Debug.Log("Ships loaded successfully.");
    }
    private void Awake()
    {
        Instance = this;
    }
    #endregion
    #region Object updates

    #endregion
    #region Spawning
    public Unit SpawnShip(ObjectType type, Teams team, Vector3 position, Quaternion rotation)
    {
        if (!assets.TryGetValue(type, out var data))
        {
            Debug.LogError($"No asset found for SpecialObject type {type}.");
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
    public Unit SpawnShip(ObjectType type, Teams team, Vector3 position)
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