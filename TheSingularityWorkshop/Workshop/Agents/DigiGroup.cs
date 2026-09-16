using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Agents;

/// <summary>The relationship between Digitens after they arrive to do something together.</summary>
public enum DigiGroupMode
{
    Cooperative,
    Competitive
}

/// <summary>
/// A persistent or temporary intentional aggregation of Digitens. This is
/// deliberately different from <see cref="DigitenCrowdField"/>: a crowd is a
/// movement optimization; a group has an objective.
/// </summary>
public sealed class DigiGroup
{
    private readonly HashSet<int> _members = new();

    public DigiGroup(string id, string objective, DigiGroupMode mode)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A group id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(objective)) throw new ArgumentException("A group objective is required.", nameof(objective));

        Id = id;
        Objective = objective;
        Mode = mode;
    }

    public string Id { get; }
    public string Objective { get; }
    public DigiGroupMode Mode { get; }
    public IReadOnlyCollection<int> Members => _members;

    public bool Add(int digitenId)
    {
        if (digitenId <= 0) throw new ArgumentOutOfRangeException(nameof(digitenId));
        return _members.Add(digitenId);
    }

    public bool Remove(int digitenId) => _members.Remove(digitenId);

    public bool Contains(int digitenId) => _members.Contains(digitenId);
}
