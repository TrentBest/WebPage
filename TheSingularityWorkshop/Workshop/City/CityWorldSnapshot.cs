namespace TheSingularityWorkshop.Workshop.City;

/// <summary>
/// Immutable, renderer-neutral description of one city-world revision.
/// Grid coordinates use +X east and +Z south; projections may choose their own screen axes.
/// </summary>
public sealed class CityWorldSnapshot
{
    private readonly CityWorldEntity[] _entities;

    private CityWorldSnapshot(string revision, CityWorldEntity[] entities)
    {
        Revision = revision;
        _entities = entities;
    }

    /// <summary>Caller-supplied identity for the source revision; not a timestamp or content hash.</summary>
    public string Revision { get; }

    /// <summary>Canonical coordinate convention for this snapshot.</summary>
    public string CoordinateConvention => "grid-xz; +X east; +Z south; origin at grid cell (0,0)";

    /// <summary>Entities ordered by stable entity ID.</summary>
    public IReadOnlyList<CityWorldEntity> Entities => Array.AsReadOnly(_entities);

    public static CityWorldSnapshot Create(string revision, IEnumerable<CityWorldEntity> entities)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(revision);
        ArgumentNullException.ThrowIfNull(entities);

        var ordered = entities.OrderBy(entity => entity.Id, StringComparer.Ordinal).ToArray();
        if (ordered.Any(entity => entity is null))
            throw new ArgumentException("A city snapshot cannot contain a null entity.", nameof(entities));

        var duplicate = ordered
            .GroupBy(entity => entity.Id, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
            throw new ArgumentException($"City entity ID '{duplicate.Key}' is duplicated.", nameof(entities));

        return new CityWorldSnapshot(revision, ordered);
    }
}

/// <summary>Stable identity and renderer-independent spatial facts for one city entity.</summary>
public sealed record CityWorldEntity
{
    public CityWorldEntity(string id, string kind, double gridX, double gridZ, int floors)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(kind);
        if (!double.IsFinite(gridX)) throw new ArgumentOutOfRangeException(nameof(gridX));
        if (!double.IsFinite(gridZ)) throw new ArgumentOutOfRangeException(nameof(gridZ));
        if (floors < 0) throw new ArgumentOutOfRangeException(nameof(floors));

        Id = id;
        Kind = kind;
        GridX = gridX;
        GridZ = gridZ;
        Floors = floors;
    }

    public string Id { get; }
    public string Kind { get; }
    public double GridX { get; }
    public double GridZ { get; }
    public int Floors { get; }
}
