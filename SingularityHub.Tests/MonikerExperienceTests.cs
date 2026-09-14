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

        Assert.Contains(".hello-moniker", home);
        Assert.Contains(".hello-line", home);
        Assert.Contains(".hello-glyph", home);
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
        Assert.Contains("@@keyframes helloGlyphWave", home);
        Assert.DoesNotContain("MADE WITH UNITY", home);
        Assert.DoesNotContain("interop.initUnity", home);

        Assert.Contains("#52e05a", css);
        Assert.Contains("#00a8ff", css);
        Assert.Contains("#ff2cff", css);
        Assert.Contains("#ff3030", css);
        Assert.Contains("#ff7a00", css);
        Assert.Contains("#ffd34d", css);
    }
}
