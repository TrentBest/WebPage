namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Spatial identity for a Workshop landmark. Geometry and presentation belong to
/// GUI builders; the Experience only consumes this world-domain descriptor.
/// </summary>
public sealed record SpatialRoom(
    string Id,
    string Name,
    string Kind,
    double X,
    double Y,
    string Description,
    string Architecture,
    string? Capability);
