using System.Collections.Generic;
using UnityEngine;
public abstract class Mission
{
    public abstract void Initialize();
    public abstract float GetScore();
}
public class PlanetDefenseMission : Mission
{
    readonly int initialPlanetCount;
    readonly int enemyShipCount = 5;
    public PlanetDefenseMission(int planetCount = 1, int enemyShipCount = 5)
    {
        initialPlanetCount = planetCount;
        this.enemyShipCount = enemyShipCount;
    }
    public override void Initialize()
    {
        List<Vector3> planetPositions = new();
        for (int i = 0; i < initialPlanetCount; i++)
        {
            var pos = Random.onUnitSphere * 10;
            ObjectManager.Instance.SpawnPlanet(pos);
            planetPositions.Add(pos);
        }
        //spawn a friendly station near each planet
        for (int i = 0; i < initialPlanetCount; i++)
        {
            var stationPos = planetPositions[i] + Random.onUnitSphere * 2;
            ObjectManager.Instance.SpawnShip(ObjectType.FriendStation, Teams.Player, stationPos);
        }
        //pick a position for the bad guys
        var enemySpawnPos = Random.onUnitSphere * 50;
        //spawn enemy ships
        for (int i = 0; i < enemyShipCount; i++)
        {
            ObjectManager.Instance.SpawnShip(ObjectType.Enemy1, Teams.Enemy, enemySpawnPos + Random.onUnitSphere * 1);
        }
    }
    public override float GetScore()
    {
        var score = (float)Planet.Count;
        if (GameManager.Player == null) score -= 0.5f;
        return score;
    }
}