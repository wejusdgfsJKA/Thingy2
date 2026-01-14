using UnityEngine;
namespace HybridBT.Template
{
    [CreateAssetMenu(menuName = "HybridBT/Template/CurveConsideration")]
    public class CurveConsideration : CurveConsideration<BTKeys>
    {
        [SerializeField] protected BTKeys valueKey;
        protected override float GetValueForCurve(Context<BTKeys> context)
        {
            return context.GetData<float>(valueKey);
        }
    }
}