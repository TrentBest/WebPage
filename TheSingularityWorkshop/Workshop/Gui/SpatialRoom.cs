namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Spatial identity for a Workshop suite.
/// The descriptor contains the domain geometry needed by navigation; visual
/// presentation remains the responsibility of GUI builders.
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
    string? Capability,
    double Width = 24,
    double Height = 20)
{
    /// <summary>Returns the rectangular footprint consumed by pathfinding.</summary>
    public SpatialObstacle Footprint => new(Id, X, Y, Width, Height);
}

/// <summary>
/// Domain obstacle consumed by the pathfinding MicroBundle.
/// Coordinates are expressed in the same 0..100 Workshop world as SpatialRoom.
/// </summary>
public readonly record struct SpatialObstacle(
    string Id,
    double CenterX,
    double CenterY,
    double Width,
    double Height);
