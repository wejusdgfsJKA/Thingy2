using Global;
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
    public Unit SpawnAlly(ObjectType type, Vector3 position)
    {
        var ally = UnitManager.Instance.SpawnShip(type, Teams.Player, position);
        ally.OnDespawn += AllyDestroyed;
        return ally;
    }
    public Unit SpawnEnemy(ObjectType type, Vector3 position)
    {
        var enemy = UnitManager.Instance.SpawnShip(type, Teams.Enemy, position);
        enemy.OnDespawn += EnemyDestroyed;
        return enemy;
    }
    public abstract void AutoResolve();
}
public class FleetBattleMission : Mission
{
    int enemyCount;
    int allyCount;
    int enemyPoints, alliedPoints;
    public FleetBattleMission(int enemyPoints = 1, int alliedPoints = 0)
    {
        if (enemyPoints < Global.GlobalSettings.MinEnemyPoints)
        {
            enemyPoints = Global.GlobalSettings.MinEnemyPoints;
        }

        this.enemyPoints = enemyPoints;
        this.alliedPoints = alliedPoints;
    }
    public override void Initialize()
    {
        UnitManager.Instance.SpawnPlayer();
        SpawnEnemies();
        SpawnAllies();
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
            enemyCount += weight;
            SpawnEnemy(chosenEnemyType, enemySpawnPos + spawnRadius * Random.insideUnitSphere).OnDespawn += (o) =>
            {
                enemyCount -= GlobalSettings.GetWeight(o.ID);
                if (enemyCount <= 0)
                {
                    GameManager.EndMission();
                }
            };
            enemyPoints -= weight;
        }
    }

    void SpawnAllies()
    {
        var spawnRadius = alliedPoints;
        var allyOptions = new List<ObjectType>() { ObjectType.Friend1, ObjectType.Friend2 };
        int retries = 0;
        while (alliedPoints > 0 && retries < Global.GlobalSettings.MaxSpawnRetries)
        {
            var chosenAllyType = allyOptions[Random.Range(0, allyOptions.Count)];
            int weight = Global.GlobalSettings.GetWeight(chosenAllyType);
            if (weight > alliedPoints)
            {
                retries++;
                continue;
            }
            retries = 0;
            SpawnAlly(chosenAllyType, spawnRadius * Random.insideUnitSphere).OnDespawn += (a) =>
            {
                allyCount -= GlobalSettings.GetWeight(a.ID);
            };
            alliedPoints -= weight;
        }
    }
    public override float GetScore()
    {
        float score = Score;
        if (GameManager.Player == null) score -= Global.GlobalSettings.GetWeight(ObjectType.Player);
        if (score == 0) score = -2;
        return score;
    }
    public override void AutoResolve()
    {
        var score = allyCount - enemyCount;
        if (GameManager.Player == null) score -= Global.GlobalSettings.GetWeight(ObjectType.Player);
        if (score == 0) score = -2;
        Score = score;
    }
}