using CosBundle = TheSingularityWorkshop.FSM_COS.IMicroBundle;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

public sealed class WebPageMicroBundleCatalog : IMicroBundleCatalog
{
    public bool TryResolve(ulong bundleId, out CosBundle? bundle)
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
