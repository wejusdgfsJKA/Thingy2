using HybridBT;
using System;
using UnityEngine;
namespace Sample
{
    [CreateAssetMenu(menuName = "Sample/HybridBT/GE_RangeCondition")]
    public class RangeConditionData : ConditionData<TestBTKeys>
    {
        public TestBTKeys RangeKey;
        public override Func<Context<TestBTKeys>, bool> Function => (c) => Vector3.Distance(
            c.GetData<Transform>(TestBTKeys.Goober).position, c.GetData<Transform>(TestBTKeys.Self).position)
        <= c.GetData<float>(RangeKey);
    }
}