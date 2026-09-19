namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>
/// Builds the Singularity Mansion as a construction-drawing stress specimen.
/// The same <see cref="SpatialLinework"/> substrate used by the Forge is populated
/// with architectural, furniture, decorative, and building-services linework.
/// </summary>
public static class SingularityMansionLineworkFactory
{
    /// <summary>Creates the baseline detailed mansion drawing.</summary>
    public static SpatialLinework Create() => Create(1);

    /// <summary>
    /// Creates the mansion at the requested detail density. Density increases the
    /// number of repeated rooms, fixtures, furniture, decoration, and construction
    /// annotation primitives while preserving the same 100 x 100 perception-space footprint.
    /// </summary>
    public static SpatialLinework Create(int density)
    {
        if (density < 1)
            throw new ArgumentOutOfRangeException(nameof(density), "Mansion density must be at least one.");

        var linework = new SpatialLinework();

        AddConstructionEnvelope(linework);
        AddGrandHall(linework);
        AddWings(linework);
        AddCourtyards(linework);
        AddRoomMatrix(linework, density);
        AddCirculation(linework);
        AddTowers(linework);
        AddPerimeterDetail(linework);
        AddConstructionAnnotations(linework);
        AddFurnitureAndDecoration(linework, density);
        AddBuildingServices(linework, density);

        return linework;
    }

    private static void AddConstructionEnvelope(SpatialLinework linework)
    {
        // Property, structural, and finished-wall envelopes.
        AddRectangle(linework, 3, 3, 94, 94);
        AddRectangle(linework, 5, 5, 90, 90);
        AddRectangle(linework, 8, 8, 84, 84);
        AddRectangle(linework, 18, 18, 64, 64);

        // Primary structural grid.
        for (var x = 18; x <= 82; x += 4)
            AddLine(linework, x, 18, x, 82);

        for (var y = 18; y <= 82; y += 4)
            AddLine(linework, 18, y, 82, y);
    }

    private static void AddGrandHall(SpatialLinework linework)
    {
        AddRectangle(linework, 34, 28, 32, 44);
        AddLine(linework, 50, 28, 50, 72);
        AddLine(linework, 34, 50, 66, 50);

        // Gallery ribs / ceiling structure.
        for (var x = 36; x <= 64; x += 2)
        {
            AddLine(linework, x, 30, x, 70);
            AddLine(linework, x, 30, 50, 26);
            AddLine(linework, x, 70, 50, 74);
        }

        // Central ceremonial stair.
        AddStairs(linework, 43, 53, 14, -16, 16);
        AddStairs(linework, 43, 47, 14, 16, 16);

        // Rotunda / central table.
        AddCircle(linework, 50, 50, 4, 24);
        AddCircle(linework, 50, 50, 2.8, 24);
        AddVase(linework, 50, 50, .9);
    }

    private static void AddWings(SpatialLinework linework)
    {
        AddRectangle(linework, 4, 30, 26, 38);
        AddRectangle(linework, 70, 30, 26, 38);
        AddRectangle(linework, 30, 4, 40, 26);
        AddRectangle(linework, 30, 70, 40, 26);

        AddRectangle(linework, 10, 34, 16, 30);
        AddRectangle(linework, 74, 34, 16, 30);
        AddRectangle(linework, 34, 10, 32, 16);
        AddRectangle(linework, 34, 74, 32, 16);

        // Door connections into the hall.
        foreach (var y in new[] { 38d, 50d, 62d })
        {
            AddDoor(linework, 30, y, 3, horizontal: false);
            AddDoor(linework, 67, y, 3, horizontal: false);
        }

        foreach (var x in new[] { 40d, 50d, 60d })
        {
            AddDoor(linework, x, 30, 3, horizontal: true);
            AddDoor(linework, x, 67, 3, horizontal: true);
        }

        AddAlcove(linework, 8, 36, 18, 12, "east");
        AddAlcove(linework, 74, 36, 18, 12, "west");
        AddAlcove(linework, 40, 10, 20, 12, "south");
        AddAlcove(linework, 40, 74, 20, 12, "north");
    }

    private static void AddCourtyards(SpatialLinework linework)
    {
        foreach (var (x, y) in new[] { (24d, 24d), (64d, 24d), (24d, 64d), (64d, 64d) })
        {
            AddRectangle(linework, x, y, 12, 12);
            AddRectangle(linework, x + 2, y + 2, 8, 8);
            AddCircle(linework, x + 6, y + 6, 2.5, 20);
            AddVase(linework, x + 6, y + 6, .65);
        }
    }

    private static void AddRoomMatrix(SpatialLinework linework, int density)
    {
        var columns = 12 * density;
        var rows = 10 * density;
        var stepX = 90d / columns;
        var stepY = 61d / rows;
        var roomWidth = stepX * .72d;
        var roomHeight = stepY * .72d;

        for (var row = 0; row < rows; row++)
        for (var column = 0; column < columns; column++)
        {
            var x = 5 + column * stepX;
            var y = 33 + row * stepY;

            AddRectangle(linework, x, y, roomWidth, roomHeight);
            AddWallCore(linework, x, y, roomWidth, roomHeight);
            AddWindow(linework, x + roomWidth / 2 - stepX * .12, y, stepX * .24, horizontal: true);
            AddDoor(linework, x + roomWidth / 2, y + roomHeight, Math.Min(stepX * .22, 1.4), horizontal: true);

            // Furniture is deliberately small and repeated: this is a line-rendering
            // stress test, not a simplified block diagram.
            AddFurnitureCluster(linework, x + roomWidth / 2, y + roomHeight / 2, Math.Min(roomWidth, roomHeight) * .25);

            if ((row + column) % 3 == 0)
                AddVase(linework, x + roomWidth * .78, y + roomHeight * .72, Math.Min(roomWidth, roomHeight) * .08);
        }
    }

    private static void AddCirculation(SpatialLinework linework)
    {
        AddStairs(linework, 27, 42, 7, 18, 18);
        AddStairs(linework, 66, 42, 7, 18, 18);

        AddRectangle(linework, 25.5, 48, 10, 6);
        AddRectangle(linework, 64.5, 48, 10, 6);
        AddRectangle(linework, 43, 50, 14, 6);
        AddRectangle(linework, 43, 44, 14, 6);

        AddStairs(linework, 42, 25, 16, 5, 16);
        AddStairs(linework, 42, 70, 16, 5, 16);

        // Handrails.
        AddLine(linework, 27, 41, 34, 41);
        AddLine(linework, 66, 41, 73, 41);
        AddLine(linework, 42, 24, 58, 24);
        AddLine(linework, 42, 76, 58, 76);

        // Ceremonial front approach and landing.
        AddStairs(linework, 44, 84, 12, 8, 10);
        AddRectangle(linework, 42, 90, 16, 5);
        AddDoor(linework, 50, 84, 4, horizontal: true);
    }

    private static void AddTowers(SpatialLinework linework)
    {
        foreach (var center in new[] { (14d, 14d), (86d, 14d), (14d, 86d), (86d, 86d) })
        {
            AddOctagon(linework, center.Item1, center.Item2, 7);
            AddOctagon(linework, center.Item1, center.Item2, 5.5);
            AddCircle(linework, center.Item1, center.Item2, 2.5, 16);
            AddVase(linework, center.Item1, center.Item2, .8);
        }
    }

    private static void AddPerimeterDetail(SpatialLinework linework)
    {
        for (var x = 8; x <= 92; x += 2)
        {
            AddLine(linework, x, 3, x + 1, 5);
            AddLine(linework, x, 97, x + 1, 95);
            AddWindow(linework, x, 5, .9, horizontal: true);
            AddWindow(linework, x, 94, .9, horizontal: true);
        }

        for (var y = 8; y <= 92; y += 2)
        {
            AddLine(linework, 3, y, 5, y + 1);
            AddLine(linework, 97, y, 95, y + 1);
            AddWindow(linework, 5, y, .9, horizontal: false);
            AddWindow(linework, 94, y, .9, horizontal: false);
        }
    }

    private static void AddConstructionAnnotations(SpatialLinework linework)
    {
        // Dimension strings and centerlines are real drawing primitives, so they
        // exercise the same renderer rather than becoming HTML decoration.
        AddDimension(linework, 8, 15, 92, 15);
        AddDimension(linework, 15, 8, 15, 92);
        AddLine(linework, 50, 3, 50, 97);
        AddLine(linework, 3, 50, 97, 50);

        for (var x = 10; x <= 90; x += 10)
            AddTick(linework, x, 14, horizontal: false);

        for (var y = 10; y <= 90; y += 10)
            AddTick(linework, 14, y, horizontal: true);
    }

    private static void AddFurnitureAndDecoration(SpatialLinework linework, int density)
    {
        var centers = new[]
        {
            (14d, 40d), (86d, 40d), (14d, 60d), (86d, 60d),
            (40d, 14d), (60d, 14d), (40d, 86d), (60d, 86d)
        };

        foreach (var (x, y) in centers)
        {
            AddFurnitureCluster(linework, x, y, 3.2);
            AddVase(linework, x + 3.5, y + 2.5, 1.1);
            AddRug(linework, x, y, 6, 4);
        }

        for (var i = 0; i < density * 20; i++)
        {
            var x = 10 + (i * 17) % 80;
            var y = 10 + (i * 29) % 80;
            AddVase(linework, x, y, .45 + (i % 3) * .12);
            AddPlant(linework, x + 1.2, y);
        }
    }

    private static void AddBuildingServices(SpatialLinework linework, int density)
    {
        // Plumbing / electrical / data routes. They intentionally share the same
        // geometric substrate so later trade-layer filtering can occur without
        // changing the renderer.
        for (var y = 20d; y <= 80; y += Math.Max(4d, 8d / density))
        {
            AddLine(linework, 20, y, 80, y);
            AddLine(linework, 20, y + .35, 80, y + .35);
        }

        for (var x = 20d; x <= 80; x += Math.Max(4d, 8d / density))
        {
            AddLine(linework, x, 20, x, 80);
            AddLine(linework, x + .35, 20, x + .35, 80);
        }
    }

    private static void AddAlcove(SpatialLinework linework, double x, double y, double width, double height, string openingSide)
    {
        AddRectangle(linework, x, y, width, height);
        AddRectangle(linework, x + 1.25, y + 1.25, width - 2.5, height - 2.5);

        switch (openingSide)
        {
            case "east":
                AddLine(linework, x + width - 1.25, y + 2, x + width - 1.25, y + height - 2);
                AddLine(linework, x + width - 1.25, y + height / 2 - 2, x + width + 1, y + height / 2 - 2);
                AddLine(linework, x + width - 1.25, y + height / 2 + 2, x + width + 1, y + height / 2 + 2);
                break;
            case "west":
                AddLine(linework, x + 1.25, y + 2, x + 1.25, y + height - 2);
                AddLine(linework, x + 1.25, y + height / 2 - 2, x - 1, y + height / 2 - 2);
                AddLine(linework, x + 1.25, y + height / 2 + 2, x - 1, y + height / 2 + 2);
                break;
            case "south":
                AddLine(linework, x + 2, y + height - 1.25, x + width - 2, y + height - 1.25);
                AddLine(linework, x + width / 2 - 2, y + height - 1.25, x + width / 2 - 2, y + height + 1);
                AddLine(linework, x + width / 2 + 2, y + height - 1.25, x + width / 2 + 2, y + height + 1);
                break;
            case "north":
                AddLine(linework, x + 2, y + 1.25, x + width - 2, y + 1.25);
                AddLine(linework, x + width / 2 - 2, y + 1.25, x + width / 2 - 2, y - 1);
                AddLine(linework, x + width / 2 + 2, y + 1.25, x + width / 2 + 2, y - 1);
                break;
        }

        AddCircle(linework, x + width / 2, y + height / 2, Math.Min(width, height) * .18, 16);
    }

    private static void AddFurnitureCluster(SpatialLinework linework, double cx, double cy, double size)
    {
        AddRectangle(linework, cx - size, cy - size * .55, size * 2, size * .55);
        AddRectangle(linework, cx - size * .75, cy + size * .15, size * 1.5, size * .45);
        AddCircle(linework, cx, cy - size * .75, size * .3, 12);
    }

    private static void AddRug(SpatialLinework linework, double cx, double cy, double width, double height)
    {
        AddRectangle(linework, cx - width / 2, cy - height / 2, width, height);
        AddRectangle(linework, cx - width / 2 + .35, cy - height / 2 + .35, width - .7, height - .7);
    }

    private static void AddVase(SpatialLinework linework, double cx, double cy, double size)
    {
        AddLine(linework, cx - size * .45, cy - size, cx + size * .45, cy - size);
        AddLine(linework, cx - size * .45, cy - size, cx - size * .25, cy + size * .75);
        AddLine(linework, cx + size * .45, cy - size, cx + size * .25, cy + size * .75);
        AddCircle(linework, cx, cy + size * .75, size * .25, 10);
        AddCircle(linework, cx, cy - size * .85, size * .18, 10);
    }

    private static void AddPlant(SpatialLinework linework, double cx, double cy)
    {
        AddCircle(linework, cx, cy + .7, .45, 10);
        AddLine(linework, cx, cy, cx - .7, cy - 1.3);
        AddLine(linework, cx, cy, cx + .7, cy - 1.3);
        AddLine(linework, cx, cy, cx, cy - 1.6);
    }

    private static void AddWindow(SpatialLinework linework, double x, double y, double length, bool horizontal)
    {
        if (horizontal)
        {
            AddLine(linework, x - length, y, x + length, y);
            AddLine(linework, x - length, y + .35, x + length, y + .35);
            AddLine(linework, x, y, x, y + .35);
        }
        else
        {
            AddLine(linework, x, y - length, x, y + length);
            AddLine(linework, x + .35, y - length, x + .35, y + length);
            AddLine(linework, x, y, x + .35, y);
        }
    }

    private static void AddDoor(SpatialLinework linework, double x, double y, double width, bool horizontal)
    {
        if (horizontal)
        {
            AddLine(linework, x - width / 2, y, x + width / 2, y);
            AddLine(linework, x - width / 2, y, x - width / 2, y - width);
            AddLine(linework, x + width / 2, y, x + width / 2, y - width);
        }
        else
        {
            AddLine(linework, x, y - width / 2, x, y + width / 2);
            AddLine(linework, x, y - width / 2, x + width, y - width / 2);
            AddLine(linework, x, y + width / 2, x + width, y + width / 2);
        }
    }

    private static void AddWallCore(SpatialLinework linework, double x, double y, double width, double height)
    {
        AddLine(linework, x + width * .08, y + height * .08, x + width * .92, y + height * .08);
        AddLine(linework, x + width * .08, y + height * .92, x + width * .92, y + height * .92);
    }

    private static void AddDimension(SpatialLinework linework, double x1, double y1, double x2, double y2)
    {
        AddLine(linework, x1, y1, x2, y2);
        var horizontal = Math.Abs(x2 - x1) >= Math.Abs(y2 - y1);
        if (horizontal)
        {
            AddTick(linework, x1, y1, false);
            AddTick(linework, x2, y2, false);
        }
        else
        {
            AddTick(linework, x1, y1, true);
            AddTick(linework, x2, y2, true);
        }
    }

    private static void AddTick(SpatialLinework linework, double x, double y, bool horizontal)
    {
        if (horizontal) AddLine(linework, x - 1, y, x + 1, y);
        else AddLine(linework, x, y - 1, x, y + 1);
    }

    private static void AddCircle(SpatialLinework linework, double cx, double cy, double radius, int segments)
    {
        var points = Enumerable.Range(0, segments)
            .Select(i =>
            {
                var angle = i * Math.PI * 2d / segments;
                return new SpatialPoint(cx + Math.Cos(angle) * radius, cy + Math.Sin(angle) * radius);
            })
            .ToArray();

        for (var i = 0; i < points.Length; i++)
            AddLine(linework, points[i].X, points[i].Y, points[(i + 1) % points.Length].X, points[(i + 1) % points.Length].Y);
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
