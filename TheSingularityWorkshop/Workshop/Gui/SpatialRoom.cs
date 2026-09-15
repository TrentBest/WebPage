namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Spatial identity for a Workshop landmark. Geometry, navigation boundaries,
/// and presentation belong to GUI builders; the Experience consumes this
/// world-domain descriptor.
/// </summary>
public sealed record SpatialRoom(
    string Id,
    string Name,
    string Kind,
    double X,
    double Y,
    double EntranceX,
    double EntranceY,
    double ExitX,
    double ExitY,
    string Description,
    string Architecture,
    string? Capability);
