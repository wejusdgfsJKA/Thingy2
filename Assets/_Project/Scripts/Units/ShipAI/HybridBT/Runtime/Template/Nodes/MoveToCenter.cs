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
        protected override Node<ShipAIKeys> GetNode(Context<ShipAIKeys> context)
        {
            var navigation = context.Navigation;
            return new LeafNode<ShipAIKeys>("Move to point", onEvaluate, onEnter, () =>
            {
                navigation.Stop();
            });
        }
    }
}
