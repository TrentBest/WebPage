namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Creates the Forge's structural linework as spatial primitives. The Forge is
/// therefore rendered through the same line substrate used by the Linework Lab,
/// rather than maintaining a second SVG-specific geometry implementation.
/// </summary>
public static class SpatialForgeLineworkFactory
{
    /// <summary>Builds the persistent line primitives that describe the Forge interior.</summary>
    public static SpatialLinework Create()
    {
        var linework = new SpatialLinework();

        // Exterior perimeter.
        AddRectangle(linework, 8, 8, 84, 84);

        // Entrance opening is represented by the two wall segments around the door.
        AddLine(linework, 8, 8, 92, 8);
        AddLine(linework, 8, 8, 8, 92);
        AddLine(linework, 92, 8, 92, 92);
        AddLine(linework, 8, 92, 46, 92);
        AddLine(linework, 54, 92, 92, 92);

        // Inner architectural frame.
        AddRectangle(linework, 11, 11, 78, 78);

        // Four physical work stations.
        AddRectangle(linework, 14, 20, 30, 23);
        AddRectangle(linework, 56, 20, 30, 23);
        AddRectangle(linework, 14, 55, 30, 23);
        AddRectangle(linework, 56, 55, 30, 23);

        // Station benches / work surfaces.
        AddLine(linework, 17, 32, 41, 32);
        AddLine(linework, 59, 32, 83, 32);
        AddLine(linework, 17, 67, 41, 67);
        AddLine(linework, 59, 67, 83, 67);

        // Central fabrication axis.
        AddLine(linework, 50, 11, 50, 20);
        AddLine(linework, 50, 43, 50, 55);
        AddLine(linework, 50, 78, 50, 89);

        return linework;
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
