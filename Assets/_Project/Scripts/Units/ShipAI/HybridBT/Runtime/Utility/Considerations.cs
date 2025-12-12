using UnityEngine;

namespace HybridBT
{
    public abstract class Consideration<T> : ScriptableObject
    {
        public abstract float Evaluate(Context<T> context);
    }
    public class ConstantConsideration<T> : Consideration<T>
    {
        public float Value;
        public override float Evaluate(Context<T> context) => Value;
    }
    public abstract class CurveConsideration<T> : Consideration<T>
    {
        public AnimationCurve Curve;
        public bool ReturnZeroForInfinity = true;
        protected abstract float GetValueForCurve(Context<T> context);
        public override float Evaluate(Context<T> context)
        {
            var value = GetValueForCurve(context);
            if (ReturnZeroForInfinity && (value == Mathf.Infinity || value == Mathf.NegativeInfinity)) return 0;
            return Mathf.Clamp01(Curve.Evaluate(value));
        }
        protected virtual void Reset()
        {
            Curve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(1f, 0f)
            );
        }
    }
    public class CompositeConsideration<T> : Consideration<T>
    {
        public enum OperationType { Average, Multiply, Add, Subtract, Divide, Max, Min }
        public bool allMustBeNonZero = true;
        public OperationType operation = OperationType.Max;
        public Consideration<T>[] considerations;

        public override float Evaluate(Context<T> context)
        {
            if (considerations == null || considerations.Length == 0) return 0f;

            float result = considerations[0].Evaluate(context);
            if (result == 0f && allMustBeNonZero) return 0f;

            // Suggestion: Only 2 Considerations per RegularComposite
            for (int i = 1; i < considerations.Length; i++)
            {
                float value = considerations[i].Evaluate(context);

                if (value == 0f && allMustBeNonZero) return 0f;

                switch (operation)
                {
                    case OperationType.Average:
                        result += value;
                        break;
                    case OperationType.Multiply:
                        result *= value;
                        break;
                    case OperationType.Add:
                        result += value;
                        break;
                    case OperationType.Subtract:
                        result -= value;
                        break;
                    case OperationType.Divide:
                        result = value != 0 ? result / value : result; // Prevent division by zero
                        break;
                    case OperationType.Max:
                        result = Mathf.Max(result, value);
                        break;
                    case OperationType.Min:
                        result = Mathf.Min(result, value);
                        break;
                }
            }

            return Mathf.Clamp01(result);
        }
    }
}