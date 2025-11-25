using System.Collections.Generic;
using UnityEngine;
public class Team
{
    public enum DetectionState
    {
        Hidden,
        Identified,
        Tracked
    }
    HashSet<Ship> members { get; } = new();
    public int MemberCount => members.Count;
    Dictionary<DetectionState, HashSet<Object>> Targets { get; } = new()
    {
        { DetectionState.Tracked, new HashSet<Object>() },
        { DetectionState.Identified, new HashSet<Object>() }
    };
    public event System.Action<Object> OnMemberAdded, OnMemberRemoved, OnTrackedTargetAdded,
        OnTrackedTargetRemoved, OnIdentifiedTargetAdded, OnIdentifiedTargetRemoved, OnTargetRemoved;
    public void AddMember(Ship ship)
    {
        if (members.Add(ship))
        {
            OnMemberAdded?.Invoke(ship);
            ship.OnDespawn += RemoveMember;
        }
    }
    public void RemoveMember(Object @object)
    {
        if (@object is not Ship ship) throw new System.ArgumentException("Object is not a Ship", nameof(@object));
        if (members.Remove(ship))
        {
            OnMemberRemoved?.Invoke(ship);
            ship.OnDespawn -= RemoveMember;
        }
    }
    public void Update()
    {
        Detect();
    }
    void Detect()
    {
        foreach (var obj in ObjectManager.Instance.Objects)
        {
            if (obj is Ship ship && members.Contains(ship)) continue;
            if (obj.AlwaysVisible)
            {
                if (!Targets[DetectionState.Identified].Contains(obj))
                {
                    Targets[DetectionState.Identified].Add(obj);
                    OnIdentifiedTargetAdded?.Invoke(obj);
                    obj.OnDespawn += RemoveTarget;
                }
                continue;
            }
            var newState = GetDetectionState(obj);
            var oldState = Targets[DetectionState.Tracked].Contains(obj) ? DetectionState.Tracked :
                           Targets[DetectionState.Identified].Contains(obj) ? DetectionState.Identified :
                           DetectionState.Hidden;
            if (oldState == newState) continue;
            switch (newState)
            {
                case DetectionState.Tracked:
                    if (oldState == DetectionState.Identified)
                    {
                        Targets[DetectionState.Identified].Remove(obj);
                        OnIdentifiedTargetRemoved?.Invoke(obj);
                    }
                    else if (oldState == DetectionState.Hidden) obj.OnDespawn += RemoveTarget;
                    Targets[DetectionState.Tracked].Add(obj);
                    OnTrackedTargetAdded?.Invoke(obj);
                    break;
                case DetectionState.Identified:
                    if (oldState == DetectionState.Tracked)
                    {
                        Targets[DetectionState.Tracked].Remove(obj);
                        OnTrackedTargetRemoved?.Invoke(obj);
                    }
                    else if (oldState == DetectionState.Hidden) obj.OnDespawn += RemoveTarget;
                    Targets[DetectionState.Identified].Add(obj);
                    OnIdentifiedTargetAdded?.Invoke(obj);
                    break;
                case DetectionState.Hidden:
                    RemoveTarget(obj);
                    break;
            }
        }
    }
    public void RemoveTarget(Object obj)
    {
        if (Targets[DetectionState.Tracked].Remove(obj))
        {
            OnTrackedTargetRemoved?.Invoke(obj);
        }
        else if (Targets[DetectionState.Identified].Remove(obj))
        {
            OnIdentifiedTargetRemoved?.Invoke(obj);
        }
        obj.OnDespawn -= RemoveTarget;
        OnTargetRemoved?.Invoke(obj);
    }
    DetectionState GetDetectionState(Object obj)
    {
        DetectionState state = DetectionState.Hidden;
        foreach (var member in members)
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
        members.Clear();
        Targets.Clear();
        OnMemberAdded = OnMemberRemoved = OnTrackedTargetAdded = OnTrackedTargetRemoved =
            OnIdentifiedTargetAdded = OnIdentifiedTargetRemoved = OnTargetRemoved = null;
    }
}
