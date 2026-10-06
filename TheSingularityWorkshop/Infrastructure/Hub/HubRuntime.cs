using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;

namespace TheSingularityWorkshop.Infrastructure.Hub;

/// <summary>Web host adapter for the Hub kernel. Runtime content is supplied by the WebPage manifest.</summary>
public sealed class HubRuntime
{
    public HubRuntime(HubKernel hub)
    {
        Hub = hub;
    }

    public HubKernel Hub { get; }
    public int ArbitrationMutations { get; private set; }

    /// <summary>Runs bounded Hub arbitration after manifest-selected bundles have been registered.</summary>
    public int ArbitrateManifest()
    {
        ArbitrationMutations = Hub.ExecuteArbitrationPipeline();
        return ArbitrationMutations;
    }
}
