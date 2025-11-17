using EventBus;
using UnityEngine;
public struct PlayerDeath : IEvent
{

}
public abstract class Mission
{
    public abstract void Initialize();
    public abstract float GetScore();
}
public class PlanetDefenseMission : Mission
{
    readonly int initialPlanetCount;
    public PlanetDefenseMission(int planetCount = 1) => initialPlanetCount = planetCount;
    public override void Initialize()
    {
        for (int i = 0; i < initialPlanetCount; i++)
        {
            ObjectManager.Instance.SpawnPlanet(Random.onUnitSphere * 10);
        }
    }
    public override float GetScore()
    {
        var score = (float)Planet.Count;
        if (GameManager.Player == null) score -= 0.5f;
        return score;
    }
}