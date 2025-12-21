using UnityEngine;

namespace Weapons
{
    public class BeamWeapon : WeaponBase
    {
        [SerializeField] Material material;
        [SerializeField] float lineThickness = .1f;
        [SerializeField] bool arc = true;
        protected override void Awake()
        {
            base.Awake();
            takeDamage = new TakeDamage(Damage, transform, damageType);
        }
        protected override void ActuallyShoot(Unit target)
        {
            var beam = WeaponManager.Instance.SpawnObject(WeaponType.Beam, transform.position) as Beam;
            beam.LineRenderer.material = material;
            beam.LineRenderer.startWidth = beam.LineRenderer.endWidth = lineThickness;
            beam.LineRenderer.SetPosition(0, transform.position);
            if (arc)
            {
                float dist = Vector3.Distance(transform.position, target.Transform.position);
                beam.LineRenderer.positionCount = 3;
                var midPoint = (transform.position + target.Position) / 2;
                beam.LineRenderer.SetPosition(1, midPoint + Random.onUnitSphere * (dist / 10));
                beam.LineRenderer.SetPosition(2, target.Transform.position);
            }
            else
            {
                beam.LineRenderer.positionCount = 2;
                beam.LineRenderer.SetPosition(1, target.Transform.position);
            }
            target.TakeDamage(takeDamage);
        }
    }
}