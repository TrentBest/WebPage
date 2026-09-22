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
}
