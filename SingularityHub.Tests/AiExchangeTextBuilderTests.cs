using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class AiExchangeTextBuilderTests
{
    [Fact]
    public void BuildIncludesProtocolGrammarAndResponseSafetyContract()
    {
        var exchange = AiExchangeTextBuilder.Build("protocol-vocabulary", "grammar-rules", null);

        Assert.Contains("PROTOCOL" + Environment.NewLine + Environment.NewLine + "protocol-vocabulary", exchange);
        Assert.Contains("GRAMMAR" + Environment.NewLine + Environment.NewLine + "grammar-rules", exchange);
        Assert.Contains("The response is a proposal only. Do not invent IDs or claim validation.", exchange);
        Assert.Contains("The host will validate and review the response before applying changes.", exchange);
        Assert.DoesNotContain("HOST CONTEXT / TASK", exchange);
    }

    [Fact]
    public void BuildUsesExplicitFallbacksWhenCompositionIsUnavailable()
    {
        var exchange = AiExchangeTextBuilder.Build(null, null, "  ");

        Assert.Contains("Protocol vocabulary is not composed.", exchange);
        Assert.Contains("Grammar definition is not composed.", exchange);
        Assert.DoesNotContain("HOST CONTEXT / TASK", exchange);
    }

    [Fact]
    public void BuildTrimsAndIncludesOptionalHostContext()
    {
        var exchange = AiExchangeTextBuilder.Build("protocol", "grammar", "  task payload  ");

        Assert.Contains("HOST CONTEXT / TASK" + Environment.NewLine + Environment.NewLine + "task payload", exchange);
        Assert.DoesNotContain("  task payload  ", exchange);
    }
}
