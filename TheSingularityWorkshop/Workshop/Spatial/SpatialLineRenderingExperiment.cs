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
        var projected = new SpatialLine3DProjector().Project(
            model.Lines,
            new SpatialOrthographicCamera(Scale: 0.72),
            SpatialLineStyleCatalog.Blueprint);

        using var linework = new SpatialLinework();
        foreach (var line in projected)
        {
            linework.BeginLine(line.Points[0]);
            linework.ContinueLine(line.Points[1]);
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

/// <summary>Produces a deliberately non-trivial 3D line specimen for the laboratory.</summary>
public sealed class SpatialLine3DModel
{
    private SpatialLine3DModel(IReadOnlyList<SpatialLine3> lines) => Lines = lines;

    public IReadOnlyList<SpatialLine3> Lines { get; }

    public static SpatialLine3DModel CreateTestTower()
    {
        var lines = new List<SpatialLine3>();
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
                    return new SpatialPoint3(Math.Cos(a) * radius, Math.Sin(a) * radius, z);
                }).ToArray();

            for (var i = 0; i < sides; i++)
            {
                var next = points[(i + 1) % sides];
                lines.Add(new SpatialLine3(lines.Count, points[i], next));
            }
        }

        var baseRing = Enumerable.Range(0, sides)
            .Select(i =>
            {
                var a = i * Math.PI * 2d / sides;
                return new SpatialPoint3(Math.Cos(a) * radius, Math.Sin(a) * radius, 0);
            }).ToArray();

        var topRing = Enumerable.Range(0, sides)
            .Select(i =>
            {
                var a = i * Math.PI * 2d / sides;
                return new SpatialPoint3(Math.Cos(a) * radius, Math.Sin(a) * radius, floors * floorHeight);
            }).ToArray();

        for (var i = 0; i < sides; i++)
            lines.Add(new SpatialLine3(lines.Count, baseRing[i], topRing[i]));

        // Cross-bracing deliberately increases primitive count without changing
        // the source geometry contract used by the projector.
        for (var floor = 0; floor < floors; floor++)
        {
            var z1 = floor * floorHeight;
            var z2 = (floor + 1) * floorHeight;
            for (var i = 0; i < sides; i += 2)
            {
                var a = i * Math.PI * 2d / sides;
                var b = ((i + 1) % sides) * Math.PI * 2d / sides;
                lines.Add(new SpatialLine3(
                    lines.Count,
                    new SpatialPoint3(Math.Cos(a) * radius, Math.Sin(a) * radius, z1),
                    new SpatialPoint3(Math.Cos(b) * radius, Math.Sin(b) * radius, z2)));
            }
        }

        return new SpatialLine3DModel(lines);
    }
}
