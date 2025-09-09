using UnityEngine;
namespace HP.Weakpoints
{
    public class WeakpointHPComponent : HPComponent
    {
        [SerializeField] protected Weakpoint[] weakpoints;
        protected override float CalculateDamage(TakeDamage dmg)
        {
            float modifier = 1;
            if (dmg.ColliderID != null)
            {
                for (int i = 0; i < weakpoints.Length; i++)
                {
                    if (weakpoints[i].Collider.GetInstanceID() == dmg.ColliderID)
                    {
                        modifier = weakpoints[i].Modifier;
                        break;
                    }
                }
            }
            return modifier * base.CalculateDamage(dmg);
        }
    }
}