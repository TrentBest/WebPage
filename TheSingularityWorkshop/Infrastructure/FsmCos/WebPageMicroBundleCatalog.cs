using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

/// <summary>
/// Transitional in-process MicroBundle source for the WebPage MVP.
///
/// The important boundary is the interface: FSM_COS asks for a MicroBundle;
/// this host-specific source supplies it. The implementation can later be
/// replaced by TheSingularityWorkshop.MicroBundleRepository.FSM_COS without
/// changing manifest execution or presentation code.
/// </summary>
public sealed class WebPageMicroBundleCatalog : IMicroBundleCatalog
{
    /// <inheritdoc />
    public bool TryResolve(ulong bundleId, out IMicroBundle? bundle)
    {
        if (bundleId == PongMicroBundle.BundleId)
        {
            bundle = new PongMicroBundle();
            return true;
        }

        bundle = null;
        return false;
    }
}
