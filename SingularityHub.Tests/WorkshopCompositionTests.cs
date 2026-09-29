using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.Composition;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopCompositionTests
{
    [Fact(DisplayName = "Workshop startup assembles the zeroth Moniker composition through FSM_COS")]
    public void StartupAssemblesMonikerComposition()
    {
        using var experience = new WorkshopExperienceService(new FsmCos(new WorkshopCompositionCatalog()));
        experience.Initialize();
        Assert.NotNull(experience.RuntimeAssembly);
        Assert.Contains(experience.RuntimeAssembly!.Bundles, bundle => bundle.Id == MonikerCompositionBundle.BundleId);
        Assert.Equal(MonikerCompositionBundle.BundleId, experience.RuntimeAssembly.Bundles.Single().Descriptor.Id);
        Assert.Equal("0.1.0-webpage", experience.RuntimeAssembly.Bundles.Single().Descriptor.Version);
        Assert.NotNull(experience.MonikerComposition);
        Assert.Equal("Moniker", experience.MonikerComposition!.Properties["composition"]);
        Assert.Equal("THE", experience.MonikerComposition.Find("the").Text);
        Assert.Equal("SINGULARITY", experience.MonikerComposition.Find("singularity").Text);
        Assert.Equal("WORKSHOP", experience.MonikerComposition.Find("workshop").Text);
    }
}
