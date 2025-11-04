using Spawning.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectType : byte
{
    Asteroid,
    Ship
}
public class GameManager : MultiManager<ObjectType>
{
    [Tooltip("Min. nr. of asteroids to be active at any given time.")][SerializeField] int asteroidIntendedCount = 2;
    readonly Dictionary<Transform, Trackable> trackables = new();
    [SerializeField] IDPoolableData<ObjectType> asteroidData;
    #region Technical
    Transform player;
    readonly float checkInterval = .2f;
    WaitForSeconds wait;
    readonly Stack<Trackable> toRemove = new();
    Coroutine coroutine;
    #endregion
    #region Setup
    private void Awake()
    {
        var objectTypes = (ObjectType[])ObjectType.GetValues(typeof(ObjectType));
        foreach (var objectType in objectTypes)
        {
            ActiveEntityCounts[objectType] = 0;
        }
        wait = new WaitForSeconds(checkInterval);
        player = transform.root;
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
    Trackable SpawnAsteroid()
    {
        //pick a position
        float dist = GlobalSettings.PlayerTrackingRange + (GlobalSettings.UpdateRange - GlobalSettings.PlayerTrackingRange) / 2;
        Vector3 pos = Random.onUnitSphere * dist;
        //pick an inertia
        float inertiaMagnitude = Random.Range(GlobalSettings.AsteroidInertia.Item1,
            GlobalSettings.AsteroidInertia.Item2);
        Vector3 inertia = Random.onUnitSphere * inertiaMagnitude;
        var asteroid = Spawn(asteroidData, pos) as InertTrackable;
        if (asteroid == null)
        {
            Debug.LogError($"Unable to convert Spawnable to InertTrackable when attempting to spawn asteroid at {System.DateTime.Now}.");
            return null;
        }
        asteroid.Inertia = inertia;
        trackables.Add(asteroid.transform, asteroid);
        ActiveEntityCounts[ObjectType.Asteroid]++;

        return asteroid;
    }
    void DeactivateObject(Trackable trackable)
    {
        trackable.gameObject.SetActive(false);
        ActiveEntityCounts[trackable.ID]--;
        trackables.Remove(trackable.transform);
    }
    public void UpdateExisting()
    {
        foreach (var t in trackables)
        {
            var tr = t.Key;
            var trk = t.Value;
            var dist = Mathf.Max(Vector3.Distance(player.position, tr.position) - trk.Signature, 0.1f);

            if (dist > GlobalSettings.UpdateRange)
            {
                //remove this object
                toRemove.Push(trk);
                continue;
            }

            if (dist <= GlobalSettings.PlayerSpottingRange)
                trk.DetectionState = DetectionState.Identified;
            else if (dist <= GlobalSettings.PlayerTrackingRange)
                trk.DetectionState = DetectionState.Tracked;
            else trk.DetectionState = DetectionState.Hidden;
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
        while (true)
        {
            yield return wait;
            UpdateExisting();
            HandleSpawning();
        }
    }
}
