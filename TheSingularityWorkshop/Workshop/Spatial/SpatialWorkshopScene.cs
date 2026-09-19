namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>The logical spatial scene presented by Explore.</summary>
public sealed class SpatialWorkshopScene
{
    private SpatialWorkshopScene(string startingAreaId, SpatialPoint startingPosition, IReadOnlyList<SpatialInteractable> interactables, SpatialViewSettings view, SpatialConstructionSite constructionSite, IReadOnlyList<ConstructionVehicle> constructionVehicles, SpatialConstructionExperience constructionExperience)
    {
        SpatialPlaceCatalog.Validate(interactables);
        ValidateNoStructuralOverlap(interactables);

        StartingAreaId = startingAreaId;
        StartingPosition = startingPosition;
        Interactables = interactables;
        View = view;
        ConstructionSite = constructionSite;
        ConstructionVehicles = constructionVehicles;
        ConstructionExperience = constructionExperience;
    }

    public string StartingAreaId { get; }
    public SpatialPoint StartingPosition { get; }
    public IReadOnlyList<SpatialInteractable> Interactables { get; }
    public SpatialViewSettings View { get; }
    public SpatialConstructionSite ConstructionSite { get; }
    public IReadOnlyList<ConstructionVehicle> ConstructionVehicles { get; }
    public SpatialConstructionExperience ConstructionExperience { get; }

    /// <summary>Creates the first campus composition from the declarative building catalog.</summary>
    public static SpatialWorkshopScene CreateDefault()
        => new("center", new SpatialPoint(50, 50),
            [
                Building("singularity-mansion", "mansion", new SpatialBounds(20, 20, 5, 4)),
                Building("image-tools", "image-workshop", new SpatialBounds(48, 11, 5, 3)),
                Building("forge", "forge", new SpatialBounds(79, 37, 5, 4)),
                Building("blueprint-library", "library", new SpatialBounds(45, 34, 4, 4)),
                Building("singularity-transit", "singularity-transit", new SpatialBounds(88, 46, 5, 4)),
                Building("npc-studio", "npc-studio", new SpatialBounds(10, 51, 5, 4)),
                Building("fsm-bench", "fsm-workbench", new SpatialBounds(21, 59, 5, 3)),
                Building("storage-bins", "storage", new SpatialBounds(94, 68, 4, 4)),
                Building("singularity-ontology-mall", "singularity-ontology-mall", new SpatialBounds(76, 62, 6, 4)),
                Building("singularity-lab", "singularity-lab", new SpatialBounds(10, 77, 7, 5)),
                Building("singularity-station", "singularity-station", new SpatialBounds(82, 8, 5, 3)),
                Building("singularity-shipyard", "singularity-shipyard", new SpatialBounds(90, 24, 5, 4)),
                Building("singularity-capitol", "singularity-capitol", new SpatialBounds(64, 9, 5, 3)),
                Building("ocean-shipyard", "ocean-shipyard", new SpatialBounds(61, 88, 5, 3)),
                new SpatialInteractable("maze", "Workshop Maze", new SpatialBounds(45, 50, 13, 8), new SpatialBounds(50, 58, 4, 2), new SpatialRectangularHitRegion(0, 0, 1, 1), "maze"),
                new SpatialInteractable("aec-remote-office", "AEC Remote Office", new SpatialBounds(14, 40, 13, 8), new SpatialBounds(20, 46, 3, 2), new SpatialRectangularHitRegion(0, 0, 1, 1), "aec-remote-office"),
                new SpatialInteractable("singularity-taxi", "Singularity Taxi", new SpatialBounds(54, 21, 8, 4), new SpatialBounds(60, 21, 3, 2), new SpatialRectangularHitRegion(0, 0, 1, 1), "singularity-taxi")
            ],
            SpatialViewSettings.CreateDefault(),
            new SpatialConstructionSite(
                "workshop-expansion",
                "WORKSHOP EXPANSION SITE",
                new SpatialBounds(34, 64, 24, 22),
                new SpatialBounds(39, 69, 14, 12),
                SpatialConstructionPhase.Foundation,
                0.22,
                "Creation and production facilities are being established here."),
            [
                new ConstructionVehicle("crane-01", "Fabrication Crane", ConstructionVehicleKind.Crane, new SpatialPoint(48, 72)),
                new ConstructionVehicle("backhoe-01", "Site Tractor", ConstructionVehicleKind.Backhoe, new SpatialPoint(38, 80)),
                new ConstructionVehicle("rover-01", "Site Rover", ConstructionVehicleKind.Rover, new SpatialPoint(40, 70)),
                new ConstructionVehicle("lifter-01", "Material Lifter", ConstructionVehicleKind.Lifter, new SpatialPoint(54, 84))
            ],
            SpatialConstructionExperienceCatalog.Expansion);

    private static void ValidateNoStructuralOverlap(IReadOnlyList<SpatialInteractable> interactables)
    {
        for (var i = 0; i < interactables.Count; i++)
        {
            for (var j = i + 1; j < interactables.Count; j++)
            {
                if (Overlaps(interactables[i].Bounds, interactables[j].Bounds))
                    throw new InvalidOperationException($"Spatial structures '{interactables[i].Name}' and '{interactables[j].Name}' overlap.");
            }
        }
    }

    private static bool Overlaps(SpatialBounds a, SpatialBounds b)
        => a.X < b.X + b.Width && b.X < a.X + a.Width &&
           a.Y < b.Y + b.Height && b.Y < a.Y + a.Height;

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
        // Generic geometry/interactions may exist before they are attached to an Explore place.
        // Root-world scenes are validated by SpatialWorkshopScene through SpatialPlaceCatalog.Validate.
        Place = SpatialPlaceCatalog.TryResolve(experienceId, out var place) ? place : null;
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
    public SpatialPlace? Place { get; }
    public IReadOnlyList<SpatialInteractionPoint> InteractionPoints { get; }
    public IReadOnlyList<SpatialOpening> Openings { get; }
    public bool IsBreathing { get; private set; }
    public NormalizedPointer? LastHover { get; private set; }
    public int InteractionCount { get; private set; }
    public event Action? Interaction;

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

/// <summary>Declares real estate allocated for active Workshop construction.</summary>
public readonly record struct SpatialConstructionSite(
    string Id,
    string Name,
    SpatialBounds Bounds,
    SpatialBounds FoundationBounds,
    SpatialConstructionPhase Phase = SpatialConstructionPhase.SitePreparation,
    double Progress = 0,
    string FuturePurpose = "Future Workshop facility.");

/// <summary>Describes how much of a future facility has become real in the spatial Experience.</summary>
public enum SpatialConstructionPhase
{
    SitePreparation,
    Foundation,
    Structure,
    Enclosure,
    InteriorSystems,
    Commissioning
}

public sealed class SpatialViewSettings
{
    private SpatialViewSettings(IReadOnlyList<SpatialDetailLevel> detailLevels, bool zoomEnabled, double minZoom, double maxZoom)
    {
        DetailLevels = detailLevels;
        ZoomEnabled = zoomEnabled;
        MinZoom = minZoom;
        MaxZoom = maxZoom;
    }

    public IReadOnlyList<SpatialDetailLevel> DetailLevels { get; }
    public bool ZoomEnabled { get; }
    public double MinZoom { get; }
    public double MaxZoom { get; }

    public static SpatialViewSettings CreateDefault(bool zoomEnabled = true, double minZoom = SpatialCamera.MinZoom, double maxZoom = SpatialCamera.MaxZoom)
    {
        if (minZoom <= 0 || maxZoom < minZoom) throw new ArgumentOutOfRangeException(nameof(minZoom), "Zoom bounds must be positive and ordered.");
        var levels = Enumerable.Range(1, 62).Select(i => new SpatialDetailLevel($"LEVEL-{i:00}", i / 64d)).ToList();
        levels.Add(SpatialDetailLevel.Device);
        levels.Add(SpatialDetailLevel.Stud);
        return new SpatialViewSettings(levels, zoomEnabled, minZoom, maxZoom);
    }
}

public readonly record struct SpatialDetailLevel(string Name, double Precision)
{
    public static SpatialDetailLevel Device => new("DEVICE", 1d / 32d);
    public static SpatialDetailLevel Stud => new("STUD", 1d / 64d);
}

public sealed record ConstructionVehicle(string Id, string Name, ConstructionVehicleKind Kind, SpatialPoint Position);
public enum ConstructionVehicleKind { Backhoe, Crane, Rover, Lifter }
