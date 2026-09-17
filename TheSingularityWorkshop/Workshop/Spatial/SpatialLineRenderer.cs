namespace TheSingularityWorkshop.Gui;

/// <summary>A renderer-neutral line ready for a perception backend.</summary>
public readonly record struct SpatialRenderedLine(
    SpatialLine Source,
    IReadOnlyList<SpatialPoint> Points,
    SpatialLineStyle Style);

/// <summary>
/// Renderer abstraction that converts persistent linework plus a reusable style
/// into perception-ready point sequences. SVG, WebGL and other backends can consume
/// the same rendered representation without owning the line data.
/// </summary>
public sealed class SpatialLineRenderer
{
    /// <summary>Produces styled line geometry for every committed line.</summary>
    public IReadOnlyList<SpatialRenderedLine> Render(
        SpatialLinework linework,
        SpatialLineStyle style)
    {
        ArgumentNullException.ThrowIfNull(linework);
        ArgumentNullException.ThrowIfNull(style);

        return linework.Lines
            .Select(line => new SpatialRenderedLine(
                line,
                BuildPoints(line, style),
                style))
            .ToArray();
    }

    private static IReadOnlyList<SpatialPoint> BuildPoints(SpatialLine line, SpatialLineStyle style)
    {
        if (style.Effect != SpatialLineEffect.Squiggly)
            return [line.Start, line.End];

        const int segments = 12;
        var dx = line.End.X - line.Start.X;
        var dy = line.End.Y - line.Start.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length <= double.Epsilon)
            return [line.Start, line.End];

        var normalX = -dy / length;
        var normalY = dx / length;
        var amplitude = Math.Min(Math.Max(style.EffectAmount, 0d), 2d);
        var points = new SpatialPoint[segments + 1];

        for (var i = 0; i <= segments; i++)
        {
            var t = i / (double)segments;
            var wave = Math.Sin(t * Math.PI * 4d) * amplitude;
            points[i] = new SpatialPoint(
                line.Start.X + dx * t + normalX * wave,
                line.Start.Y + dy * t + normalY * wave);
        }

        return points;
    }
}
