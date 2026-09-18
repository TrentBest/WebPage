namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>Declarative definition of the open spatial canvas surrounding Singularity Island.</summary>
/// <remarks>
/// Empty space is a feature, not missing content. Parcels are intentionally unclaimed so a future
/// creation tool can place user-authored Experiences, structures, games, experiments, and MicroBundles.
/// </remarks>
public sealed record SpatialUniverseManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialUniverseParcel> Parcels)
{
    public static SpatialUniverseManifest SingularityUniverse { get; } = new(
        "singularity-universe",
        "SINGULARITY UNIVERSE",
        [
            new("island-core", "ISLAND CORE", new SpatialBounds(18, 18, 64, 52), SpatialUniverseParcelKind.ReservedExperience),
            new("north-expanse", "NORTH EXPANSE", new SpatialBounds(8, 4, 84, 12), SpatialUniverseParcelKind.Open),
            new("east-expanse", "EAST EXPANSE", new SpatialBounds(82, 16, 14, 64), SpatialUniverseParcelKind.Open),
            new("south-expanse", "SOUTH EXPANSE", new SpatialBounds(8, 70, 84, 22), SpatialUniverseParcelKind.Open),
            new("west-expanse", "WEST EXPANSE", new SpatialBounds(4, 16, 14, 64), SpatialUniverseParcelKind.Open),
            new("deep-space", "DEEP SPACE", new SpatialBounds(0, 0, 100, 100), SpatialUniverseParcelKind.InfiniteCanvas)
        ]);

    public IReadOnlyList<SpatialUniverseParcel> OpenParcels
        => [.. Parcels.Where(x => x.Kind is SpatialUniverseParcelKind.Open or SpatialUniverseParcelKind.InfiniteCanvas)];
}

/// <summary>A region that can eventually be claimed by a composed Experience.</summary>
public readonly record struct SpatialUniverseParcel(string Id, string Name, SpatialBounds Bounds, SpatialUniverseParcelKind Kind);

public enum SpatialUniverseParcelKind
{
    Open,
    ReservedExperience,
    InfiniteCanvas
}
