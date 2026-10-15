using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Geometry and semantics for a reusable floor-plan sketch.</summary>
public sealed record PlanRoom(
    string Id, string Name, double X, double Y, double Width, double Height, string Use);

/// <summary>
/// A wall opening. Width and hinge position use the plan's coordinate units.
/// Door behavior is semantic data; renderers decide how to draw it.
/// </summary>
public sealed record PlanOpening(
    string Id, string WallId, double Offset, double Width,
    PlanOpeningKind Kind, PlanHinge Hinge = PlanHinge.Start,
    PlanSwingDirection Swing = PlanSwingDirection.IntoRoom,
    double OpenFraction = 1.0);

public sealed record PlanWall(
    string Id, double X1, double Y1, double X2, double Y2, double Thickness = 0.12);

public sealed record PlanFixture(
    string Id, string Name, string Kind, double X, double Y, double Width, double Height);

public enum PlanOpeningKind : byte
{
    HingedDoor,
    SlidingDoor,
    PocketDoor,
    RevolvingDoor,
    Archway,
    Window
}

public enum PlanHinge : byte { Start, End }
public enum PlanSwingDirection : byte { IntoRoom, OutOfRoom, Bidirectional }

/// <summary>
/// A renderer-neutral floor plan: shared world coordinates, rooms, walls, openings and fixtures.
/// Use one coordinate system across floors so plans can later align with building and city data.
/// </summary>
public sealed class BuildingPlan
{
    public BuildingPlan(string id, string name, double width, double height,
        IReadOnlyList<PlanRoom> rooms, IReadOnlyList<PlanWall> walls,
        IReadOnlyList<PlanOpening> openings, IReadOnlyList<PlanFixture>? fixtures = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Plan ID is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Plan name is required.", nameof(name));
        if (!double.IsFinite(width) || width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (!double.IsFinite(height) || height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        ArgumentNullException.ThrowIfNull(rooms);
        ArgumentNullException.ThrowIfNull(walls);
        ArgumentNullException.ThrowIfNull(openings);

        Id = id;
        Name = name;
        Width = width;
        Height = height;
        Rooms = rooms.ToArray();
        Walls = walls.ToArray();
        Openings = openings.ToArray();
        Fixtures = (fixtures ?? Array.Empty<PlanFixture>()).ToArray();
        Validate();
    }

    public string Id { get; }
    public string Name { get; }
    public double Width { get; }
    public double Height { get; }
    public IReadOnlyList<PlanRoom> Rooms { get; }
    public IReadOnlyList<PlanWall> Walls { get; }
    public IReadOnlyList<PlanOpening> Openings { get; }
    public IReadOnlyList<PlanFixture> Fixtures { get; }

    private void Validate()
    {
        EnsureUnique(Rooms.Select(x => x.Id), "room");
        EnsureUnique(Walls.Select(x => x.Id), "wall");
        EnsureUnique(Openings.Select(x => x.Id), "opening");
        EnsureUnique(Fixtures.Select(x => x.Id), "fixture");

        foreach (var room in Rooms)
        {
            RequireId(room.Id, "room");
            if (!FinitePositive(room.Width) || !FinitePositive(room.Height)
                || !double.IsFinite(room.X) || !double.IsFinite(room.Y)
                || room.X < 0 || room.Y < 0 || room.X + room.Width > Width || room.Y + room.Height > Height)
                throw new ArgumentException($"Room '{room.Id}' has invalid or out-of-bounds geometry.");
        }

        var wallIds = Walls.Select(x => x.Id).ToHashSet(StringComparer.Ordinal);
        foreach (var wall in Walls)
        {
            RequireId(wall.Id, "wall");
            if (!double.IsFinite(wall.X1) || !double.IsFinite(wall.Y1)
                || !double.IsFinite(wall.X2) || !double.IsFinite(wall.Y2)
                || !FinitePositive(wall.Thickness) || (wall.X1 == wall.X2 && wall.Y1 == wall.Y2)
                || wall.X1 < 0 || wall.X1 > Width || wall.X2 < 0 || wall.X2 > Width
                || wall.Y1 < 0 || wall.Y1 > Height || wall.Y2 < 0 || wall.Y2 > Height)
                throw new ArgumentException($"Wall '{wall.Id}' has invalid or out-of-bounds geometry.");
        }

        foreach (var opening in Openings)
        {
            RequireId(opening.Id, "opening");
            if (!wallIds.Contains(opening.WallId))
                throw new ArgumentException($"Opening '{opening.Id}' refers to missing wall '{opening.WallId}'.");
            if (!FinitePositive(opening.Width) || !double.IsFinite(opening.Offset) || opening.Offset < 0)
                throw new ArgumentException($"Opening '{opening.Id}' has invalid dimensions.");
            var wall = Walls.First(x => string.Equals(x.Id, opening.WallId, StringComparison.Ordinal));
            var wallLength = Math.Sqrt(Math.Pow(wall.X2 - wall.X1, 2) + Math.Pow(wall.Y2 - wall.Y1, 2));
            if (opening.Offset + opening.Width > wallLength)
                throw new ArgumentException($"Opening '{opening.Id}' extends beyond wall '{opening.WallId}'.");
            if (!double.IsFinite(opening.OpenFraction) || opening.OpenFraction < 0 || opening.OpenFraction > 1)
                throw new ArgumentException($"Opening '{opening.Id}' open fraction must be between 0 and 1.");
        }

        foreach (var fixture in Fixtures)
        {
            RequireId(fixture.Id, "fixture");
            if (!FinitePositive(fixture.Width) || !FinitePositive(fixture.Height)
                || !double.IsFinite(fixture.X) || !double.IsFinite(fixture.Y)
                || fixture.X < 0 || fixture.Y < 0 || fixture.X + fixture.Width > Width || fixture.Y + fixture.Height > Height)
                throw new ArgumentException($"Fixture '{fixture.Id}' has invalid or out-of-bounds geometry.");
        }
    }

    private static bool FinitePositive(double value) => double.IsFinite(value) && value > 0;
    private static void RequireId(string? id, string kind)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException($"Every {kind} requires an ID.");
    }

    private static void EnsureUnique(IEnumerable<string> ids, string kind)
    {
        var values = ids.ToArray();
        if (values.Any(string.IsNullOrWhiteSpace) || values.Distinct(StringComparer.Ordinal).Count() != values.Length)
            throw new ArgumentException($"Every {kind} requires a unique, non-empty ID.");
    }
}

/// <summary>Small example plan for fast iteration; deliberately not a building standard.</summary>
public static class PlanSketchExamples
{
    public static BuildingPlan CompactApartment => new(
        "example.compact-apartment", "Compact apartment", 12, 9,
        [
            new("living", "Living / dining", 0.12, 0.12, 7.1, 4.1, "Living"),
            new("bedroom", "Bedroom", 7.35, 0.12, 4.53, 3.8, "Sleeping"),
            new("bath", "Bathroom", 7.35, 4.15, 2.0, 2.0, "Bathing"),
            new("kitchen", "Kitchen", 0.12, 4.35, 4.0, 2.0, "Cooking")
        ],
        [
            new("north", 0, 0, 12, 0), new("east", 12, 0, 12, 9),
            new("south", 12, 9, 0, 9), new("west", 0, 9, 0, 0),
            new("bedroom-divider", 7.23, 0, 7.23, 3.0),
            new("bath-divider", 7.23, 4.0, 7.23, 6.3)
        ],
        [
            new("front-door", "south", 1.0, 0.9, PlanOpeningKind.HingedDoor,
                PlanHinge.Start, PlanSwingDirection.IntoRoom),
            new("bedroom-door", "bedroom-divider", 0.8, 0.82, PlanOpeningKind.SlidingDoor),
            new("bath-door", "bath-divider", 0.35, 0.72, PlanOpeningKind.PocketDoor)
        ],
        [
            new("sofa", "Sofa", "Seating", 1.0, 1.1, 2.3, 0.95),
            new("island", "Kitchen island", "Worktop", 2.0, 5.0, 1.7, 0.8)
        ]);
}

/// <summary>
/// WebPage host MicroBundle adapter. It exposes plan capability metadata; the host renderer
/// remains responsible for drawing and interaction. No ProtocolAi or GrammarAi dependency is required.
/// </summary>
public sealed class PlanViewMicroBundleProvider : IMicroBundleProvider
{
    public const int BundleId = 3101;

    public MicroBundleManifestation Manifest(IStateContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new MicroBundleManifestation(BundleId, "building.plan", "PlanView")
        {
            Parameters = new Dictionary<string, int>
            {
                ["schemaVersion"] = 1,
                ["rooms"] = PlanSketchExamples.CompactApartment.Rooms.Count,
                ["walls"] = PlanSketchExamples.CompactApartment.Walls.Count,
                ["openings"] = PlanSketchExamples.CompactApartment.Openings.Count,
                ["fixtures"] = PlanSketchExamples.CompactApartment.Fixtures.Count
            }
        };
    }
}
