using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// A portable model of the Workshop used when the AEC Remote Office presents
/// the Workshop as an editable physical model rather than merely a map.
/// </summary>
public sealed class SpatialWorkshopModel
{
    public SpatialWorkshopModel(
        SpatialBounds extents,
        IEnumerable<SpatialWorkshopModelStructure> structures)
    {
        if (extents.Width <= 0 || extents.Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(extents));

        ArgumentNullException.ThrowIfNull(structures);

        Extents = extents;
        Structures = structures.ToArray();

        if (Structures.Any(structure => !Contains(extents, structure.Bounds)))
            throw new ArgumentException("Every Workshop structure must remain inside the model extents.", nameof(structures));
    }

    public SpatialBounds Extents { get; }

    public IReadOnlyList<SpatialWorkshopModelStructure> Structures { get; }

    public IReadOnlyList<SpatialWorkshopModelStructure> Core
        => Structures.Where(structure => structure.IsCore).ToArray();

    public IReadOnlyList<SpatialWorkshopModelStructure> Extensions
        => Structures.Where(structure => !structure.IsCore).ToArray();

    public SpatialWorkshopModelStructure? Find(string id)
        => Structures.FirstOrDefault(structure =>
            structure.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    private static bool Contains(SpatialBounds outer, SpatialBounds inner)
        => inner.X >= outer.X &&
           inner.Y >= outer.Y &&
           inner.X + inner.Width <= outer.X + outer.Width &&
           inner.Y + inner.Height <= outer.Y + outer.Height);
}

/// <summary>
/// A structure in the Workshop model. Core is a property of the Workshop
/// abstraction itself; an extension is still fully interactable but is not
/// part of the Workshop's owned core.
/// </summary>
public sealed record SpatialWorkshopModelStructure(
    string Id,
    string Name,
    SpatialBounds Bounds,
    bool IsCore,
    string? ExperienceId = null);
