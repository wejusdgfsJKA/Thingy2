using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public enum TargetStrategyType
    {
        ClosestTarget
    }
    public abstract class TargetStrategy
    {
        public Object CurrentTarget { get; protected set; } = null;
        protected float currentTargetScore = 0;
        protected Transform self;
        protected float maxRange, angle;
        protected Dictionary<ObjectType, float> targetPriorities = new();
        public TargetStrategy(Turret turret)
        {
            self = turret.transform;
            maxRange = turret.Range;
            angle = turret.Angle;
            for (int i = 0; i < turret.TargetPriorities.Count; i++)
            {
                var p = turret.TargetPriorities[i];
                targetPriorities.Add(p.Type, p.Weight);
            }
        }
        public void Reset()
        {
            CurrentTarget = null;
            currentTargetScore = 0;
        }
        public void ConsiderTarget(Object @object)
        {
            if (ValidTarget(@object))
            {
                if (!targetPriorities.TryGetValue(@object.ID, out var weight))
                {
                    weight = 1;
                }
                float score = ScoreTarget(@object) * weight;

                if (score > currentTargetScore)
                {
                    currentTargetScore = score;
                    CurrentTarget = @object;
                }
            }
        }
        public abstract float ScoreTarget(Object @object);
        public virtual bool ValidTarget(Object @object)
        {
            //check range
            if (Vector3.Distance(self.position, @object.Transform.position) > maxRange) return false;

            //check angle
            if (angle < 360)
            {
                Vector3 directionToTarget = (@object.Transform.position - self.position).normalized;
                float angleToTarget = Vector3.Angle(self.forward, directionToTarget);
                if (angleToTarget > angle / 2) return false;
            }
            return true;
        }
    }
    public class ClosestTargetStrategy : TargetStrategy
    {
        public ClosestTargetStrategy(Turret turret) : base(turret) { }
        public override float ScoreTarget(Object @object)
        {
            float distance = Vector3.Distance(@object.Transform.position, self.position);
            return distance <= maxRange ? maxRange / (distance + Mathf.Epsilon) : 0;
        }
    }
}