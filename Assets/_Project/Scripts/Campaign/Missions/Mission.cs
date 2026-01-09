using System.Collections.Generic;
using UnityEngine;
public abstract class Mission
{
    public float Score { get; protected set; }
    public abstract void Initialize();
    public abstract float GetScore();
    protected void EnemyDestroyed(Unit @object)
    {
        Score += Global.GlobalSettings.GetWeight(@object.ID);
    }
    protected void AllyDestroyed(Unit @object)
    {
        Score -= Global.GlobalSettings.GetWeight(@object.ID);
    }
    public Unit SpawnEnemy(ObjectType type, Vector3 position)
    {
        var enemy = UnitManager.Instance.SpawnShip(type, Teams.Enemy, position);
        enemy.OnDespawn += (o) =>
        {
            EnemyDestroyed(o);
        };
        return enemy;
    }
}
public class FleetBattleMission : Mission
{
    int enemyCount;
    int enemyPoints, alliedPoints;
    public FleetBattleMission(int enemyPoints = 1, int alliedPoints = 0)
    {
        this.enemyPoints = enemyPoints;
        this.alliedPoints = alliedPoints;
    }
    public override void Initialize()
    {
        UnitManager.Instance.SpawnPlayer();
        SpawnEnemies();
    }
    void SpawnEnemies()
    {
        //pick a position for the bad guys
        var enemySpawnPos = Random.onUnitSphere * 50;
        var spawnRadius = enemyPoints;
        var enemyOptions = new List<ObjectType>() { ObjectType.Enemy1, ObjectType.Enemy2 };
        int retries = 0;
        while (enemyPoints > 0 && retries < Global.GlobalSettings.MaxSpawnRetries)
        {
            var chosenEnemyType = enemyOptions[Random.Range(0, enemyOptions.Count)];
            int weight = Global.GlobalSettings.GetWeight(chosenEnemyType);
            if (weight > enemyPoints)
            {
                retries++;
                continue;
            }
            retries = 0;
            SpawnEnemy(chosenEnemyType, enemySpawnPos + spawnRadius * Random.onUnitSphere).OnDespawn += (o) =>
            {
                SubtractEnemy();
            };
            enemyPoints -= weight;
        }
    }
    void SubtractEnemy()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            GameManager.EndMission();
        }
    }
    public override float GetScore()
    {
        float score = Score;
        if (GameManager.Player == null) score -= Global.GlobalSettings.GetWeight(ObjectType.Player);
        return score;
    }
}