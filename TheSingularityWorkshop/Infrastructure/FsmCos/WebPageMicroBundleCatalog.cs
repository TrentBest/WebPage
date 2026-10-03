using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

public sealed class WebPageMicroBundleCatalog : IMicroBundleCatalog
{
    public bool TryResolve(MicroBundleDependencyRequest request, out IMicroBundle? bundle)
    {
        if (request.BundleId == PongMicroBundle.BundleId)
        {
            bundle = new PongMicroBundle();
            return true;
        }

        bundle = null;
        return false;
    }
}
