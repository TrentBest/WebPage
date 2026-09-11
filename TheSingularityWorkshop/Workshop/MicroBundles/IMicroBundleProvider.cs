using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Provides the platform-specific manifestation of a micro-bundle.
/// The lifecycle remains owned by FSM_API; the provider decides how the
/// current state becomes visible in its host environment.
/// </summary>
public interface IMicroBundleProvider
{
    /// <summary>
    /// Produces the manifestation description for the supplied state context.
    /// </summary>
    MicroBundleManifestation Manifest(IStateContext context);
}
