using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Runtime arrangement layer for a spatial plan. The authored scene remains the
/// architectural baseline; this layer records deliberate placement changes made
/// while exploring or editing the plan.
/// </summary>
public sealed class SpatialWorkshopArrangement
{
    private readonly Dictionary<string, SpatialBounds> _bounds;

    public SpatialWorkshopArrangement(SpatialWorkshopScene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        _bounds = scene.Interactables.ToDictionary(item => item.Id, item => item.Bounds);
    }

    public IReadOnlyDictionary<string, SpatialBounds> Bounds => _bounds;

    public SpatialBounds GetBounds(SpatialInteractable item)
        => _bounds.TryGetValue(item.Id, out var bounds) ? bounds : item.Bounds;

    public bool TryMove(SpatialWorkshopScene scene, string id, double deltaX, double deltaY)
    {
        ArgumentNullException.ThrowIfNull(scene);

        if (!_bounds.TryGetValue(id, out var current))
            return false;

        var candidate = new SpatialBounds(
            current.X + deltaX,
            current.Y + deltaY,
            current.Width,
            current.Height);

        if (candidate.X < 0 || candidate.Y < 0 ||
            candidate.X + candidate.Width > 100 ||
            candidate.Y + candidate.Height > 100)
            return false;

        foreach (var item in scene.Interactables)
        {
            if (item.Id == id)
                continue;

            if (Overlaps(candidate, GetBounds(item)))
                return false;
        }

        _bounds[id] = candidate;
        return true;
    }

    public void Reset(SpatialWorkshopScene scene)
    {
        ArgumentNullException.ThrowIfNull(scene);

        _bounds.Clear();
        foreach (var item in scene.Interactables)
            _bounds[item.Id] = item.Bounds;
    }

    private static bool Overlaps(SpatialBounds a, SpatialBounds b)
        => a.X < b.X + b.Width &&
           b.X < a.X + a.Width &&
           a.Y < b.Y + b.Height &&
           b.Y < a.Y + a.Height;
}
