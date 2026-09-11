using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Infrastructure.Hub;

/// <summary>Web host adapter: owns composition only; Hub remains platform-neutral.</summary>
public sealed class HubRuntime
{
    public HubRuntime(SingularityHub hub)
    {
        Hub = hub;
        Hub.LoadBundle(new WorkshopDemoBundle());
        ArbitrationMutations = Hub.ExecuteArbitrationPipeline();
    }

    public SingularityHub Hub { get; }
    public int ArbitrationMutations { get; }
}
