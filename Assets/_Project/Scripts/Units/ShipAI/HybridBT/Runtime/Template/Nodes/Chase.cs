using Global;
using System;
using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/Chase")]
    public class Chase : LeafNodeData<ShipAIKeys>
    {
        [SerializeField] float stopDistance = 2;
        [SerializeField] float chaseErrorThreshold = 0.1f;
        protected override Func<Context<ShipAIKeys>, NodeState> onEvaluate => (ctx) =>
        {
            var prevPos = ctx.GetData<Vector3>(ShipAIKeys.PrevTargetPos);
            var currentPos = ctx.GetData<Unit>(ShipAIKeys.Target).Position;
            if (Vector3.Magnitude(prevPos - currentPos) >= chaseErrorThreshold)
            {
                ctx.Navigation.Destination = currentPos + stopDistance * UnityEngine.Random.onUnitSphere; ;
                ctx.SetData(ShipAIKeys.PrevTargetPos, currentPos);
            }
            ctx.Navigation.Rotate(Quaternion.LookRotation(currentPos - ctx.Ship.Position), GlobalSettings.AITickCooldown);
            return NodeState.RUNNING;
        };
        protected override Action<Context<ShipAIKeys>> onEnter => (ctx) =>
        {
            var obj = ctx.GetData<Unit>(ShipAIKeys.Target);
            if (obj != null)
            {
                ctx.Navigation.UpdateRotation = false;
                ctx.Navigation.Destination = obj.Position + stopDistance * UnityEngine.Random.onUnitSphere;
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