namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds a deliberately extensive architectural linework scene for rendering-scale experiments.
/// The mansion is a stress environment: repeated rooms, corridors, wings, courtyards, stairs,
/// and decorative geometry all exercise the same line substrate used by the Forge.
/// </summary>
public static class SingularityMansionLineworkFactory
{
    /// <summary>Creates the full mansion test structure.</summary>
    public static SpatialLinework Create()
    {
        var linework = new SpatialLinework();

        // Main footprint and wings.
        AddRectangle(linework, 4, 4, 92, 92);
        AddRectangle(linework, 18, 18, 64, 64);
        AddRectangle(linework, 4, 30, 22, 38);
        AddRectangle(linework, 74, 30, 22, 38);
        AddRectangle(linework, 30, 4, 40, 22);
        AddRectangle(linework, 30, 70, 40, 26);

        // Grand hall.
        AddRectangle(linework, 36, 30, 28, 40);
        AddLine(linework, 50, 30, 50, 70);
        AddLine(linework, 36, 50, 64, 50);

        // Four long galleries.
        AddRectangle(linework, 10, 34, 14, 30);
        AddRectangle(linework, 76, 34, 14, 30);
        AddRectangle(linework, 34, 10, 32, 14);
        AddRectangle(linework, 34, 76, 32, 14);

        // Courtyards / light wells.
        AddRectangle(linework, 24, 24, 12, 12);
        AddRectangle(linework, 64, 24, 12, 12);
        AddRectangle(linework, 24, 64, 12, 12);
        AddRectangle(linework, 64, 64, 12, 12);

        // Repeated guest rooms create a controlled high-line-count workload.
        for (var row = 0; row < 4; row++)
        for (var column = 0; column < 6; column++)
        {
            var x = 6 + column * 14;
            var y = 34 + row * 8;
            AddRectangle(linework, x, y, 10, 6);
            AddLine(linework, x + 5, y, x + 5, y + 6);
        }

        // Ballroom ribs / architectural repetition.
        for (var x = 38; x <= 62; x += 4)
            AddLine(linework, x, 32, x, 68);

        // Stair flights.
        AddStairs(linework, 28, 42, 8, 18, 8);
        AddStairs(linework, 64, 42, 8, 18, 8);
        AddStairs(linework, 42, 24, 16, 6, 8);
        AddStairs(linework, 42, 70, 16, 6, 8);

        // Decorative perimeter rhythm.
        for (var x = 8; x <= 92; x += 4)
        {
            AddLine(linework, x, 4, x + 2, 7);
            AddLine(linework, x, 96, x + 2, 93);
        }

        for (var y = 8; y <= 92; y += 4)
        {
            AddLine(linework, 4, y, 7, y + 2);
            AddLine(linework, 96, y, 93, y + 2);
        }

        return linework;
    }

    private static void AddStairs(SpatialLinework linework, double x, double y, double width, double height, int steps)
    {
        for (var i = 0; i <= steps; i++)
        {
            var offset = height * i / steps;
            AddLine(linework, x, y + offset, x + width, y + offset);
        }
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
