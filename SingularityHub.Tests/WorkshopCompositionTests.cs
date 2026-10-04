using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopCompositionTests
{
    [Fact(DisplayName = "Workshop startup assembles the zeroth Moniker composition through FSM_COS")]
    public void StartupAssemblesMonikerComposition()
    {
        using var experience = new WorkshopExperienceService();
        experience.Initialize();
        AdvanceToGateway(experience);
        experience.RequestEntry();
        AdvanceToLanding(experience);

        Assert.NotNull(experience.RuntimeAssembly);
        Assert.Contains(experience.RuntimeAssembly!.Bundles, bundle => bundle.Id == MonikerMicroBundle.BundleId);
        Assert.NotNull(experience.MonikerComposition);
        Assert.Equal("Moniker", experience.MonikerComposition!.Properties["composition"]);
        Assert.Equal("THE", experience.MonikerComposition.Find("the").Text);
        Assert.Equal("SINGULARITY", experience.MonikerComposition.Find("singularity").Text);
        Assert.Equal("WORKSHOP", experience.MonikerComposition.Find("workshop").Text);
    }

    [Fact(DisplayName = "FSM_COS composes ProtocolAI and GrammarAI into an extractable exchange")]
    public void ExplicitlyComposesAiExchange()
    {
        var compositionSystem = new FsmCos(new WorkshopCompositionCatalog());

        var runtime = compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 4001UL,
            Bundles: new[]
            {
                BundleRequest.Unconfigured(AiExchangeCompositionBundle.BundleId)
            }));

        var ai = runtime.TryGetBundle<AiExchangeCompositionBundle>(
            AiExchangeCompositionBundle.BundleId,
            out var aiBundle)
            ? aiBundle
            : null;

        Assert.NotNull(ai);
        Assert.NotNull(ai!.Protocol);
        Assert.NotNull(ai.Grammar);
        Assert.NotNull(ai.ExchangeText);
        Assert.Contains("PROTOCOL", ai.ExchangeText!, StringComparison.Ordinal);
        Assert.Contains("GRAMMAR", ai.ExchangeText!, StringComparison.Ordinal);
        Assert.Contains("Extract to Clipboard", ai.Composition!.Find("ai-extract").Text);
        Assert.Equal("ai.extract", ai.Composition.Find("ai-extract").Properties["command"]);
        Assert.Equal("ai.submit", ai.Composition.Find("ai-submit").Properties["command"]);
    }

    [Fact(DisplayName = "Explicit FSM_COS AI composition includes its dependency closure")]
    public void ExplicitAiCompositionIncludesClosure()
    {
        var compositionSystem = new FsmCos(new WorkshopCompositionCatalog());

        var runtime = compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 4002UL,
            Bundles: new[]
            {
                BundleRequest.Unconfigured(AiExchangeCompositionBundle.BundleId)
            }));

        Assert.Contains(
            runtime.Bundles,
            bundle => bundle.Id == AiExchangeCompositionBundle.ProtocolBundleId);
        Assert.Contains(
            runtime.Bundles,
            bundle => bundle.Id == AiExchangeCompositionBundle.GrammarBundleId);
        Assert.Contains(
            runtime.Bundles,
            bundle => bundle.Id == AiExchangeCompositionBundle.BundleId);
    }

    private static void AdvanceToGateway(WorkshopExperienceService service)
    {
        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Gateway"; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
    }

    private static void AdvanceToLanding(WorkshopExperienceService service)
    {
        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Landing"; i++)
            service.Tick();

        Assert.Equal("Landing", service.FirstContact.CurrentState);
        Assert.Equal("LivingGui", service.CurrentState);
    }
}
