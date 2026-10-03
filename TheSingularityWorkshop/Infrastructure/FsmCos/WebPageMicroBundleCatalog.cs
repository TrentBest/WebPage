using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

public sealed class WebPageMicroBundleCatalog : IMicroBundleCatalog
{
    public bool TryResolve(ulong bundleId, out TheSingularityWorkshop.MicroBundleDomain.IMicroBundle? bundle)
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
