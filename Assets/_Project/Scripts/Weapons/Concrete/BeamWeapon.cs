using HP;
using UnityEngine;

namespace Weapons
{
    public class BeamWeapon : WeaponBase
    {
        [SerializeField] Material material;
        [SerializeField] float lineThickness = .1f;
        protected override void Awake()
        {
            base.Awake();
            takeDamage = new TakeDamage(Damage, transform, damageType);
        }
        protected override void ActuallyShoot(Object target)
        {
            var beam = RandomShitManager.Instance.SpawnObject(RandomShit.Beam, transform.position) as Beam;
            beam.LineRenderer.material = material;
            beam.LineRenderer.startWidth = beam.LineRenderer.endWidth = lineThickness;
            beam.LineRenderer.SetPosition(0, transform.position);
            beam.LineRenderer.SetPosition(1, target.Transform.position);
            target.TakeDamage(takeDamage);
        }
    }
}