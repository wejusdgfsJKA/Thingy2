using Spawning.Pooling;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace Weapons
{
    public class RandomShitManager : MultiManager<RandomShit>
    {
        static readonly Dictionary<RandomShit, RandomShitData> assets = new();
        static readonly List<string> addresses = new()
        {
            "Beam"
        };
        public static RandomShitManager Instance { get; protected set; }
        private void Awake()
        {
            Instance = this;
        }
        public async static Task LoadAssets()
        {
            if (assets.Count > 0) return;
            Task<RandomShitData>[] tasks = new Task<RandomShitData>[addresses.Count];
            for (int i = 0; i < addresses.Count; i++)
            {
                tasks[i] = Addressables.LoadAssetAsync<RandomShitData>(addresses[i]).Task;
            }
            await Task.WhenAll(tasks);
            for (int i = 0; i < tasks.Length; i++)
            {
                var result = tasks[i].Result;
                assets.Add(result.ID, result);
            }
            Debug.Log("Assets loaded successfully.");
        }
        public IDPoolable<RandomShit> SpawnObject(RandomShit id, Vector3 spawnPoint)
        {
            return (IDPoolable<RandomShit>)Spawn(assets[id], spawnPoint);
        }
    }
}