using HybridBT;
using System;
using UnityEngine;
namespace Sample
{
    [CreateAssetMenu(menuName = "Sample/HybridBT/DebugNode")]
    public class DebugNodeData : LeafNodeData<TestBTKeys>
    {
        public string Text;
        protected override Func<Context<TestBTKeys>, NodeState> onEvaluate => (context) =>
                {
                    Debug.Log(Text);
                    return NodeState.RUNNING;
                };
    }
}