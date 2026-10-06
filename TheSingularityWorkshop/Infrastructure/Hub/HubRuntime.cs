using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using TheSingularityWorkshop.SingularityHub;

using TheSingularityWorkshop.Workshop.Configuration;

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
    public WorkshopManifestDocument? Manifest { get; private set; }

    /// <summary>Loads the file-backed Hub configuration into the platform-neutral Hub registry.</summary>
    public void Configure(WorkshopManifestDocument manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        if (Manifest is not null)
            throw new InvalidOperationException("The Workshop Hub has already been configured.");

        foreach (var hub in manifest.Hub is null ? Array.Empty<SingularityHubDefinition>() : new[] { manifest.Hub.ToDefinition() })
        {
            if (!Hub.Configuration.Register(hub))
                throw new InvalidOperationException($"The Workshop Hub configuration could not register Hub {hub.Id}.");
        }

        Manifest = manifest;
    }
}
