using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class MonikerExperienceTests
{
    [Fact(DisplayName = "Moniker experience does not expose a Unity runtime handoff")]
    public void MonikerExperienceDoesNotExposeUnityRuntimeHandoff()
    {
        var service = new WorkshopExperienceService();

        service.Initialize();
        service.RequestEntry();

        Assert.Equal("FlexHello", service.CurrentState);
        Assert.False(service.ShowUnity);
    }

    [Fact(DisplayName = "Moniker presentation is the Workshop cyan and magenta water field")]
    public void MonikerPresentationUsesWorkshopPaletteAndSwayContract()
    {
        var css = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "wwwroot", "css", "app.css"));

        Assert.Contains(".flex-hello-moniker", css);
        Assert.Contains("monikerKelpSway", css);
        Assert.Contains("#00eaff", css);
        Assert.Contains("#ff2cff", css);
    }
}
