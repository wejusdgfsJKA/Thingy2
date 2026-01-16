using System;
using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/MoveToPlayer")]
    public class MoveToPlayer : LeafNodeData<ShipAIKeys>
    {
        [Tooltip("If true, if no player is found the ship will move to the center of the map.")] public bool MoveToCenterIfFail = true;
        protected override Func<Context<ShipAIKeys>, NodeState> onEvaluate => (ctx) =>
        {
            var player = GameManager.Player;
            var targetPos = Vector3.zero;
            if (player == null)
            {
                if (!MoveToCenterIfFail) return NodeState.FAILURE;
            }
            else
            {
                targetPos = player.Position;
            }
            ctx.Navigation.Destination = targetPos;
            return NodeState.RUNNING;
        };
        protected override Action<Context<ShipAIKeys>> onExit => (ctx) =>
        {
            ctx.Navigation.Stop();
        };
    }
}
