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

    [Fact(DisplayName = "Moniker presentation is a phased six-color living glyph field")]
    public void MonikerPresentationUsesWorkshopPaletteAndPhasedGlyphContract()
    {
        var css = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "wwwroot", "css", "app.css"));
        var home = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "Pages", "Home.razor"));

        Assert.Contains(".flex-hello-moniker", home);
        Assert.Contains(".flex-hello-line", home);
        Assert.Contains(".flex-hello-glyph", home);
        Assert.Contains("THE", home);
        Assert.Contains("SINGULARITY", home);
        Assert.Contains("WORKSHOP", home);
        Assert.Contains("--phase", home);
        Assert.Contains("MonikerPhaseStepDegrees = 27", home);
        Assert.Contains("phase = glyphIndex * phaseStep", home);
        Assert.Contains("sin(calc(var(--wave-time)", home);
        Assert.Contains("rotateX(calc(sin", home);
        Assert.Contains("rotateY(calc(sin", home);
        Assert.Contains("rotateZ(calc(sin", home);
        Assert.Contains("@@property --wave-time", home);
        Assert.Contains("@@keyframes flexGlyphWave", home);
        Assert.DoesNotContain("MADE WITH UNITY", home);
        Assert.DoesNotContain("interop.initUnity", home);

        Assert.Contains("#52e05a", css); // green
        Assert.Contains("#00a8ff", css); // blue
        Assert.Contains("#ff2cff", css); // magenta
        Assert.Contains("#ff3030", css); // red
        Assert.Contains("#ff7a00", css); // orange
        Assert.Contains("#ffd34d", css); // yellow
    }
}
