using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.Composition;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopCompositionTests
{
    [Fact(DisplayName = "Workshop startup assembles the zeroth Moniker composition through FSM_COS")]
    public void StartupAssemblesMonikerComposition()
    {
        using var experience = new WorkshopExperienceService();
        experience.Initialize();
        SelectFsmCos(experience);

        Assert.NotNull(experience.RuntimeAssembly);
        Assert.Contains(experience.RuntimeAssembly!.Bundles, bundle => bundle.Id == MonikerCompositionBundle.BundleId);
        Assert.NotNull(experience.MonikerComposition);
        Assert.Equal("Moniker", experience.MonikerComposition!.Properties["composition"]);
        Assert.Equal("THE", experience.MonikerComposition.Find("the").Text);
        Assert.Equal("SINGULARITY", experience.MonikerComposition.Find("singularity").Text);
        Assert.Equal("WORKSHOP", experience.MonikerComposition.Find("workshop").Text);
    }

    [Fact(DisplayName = "Workshop startup composes ProtocolAI and GrammarAI into an extractable exchange")]
    public void StartupComposesAiExchange()
    {
        using var experience = new WorkshopExperienceService();
        experience.Initialize();
        SelectFsmCos(experience);

        var ai = experience.AiExchangeComposition;
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

    [Fact(DisplayName = "Workshop composition includes the GUI-facing AI exchange bundle and its dependencies")]
    public void StartupIncludesAiCompositionClosure()
    {
        using var experience = new WorkshopExperienceService();
        experience.Initialize();
        SelectFsmCos(experience);

        Assert.Contains(experience.RuntimeAssembly!.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.ProtocolBundleId);
        Assert.Contains(experience.RuntimeAssembly.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.GrammarBundleId);
        Assert.Contains(experience.RuntimeAssembly.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.BundleId);
    }

    private static void SelectFsmCos(WorkshopExperienceService experience)
    {
        while (experience.FirstContact.CurrentState != "Gateway")
            experience.Tick();

        experience.RequestEntry();
        experience.Tick();
        experience.SelectIntegration(WorkshopIntegrationMode.FsmCos);
    }
}
