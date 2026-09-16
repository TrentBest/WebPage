using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Agents;

/// <summary>
/// Movement helpers for an intentional DigiGroup.
///
/// A group does not replace its members with one giant actor. Each member keeps
/// an individual position and chooses its own adjacent move. Group cohesion is
/// simply another weight in that local decision: members prefer to remain near
/// the group's center while still following the group's desired direction.
/// </summary>
public static class DigiGroupMovement
{
    private static readonly DigitenVector[] Directions =
    {
        new(0, -1), new(1, -1), new(1, 0), new(1, 1),
        new(0, 1), new(-1, 1), new(-1, 0), new(-1, -1)
    };

    /// <summary>
    /// Calculates the geometric center of the currently represented members.
    /// </summary>
    public static (double X, double Y) CalculateCenter(
        DigiGroup group,
        IReadOnlyDictionary<int, (int X, int Y)> positions)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(positions);

        if (group.Members.Count == 0)
            return (0, 0);

        var x = 0d;
        var y = 0d;
        var count = 0;
        foreach (var member in group.Members)
        {
            if (!positions.TryGetValue(member, out var position))
                continue;

            x += position.X;
            y += position.Y;
            count++;
        }

        return count == 0 ? (0, 0) : (x / count, y / count);
    }

    /// <summary>
    /// Chooses one adjacent move for a group member.
    ///
    /// Desired travel remains the primary influence. Cohesion adds a secondary
    /// pull toward the current group center, so members can shuffle independently
    /// instead of tracing a rigid formation. Occupied cells remain obstacles.
    /// </summary>
    public static DigitenVector ChooseMemberDirection(
        DigitenCityTexture city,
        DigiGroup group,
        int memberId,
        (int X, int Y) memberPosition,
        (double X, double Y) groupCenter,
        DigitenVector desiredDirection,
        double cohesionWeight = 0.35)
    {
        ArgumentNullException.ThrowIfNull(city);
        ArgumentNullException.ThrowIfNull(group);

        if (!group.Contains(memberId))
            throw new InvalidOperationException("The member must belong to the group.");
        if (cohesionWeight < 0)
            throw new ArgumentOutOfRangeException(nameof(cohesionWeight));

        var desired = Normalize(desiredDirection);
        var toCenter = Normalize(new DigitenVector(
            Math.Sign(groupCenter.X - memberPosition.X),
            Math.Sign(groupCenter.Y - memberPosition.Y)));

        var best = DigitenVector.Zero;
        var bestScore = double.NegativeInfinity;

        foreach (var candidate in Directions)
        {
            var x = memberPosition.X + candidate.X;
            var y = memberPosition.Y + candidate.Y;
            if (!city.IsWalkable(x, y) || city.IsOccupied(x, y))
                continue;

            var travelScore = candidate.X * desired.X + candidate.Y * desired.Y;
            var cohesionScore = candidate.X * toCenter.X + candidate.Y * toCenter.Y;
            var score = travelScore + cohesionWeight * cohesionScore;
            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private static (double X, double Y) Normalize(DigitenVector direction)
    {
        var length = Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
        return length == 0 ? (0, 0) : (direction.X / length, direction.Y / length);
    }
}
