using Spawning.Pooling;
using UnityEngine;
namespace Weapons
{
    [CreateAssetMenu(menuName = "ScriptableObjects/WeaponData")]
    public class WeaponData : IDPoolableData<WeaponType>
    {

    }
}