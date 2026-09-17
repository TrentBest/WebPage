namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds an intentionally extensive architectural linework scene for rendering-scale experiments.
/// The mansion is a stress environment: repeated rooms, corridors, wings, courtyards, stairs,
/// decorative geometry, and nested architectural motifs all exercise the same line substrate used by the Forge.
/// </summary>
public static class SingularityMansionLineworkFactory
{
    /// <summary>Creates the full mansion test structure.</summary>
    public static SpatialLinework Create()
    {
        var linework = new SpatialLinework();

        AddRectangle(linework, 4, 4, 92, 92);
        AddRectangle(linework, 8, 8, 84, 84);
        AddRectangle(linework, 18, 18, 64, 64);

        // Four wings and the grand central hall.
        AddRectangle(linework, 4, 30, 22, 38);
        AddRectangle(linework, 74, 30, 22, 38);
        AddRectangle(linework, 30, 4, 40, 22);
        AddRectangle(linework, 30, 70, 40, 26);
        AddRectangle(linework, 36, 30, 28, 40);
        AddLine(linework, 50, 30, 50, 70);
        AddLine(linework, 36, 50, 64, 50);

        // Long galleries.
        AddRectangle(linework, 10, 34, 14, 30);
        AddRectangle(linework, 76, 34, 14, 30);
        AddRectangle(linework, 34, 10, 32, 14);
        AddRectangle(linework, 34, 76, 32, 14);

        // Courtyards / light wells.
        foreach (var (x, y) in new[] { (24d, 24d), (64d, 24d), (24d, 64d), (64d, 64d) })
        {
            AddRectangle(linework, x, y, 12, 12);
            AddRectangle(linework, x + 2, y + 2, 8, 8);
        }

        // 12 x 10 room matrix: deliberately repetitive geometry for scaling tests.
        for (var row = 0; row < 10; row++)
        for (var column = 0; column < 12; column++)
        {
            var x = 5 + column * 7.5;
            var y = 33 + row * 6.1;
            AddRectangle(linework, x, y, 5.5, 4.5);
            AddLine(linework, x + 2.75, y, x + 2.75, y + 4.5);
            AddLine(linework, x, y + 2.25, x + 5.5, y + 2.25);
        }

        // Grand hall ribs.
        for (var x = 37; x <= 63; x += 2)
        {
            AddLine(linework, x, 31, x, 69);
            AddLine(linework, x, 31, 50, 28);
            AddLine(linework, x, 69, 50, 72);
        }

        // Four stair flights.
        AddStairs(linework, 28, 42, 8, 18, 16);
        AddStairs(linework, 64, 42, 8, 18, 16);
        AddStairs(linework, 42, 24, 16, 6, 16);
        AddStairs(linework, 42, 70, 16, 6, 16);

        // Towers / rotundas represented as octagonal footprints.
        foreach (var center in new[] { (14d, 14d), (86d, 14d), (14d, 86d), (86d, 86d) })
            AddOctagon(linework, center.Item1, center.Item2, 7);

        // Perimeter rhythm and crenellation.
        for (var x = 8; x <= 92; x += 2)
        {
            AddLine(linework, x, 4, x + 1, 6);
            AddLine(linework, x, 96, x + 1, 94);
        }

        for (var y = 8; y <= 92; y += 2)
        {
            AddLine(linework, 4, y, 6, y + 1);
            AddLine(linework, 96, y, 94, y + 1);
        }

        return linework;
    }

    private static void AddOctagon(SpatialLinework linework, double cx, double cy, double radius)
    {
        var points = Enumerable.Range(0, 8)
            .Select(i =>
            {
                var angle = Math.PI / 8 + i * Math.PI / 4;
                return new SpatialPoint(cx + Math.Cos(angle) * radius, cy + Math.Sin(angle) * radius);
            })
            .ToArray();

        for (var i = 0; i < points.Length; i++)
            AddLine(linework, points[i].X, points[i].Y, points[(i + 1) % points.Length].X, points[(i + 1) % points.Length].Y);
    }

    private static void AddStairs(SpatialLinework linework, double x, double y, double width, double height, int steps)
    {
        for (var i = 0; i <= steps; i++)
            AddLine(linework, x, y + height * i / steps, x + width, y + height * i / steps);
    }

    private static void AddRectangle(SpatialLinework linework, double x, double y, double width, double height)
    {
        AddLine(linework, x, y, x + width, y);
        AddLine(linework, x + width, y, x + width, y + height);
        AddLine(linework, x + width, y + height, x, y + height);
        AddLine(linework, x, y + height, x, y);
    }

    private static void AddLine(SpatialLinework linework, double x1, double y1, double x2, double y2)
    {
        linework.BeginLine(new SpatialPoint(x1, y1));
        linework.ContinueLine(new SpatialPoint(x2, y2));
        linework.EndLine();
    }
}
