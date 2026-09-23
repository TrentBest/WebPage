using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class MonikerExperienceTests
{
    [Fact(DisplayName = "Moniker experience does not expose a Unity runtime handoff")]
    public void MonikerExperienceDoesNotExposeUnityRuntimeHandoff()
    {
        using var service = new WorkshopExperienceService();

        service.Initialize();
        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Gateway"; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
        service.RequestEntry();
        service.Tick();

        Assert.Equal("Moniker", service.FirstContact.CurrentState);
        Assert.False(service.ShowUnity);
    }

    [Fact(DisplayName = "Moniker presentation is a three-second living gateway glyph")]
    public void MonikerPresentationUsesWorkshopPaletteAndPhasedGlyphContract()
    {
        var home = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.Contains(".moniker-foreground", home);
        Assert.Contains("THE", home);
        Assert.Contains("SINGULARITY", home);
        Assert.Contains("WORKSHOP", home);
        Assert.Contains("Moniker", home);
        Assert.DoesNotContain("MADE WITH UNITY", home);
        Assert.DoesNotContain("interop.initUnity", home);
    }

    [Fact(DisplayName = "Gateway presentation remains a visible button and warning card")]
    public void GatewayPresentationRemainsStable()
    {
        var gateway = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "WorkshopGatewayView.razor"));

        Assert.Contains("enter-workshop", gateway);
        Assert.Contains("ENTER THE WORKSHOP", gateway);
        Assert.Contains("SYSTEM ADVISORY: MAXIMUM OVERDRIVE ACTIVE", gateway);
        Assert.Contains("border:2px solid #00eaff", gateway);
    }

    [Fact(DisplayName = "First-contact presentation is driven by explicit FSM phases")]
    public void FirstContactPresentationUsesExplicitPhasesWithoutAFlash()
    {
        using var service = new WorkshopExperienceService();

        service.Initialize();

        Assert.Equal("FirstOnly", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);

        // FirstOnly owns the entire fade-in. The transition is evaluated
        // immediately after the tick that reaches full visibility.
        for (var i = 0; i < 30; i++)
            service.Tick();

        Assert.Equal("FirstFadingSecondComingIn", service.FirstContact.CurrentState);
        Assert.Equal(1d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);

        // FSM_API defers OnEnter for the newly selected state until the next
        // tick. Therefore the handoff frame remains exactly 1 / 0.
        service.Tick();

        Assert.Equal("FirstFadingSecondComingIn", service.FirstContact.CurrentState);
        Assert.True(service.FirstContact.StatementOpacity < 1d);
        Assert.True(service.FirstContact.QuestionOpacity > 0d);
        Assert.Equal(
            1d,
            service.FirstContact.StatementOpacity + service.FirstContact.QuestionOpacity,
            6);

        // Complete the crossfade. The transition to SecondOnly happens on
        // the same tick that the incoming label reaches exactly 1.
        for (var i = 0; i < 29; i++)
            service.Tick();

        Assert.Equal("SecondOnly", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(1d, service.FirstContact.QuestionOpacity, 6);

        // SecondOnly owns the final fade-out. There is no return to either
        // of the preceding presentation phases.
        service.Tick();

        Assert.Equal("SecondOnly", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.InRange(service.FirstContact.QuestionOpacity, 0d, 1d);
        Assert.True(service.FirstContact.QuestionOpacity < 1d);

        for (var i = 0; i < 29; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);
    }

    [Fact(DisplayName = "Gateway does not render a duplicate first-contact question")]
    public void GatewayDoesNotReintroduceFirstContactQuestion()
    {
        var view = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Components", "FirstContactView.razor"));

        Assert.DoesNotContain("gateway-memory", view);
        Assert.DoesNotContain("HOW MANY WORDS IS A LIVING IMAGE WORTH?</div>", view);
    }
}
