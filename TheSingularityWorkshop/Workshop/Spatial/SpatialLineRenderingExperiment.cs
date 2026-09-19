namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Defines the laboratory's line-rendering experiment: project a 3D drawing into
/// the same linework substrate used by the Mansion, then run the normal rendering lifecycle.
/// </summary>
public sealed record SpatialLineRenderingExperiment(
    string Id,
    string Name,
    string Description,
    SpatialLineRenderingExperimentStats Stats)
{
    public static SpatialLineRenderingExperiment CreateDefault()
    {
        var model = SpatialLine3DModel.CreateTestTower();
        var projected = SpatialLine3DProjector.Project(model, new SpatialPoint(50, 50), 0.72);

        var linework = new SpatialLinework();
        foreach (var line in projected)
        {
            linework.BeginLine(line.Start);
            linework.ContinueLine(line.End);
            linework.EndLine();
        }

        var rendered = new SpatialLineRenderer().Render(linework, SpatialLineStyleCatalog.Blueprint);
        var texture = linework.TextureBuffer;

        return new SpatialLineRenderingExperiment(
            "line-rendering-3d",
            "3D LINE RENDERING EXPERIMENT",
            "A laboratory instrument for measuring the complete path from 3D geometry through projection, FSM-backed linework, renderer conversion, and GPU-shaped texture payload.",
            new SpatialLineRenderingExperimentStats(
                model.Lines.Count,
                projected.Count,
                rendered.Count,
                texture.Width,
                texture.Width * 4 * sizeof(float)));
    }
}

public readonly record struct SpatialLineRenderingExperimentStats(
    int Source3DLines,
    int ProjectedLines,
    int RenderedLines,
    int TextureWidth,
    int GpuPayloadBytes);

/// <summary>A renderer-neutral 3D line primitive.</summary>
public readonly record struct SpatialLine3D(
    double X1, double Y1, double Z1,
    double X2, double Y2, double Z2);

public sealed class SpatialLine3DModel
{
    private SpatialLine3DModel(IReadOnlyList<SpatialLine3D> lines) => Lines = lines;

    public IReadOnlyList<SpatialLine3D> Lines { get; }

    public static SpatialLine3DModel CreateTestTower()
    {
        var lines = new List<SpatialLine3D>();
        const int floors = 12;
        const int sides = 8;
        const double radius = 12;
        const double floorHeight = 3;

        for (var floor = 0; floor <= floors; floor++)
        {
            var z = floor * floorHeight;
            var points = Enumerable.Range(0, sides)
                .Select(i =>
                {
                    var a = i * Math.PI * 2d / sides;
                    return (X: Math.Cos(a) * radius, Y: Math.Sin(a) * radius, Z: z);
                }).ToArray();

            for (var i = 0; i < sides; i++)
            {
                var next = points[(i + 1) % sides];
                lines.Add(new SpatialLine3D(points[i].X, points[i].Y, points[i].Z, next.X, next.Y, next.Z));
            }
        }

        var baseRing = Enumerable.Range(0, sides).Select(i =>
        {
            var a = i * Math.PI * 2d / sides;
            return (X: Math.Cos(a) * radius, Y: Math.Sin(a) * radius, Z: 0d);
        }).ToArray();

        var topRing = Enumerable.Range(0, sides).Select(i =>
        {
            var a = i * Math.PI * 2d / sides;
            return (X: Math.Cos(a) * radius, Y: Math.Sin(a) * radius, Z: floors * floorHeight);
        }).ToArray();

        for (var i = 0; i < sides; i++)
            lines.Add(new SpatialLine3D(baseRing[i].X, baseRing[i].Y, baseRing[i].Z, topRing[i].X, topRing[i].Y, topRing[i].Z));

        // Cross-bracing and floor-to-floor interior structure deliberately increase
        // the primitive count without changing the projection contract.
        for (var floor = 0; floor < floors; floor++)
        {
            var z1 = floor * floorHeight;
            var z2 = (floor + 1) * floorHeight;
            for (var i = 0; i < sides; i += 2)
            {
                var a = i * Math.PI * 2d / sides;
                var b = ((i + 1) % sides) * Math.PI * 2d / sides;
                lines.Add(new SpatialLine3D(Math.Cos(a) * radius, Math.Sin(a) * radius, z1,
                    Math.Cos(b) * radius, Math.Sin(b) * radius, z2));
            }
        }

        return new SpatialLine3DModel(lines);
    }
}

public static class SpatialLine3DProjector
{
    /// <summary>Applies a simple orthographic axonometric projection into 2D perception space.</summary>
    public static IReadOnlyList<SpatialLine> Project(
        SpatialLine3DModel model,
        SpatialPoint origin,
        double scale)
    {
        ArgumentNullException.ThrowIfNull(model);
        var projected = new List<SpatialLine>(model.Lines.Count);

        foreach (var line in model.Lines)
        {
            var start = ProjectPoint(line.X1, line.Y1, line.Z1, origin, scale);
            var end = ProjectPoint(line.X2, line.Y2, line.Z2, origin, scale);
            projected.Add(new SpatialLine(projected.Count, start, end));
        }

        return projected;
    }

    private static SpatialPoint ProjectPoint(double x, double y, double z, SpatialPoint origin, double scale)
    {
        var projectedX = (x - y) * 0.70710678118;
        var projectedY = (x + y) * 0.35355339059 - z;
        return new SpatialPoint(origin.X + projectedX * scale, origin.Y + projectedY * scale);
    }
}
