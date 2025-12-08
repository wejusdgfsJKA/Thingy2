using System.Collections.Generic;
using UnityEngine;
public enum DetectionState
{
    /// <summary>
    /// We can fire on this target
    /// </summary>
    Identified,
    /// <summary>
    /// We have a rough idea of the target's tonnage and position
    /// </summary>
    Tracked,
    /// <summary>
    /// This is the first time we are seeing this object.
    /// </summary>
    Hidden
}

public class Team
{
    public HashSet<Unit> Members { get; } = new();
    public HashSet<Object> IdentifiedTargets { get; } = new();
    public HashSet<Object> TrackedTargets { get; } = new();
    public event System.Action<Object> OnMemberAdded, OnMemberRemoved, OnTrackedTargetAdded,
        OnIdentifiedTargetAdded, OnTargetRemoved;
    readonly int index;
    public Team(int index)
    {
        this.index = index;
    }
    public void AddMember(Unit ship)
    {
        if (Members.Add(ship))
        {
            OnMemberAdded?.Invoke(ship);
            ship.OnDespawn += RemoveMember;
            ship.Team = this;
        }
    }
    public void RemoveMember(Object @object)
    {
        if (@object is not Unit ship) return;
        if (Members.Remove(ship))
        {
            ship.OnDespawn -= RemoveMember;
            OnMemberRemoved?.Invoke(ship);
            ship.Team = null;
        }
    }
    public void Tick(float deltaTime)
    {
        PerformDetection();
    }
    void PerformDetection()
    {
        var enemyTeam = index == 0 ? GameManager.Teams[1] : GameManager.Teams[0];
        foreach (var obj in enemyTeam.Members)
        {
            var newState = GetDetectionState(obj);
            var oldState = GetObjectDetectionState(obj);
            if (oldState == newState) continue;
            switch (newState)
            {
                case DetectionState.Tracked:
                    if (oldState == DetectionState.Identified)
                    {
                        IdentifiedTargets.Remove(obj);
                    }
                    else if (oldState == DetectionState.Hidden) obj.OnDespawn += RemoveTarget;
                    TrackedTargets.Add(obj);
                    OnTrackedTargetAdded?.Invoke(obj);
                    break;
                case DetectionState.Identified:
                    if (oldState == DetectionState.Tracked)
                    {
                        TrackedTargets.Remove(obj);
                    }
                    else if (oldState == DetectionState.Hidden) obj.OnDespawn += RemoveTarget;
                    IdentifiedTargets.Add(obj);
                    OnIdentifiedTargetAdded?.Invoke(obj);
                    break;
                case DetectionState.Hidden:
                    RemoveTarget(obj);
                    break;
            }
        }
    }
    public DetectionState GetObjectDetectionState(Object obj)
    {
        return TrackedTargets.Contains(obj) ? DetectionState.Tracked :
            IdentifiedTargets.Contains(obj) ? DetectionState.Identified :
            DetectionState.Hidden;
    }
    public void RemoveTarget(Object obj)
    {
        IdentifiedTargets.Remove(obj);
        TrackedTargets.Remove(obj);
        obj.OnDespawn -= RemoveTarget;
        OnTargetRemoved?.Invoke(obj);
    }
    DetectionState GetDetectionState(Object obj)
    {
        DetectionState state = DetectionState.Tracked;
        foreach (var member in Members)
        {
            float distance = Vector3.Distance(member.Transform.position, obj.Transform.position) - obj.Signature;
            if (distance <= member.ScanRange)
            {
                return DetectionState.Identified;
            }
        }
        return state;
    }
    public void Clear()
    {
        Members.Clear();
        IdentifiedTargets.Clear();
        TrackedTargets.Clear();
        OnMemberAdded = OnMemberRemoved = OnTrackedTargetAdded = OnIdentifiedTargetAdded = OnTargetRemoved = null;
    }
}
