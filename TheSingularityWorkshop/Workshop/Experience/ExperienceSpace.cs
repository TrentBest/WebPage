namespace TheSingularityWorkshop.Workshop.Experience;

/// <summary>
/// Spatial manifestation of an Experience in the Workshop floorplan.
///
/// An Experience is not a room. It is a composed environment of capabilities
/// and MicroBundles. This descriptor supplies only the spatial facts required
/// to locate and enter that Experience; its behavior belongs to MicroBundles,
/// its composition belongs to the Experience contract, and its presentation
/// belongs to GUI builders.
/// </summary>
public sealed record ExperienceSpace(
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
    /// <summary>
    /// Returns the rectangular footprint consumed by pathfinding.
    /// X/Y are the visual footprint's top-left coordinates, so the obstacle
    /// center is derived from the dimensions rather than treating X/Y as its
    /// center. This keeps the rendered blueprint and navigation geometry in
    /// the same coordinate system.
    /// </summary>
    public SpatialObstacle Footprint =>
        new(Id, X + Width / 2, Y + Height / 2, Width, Height);
}

/// <summary>
/// Domain obstacle consumed by spatial pathfinding.
/// Coordinates are expressed in the Workshop's 0..100 world.
/// </summary>
public readonly record struct SpatialObstacle(
    string Id,
    double CenterX,
    double CenterY,
    double Width,
    double Height);