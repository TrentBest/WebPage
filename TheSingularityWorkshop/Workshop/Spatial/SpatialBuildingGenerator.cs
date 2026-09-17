namespace TheSingularityWorkshop.Gui;

/// <summary>Describes a composable 2D building that can later be projected into additional dimensions.</summary>
public sealed record SpatialBuildingSpec(
    string Id,
    string Name,
    SpatialBounds Bounds,
    int Columns = 0,
    int Rows = 0,
    int Detail = 1,
    string Accent = "#00eaff",
    string Fill = "#0d2026",
    IReadOnlyList<SpatialOpening>? Openings = null)
{
    /// <summary>Returns the declared openings or an empty collection.</summary>
    public IReadOnlyList<SpatialOpening> BuildingOpenings => Openings ?? [];
}

/// <summary>
/// Generates architectural linework from a compact building specification.
/// This is deliberately a generator rather than a drawing tool: the user composes a building
/// from semantic dimensions and modules, while the renderer remains responsible for perception.
/// </summary>
public static class SpatialBuildingGenerator
{
    /// <summary>Creates the geometry descriptor consumed by navigation and renderers.</summary>
    public static SpatialBuildingGeometry CreateGeometry(SpatialBuildingSpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);
        return new SpatialBuildingGeometry(spec.Id, spec.Bounds, spec.Accent, spec.Fill, spec.BuildingOpenings);
    }

    /// <summary>
    /// Creates linework for a building footprint and optional room matrix.
    /// Detail multiplies the room grid without changing the building's world-space footprint.
    /// </summary>
    public static SpatialLinework CreateLinework(SpatialBuildingSpec spec)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec.Detail < 1) throw new ArgumentOutOfRangeException(nameof(spec), "Building detail must be at least one.");
        if (spec.Columns < 0) throw new ArgumentOutOfRangeException(nameof(spec), "Building columns cannot be negative.");
        if (spec.Rows < 0) throw new ArgumentOutOfRangeException(nameof(spec), "Building rows cannot be negative.");

        var linework = new SpatialLinework();
        AddRectangle(linework, spec.Bounds);

        var columns = spec.Columns * spec.Detail;
        var rows = spec.Rows * spec.Detail;
        if (columns == 0 || rows == 0) return linework;

        var stepX = spec.Bounds.Width / columns;
        var stepY = spec.Bounds.Height / rows;
        for (var column = 1; column < columns; column++)
        {
            var x = spec.Bounds.X + stepX * column;
            AddLine(linework, x, spec.Bounds.Y, x, spec.Bounds.Y + spec.Bounds.Height);
        }

        for (var row = 1; row < rows; row++)
        {
            var y = spec.Bounds.Y + stepY * row;
            AddLine(linework, spec.Bounds.X, y, spec.Bounds.X + spec.Bounds.Width, y);
        }

        return linework;
    }

    /// <summary>Creates an interactable whose collision and interaction metadata share the same footprint.</summary>
    public static SpatialInteractable CreateInteractable(SpatialBuildingSpec spec, string experienceId, SpatialBounds interactionPoint)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (string.IsNullOrWhiteSpace(experienceId)) throw new ArgumentException("An experience ID is required.", nameof(experienceId));

        // Keep the building's broad-phase footprint intact, but inset the precise pointer region.
        // This leaves the exact edge/corner pixels available to empty-space navigation.
        return new SpatialInteractable(
            spec.Id,
            spec.Name,
            spec.Bounds,
            interactionPoint,
            new SpatialRectangularHitRegion(.05, .05, .9, .9),
            experienceId,
            openings: spec.BuildingOpenings);
    }

    private static void AddRectangle(SpatialLinework linework, SpatialBounds bounds)
    {
        AddLine(linework, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y);
        AddLine(linework, bounds.X + bounds.Width, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);
        AddLine(linework, bounds.X + bounds.Width, bounds.Y + bounds.Height, bounds.X, bounds.Y + bounds.Height);
        AddLine(linework, bounds.X, bounds.Y + bounds.Height, bounds.X, bounds.Y);
    }

    private static void AddLine(SpatialLinework linework, double x1, double y1, double x2, double y2)
    {
        linework.BeginLine(new SpatialPoint(x1, y1));
        linework.ContinueLine(new SpatialPoint(x2, y2));
        linework.EndLine();
    }
}
