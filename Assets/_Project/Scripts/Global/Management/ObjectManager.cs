using Spawning;
using Spawning.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectType : byte
{
    Asteroid,
    Ship,
    Planet
}
public class ObjectManager : MultiManager<ObjectType>
{
    [Tooltip("Min. nr. of asteroids to be active at any given time.")][SerializeField] int asteroidIntendedCount = 2;
    readonly Dictionary<Transform, Trackable> toTrack = new();
    [SerializeField] TrackableData asteroidData;
    [SerializeField] TrackableData planetData;
    [SerializeField] SpawnableData playerData;
    public static ObjectManager Instance { get; protected set; }
    #region Technical
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
        Instance = this;
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
        toTrack.Add(asteroid.transform, asteroid);
        ActiveEntityCounts[ObjectType.Asteroid]++;

        return asteroid;
    }
    void DeactivateObject(Trackable trackable)
    {
        trackable.gameObject.SetActive(false);
        ActiveEntityCounts[trackable.ID]--;
        toTrack.Remove(trackable.transform);
    }
    public void UpdateExisting()
    {
        foreach (var t in toTrack)
        {
            var tr = t.Key;
            var trk = t.Value;
            var dist = Mathf.Max(Vector3.Distance(GameManager.Player.position, tr.position) - trk.Signature, 0.1f);

            if (!trk.Persistent && dist > GlobalSettings.UpdateRange)
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
        yield return new WaitUntil(() => GameManager.Player != null);
        while (true)
        {
            yield return wait;
            UpdateExisting();
            HandleSpawning();
        }
    }
    #endregion
    public void SpawnPlayer()
    {
        //var s = Instantiate(playerData.Prefab, Vector3.zero, Quaternion.identity);
        //s.Initialize(playerData);
        Spawn(playerData, Vector3.zero);
    }
    public void SpawnPlanet(Vector3 position)
    {
        //var s = Instantiate(planetData.Prefab, position, Quaternion.identity);
        //s.Initialize(planetData);
        Spawn(planetData, position);
    }
}
