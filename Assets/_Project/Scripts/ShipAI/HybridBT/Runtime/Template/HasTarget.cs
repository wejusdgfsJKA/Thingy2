using System;
using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/HasTarget")]
    public class HasTarget : ConditionData<ShipAIKeys>
    {
        public override Func<Context<ShipAIKeys>, bool> Function => (ctx) =>
        {
            return ctx.GetData<Object>(ShipAIKeys.Target);
        };
    }
}
