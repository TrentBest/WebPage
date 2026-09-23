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
    [Fact(DisplayName = "First-contact labels crossfade on a 180 degree phase boundary")]
    public void FirstContactLabelsCrossfadeWithoutAFlash()
    {
        using var service = new WorkshopExperienceService();

        service.Initialize();

        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);

        // Complete the statement's fade-in. The next FSM phase must begin with
        // the statement fully visible and the question fully invisible.
        for (var i = 0; i < 30; i++)
            service.Tick();

        Assert.Equal("Question", service.FirstContact.CurrentState);
        Assert.Equal(1d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);

        // The very next frame is the crossfade: statement falls while question rises.
        service.Tick();

        Assert.InRange(service.FirstContact.StatementOpacity, 0d, 1d);
        Assert.InRange(service.FirstContact.QuestionOpacity, 0d, 1d);
        Assert.True(service.FirstContact.StatementOpacity < 1d);
        Assert.True(service.FirstContact.QuestionOpacity > 0d);
        Assert.Equal(
            1d,
            service.FirstContact.StatementOpacity + service.FirstContact.QuestionOpacity,
            6);

        // At the midpoint the two labels meet exactly at the phase boundary.
        for (var i = 0; i < 29; i++)
            service.Tick();

        Assert.Equal(30L, service.FirstContact.StateTicks);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(1d, service.FirstContact.QuestionOpacity, 6);

        // The second label then fades away; the landing state follows immediately.
        for (var i = 0; i < 30; i++)
            service.Tick();

        Assert.Equal("Gateway", service.FirstContact.CurrentState);
        Assert.Equal(0d, service.FirstContact.StatementOpacity, 6);
        Assert.Equal(0d, service.FirstContact.QuestionOpacity, 6);
    }

}
