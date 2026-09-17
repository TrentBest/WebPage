namespace TheSingularityWorkshop.Gui;

/// <summary>Semantic module kinds used to compose a building without drawing individual lines.</summary>
public enum SpatialBuildingModuleKind
{
    Room,
    Corridor,
    Courtyard,
    Stair,
    Elevator,
    Escalator,
    Apartment,
    Shop,
    Arcade
}

/// <summary>A semantic building module that a generator can turn into perception geometry.</summary>
public readonly record struct SpatialBuildingModule(
    string Id,
    SpatialBuildingModuleKind Kind,
    SpatialBounds Bounds,
    int Floors = 1,
    int Detail = 1);

/// <summary>
/// A composable building recipe. It describes intent and composition; it does not contain renderer-specific lines.
/// The same recipe can therefore feed the current 2D line renderer, an orthogonal 3D projection, or a future Revit adapter.
/// </summary>
public sealed class SpatialBuildingRecipe
{
    private readonly List<SpatialBuildingModule> _modules = [];
    private readonly List<SpatialOpening> _openings = [];

    public SpatialBuildingRecipe(string id, string name, SpatialBounds bounds)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A building ID is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A building name is required.", nameof(name));

        Id = id;
        Name = name;
        Bounds = bounds;
    }

    public string Id { get; }
    public string Name { get; }
    public SpatialBounds Bounds { get; }
    public IReadOnlyList<SpatialBuildingModule> Modules => _modules;
    public IReadOnlyList<SpatialOpening> Openings => _openings;

    /// <summary>Adds a semantic module to the recipe.</summary>
    public SpatialBuildingRecipe AddModule(SpatialBuildingModule module)
    {
        if (string.IsNullOrWhiteSpace(module.Id))
            throw new ArgumentException("A building module ID is required.", nameof(module));
        if (module.Floors < 1)
            throw new ArgumentOutOfRangeException(nameof(module), "A building module must have at least one floor.");
        if (module.Detail < 1)
            throw new ArgumentOutOfRangeException(nameof(module), "A building module detail level must be at least one.");

        _modules.Add(module);
        return this;
    }

    /// <summary>Adds a semantic door, window, or passage to the recipe.</summary>
    public SpatialBuildingRecipe AddOpening(SpatialOpening opening)
    {
        _openings.Add(opening);
        return this;
    }
}

/// <summary>Turns semantic building recipes into the current perception-neutral line substrate.</summary>
public static class SpatialBuildingRecipeGenerator
{
    /// <summary>Generates a floor-plan linework representation from a building recipe.</summary>
    public static SpatialLinework CreateLinework(SpatialBuildingRecipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);

        var linework = new SpatialLinework();
        AddRectangle(linework, recipe.Bounds);

        foreach (var module in recipe.Modules)
        {
            AddRectangle(linework, module.Bounds);

            if (module.Kind is SpatialBuildingModuleKind.Stair or SpatialBuildingModuleKind.Escalator)
                AddRepeatedLines(linework, module.Bounds, Math.Max(4, 4 * module.Detail));

            if (module.Kind == SpatialBuildingModuleKind.Elevator)
                AddCross(linework, module.Bounds);

            if (module.Kind == SpatialBuildingModuleKind.Arcade)
                AddGrid(linework, module.Bounds, Math.Max(2, 2 * module.Detail), Math.Max(2, 2 * module.Detail));
        }

        return linework;
    }

    private static void AddGrid(SpatialLinework linework, SpatialBounds bounds, int columns, int rows)
    {
        for (var column = 1; column < columns; column++)
        {
            var x = bounds.X + bounds.Width * column / columns;
            AddLine(linework, x, bounds.Y, x, bounds.Y + bounds.Height);
        }

        for (var row = 1; row < rows; row++)
        {
            var y = bounds.Y + bounds.Height * row / rows;
            AddLine(linework, bounds.X, y, bounds.X + bounds.Width, y);
        }
    }

    private static void AddRepeatedLines(SpatialLinework linework, SpatialBounds bounds, int count)
    {
        for (var i = 1; i < count; i++)
        {
            var y = bounds.Y + bounds.Height * i / count;
            AddLine(linework, bounds.X, y, bounds.X + bounds.Width, y);
        }
    }

    private static void AddCross(SpatialLinework linework, SpatialBounds bounds)
    {
        AddLine(linework, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height);
        AddLine(linework, bounds.X + bounds.Width, bounds.Y, bounds.X, bounds.Y + bounds.Height);
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
