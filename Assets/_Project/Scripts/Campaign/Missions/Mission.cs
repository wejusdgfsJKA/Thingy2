using Global;
using UnityEngine;
public abstract class Mission
{
    public float EnemiesDestroyedScore { get; protected set; }
    public abstract void Initialize();
    public abstract float GetScore();
    protected void EnemyDestroyed(Unit @object)
    {
        EnemiesDestroyedScore += GlobalSettings.GetWeight(@object.ID);
    }
}
public class FleetBattleMission : Mission
{
    int enemyShipCount;
    public FleetBattleMission(int enemyShipCount = 1)
    {
        this.enemyShipCount = enemyShipCount;
    }
    public override void Initialize()
    {
        UnitManager.Instance.SpawnPlayer();

        //pick a position for the bad guys
        var enemySpawnPos = Random.onUnitSphere * 12;
        //spawn enemy ships
        for (int i = 0; i < enemyShipCount; i++)
        {
            var enemy = UnitManager.Instance.SpawnShip(ObjectType.Enemy1, Teams.Enemy, enemySpawnPos + Random.onUnitSphere * 1);
            enemy.OnDespawn += (o) =>
            {
                EnemyDestroyed(o);
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
    public override float GetScore()
    {
        float score = EnemiesDestroyedScore;
        if (GameManager.Player == null) score -= 0.5f;
        return score;
    }
}