namespace TheSingularityWorkshop.Gui;

/// <summary>Deterministic seed carried by generated spatial content.</summary>
/// <remarks>
/// A seed is data, not the generated object itself. This lets a vast environment be represented compactly
/// and regenerated on demand without persisting every decorative object or occupant.
/// </remarks>
public readonly record struct SpatialSeed(string Value)
{
    public uint Hash
    {
        get
        {
            unchecked
            {
                uint hash = 2166136261;
                foreach (var character in Value)
                {
                    hash ^= character;
                    hash *= 16777619;
                }
                return hash;
            }
        }
    }
}

/// <summary>Describes how an interactable chooses to present itself when activated.</summary>
public enum SpatialPresentationMode
{
    Popup,
    Takeover,
    Inline
}

/// <summary>Semantic categories used when populating large spatial environments.</summary>
public enum SpatialGeneratedObjectKind
{
    Art,
    Functionality,
    Shop,
    Arcade,
    Information,
    Person
}

/// <summary>
/// A generated object whose identity, placement, presentation and optional functionality are all reproducible from a seed.
/// </summary>
public readonly record struct SpatialGeneratedObject(
    string Id,
    string Name,
    SpatialGeneratedObjectKind Kind,
    SpatialSeed Seed,
    SpatialPoint Position,
    SpatialBounds InteractionPoint,
    SpatialPresentationMode Presentation,
    string? CapabilityId);

/// <summary>
/// Compact description of a potentially large generated population. The population is intentionally not materialized here.
/// </summary>
public sealed class SpatialPopulationManifest
{
    public SpatialPopulationManifest(string id, SpatialBounds bounds, double density, SpatialSeed seed)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A population manifest requires an ID.", nameof(id));
        if (density <= 0) throw new ArgumentOutOfRangeException(nameof(density), "Population density must be greater than zero.");

        Id = id;
        Bounds = bounds;
        Density = density;
        Seed = seed;
    }

    public string Id { get; }
    public SpatialBounds Bounds { get; }
    public double Density { get; }
    public SpatialSeed Seed { get; }
}

/// <summary>Generates deterministic spatial content for only the region currently being explored.</summary>
public static class SpatialSeededPopulationGenerator
{
    private const double BaseCellSize = 10d;

    /// <summary>
    /// Generates a bounded slice of a population. The same manifest and region always produce the same objects.
    /// </summary>
    public static IReadOnlyList<SpatialGeneratedObject> Generate(SpatialPopulationManifest manifest, SpatialBounds region)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var visibleRegion = Intersect(manifest.Bounds, region);
        if (visibleRegion is null) return [];

        var cellSize = BaseCellSize / Math.Sqrt(manifest.Density);
        var minCellX = (int)Math.Floor(visibleRegion.Value.X / cellSize);
        var maxCellX = (int)Math.Floor((visibleRegion.Value.X + visibleRegion.Value.Width) / cellSize);
        var minCellY = (int)Math.Floor(visibleRegion.Value.Y / cellSize);
        var maxCellY = (int)Math.Floor((visibleRegion.Value.Y + visibleRegion.Value.Height) / cellSize);
        var generated = new List<SpatialGeneratedObject>();

        for (var cellY = minCellY; cellY <= maxCellY; cellY++)
        {
            for (var cellX = minCellX; cellX <= maxCellX; cellX++)
            {
                var cellSeed = new SpatialSeed($"{manifest.Seed.Value}:{cellX}:{cellY}");
                var hash = cellSeed.Hash;

                // Leave some cells empty. This produces breathing room instead of a uniform grid of clutter.
                if ((hash & 0x3u) == 0) continue;

                var x = cellX * cellSize + Unit(hash, 8, 15) * cellSize / 16d;
                var y = cellY * cellSize + Unit(hash, 16, 23) * cellSize / 16d;
                var position = new SpatialPoint(x, y);
                if (!visibleRegion.Value.Contains(position)) continue;

                var kind = (SpatialGeneratedObjectKind)((hash >> 24) % 6);
                var presentation = (SpatialPresentationMode)((hash >> 27) % 3);
                var capability = kind == SpatialGeneratedObjectKind.Functionality
                    ? $"capability:{hash:X8}"
                    : null;

                generated.Add(new SpatialGeneratedObject(
                    $"{manifest.Id}:{cellX}:{cellY}",
                    NameFor(kind, hash),
                    kind,
                    cellSeed,
                    position,
                    new SpatialBounds(position.X - 1, position.Y - 1, 2, 2),
                    presentation,
                    capability));
            }
        }

        return generated;
    }

    private static int Unit(uint hash, int startBit, int endBit)
        => (int)((hash >> startBit) & ((1u << (endBit - startBit + 1)) - 1u));

    private static string NameFor(SpatialGeneratedObjectKind kind, uint hash)
        => $"{kind} {hash & 0xFFFF:X4}";

    private static SpatialBounds? Intersect(SpatialBounds a, SpatialBounds b)
    {
        var left = Math.Max(a.X, b.X);
        var top = Math.Max(a.Y, b.Y);
        var right = Math.Min(a.X + a.Width, b.X + b.Width);
        var bottom = Math.Min(a.Y + a.Height, b.Y + b.Height);
        return right <= left || bottom <= top ? null : new SpatialBounds(left, top, right - left, bottom - top);
    }
}
