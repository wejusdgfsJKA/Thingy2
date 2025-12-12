using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/CurveConsideration")]
    public class CurveConsideration : CurveConsideration<ShipAIKeys>
    {
        [SerializeField] protected ShipAIKeys valueKey;
        protected override float GetValueForCurve(Context<ShipAIKeys> context)
        {
            return context.GetData<float>(valueKey);
        }
    }
}