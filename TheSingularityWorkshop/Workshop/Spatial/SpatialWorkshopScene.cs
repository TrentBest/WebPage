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

    /// <summary>Creates the first campus composition from the declarative building catalog.</summary>
    public static SpatialWorkshopScene CreateDefault()
        => new("center", new SpatialPoint(50, 50),
            [
                Building("singularity-mansion", "mansion", new SpatialBounds(34, 22, 4, 3)),
                Building("image-tools", "image-workshop", new SpatialBounds(49, 23, 4, 3)),
                Building("forge", "forge", new SpatialBounds(77, 29, 4, 3)),
                Building("blueprint-library", "library", new SpatialBounds(49, 43, 3, 3)),
                Building("singularity-transit", "singularity-transit", new SpatialBounds(78, 49, 4, 3)),
                Building("npc-studio", "npc-studio", new SpatialBounds(11, 45, 4, 3)),
                Building("fsm-bench", "fsm-workbench", new SpatialBounds(36, 43, 4, 3)),
                Building("storage-bins", "storage", new SpatialBounds(87, 66, 3, 3)),
                Building("singularity-ontology-mall", "singularity-ontology-mall", new SpatialBounds(50, 58, 5, 3))
            ],
            SpatialViewSettings.CreateDefault(),
            [
                new ConstructionVehicle("crane-01", "Fabrication Crane", ConstructionVehicleKind.Crane, new SpatialPoint(64, 72)),
                new ConstructionVehicle("backhoe-01", "Site Tractor", ConstructionVehicleKind.Backhoe, new SpatialPoint(37, 70)),
                new ConstructionVehicle("rover-01", "Site Rover", ConstructionVehicleKind.Rover, new SpatialPoint(31, 55)),
                new ConstructionVehicle("lifter-01", "Material Lifter", ConstructionVehicleKind.Lifter, new SpatialPoint(89, 47))
            ]);

    private static SpatialInteractable Building(string id, string experienceId, SpatialBounds interactionPoint)
    {
        var spec = SingularityCampusCatalog.Buildings.Single(item => item.Id == id);
        return SpatialBuildingGenerator.CreateInteractable(spec, experienceId, interactionPoint);
    }
}

public readonly record struct SpatialPoint(double X, double Y);

public readonly record struct SpatialBounds(double X, double Y, double Width, double Height)
{
    public bool Contains(SpatialPoint point) => point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
    public NormalizedPointer Normalize(SpatialPoint point) => new(Width <= 0 ? 0 : (point.X - X) / Width, Height <= 0 ? 0 : (point.Y - Y) / Height);
}

public readonly record struct NormalizedPointer(double X, double Y);

/// <summary>Hit-test contract for non-rectangular spatial objects.</summary>
public interface ISpatialHitRegion
{
    bool Contains(NormalizedPointer point);
}

public readonly record struct SpatialRectangularHitRegion(double X, double Y, double Width, double Height) : ISpatialHitRegion
{
    public bool Contains(NormalizedPointer point) => point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height;
}

/// <summary>Normalized polygon hit region using a standard point-in-polygon test.</summary>
public sealed class SpatialPolygonHitRegion : ISpatialHitRegion
{
    private readonly IReadOnlyList<NormalizedPointer> _vertices;

    public SpatialPolygonHitRegion(IEnumerable<NormalizedPointer> vertices)
    {
        _vertices = vertices?.ToArray() ?? throw new ArgumentNullException(nameof(vertices));
        if (_vertices.Count < 3) throw new ArgumentException("A polygon requires at least three vertices.", nameof(vertices));
    }

    public bool Contains(NormalizedPointer point)
    {
        var inside = false;
        for (var i = 0; i < _vertices.Count; i++)
        {
            var a = _vertices[i];
            var b = _vertices[(i + 1) % _vertices.Count];
            if ((a.Y > point.Y) == (b.Y > point.Y)) continue;
            var x = (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X;
            if (point.X < x) inside = !inside;
        }
        return inside;
    }
}

public readonly record struct SpatialInteractionPoint(string Id, string Name, SpatialBounds Bounds);

public sealed class SpatialInteractable
{
    public SpatialInteractable(string id, string name, SpatialBounds bounds, SpatialBounds interactionPoint, ISpatialHitRegion hitRegion, string experienceId, IReadOnlyList<SpatialInteractionPoint>? interactionPoints = null, IReadOnlyList<SpatialOpening>? openings = null)
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

    public static SpatialInteractable CreateVehicle(string id, string name, SpatialBounds bounds, IReadOnlyList<SpatialInteractionPoint> interactionPoints, string experienceId)
    {
        if (interactionPoints.Count == 0) throw new ArgumentException("A vehicle interactable must expose at least one interaction point.", nameof(interactionPoints));
        return new SpatialInteractable(id, name, bounds, interactionPoints[0].Bounds, new SpatialRectangularHitRegion(0, 0, 1, 1), experienceId, interactionPoints);
    }

    public string Id { get; }
    public string Name { get; }
    public SpatialBounds Bounds { get; }
    public SpatialBounds InteractionPoint { get; }
    public ISpatialHitRegion HitRegion { get; }
    public string ExperienceId { get; }
    public IReadOnlyList<SpatialInteractionPoint> InteractionPoints { get; }
    public IReadOnlyList<SpatialOpening> Openings { get; }
    public bool IsBreathing { get; private set; }
    public NormalizedPointer? LastHover { get; private set; }
    public int InteractionCount { get; private set; }
    public event Action? Interaction;

    /// <summary>Checks the cheap maximum bounding box before asking the object's actual shape.</summary>
    public bool HitTest(SpatialPoint point)
    {
        if (!Bounds.Contains(point)) return false;
        return HitRegion.Contains(Bounds.Normalize(point));
    }

    public bool OnHover(NormalizedPointer point)
    {
        if (!HitRegion.Contains(point)) return false;
        LastHover = point;
        IsBreathing = true;
        return true;
    }

    public void OnHoverExit() => IsBreathing = false;
    public void OnInteraction() { InteractionCount++; Interaction?.Invoke(); }
    public bool OnClick(NormalizedPointer point) => HitRegion.Contains(point);
}

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

public readonly record struct SpatialDetailLevel(string Name, double Precision)
{
    public static SpatialDetailLevel Device => new("DEVICE", 1d / 32d);
    public static SpatialDetailLevel Stud => new("STUD", 1d / 64d);
}

public sealed record ConstructionVehicle(string Id, string Name, ConstructionVehicleKind Kind, SpatialPoint Position);
public enum ConstructionVehicleKind { Backhoe, Crane, Rover, Lifter }
