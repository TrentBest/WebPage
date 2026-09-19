using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratoryManifestTests
{
    [Fact]
    public void Laboratory_ModelsScannerGuardAndAdministrationFlow()
    {
        var lab = SpatialLaboratoryManifest.CreateDefault();

        Assert.Equal("lab-badge-scanner", lab.Security.ScannerId);
        Assert.Contains(lab.Security.Guards, x => x.Name == "Guard #1");
        Assert.Equal("lab-administrator", lab.Administration.DigitenId);
        Assert.Contains(lab.Administration.Services, x => x.Id == "guided-tour");
        Assert.Contains(lab.Administration.Services, x => x.Id == "visitor-pass");
        Assert.Contains(lab.Administration.Services, x => x.Id == "lab-space");

        lab.Security.ScanBadge();
        Assert.True(lab.Security.BadgeAccepted);
    }

    [Fact]
    public void Laboratory_DescendsIntoPhysicsDomains()
    {
        var lab = SpatialLaboratoryManifest.CreateDefault();

        Assert.Equal("Mechanics", lab.FindFloor(1)!.Name);
        Assert.Equal("Thermodynamics", lab.FindFloor(3)!.Name);
        Assert.Equal("Hydrodynamics", lab.FindFloor(4)!.Name);
        Assert.Equal("Simulation Baking", lab.FindFloor(8)!.Name);
    }
}
