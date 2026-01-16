using Global;
using System;
using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/Chase")]
    public class Chase : LeafNodeData<ShipAIKeys>
    {
        [SerializeField] float maxDistance = 10, minDistance = 0;
        protected override Func<Context<ShipAIKeys>, NodeState> onEvaluate => (ctx) =>
        {
            var target = ctx.GetData<Unit>(ShipAIKeys.Target);
            if (target == null) return NodeState.FAILURE;
            var targetPos = target.Position;
            var dist = Vector3.Distance(ctx.Ship.Position, targetPos);
            if (dist <= minDistance)
            {
                ctx.Navigation.Destination = ctx.Ship.Position + (ctx.Ship.Position - targetPos).normalized * minDistance;
            }
            else if (dist >= maxDistance)
            {
                ctx.Navigation.Destination = targetPos + maxDistance * UnityEngine.Random.onUnitSphere; ;
            }
            ctx.Navigation.Rotate(Quaternion.LookRotation(targetPos - ctx.Ship.Position), GlobalSettings.AITickCooldown);
            return NodeState.RUNNING;
        };
        protected override Action<Context<ShipAIKeys>> onEnter => (ctx) =>
        {
            var obj = ctx.GetData<Unit>(ShipAIKeys.Target);
            if (obj != null)
            {
                ctx.Navigation.UpdateRotation = false;
                ctx.Navigation.Destination = obj.Position + maxDistance * UnityEngine.Random.onUnitSphere;
                ctx.SetData(ShipAIKeys.PrevTargetPos, obj.Position);
            }
        };
        protected override Action<Context<ShipAIKeys>> onExit => (context) =>
        {
            context.Navigation.UpdateRotation = true;
            context.Navigation.Stop();
        };
    }
}