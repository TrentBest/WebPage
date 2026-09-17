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

    /// <summary>Creates the first high-tech Workshop complex around a clear central origin.</summary>
    public static SpatialWorkshopScene CreateDefault()
        => new("center", new SpatialPoint(50, 50),
            [
                new SpatialInteractable("forge", "The Forge", new SpatialBounds(72, 14, 20, 25), new SpatialBounds(78, 41, 4, 3), new SpatialRectangularHitRegion(0, 0, 1, 1), "forge", openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 7, 13), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Left, 7, 12)]),
                new SpatialInteractable("image-tools", "Image Workshop", new SpatialBounds(8, 14, 18, 14), new SpatialBounds(17, 29, 4, 3), new SpatialRectangularHitRegion(.08, .08, .84, .84), "image-workshop", openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 7, 11), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Right, 4, 9)]),
                new SpatialInteractable("npc-studio", "NPC Studio", new SpatialBounds(8, 72, 20, 14), new SpatialBounds(18, 68, 4, 3), new SpatialRectangularHitRegion(.06, .06, .88, .88), "npc-studio", openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Top, 8, 12), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Right, 4, 10)]),
                new SpatialInteractable("fsm-bench", "FSM Workbench", new SpatialBounds(68, 56, 16, 11), new SpatialBounds(75, 52, 4, 3), new SpatialRectangularHitRegion(.04, .04, .92, .92), "fsm-workbench", openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Top, 6, 10)]),
                new SpatialInteractable("storage-bins", "Storage Bins", new SpatialBounds(88, 58, 8, 17), new SpatialBounds(85, 66, 3, 3), new SpatialRectangularHitRegion(.05, .05, .9, .9), "storage", openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Left, 7, 11)]),
                new SpatialInteractable("blueprint-library", "Blueprint Library", new SpatialBounds(32, 12, 18, 14), new SpatialBounds(41, 28, 3, 3), new SpatialRectangularHitRegion(.05, .05, .9, .9), "library", openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 7, 11), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Top, 4, 10)])
            ],
            SpatialViewSettings.CreateDefault(),
            [
                new ConstructionVehicle("crane-01", "Fabrication Crane", ConstructionVehicleKind.Crane, new SpatialPoint(64, 72)),
                new ConstructionVehicle("backhoe-01", "Site Tractor", ConstructionVehicleKind.Backhoe, new SpatialPoint(37, 70)),
                new ConstructionVehicle("rover-01", "Site Rover", ConstructionVehicleKind.Rover, new SpatialPoint(31, 55)),
                new ConstructionVehicle("lifter-01", "Material Lifter", ConstructionVehicleKind.Lifter, new SpatialPoint(89, 47))
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

/// <summary>A named place from which an interactable can be operated.</summary>
public readonly record struct SpatialInteractionPoint(string Id, string Name, SpatialBounds Bounds);

/// <summary>A world object that owns its spatial hover and interaction lifecycle.</summary>
public sealed class SpatialInteractable
{
    public SpatialInteractable(string id, string name, SpatialBounds bounds, SpatialBounds interactionPoint, SpatialRectangularHitRegion hitRegion, string experienceId, IReadOnlyList<SpatialInteractionPoint>? interactionPoints = null, IReadOnlyList<SpatialOpening>? openings = null)
    {
        Id = id;
        Name = name;
        Bounds = bounds;
        InteractionPoint = interactionPoint;
        HitRegion = hitRegion;
        ExperienceId = experienceId;
        InteractionPoints = interactionPoints is { Count: > 0 } ? interactionPoints : [new SpatialInteractionPoint("default", "Primary", interactionPoint)];
        Openings = openings ?? [];
    }

    /// <summary>Creates a vehicle-shaped interactable whose stations can be approached independently.</summary>
    public static SpatialInteractable CreateVehicle(string id, string name, SpatialBounds bounds, IReadOnlyList<SpatialInteractionPoint> interactionPoints, string experienceId)
    {
        if (interactionPoints.Count == 0)
            throw new ArgumentException("A vehicle interactable must expose at least one interaction point.", nameof(interactionPoints));
        return new SpatialInteractable(id, name, bounds, interactionPoints[0].Bounds, new SpatialRectangularHitRegion(0, 0, 1, 1), experienceId, interactionPoints);
    }

    public string Id { get; }
    public string Name { get; }
    public SpatialBounds Bounds { get; }
    public SpatialBounds InteractionPoint { get; }
    public SpatialRectangularHitRegion HitRegion { get; }
    public string ExperienceId { get; }
    public IReadOnlyList<SpatialInteractionPoint> InteractionPoints { get; }
    public IReadOnlyList<SpatialOpening> Openings { get; }
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

    /// <summary>Signals that the visitor has physically arrived at this object's selected interaction position.</summary>
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
public enum ConstructionVehicleKind { Backhoe, Crane, Rover, Lifter }
