using System.Collections.Generic;
using UnityEngine;
public enum DetectionState
{
    Hidden,
    Identified,
    Tracked
}

public class Team
{
    public HashSet<Unit> Members { get; } = new();
    public HashSet<Object> IdentifiedTargets { get; } = new();
    public HashSet<Object> TrackedTargets { get; } = new();
    public HashSet<Object> TrackedHostiles { get; } = new();
    public event System.Action<Object> OnMemberAdded, OnMemberRemoved, OnTrackedTargetAdded,
        OnIdentifiedTargetAdded, OnTargetRemoved;
    System.Action onTick;
    public void AddMember(Unit ship)
    {
        if (Members.Add(ship))
        {
            OnMemberAdded?.Invoke(ship);
            onTick += ship.Tick;
            ship.Team = this;
        }
    }
    public void RemoveMember(Unit ship)
    {
        if (Members.Remove(ship))
        {
            OnMemberRemoved?.Invoke(ship);
            onTick -= ship.Tick;
        }
    }
    public void Tick()
    {
        PerformDetection();
        onTick?.Invoke();
    }
    void PerformDetection()
    {
        foreach (var obj in ObjectManager.Instance.Objects)
        {
            if (obj is Unit ship && Members.Contains(ship)) continue;
            if (obj.AlwaysVisible)
            {
                if (!IdentifiedTargets.Contains(obj))
                {
                    IdentifiedTargets.Add(obj);
                    OnIdentifiedTargetAdded?.Invoke(obj);
                    obj.OnDespawn += RemoveTarget;
                }
                continue;
            }
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
                        var b = TrackedTargets.Remove(obj) || TrackedHostiles.Remove(obj);
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
        return TrackedTargets.Contains(obj) || TrackedHostiles.Contains(obj) ? DetectionState.Tracked :
            IdentifiedTargets.Contains(obj) ? DetectionState.Identified :
            DetectionState.Hidden;
    }
    public void AddUnidentifiedTarget(Object obj)
    {
        if (!TrackedHostiles.Contains(obj))
        {
            TrackedHostiles.Add(obj);
            if (TrackedTargets.Contains(obj))
            {
                TrackedTargets.Remove(obj);
            }
            else
            {
                OnTrackedTargetAdded?.Invoke(obj);
                IdentifiedTargets.Remove(obj);
            }
        }
    }
    public void RemoveTarget(Object obj)
    {
        IdentifiedTargets.Remove(obj);
        TrackedTargets.Remove(obj);
        TrackedHostiles.Remove(obj);
        obj.OnDespawn -= RemoveTarget;
        OnTargetRemoved?.Invoke(obj);
    }
    DetectionState GetDetectionState(Object obj)
    {
        DetectionState state = DetectionState.Hidden;
        foreach (var member in Members)
        {
            float distance = Vector3.Distance(member.Transform.position, obj.Transform.position) - obj.Signature;
            if (distance <= member.ScanRange / 2)
            {
                return DetectionState.Identified;
            }
            if (distance <= member.ScanRange)
            {
                state = DetectionState.Tracked;
            }
        }
        return state;
    }
    public void Clear()
    {
        Members.Clear();
        IdentifiedTargets.Clear();
        TrackedTargets.Clear();
        TrackedHostiles.Clear();
        OnMemberAdded = OnMemberRemoved = OnTrackedTargetAdded = OnIdentifiedTargetAdded = OnTargetRemoved = null;
        onTick = null;
    }
}
