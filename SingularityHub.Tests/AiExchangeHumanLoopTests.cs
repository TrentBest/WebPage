using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class AiExchangeHumanLoopTests
{
    [Fact(DisplayName = "AI exchange alternates through numbered human-in-the-loop rounds")]
    public void AiExchangeAlternatesThroughNumberedRounds()
    {
        using var experience = new WorkshopExperienceService();

        experience.Initialize();

        var exchange = Assert.NotNull(experience.AiExchangeComposition);

        var first = exchange!.BeginExchange();
        Assert.Equal(1, first.Number);
        Assert.NotEmpty(first.Outbound);
        Assert.Null(first.Response);

        var firstResponse = "ROUND 1 RESPONSE";
        var completed = exchange.RecordResponse(firstResponse);

        Assert.Equal(1, completed.Number);
        Assert.Equal(firstResponse, completed.Response);
        Assert.Single(exchange.Rounds);

        var second = exchange.BeginExchange();

        Assert.Equal(2, second.Number);
        Assert.Equal(first.Outbound, second.Outbound);
        Assert.Null(second.Response);
        Assert.Equal(2, exchange.Rounds.Count);
    }

    [Fact(DisplayName = "AI exchange refuses inbound data before an outbound round exists")]
    public void AiExchangeRequiresOutboundRoundBeforeResponse()
    {
        using var experience = new WorkshopExperienceService();

        experience.Initialize();

        var exchange = Assert.NotNull(experience.AiExchangeComposition);

        Assert.Throws<InvalidOperationException>(() =>
            exchange!.RecordResponse("response"));
    }
}
