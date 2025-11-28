using UnityEngine;
public abstract class Mission
{
    public abstract void Initialize();
    public abstract float GetScore();
}
public class PlanetDefenseMission : Mission
{
    int initialPlanetCount;
    int enemyShipCount;
    public PlanetDefenseMission(int planetCount = 1, int enemyShipCount = 1)
    {
        initialPlanetCount = planetCount;
        this.enemyShipCount = enemyShipCount;
    }
    public override void Initialize()
    {
        ObjectManager.Instance.SpawnPlayer();
        for (int i = 0; i < initialPlanetCount; i++)
        {
            var pos = Random.onUnitSphere * 10;
            ObjectManager.Instance.SpawnPlanet(pos).OnDespawn += (p) => SubtractPlanet();
            pos += Random.onUnitSphere * 2;
            ObjectManager.Instance.SpawnShip(ObjectType.FriendStation, Teams.Player, pos);
        }

        //pick a position for the bad guys
        var enemySpawnPos = Random.onUnitSphere * 100;
        //spawn enemy ships
        for (int i = 0; i < enemyShipCount; i++)
        {
            var enemy = ObjectManager.Instance.SpawnShip(ObjectType.Enemy1, Teams.Enemy, enemySpawnPos + Random.onUnitSphere * 1);
            enemy.OnDespawn += (o) =>
            {
                SubtractEnemy();
            };
        }
    }
    void SubtractEnemy()
    {
        enemyShipCount--;
        if (enemyShipCount <= 0)
        {
            GameManager.EndMission();
        }
    }
    void SubtractPlanet()
    {
        initialPlanetCount--;
        if (initialPlanetCount <= 0)
        {
            GameManager.EndMission();
        }
    }
    public override float GetScore()
    {
        var score = (float)initialPlanetCount;
        if (GameManager.Player == null) score -= 0.5f;
        return score;
    }
}