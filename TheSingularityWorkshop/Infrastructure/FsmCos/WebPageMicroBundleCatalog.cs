using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

public sealed class WebPageMicroBundleCatalog : IMicroBundleCatalog
{
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
