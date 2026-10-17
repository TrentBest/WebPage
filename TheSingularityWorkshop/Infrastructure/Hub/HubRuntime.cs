using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;

namespace TheSingularityWorkshop.Infrastructure.Hub;

/// <summary>Stable per-session adapter for the Hub kernel and active manifest bundle set.</summary>
public sealed class HubRuntime
{
    public HubRuntime(HubKernel hub)
    {
        Hub = hub ?? throw new ArgumentNullException(nameof(hub));
    }

    /// <summary>The stable Hub instance shared with PageFSM for this browser session.</summary>
    public HubKernel Hub { get; }
    public int ArbitrationMutations { get; private set; }

    /// <summary>Runs bounded Hub arbitration for the active manifest.</summary>
    public int ArbitrateManifest()
    {
        ArbitrationMutations = Hub.ExecuteArbitrationPipeline();
        return ArbitrationMutations;
    }

    /// <summary>
    /// Replaces manifest-owned Hub bundle state without replacing the Hub object itself.
    /// PageFSM holds this Hub instance for its lifetime, so preserving object identity is required.
    /// The previous loaded bundles are restored if activation fails.
    /// </summary>
    public int ActivateManifestBundle(HubBundle? bundle)
    {
        var previousBundles = Hub.LoadedBundles.ToArray();
        Hub.ResetManifestRuntime();

        try
        {
            if (bundle is not null && !Hub.LoadBundle(bundle))
                throw new InvalidOperationException($"The active Hub could not load MicroBundle {bundle.Id}.");

            ArbitrationMutations = Hub.ExecuteArbitrationPipeline();
            return ArbitrationMutations;
        }
        catch
        {
            Hub.ResetManifestRuntime();
            foreach (var previous in previousBundles)
                Hub.LoadBundle(previous);
            ArbitrationMutations = Hub.ExecuteArbitrationPipeline();
            throw;
        }
    }
}
