namespace TheSingularityWorkshop.Gui;

using System;

/// <summary>
/// Describes how a spatial object is being perceived without changing the
/// persistent world position of the visitor.
/// </summary>
public enum SpatialPresentationMode
{
    Plan,
    InteriorPlan,
    Elevation
}

/// <summary>
/// Camera/presentation state anchored to a persistent world position.
/// </summary>
public readonly record struct SpatialViewpoint(
    SpatialPresentationMode Mode,
    SpatialPoint Anchor,
    string? FocusId = null)
{
    public static SpatialViewpoint Plan(SpatialPoint anchor)
        => new(SpatialPresentationMode.Plan, anchor);

    public static SpatialViewpoint Interior(SpatialPoint anchor)
        => new(SpatialPresentationMode.InteriorPlan, anchor);

    public static SpatialViewpoint Elevation(SpatialPoint anchor, string focusId)
    {
        if (string.IsNullOrWhiteSpace(focusId))
            throw new ArgumentException("An elevation focus is required.", nameof(focusId));

        return new(SpatialPresentationMode.Elevation, anchor, focusId);
    }

    /// <summary>
    /// The camera remains conceptually centered on the visitor in every mode.
    /// Elevation changes perception, not world ownership or actor position.
    /// </summary>
    public bool IsAvatarAnchored => true;
}
