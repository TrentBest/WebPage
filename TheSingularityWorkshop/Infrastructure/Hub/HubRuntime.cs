using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;

namespace TheSingularityWorkshop.Infrastructure.Hub;

/// <summary>Web host adapter for the Hub kernel. Runtime content is supplied by the active manifest.</summary>
public sealed class HubRuntime
{
    public HubRuntime(HubKernel hub)
    {
        Hub = hub ?? throw new ArgumentNullException(nameof(hub));
    }

    public HubKernel Hub { get; private set; }
    public int ArbitrationMutations { get; private set; }

    /// <summary>Runs bounded Hub arbitration after manifest-selected bundles have been registered.</summary>
    public int ArbitrateManifest()
    {
        ArbitrationMutations = Hub.ExecuteArbitrationPipeline();
        return ArbitrationMutations;
    }

    /// <summary>Replaces the session Hub after a candidate manifest has composed successfully.</summary>
    public void ReplaceHub(HubKernel hub)
    {
        Hub = hub ?? throw new ArgumentNullException(nameof(hub));
        ArbitrationMutations = 0;
    }
}
