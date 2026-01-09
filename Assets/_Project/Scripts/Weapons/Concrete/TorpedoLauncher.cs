using UnityEngine;

namespace Weapons
{
    public class TorpedoLauncher : WeaponBase
    {
        [SerializeField] float speed = 10f;
        [SerializeField] float trackingSpeed = 5f;
        [SerializeField] float lifeTime = 20f;
        protected override void ActuallyShoot(Unit target)
        {
            var torpedo = WeaponManager.Instance.SpawnObject(WeaponType.Torpedo, transform.position, transform.rotation) as PlasmaTorpedo;
            torpedo.Initialize(target, takeDamage, lifeTime, speed, trackingSpeed);
        }
    }
}