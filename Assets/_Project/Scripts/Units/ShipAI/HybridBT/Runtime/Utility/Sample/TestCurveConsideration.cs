using HybridBT;
using UnityEngine;
namespace Sample
{
    [CreateAssetMenu(menuName = "Sample/HybridBT/CurveConsideration")]
    public class TestCurveConsideration : CurveConsideration<TestBTKeys>
    {
        public float MaxDistance = 20;
        protected override float GetValueForCurve(Context<TestBTKeys> context)
        {
            var tr = context.GetData<Transform>(TestBTKeys.Goober);
            return tr == null ? 0 : Mathf.InverseLerp(0, MaxDistance,
                Vector3.Distance(tr.position, context.GetData<Transform>(TestBTKeys.Self).position));
        }
    }
}