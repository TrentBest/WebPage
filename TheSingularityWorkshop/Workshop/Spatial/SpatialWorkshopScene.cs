namespace TheSingularityWorkshop.Gui;

/// <summary>The logical spatial scene presented by Explore.</summary>
public sealed class SpatialWorkshopScene
{
    private SpatialWorkshopScene(string startingAreaId, SpatialPoint startingPosition, IReadOnlyList<SpatialInteractable> interactables, SpatialViewSettings view, IReadOnlyList<ConstructionVehicle> constructionVehicles)
    {
        StartingAreaId = startingAreaId;
        StartingPosition = startingPosition;
        Interactables = interactables;
        View = view;
        ConstructionVehicles = constructionVehicles;
    }

    public string StartingAreaId { get; }
    public SpatialPoint StartingPosition { get; }
    public IReadOnlyList<SpatialInteractable> Interactables { get; }
    public SpatialViewSettings View { get; }
    public IReadOnlyList<ConstructionVehicle> ConstructionVehicles { get; }

    /// <summary>Creates the first high-tech Workshop complex.</summary>
    public static SpatialWorkshopScene CreateDefault()
        => new("engineering", new SpatialPoint(73, 42.5),
            [
                new SpatialInteractable("forge", "The Forge", new SpatialBounds(58, 20, 30, 45), new SpatialBounds(70, 39, 6, 5), new SpatialRectangularHitRegion(0, 0, 1, 1)),
                new SpatialInteractable("image-tools", "Image Workshop", new SpatialBounds(8, 18, 18, 14), new SpatialBounds(17, 24, 4, 3), new SpatialRectangularHitRegion(.08, .08, .84, .84)),
                new SpatialInteractable("npc-studio", "NPC Studio", new SpatialBounds(12, 62, 20, 16), new SpatialBounds(21, 69, 4, 3), new SpatialRectangularHitRegion(.06, .06, .88, .88)),
                new SpatialInteractable("fsm-bench", "FSM Workbench", new SpatialBounds(61, 32, 16, 12), new SpatialBounds(69, 37, 4, 3), new SpatialRectangularHitRegion(.04, .04, .92, .92)),
                new SpatialInteractable("storage-bins", "Storage Bins", new SpatialBounds(80, 25, 12, 20), new SpatialBounds(86, 34, 3, 3), new SpatialRectangularHitRegion(.05, .05, .9, .9)),
                new SpatialInteractable("blueprint-library", "Blueprint Library", new SpatialBounds(38, 16, 16, 14), new SpatialBounds(46, 22, 3, 3), new SpatialRectangularHitRegion(.05, .05, .9, .9))
            ],
            SpatialViewSettings.CreateDefault(),
            [
                new ConstructionVehicle("crane-01", "Fabrication Crane", ConstructionVehicleKind.Crane, new SpatialPoint(54, 55)),
                new ConstructionVehicle("backhoe-01", "Site Tractor", ConstructionVehicleKind.Backhoe, new SpatialPoint(43, 52)),
                new ConstructionVehicle("rover-01", "Site Rover", ConstructionVehicleKind.Rover, new SpatialPoint(35, 55)),
                new ConstructionVehicle("lifter-01", "Material Lifter", ConstructionVehicleKind.Lifter, new SpatialPoint(87, 56))
            ]);
}

/// <summary>Logical two-dimensional position in Workshop world coordinates.</summary>
public readonly record struct SpatialPoint(double X, double Y);

/// <summary>Maximum spatial rectangle occupied by an interactable.</summary>
public readonly record struct SpatialBounds(double X, double Y, double Width, double Height);

/// <summary>Normalized pointer coordinate relative to an interactable.</summary>
public readonly record struct NormalizedPointer(double X, double Y);

/// <summary>Simple normalized rectangular hit region. More expressive regions can replace it later.</summary>
public readonly record struct SpatialRectangularHitRegion(double X, double Y, double Width, double Height)
{
    public bool Contains(NormalizedPointer point) => point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
}

/// <summary>A world object that owns its spatial hover and interaction lifecycle.</summary>
public sealed class SpatialInteractable
{
    public SpatialInteractable(string id, string name, SpatialBounds bounds, SpatialBounds interactionPoint, SpatialRectangularHitRegion hitRegion)
    {
        Id = id;
        Name = name;
        Bounds = bounds;
        InteractionPoint = interactionPoint;
        HitRegion = hitRegion;
    }

    public string Id { get; }
    public string Name { get; }
    public SpatialBounds Bounds { get; }
    public SpatialBounds InteractionPoint { get; }
    public SpatialRectangularHitRegion HitRegion { get; }
    public bool IsBreathing { get; private set; }
    public NormalizedPointer? LastHover { get; private set; }
    public int InteractionCount { get; private set; }
    public event Action? Interaction;

    public bool OnHover(NormalizedPointer point)
    {
        if (!HitRegion.Contains(point)) return false;
        LastHover = point;
        IsBreathing = true;
        return true;
    }

    public void OnHoverExit() => IsBreathing = false;

    /// <summary>Signals that the visitor has physically arrived at this object's interaction position.</summary>
    public void OnInteraction()
    {
        InteractionCount++;
        Interaction?.Invoke();
    }

    public bool OnClick(NormalizedPointer point) => HitRegion.Contains(point);
}

/// <summary>Presentation-independent view controls for a spatial experience.</summary>
public sealed class SpatialViewSettings
{
    private SpatialViewSettings(IReadOnlyList<SpatialDetailLevel> detailLevels) => DetailLevels = detailLevels;
    public IReadOnlyList<SpatialDetailLevel> DetailLevels { get; }
    public static SpatialViewSettings CreateDefault()
    {
        var levels = Enumerable.Range(1, 62).Select(i => new SpatialDetailLevel($"LEVEL-{i:00}", i / 64d)).ToList();
        levels.Add(SpatialDetailLevel.Device);
        levels.Add(SpatialDetailLevel.Stud);
        return new SpatialViewSettings(levels);
    }
}

/// <summary>A selectable level of spatial detail.</summary>
public readonly record struct SpatialDetailLevel(string Name, double Precision)
{
    public static SpatialDetailLevel Device => new("DEVICE", 1d / 32d);
    public static SpatialDetailLevel Stud => new("STUD", 1d / 64d);
}

/// <summary>A piece of construction activity visible while the complex is being built.</summary>
public sealed record ConstructionVehicle(string Id, string Name, ConstructionVehicleKind Kind, SpatialPoint Position);

/// <summary>Construction vehicle roles used by the initial Under Construction scene.</summary>
public enum ConstructionVehicleKind
{
    Backhoe,
    Crane,
    Rover,
    Lifter
}
