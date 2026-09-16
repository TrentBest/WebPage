namespace TheSingularityWorkshop.Gui;

/// <summary>Primitive families understood by the Workshop spatial geometry layer.</summary>
public enum SpatialGeometryKind
{
    Line,
    Rectangle,
    Circle,
    Wall
}

/// <summary>Semantic opening cut into a wall perimeter.</summary>
public enum SpatialOpeningKind
{
    Door,
    Window,
    Passage
}

/// <summary>A wall opening expressed along one edge of an axis-aligned building.</summary>
public readonly record struct SpatialOpening(
    SpatialOpeningKind Kind,
    SpatialGeometryEdge Edge,
    double Start,
    double End)
{
    public double Length => Math.Max(0, End - Start);
}

/// <summary>One of the four edges of an axis-aligned spatial rectangle.</summary>
public enum SpatialGeometryEdge
{
    Top,
    Right,
    Bottom,
    Left
}

/// <summary>A paired-line wall with a muted fill between its high-energy edge lines.</summary>
public readonly record struct SpatialWall(
    SpatialLine Outer,
    SpatialLine Inner,
    string Accent,
    string Fill);

/// <summary>Axis-aligned building geometry used by both rendering and navigation.</summary>
public sealed record SpatialBuildingGeometry(
    string Id,
    SpatialBounds Bounds,
    string Accent,
    string Fill,
    IReadOnlyList<SpatialOpening> Openings)
{
    public SpatialBuildingGeometry(string id, SpatialBounds bounds, string accent, string fill)
        : this(id, bounds, accent, fill, [])
    {
    }
}

/// <summary>
/// Stateless geometry engine for the Workshop perception layer.
/// It deliberately owns no DOM or renderer state: geometry is data.
/// </summary>
public static class SpatialGeometryEngine
{
    /// <summary>Builds the current Workshop building set from spatial interactables.</summary>
    public static IReadOnlyList<SpatialBuildingGeometry> FromInteractables(IReadOnlyList<SpatialInteractable> interactables)
        => interactables
            .Select(item => new SpatialBuildingGeometry(
                item.Id,
                item.Bounds,
                AccentFor(item.Id),
                FillFor(item.Id),
                item.Openings))
            .ToArray();

    /// <summary>Returns the two neon perimeter lines for one building edge.</summary>
    public static IReadOnlyList<SpatialWall> BuildWalls(SpatialBuildingGeometry building)
    {
        var b = building.Bounds;
        var inset = Math.Min(0.8, Math.Min(b.Width, b.Height) / 5d);
        var outer = new[]
        {
            new SpatialLine(0, new SpatialPoint(b.X, b.Y), new SpatialPoint(b.X + b.Width, b.Y)),
            new SpatialLine(1, new SpatialPoint(b.X + b.Width, b.Y), new SpatialPoint(b.X + b.Width, b.Y + b.Height)),
            new SpatialLine(2, new SpatialPoint(b.X + b.Width, b.Y + b.Height), new SpatialPoint(b.X, b.Y + b.Height)),
            new SpatialLine(3, new SpatialPoint(b.X, b.Y + b.Height), new SpatialPoint(b.X, b.Y))
        };
        var inner = new[]
        {
            new SpatialLine(4, new SpatialPoint(b.X + inset, b.Y + inset), new SpatialPoint(b.X + b.Width - inset, b.Y + inset)),
            new SpatialLine(5, new SpatialPoint(b.X + b.Width - inset, b.Y + inset), new SpatialPoint(b.X + b.Width - inset, b.Y + b.Height - inset)),
            new SpatialLine(6, new SpatialPoint(b.X + b.Width - inset, b.Y + b.Height - inset), new SpatialPoint(b.X + inset, b.Y + b.Height - inset)),
            new SpatialLine(7, new SpatialPoint(b.X + inset, b.Y + b.Height - inset), new SpatialPoint(b.X + inset, b.Y + inset))
        };

        return Enumerable.Range(0, 4)
            .Select(i => new SpatialWall(outer[i], inner[i], building.Accent, building.Fill))
            .ToArray();
    }

    /// <summary>Tests whether a movement segment intersects a building's collision volume.</summary>
    public static bool SegmentBlocked(SpatialPoint start, SpatialPoint end, SpatialBounds bounds, double clearance = 0.75)
    {
        var expanded = new SpatialBounds(
            bounds.X - clearance,
            bounds.Y - clearance,
            bounds.Width + clearance * 2,
            bounds.Height + clearance * 2);

        if (Contains(expanded, start) || Contains(expanded, end)) return true;

        var corners = new[]
        {
            new SpatialPoint(expanded.X, expanded.Y),
            new SpatialPoint(expanded.X + expanded.Width, expanded.Y),
            new SpatialPoint(expanded.X + expanded.Width, expanded.Y + expanded.Height),
            new SpatialPoint(expanded.X, expanded.Y + expanded.Height)
        };

        for (var i = 0; i < corners.Length; i++)
        {
            if (SegmentsIntersect(start, end, corners[i], corners[(i + 1) % corners.Length])) return true;
        }

        return false;
    }

    /// <summary>Finds a short collision-free route using visibility around rectangle corners.</summary>
    public static IReadOnlyList<SpatialPoint> FindPath(
        SpatialPoint start,
        SpatialPoint target,
        IReadOnlyList<SpatialBuildingGeometry> obstacles,
        double clearance = 1.5)
    {
        if (IsClear(start, target, obstacles, clearance)) return [target];

        var candidates = new List<SpatialPoint>();
        foreach (var obstacle in obstacles)
        {
            var b = obstacle.Bounds;
            candidates.Add(new SpatialPoint(b.X - clearance, b.Y - clearance));
            candidates.Add(new SpatialPoint(b.X + b.Width + clearance, b.Y - clearance));
            candidates.Add(new SpatialPoint(b.X + b.Width + clearance, b.Y + b.Height + clearance));
            candidates.Add(new SpatialPoint(b.X - clearance, b.Y + b.Height + clearance));
        }

        var current = start;
        var route = new List<SpatialPoint>();
        var remaining = candidates
            .Where(point => !InsideAny(point, obstacles, clearance))
            .Distinct()
            .ToList();

        for (var guard = 0; guard < obstacles.Count + 2 && !IsClear(current, target, obstacles, clearance); guard++)
        {
            var next = remaining
                .Where(point => IsClear(current, point, obstacles, clearance))
                .OrderBy(point => Distance(point, target))
                .FirstOrDefault();

            if (next == default) break;
            route.Add(next);
            current = next;
            remaining.Remove(next);
        }

        if (!IsClear(current, target, obstacles, clearance))
            return [];

        route.Add(target);
        return route;
    }

    private static bool IsClear(SpatialPoint start, SpatialPoint end, IReadOnlyList<SpatialBuildingGeometry> obstacles, double clearance)
        => obstacles.All(obstacle => !SegmentBlocked(start, end, obstacle.Bounds, clearance));

    private static bool InsideAny(SpatialPoint point, IReadOnlyList<SpatialBuildingGeometry> obstacles, double clearance)
        => obstacles.Any(obstacle => Contains(new SpatialBounds(obstacle.Bounds.X - clearance, obstacle.Bounds.Y - clearance, obstacle.Bounds.Width + clearance * 2, obstacle.Bounds.Height + clearance * 2), point));

    private static bool Contains(SpatialBounds bounds, SpatialPoint point)
        => point.X >= bounds.X && point.X <= bounds.X + bounds.Width && point.Y >= bounds.Y && point.Y <= bounds.Y + bounds.Height;

    private static bool SegmentsIntersect(SpatialPoint a, SpatialPoint b, SpatialPoint c, SpatialPoint d)
    {
        static double Cross(SpatialPoint p, SpatialPoint q, SpatialPoint r)
            => (q.X - p.X) * (r.Y - p.Y) - (q.Y - p.Y) * (r.X - p.X);

        var abC = Cross(a, b, c);
        var abD = Cross(a, b, d);
        var cdA = Cross(c, d, a);
        var cdB = Cross(c, d, b);
        return ((abC > 0 && abD < 0) || (abC < 0 && abD > 0)) &&
               ((cdA > 0 && cdB < 0) || (cdA < 0 && cdB > 0));
    }

    private static double Distance(SpatialPoint a, SpatialPoint b)
        => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    private static string AccentFor(string id) => id switch
    {
        "image-tools" => "#ff2bd6",
        "npc-studio" => "#ffe04a",
        "fsm-bench" => "#00eaff",
        "storage-bins" => "#52e05a",
        "blueprint-library" => "#f7f7ff",
        _ => "#ff3b8d"
    };

    private static string FillFor(string id) => id switch
    {
        "image-tools" => "#251126",
        "npc-studio" => "#26210c",
        "fsm-bench" => "#0d2026",
        "storage-bins" => "#102115",
        "blueprint-library" => "#17171f",
        _ => "#21101a"
    };
}
