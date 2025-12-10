using System.Collections.Generic;
public enum DetectionState : byte
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
    /// This is the first time we are seeing this SpecialObject.
    /// </summary>
    Hidden
}

public class Team
{
    public HashSet<Unit> Members { get; } = new();
    public event System.Action<Unit> OnMemberAdded, OnMemberRemoved;
    public readonly int Index;
    public Team(int index)
    {
        Index = index;
    }
    public void AddMember(Unit ship)
    {
        if (Members.Add(ship))
        {
            OnMemberAdded?.Invoke(ship);
            ship.OnDespawn += RemoveMember;
            ship.Team = Index;
        }
    }
    public void RemoveMember(Unit ship)
    {
        if (Members.Remove(ship))
        {
            ship.OnDespawn -= RemoveMember;
            OnMemberRemoved?.Invoke(ship);
        }
    }
    public void Clear()
    {
        Members.Clear();
        OnMemberAdded = OnMemberRemoved = null;
    }
}
