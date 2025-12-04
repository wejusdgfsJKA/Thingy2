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
            foreach (var address in addresses)
            {
                var data = await Addressables.LoadAssetAsync<RandomShitData>(address).Task;
                assets.Add(data.ID, data);
            }
        }
        public IDPoolable<RandomShit> SpawnObject(RandomShit id, Vector3 spawnPoint)
        {
            return (IDPoolable<RandomShit>)Spawn(assets[id], spawnPoint);
        }
    }
}