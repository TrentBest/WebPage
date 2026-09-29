using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.Composition;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WebForgeCompositionTests
{
    [Fact(DisplayName = "FSM_COS assembles the WebForge shell and its showcase capabilities")]
    public void FsmCosAssemblesWebForgeShell()
    {
        var catalog = new WorkshopCompositionCatalog();
        var assembly = new FsmCos(catalog).Execute(
            new RuntimeManifest(
                0x574542464F524745UL,
                new[]
                {
                    BundleRequest.Unconfigured(WorkshopShellCompositionBundle.BundleId),
                    BundleRequest.Unconfigured(MonikerCompositionBundle.BundleId),
                    BundleRequest.Unconfigured(AiExchangeCompositionBundle.BundleId)
                }));

        Assert.True(assembly.TryGetBundle<WorkshopShellCompositionBundle>(
            WorkshopShellCompositionBundle.BundleId,
            out var shell));
        Assert.NotNull(shell!.Composition);

        Assert.True(assembly.TryGetBundle<MonikerCompositionBundle>(
            MonikerCompositionBundle.BundleId,
            out var moniker));
        Assert.NotNull(moniker!.Composition);

        Assert.True(assembly.TryGetBundle<AiExchangeCompositionBundle>(
            AiExchangeCompositionBundle.BundleId,
            out var ai));
        Assert.NotNull(ai!.Composition);

        Assert.Equal(
            new[]
            {
                WorkshopShellCompositionBundle.BundleId,
                MonikerCompositionBundle.BundleId,
                AiExchangeCompositionBundle.ProtocolBundleId,
                AiExchangeCompositionBundle.GrammarBundleId,
                AiExchangeCompositionBundle.BundleId
            },
            assembly.Bundles.Select(bundle => bundle.Id));
    }
}
