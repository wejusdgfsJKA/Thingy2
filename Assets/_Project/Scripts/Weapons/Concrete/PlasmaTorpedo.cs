using Global;
using Spawning.Pooling;
using UnityEngine;
namespace Weapons
{
    public class PlasmaTorpedo : IDPoolable<WeaponType>
    {
        TakeDamage damage;
        Unit target;
        float speed, trackingSpeed, lifeTime;
        public void Initialize(Unit target, TakeDamage damage, float lifeTime, float speed, float trackingSpeed)
        {
            this.damage = damage;
            this.target = target;
            this.lifeTime = lifeTime;
            this.speed = speed;
            this.trackingSpeed = trackingSpeed;
        }
        void Update()
        {
            if (target == null || lifeTime <= 0)
            {
                target = null;
                gameObject.SetActive(false);
                return;
            }

            if (Vector3.Distance(transform.position, target.Position) <= GlobalSettings.TorpedoHitRange)
            {
                target.TakeDamage(damage);
                target = null;
                gameObject.SetActive(false);
                return;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(
                target.Position - transform.position), trackingSpeed * Time.deltaTime);
            lifeTime -= Time.deltaTime;
            transform.position += speed * Time.deltaTime * transform.forward;
        }
    }
}