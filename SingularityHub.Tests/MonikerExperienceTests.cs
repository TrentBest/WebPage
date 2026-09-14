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

    [Fact(DisplayName = "Moniker presentation uses the Workshop six-color water-field palette")]
    public void MonikerPresentationUsesWorkshopPaletteAndSwayContract()
    {
        var css = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "wwwroot", "css", "app.css"));

        Assert.Contains(".flex-hello-moniker", css);
        Assert.Contains("monikerKelpSway", css);
        Assert.Contains("#52e05a", css); // green
        Assert.Contains("#00a8ff", css); // blue
        Assert.Contains("#ff2cff", css); // magenta
        Assert.Contains("#ff3030", css); // red
        Assert.Contains("#ff7a00", css); // orange
        Assert.Contains("#ffd34d", css); // yellow
    }
}
