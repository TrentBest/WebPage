using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Small, deterministic grid pathfinder for the spatial world.
/// It is intentionally renderer-independent: a Diablo-like click-to-move
/// manifestation can consume the returned path without owning navigation rules.
/// </summary>
public static class DiabloPathfinder
{
    private static readonly (int X, int Y)[] Directions =
    [
        (1, 0), (-1, 0), (0, 1), (0, -1),
        (1, 1), (1, -1), (-1, 1), (-1, -1)
    ];

    public static IReadOnlyList<SpatialGridPoint> FindPath(
        int width,
        int height,
        Func<int, int, bool> isWalkable,
        SpatialGridPoint start,
        SpatialGridPoint goal)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));
        if (!Inside(start, width, height) || !Inside(goal, width, height))
            return Array.Empty<SpatialGridPoint>();
        if (!isWalkable(start.X, start.Y) || !isWalkable(goal.X, goal.Y))
            return Array.Empty<SpatialGridPoint>();
        if (start == goal)
            return [start];

        var open = new PriorityQueue<SpatialGridPoint, double>();
        var cameFrom = new Dictionary<SpatialGridPoint, SpatialGridPoint>();
        var cost = new Dictionary<SpatialGridPoint, double> { [start] = 0 };
        var closed = new HashSet<SpatialGridPoint>();

        open.Enqueue(start, Heuristic(start, goal));

        while (open.TryDequeue(out var current, out _))
        {
            if (!closed.Add(current))
                continue;

            if (current == goal)
                return Reconstruct(cameFrom, current);

            foreach (var direction in Directions)
            {
                var next = new SpatialGridPoint(current.X + direction.X, current.Y + direction.Y);
                if (!Inside(next, width, height) || !isWalkable(next.X, next.Y))
                    continue;

                // Do not let a diagonal step cut through the corner of two walls.
                if (direction.X != 0 && direction.Y != 0 &&
                    (!isWalkable(current.X + direction.X, current.Y) ||
                     !isWalkable(current.X, current.Y + direction.Y)))
                    continue;

                var step = direction.X != 0 && direction.Y != 0 ? 1.41421356237 : 1d;
                var candidate = cost[current] + step;

                if (cost.TryGetValue(next, out var known) && candidate >= known)
                    continue;

                cost[next] = candidate;
                cameFrom[next] = current;
                open.Enqueue(next, candidate + Heuristic(next, goal));
            }
        }

        return Array.Empty<SpatialGridPoint>();
    }

    private static bool Inside(SpatialGridPoint point, int width, int height)
        => point.X >= 0 && point.X < width && point.Y >= 0 && point.Y < height;

    private static double Heuristic(SpatialGridPoint a, SpatialGridPoint b)
    {
        var dx = Math.Abs(a.X - b.X);
        var dy = Math.Abs(a.Y - b.Y);
        var diagonal = Math.Min(dx, dy);
        return diagonal * 1.41421356237 + Math.Max(dx, dy) - diagonal;
    }

    private static IReadOnlyList<SpatialGridPoint> Reconstruct(
        Dictionary<SpatialGridPoint, SpatialGridPoint> cameFrom,
        SpatialGridPoint current)
    {
        var result = new List<SpatialGridPoint> { current };
        while (cameFrom.TryGetValue(current, out var previous))
        {
            current = previous;
            result.Add(current);
        }
        result.Reverse();
        return result;
    }
}

/// <summary>A discrete navigation sample used by the spatial pathfinder.</summary>
public readonly record struct SpatialGridPoint(int X, int Y);
