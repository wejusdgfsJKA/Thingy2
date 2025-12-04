using HP;
using UnityEngine;

namespace Weapons
{
    public class BeamWeapon : WeaponBase
    {
        TakeDamage takeDamage;
        [SerializeField] Material material;
        protected override void Awake()
        {
            base.Awake();
            takeDamage = new TakeDamage(Damage, transform, DamageType.Energy);
        }
        protected override void ActuallyShoot(Object target)
        {
            var beam = RandomShitManager.Instance.SpawnObject(RandomShit.Beam, transform.position) as Beam;
            beam.LineRenderer.material = material;
            beam.LineRenderer.SetPosition(0, transform.position);
            beam.LineRenderer.SetPosition(1, target.Transform.position);
            target.TakeDamage(takeDamage);
        }
    }
}