using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.Gui;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Owns Workshop world pathfinding.
/// A path is a capability of the Experience, not a concern of the page or the
/// spatial GUI builder. The implementation uses a deterministic four-direction
/// A* grid so the visitor can travel around suite footprints without clipping
/// through their architecture.
/// </summary>
public sealed class PathfindingMicroBundle : IDisposable
{
    public const int BundleId = 2105;
    public const double GridResolution = 1;
    public const double Clearance = 2;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;
    private IReadOnlyList<SpatialWaypoint> _path = Array.Empty<SpatialWaypoint>();
    private int _pathIndex;

    public PathfindingMicroBundle()
    {
        _lifecycle = new MicroBundle(
            BundleId,
            "WORKSHOP PATHFINDING",
            new WebMicroBundleProvider());
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    /// <summary>The currently planned route, including the destination.</summary>
    public IReadOnlyList<SpatialWaypoint> CurrentPath => _path;

    /// <summary>True while the route still contains waypoints to consume.</summary>
    public bool IsWalking => _pathIndex < _path.Count;

    /// <summary>
    /// Plans a route through the current Workshop suite footprints.
    /// Returns false when the requested destination cannot be reached.
    /// </summary>
    public bool TryPlan(
        double startX,
        double startY,
        double targetX,
        double targetY,
        IReadOnlyList<SpatialObstacle> obstacles)
    {
        if (_disposed)
            return false;

        var start = ToCell(startX, startY);
        var goal = ToCell(targetX, targetY);

        if (!IsWalkable(start, obstacles))
            start = FindNearestWalkable(start, obstacles);

        if (!IsWalkable(goal, obstacles))
            goal = FindNearestWalkable(goal, obstacles);

        var cells = FindPath(start, goal, obstacles);
        if (cells.Count == 0)
        {
            _path = Array.Empty<SpatialWaypoint>();
            _pathIndex = 0;
            return false;
        }

        _path = CollapseCollinear(cells.Select(FromCell).ToArray());
        _pathIndex = _path.Count > 0 && SamePosition(_path[0], startX, startY) ? 1 : 0;
        return true;
    }

    /// <summary>
    /// Consumes one planned waypoint. Navigation remains the authoritative owner
    /// of the visitor coordinates; this bundle only supplies the next destination.
    /// </summary>
    public bool TryTakeNext(out SpatialWaypoint waypoint)
    {
        if (_disposed || _pathIndex >= _path.Count)
        {
            waypoint = default;
            return false;
        }

        waypoint = _path[_pathIndex++];
        return true;
    }

    public void ClearPath()
    {
        _path = Array.Empty<SpatialWaypoint>();
        _pathIndex = 0;
    }

    public void Update() => _lifecycle.Update();

    public void Invalidate()
    {
        ClearPath();
        _lifecycle.Invalidate();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }

    private static IReadOnlyList<GridCell> FindPath(
        GridCell start,
        GridCell goal,
        IReadOnlyList<SpatialObstacle> obstacles)
    {
        if (start == goal)
            return [start];

        var open = new PriorityQueue<GridCell, double>();
        var cameFrom = new Dictionary<GridCell, GridCell>();
        var cost = new Dictionary<GridCell, double> { [start] = 0 };
        var closed = new HashSet<GridCell>();

        open.Enqueue(start, Heuristic(start, goal));

        while (open.TryDequeue(out var current, out _))
        {
            if (!closed.Add(current))
                continue;

            if (current == goal)
                return Reconstruct(cameFrom, current);

            foreach (var next in Neighbors(current))
            {
                if (!IsWalkable(next, obstacles) || closed.Contains(next))
                    continue;

                var nextCost = cost[current] + 1;
                if (cost.TryGetValue(next, out var knownCost) && nextCost >= knownCost)
                    continue;

                cost[next] = nextCost;
                cameFrom[next] = current;
                open.Enqueue(next, nextCost + Heuristic(next, goal));
            }
        }

        return Array.Empty<GridCell>();
    }

    private static IReadOnlyList<GridCell> Reconstruct(
        IReadOnlyDictionary<GridCell, GridCell> cameFrom,
        GridCell current)
    {
        var path = new List<GridCell> { current };
        while (cameFrom.TryGetValue(current, out var previous))
        {
            current = previous;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    private static IReadOnlyList<SpatialWaypoint> CollapseCollinear(IReadOnlyList<SpatialWaypoint> points)
    {
        if (points.Count < 3)
            return points;

        var result = new List<SpatialWaypoint> { points[0] };
        var previousDirection = Direction(points[0], points[1]);

        for (var index = 1; index < points.Count - 1; index++)
        {
            var nextDirection = Direction(points[index], points[index + 1]);
            if (nextDirection != previousDirection)
            {
                result.Add(points[index]);
                previousDirection = nextDirection;
            }
        }

        result.Add(points[^1]);
        return result;
    }

    private static GridCell FindNearestWalkable(GridCell origin, IReadOnlyList<SpatialObstacle> obstacles)
    {
        if (IsWalkable(origin, obstacles))
            return origin;

        for (var radius = 1; radius <= 100; radius++)
        {
            for (var x = origin.X - radius; x <= origin.X + radius; x++)
            {
                for (var y = origin.Y - radius; y <= origin.Y + radius; y++)
                {
                    var candidate = new GridCell(x, y);
                    if (IsWalkable(candidate, obstacles))
                        return candidate;
                }
            }
        }

        return new GridCell(50, 50);
    }

    private static bool IsWalkable(GridCell cell, IReadOnlyList<SpatialObstacle> obstacles)
    {
        if (cell.X < 4 || cell.X > 96 || cell.Y < 8 || cell.Y > 92)
            return false;

        var point = FromCell(cell);
        return obstacles.All(obstacle =>
            Math.Abs(point.X - obstacle.CenterX) > obstacle.Width / 2 + Clearance ||
            Math.Abs(point.Y - obstacle.CenterY) > obstacle.Height / 2 + Clearance);
    }

    private static IEnumerable<GridCell> Neighbors(GridCell cell)
    {
        yield return new GridCell(cell.X + 1, cell.Y);
        yield return new GridCell(cell.X - 1, cell.Y);
        yield return new GridCell(cell.X, cell.Y + 1);
        yield return new GridCell(cell.X, cell.Y - 1);
    }

    private static double Heuristic(GridCell a, GridCell b)
        => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private static GridCell ToCell(double x, double y)
        => new((int)Math.Round(x / GridResolution), (int)Math.Round(y / GridResolution));

    private static SpatialWaypoint FromCell(GridCell cell)
        => new(cell.X * GridResolution, cell.Y * GridResolution);

    private static bool SamePosition(SpatialWaypoint point, double x, double y)
        => Math.Abs(point.X - x) < .001 && Math.Abs(point.Y - y) < .001;

    private static DirectionKind Direction(SpatialWaypoint a, SpatialWaypoint b)
        => Math.Abs(b.X - a.X) > Math.Abs(b.Y - a.Y)
            ? DirectionKind.Horizontal
            : DirectionKind.Vertical;

    private readonly record struct GridCell(int X, int Y);
    private enum DirectionKind { Horizontal, Vertical }
}

/// <summary>A world-space point produced by the pathfinding capability.</summary>
public readonly record struct SpatialWaypoint(double X, double Y);
