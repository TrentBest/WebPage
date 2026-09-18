using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialTransitAndLaboratoryTests
{
    [Fact(DisplayName = "Incremental Unit Test 53 — Tram manifest defines rails, platform, passengers, and onward destinations")]
    public void TramManifestDefinesCompleteFirstJourney()
    {
        var transit = SpatialTransitManifest.CreateDefault();

        Assert.True(transit.TrackCenterline.Count >= 2);
        Assert.True(transit.PlatformBounds.Width > 0);
        Assert.True(transit.PlatformBounds.Height > 0);
        Assert.Equal(10, transit.Passengers.Count);
        Assert.Contains(transit.Destinations, item => item.Id == "space-elevator");
        Assert.True(transit.Timing.CycleSeconds > transit.Timing.ArrivalSeconds);
    }

    [Fact(DisplayName = "Incremental Unit Test 54 — Singularity Lab manifest is a multi-floor data composition")]
    public void SingularityLabManifestIsDataDriven()
    {
        var lab = SpatialLaboratoryManifest.CreateDefault();

        Assert.Equal("singularity-lab", lab.Id);
        Assert.True(lab.Floors.Count >= 5);
        Assert.Contains(lab.Simulators, item => item.Scale == "PLANETARY");
        Assert.Contains(lab.Simulators, item => item.Scale == "SOLAR SYSTEM");
        Assert.Contains(lab.Simulators, item => item.Scale == "GALAXY");
        Assert.Contains(lab.Lessons, item => item.Concepts.Contains("brittleness"));
        Assert.Contains(lab.Lessons, item => item.Concepts.Contains("ductility"));
        Assert.Contains(lab.Lessons, item => item.Concepts.Contains("conductivity"));
        Assert.Contains(lab.Simulators, item => item.Scale == "TERRAIN");
    }
}
