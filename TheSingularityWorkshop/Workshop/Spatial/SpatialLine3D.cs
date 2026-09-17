namespace TheSingularityWorkshop.Gui;

/// <summary>A point in the Workshop's renderer-neutral three-dimensional coordinate space.</summary>
public readonly record struct SpatialPoint3(double X, double Y, double Z);

/// <summary>A three-dimensional line primitive independent of the current SVG perception layer.</summary>
public readonly record struct SpatialLine3(int Index, SpatialPoint3 Start, SpatialPoint3 End);

/// <summary>Two RGBA texels containing the six floating-point coordinates of one 3D line.</summary>
public readonly record struct SpatialLine3TextureRecord(
    SpatialLineTextureTexel Start,
    SpatialLineTextureTexel End);

/// <summary>
/// GPU-oriented representation for 3D lines. Two texels are used because a line has six coordinates:
/// X1,Y1,Z1 and X2,Y2,Z2. The fourth channel is reserved for future adjacency/metadata.
/// </summary>
public sealed class SpatialLine3TextureBuffer
{
    private SpatialLine3TextureBuffer(IReadOnlyList<SpatialLine3TextureRecord> records) => Records = records;

    public int Width => Records.Count * 2;
    public const int Height = 1;
    public IReadOnlyList<SpatialLine3TextureRecord> Records { get; }

    /// <summary>Encodes each 3D line into two RGBA records without projecting it.</summary>
    public static SpatialLine3TextureBuffer FromLines(IReadOnlyList<SpatialLine3> lines)
        => new(lines.Select(line => new SpatialLine3TextureRecord(
            new SpatialLineTextureTexel((float)line.Start.X, (float)line.Start.Y, (float)line.Start.Z, 0),
            new SpatialLineTextureTexel((float)line.End.X, (float)line.End.Y, (float)line.End.Z, 0))).ToArray());
}

/// <summary>Simple orthographic camera used to test a Bard's Tale-style spatial perception without requiring a 3D engine.</summary>
public readonly record struct SpatialOrthographicCamera(
    double YawRadians = -0.7853981633974483,
    double PitchRadians = 0.5235987755982988,
    double Scale = 1d)
{
    /// <summary>Projects a 3D point into the existing 2D line perception coordinate system.</summary>
    public SpatialPoint Project(SpatialPoint3 point)
    {
        var cosYaw = Math.Cos(YawRadians);
        var sinYaw = Math.Sin(YawRadians);
        var cameraX = point.X * cosYaw - point.Z * sinYaw;
        var cameraZ = point.X * sinYaw + point.Z * cosYaw;

        var cosPitch = Math.Cos(PitchRadians);
        var sinPitch = Math.Sin(PitchRadians);
        var screenY = point.Y * cosPitch - cameraZ * sinPitch;
        var screenX = cameraX;

        return new SpatialPoint(screenX * Scale, screenY * Scale);
    }
}

/// <summary>Projects 3D lines into the same rendered-line contract consumed by the current SVG backend.</summary>
public sealed class SpatialLine3DProjector
{
    /// <summary>Projects every 3D line while preserving the original 3D source for later GPU backends.</summary>
    public IReadOnlyList<SpatialRenderedLine3D> Project(
        IReadOnlyList<SpatialLine3> lines,
        SpatialOrthographicCamera camera,
        SpatialLineStyle style)
    {
        ArgumentNullException.ThrowIfNull(lines);
        ArgumentNullException.ThrowIfNull(style);

        return lines
            .Select(line => new SpatialRenderedLine3D(
                line,
                [camera.Project(line.Start), camera.Project(line.End)],
                style))
            .ToArray();
    }
}

/// <summary>A 3D source line plus its projected perception-space representation.</summary>
public readonly record struct SpatialRenderedLine3D(
    SpatialLine3 Source,
    IReadOnlyList<SpatialPoint> Points,
    SpatialLineStyle Style);
