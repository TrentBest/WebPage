using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.Composition;
using Xunit;

namespace SingularityHub.Tests;

public sealed class AiExchangeCompositionTests
{
    [Fact]
    public void ProtocolAiTerminalComposesBesideThePrimaryExperience()
    {
        var catalog = new WorkshopCompositionCatalog();
        var assembly = new FsmCos(catalog).Execute(new RuntimeManifest(
            RuntimeId: 300,
            Bundles: [new MicroBundleManifestEntry(AiExchangeCompositionBundle.BundleId, "0.1.0")]));

        Assert.True(assembly.TryGetBundle<AiExchangeCompositionBundle>(
            AiExchangeCompositionBundle.BundleId, out var terminal));
        Assert.NotNull(terminal);
        Assert.NotNull(terminal!.Protocol);
        Assert.NotNull(terminal.Grammar);
        Assert.False(string.IsNullOrWhiteSpace(terminal.ExchangeText));

        Assert.Contains("PROTOCOL", terminal.ExchangeText, StringComparison.Ordinal);
        Assert.Contains("GRAMMAR", terminal.ExchangeText, StringComparison.Ordinal);
    }
}
