using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [Range(0.2f, 0.8f)] public float min, max;
    [SerializeField] int asteroidNr = 10;
    Transform player;
    public Transform Player
    {
        get { return player; }
        set
        {
            if (!player) player = value;
        }
    }
    public AsteroidManager asteroidManager;
    public int checkInterval = 10;
    WaitForSeconds wait;
    readonly Stack<Trackable> toRemove = new();
    Coroutine coroutine;
    #region Setup
    private void Awake()
    {
        wait = new WaitForSeconds(checkInterval);
    }
    private void OnEnable()
    {
        coroutine = StartCoroutine(UpdateCoroutine());
    }
    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
    #endregion
    public void SpawnAsteroid()
    {
        asteroidManager.SpawnAsteroid(player.position + Random.onUnitSphere *
            (GlobalSettings.UpdateRange - .1f));
    }
    public void RunUpdate()
    {
        foreach (var t in ComponentManager<Trackable>.dict.Values)
        {
            if (Vector3.Distance(t.transform.position, player.transform.position) > GlobalSettings.UpdateRange)
            {
                toRemove.Push(t);
            }
        }
        while (toRemove.Count > 0) toRemove.Pop().gameObject.SetActive(false);
    }
    public IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            yield return wait;
            RunUpdate();
            while (asteroidManager.ActiveEntityCount < asteroidNr)
            {
                SpawnAsteroid();
            }
        }
    }
}
