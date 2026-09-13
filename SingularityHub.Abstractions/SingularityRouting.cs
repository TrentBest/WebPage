namespace TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Describes a destination exposed through the Hub routing surface.
/// A route is presentation metadata; the referenced Experience owns the actual
/// composition and MicroBundle requirements.
/// </summary>
public readonly record struct SingularityRoute(
    ulong RouteId,
    string Path,
    string Tab,
    ulong ExperienceId);

/// <summary>
/// Hub-owned routing boundary. Tools, tab panels, and other MicroBundle-backed
/// surfaces may establish routes through the Hub instead of maintaining their
/// own disconnected navigation tables.
/// </summary>
public interface ISingularityRouting
{
    /// <summary>Gets the currently registered routes.</summary>
    IReadOnlyCollection<SingularityRoute> Routes { get; }

    /// <summary>Registers a route owned by a Hub consumer.</summary>
    bool Register(SingularityRoute route);

    /// <summary>Removes a route by its stable path.</summary>
    bool Remove(string path);

    /// <summary>Resolves a path to its Hub-owned destination.</summary>
    bool TryResolve(string path, out SingularityRoute route);
}
