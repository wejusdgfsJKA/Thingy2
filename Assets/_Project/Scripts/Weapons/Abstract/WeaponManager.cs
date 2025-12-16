using Spawning.Pooling;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace Weapons
{
    public class WeaponManager : MultiManager<RandomShit>
    {
        static readonly Dictionary<RandomShit, WeaponData> assets = new();
        static readonly string weaponLabel = "Weapon";
        public static WeaponManager Instance { get; protected set; }
        private void Awake()
        {
            Instance = this;
        }
        static void LoadAsset(WeaponData data)
        {
            assets.Add(data.ID, data);
        }
        public async static Task LoadAssets()
        {
            if (assets.Count > 0) return;
            await Addressables.LoadAssetsAsync<WeaponData>(weaponLabel, LoadAsset).Task;
            Debug.Log("Weapons loaded successfully.");
        }
        public IDPoolable<RandomShit> SpawnObject(RandomShit id, Vector3 spawnPoint)
        {
            return (IDPoolable<RandomShit>)Spawn(assets[id], spawnPoint);
        }
    }
}