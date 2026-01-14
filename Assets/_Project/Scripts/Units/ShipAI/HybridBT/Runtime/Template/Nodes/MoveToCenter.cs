using System;
using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/MoveToCenter")]
    public class MoveToCenter : LeafNodeData<ShipAIKeys>
    {
        protected override Func<Context<ShipAIKeys>, NodeState> onEvaluate => (ctx) =>
        {
            return NodeState.RUNNING;
        };
        protected override Action<Context<ShipAIKeys>> onEnter => (ctx) =>
        {
            ctx.Navigation.Destination = Vector3.zero;
        };
        protected override Action<Context<ShipAIKeys>> onExit => (ctx) =>
        {
            ctx.Navigation.Stop();
        };
    }
}
