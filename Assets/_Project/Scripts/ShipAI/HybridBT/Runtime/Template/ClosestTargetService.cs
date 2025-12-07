using System;
using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/ClosestTargetService")]
    public class ClosestTargetService : ServiceData<ShipAIKeys>
    {
        public override Action<Context<ShipAIKeys>> Action => (ctx) =>
            {
                Object target = null;
                float closestTargetDist = float.PositiveInfinity;
                foreach (var unit in ctx.Ship.Team.IdentifiedTargets)
                {
                    float dist = Vector3.Distance(unit.Transform.position, ctx.Ship.transform.position);
                    if (dist < closestTargetDist)
                    {
                        closestTargetDist = dist;
                        target = unit;
                    }
                }
                if (target == null)
                {
                    foreach (var unit in ctx.Ship.Team.TrackedTargets)
                    {
                        float dist = Vector3.Distance(unit.Transform.position, ctx.Ship.transform.position);
                        if (dist < closestTargetDist)
                        {
                            closestTargetDist = dist;
                            target = unit;
                        }
                    }
                }
                ctx.SetData<Object>(ShipAIKeys.Target, target);
            };
    }
}