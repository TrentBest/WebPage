using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;

namespace TheSingularityWorkshop.Infrastructure.Hub;

/// <summary>Web host adapter: owns composition only; Hub remains platform-neutral.</summary>
public sealed class HubRuntime
{
    public HubRuntime(HubKernel hub)
    {
        Hub = hub;
        Hub.LoadBundle(new WorkshopDemoBundle());
        ArbitrationMutations = Hub.ExecuteArbitrationPipeline();
    }

    public HubKernel Hub { get; }
    public int ArbitrationMutations { get; }
}
