using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratoryDestructionCatalogTests
{
    [Fact]
    public void DestructionCatalog_ModelsPhysicsDemonstrationsRatherThanGameRules()
    {
        var catalog = SpatialLaboratoryDestructionCatalog.CreateDefault();

        Assert.Contains(catalog.Sandboxes, x => x.Id == "castle-impact");
        Assert.Contains(catalog.Sandboxes, x => x.Id == "catapult");
        Assert.Contains(catalog.Sandboxes, x => x.Id == "ramp-ball");
    }

    [Fact]
    public void DestructionCatalog_IncludesMovableThreeDimensionalLauncher()
    {
        var catalog = SpatialLaboratoryDestructionCatalog.CreateDefault();
        var launcher = catalog.Find("orbital-launcher");

        Assert.NotNull(launcher);
        Assert.Contains("track", launcher.Value.ApparatusIds);
        Assert.Contains("launcher-carriage", launcher.Value.ApparatusIds);
        Assert.Contains("trajectory-volume", launcher.Value.ApparatusIds);
    }
}
