using TheSingularityWorkshop.FSM_COS;

namespace TheSingularityWorkshop.Workshop.Composition;

/// <summary>Initial WebForge catalog for the semantic composition slice.</summary>
public sealed class WorkshopCompositionCatalog : IMicroBundleCatalog
{
    private readonly IMicroBundle _moniker = new MonikerCompositionBundle();

    public bool TryResolve(ulong bundleId, out IMicroBundle? bundle)
    {
        if (bundleId == MonikerCompositionBundle.BundleId)
        {
            bundle = _moniker;
            return true;
        }

        bundle = null;
        return false;
    }
}
