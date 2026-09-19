using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratoryExperimentCatalogTests
{
    [Fact]
    public void ExperimentCatalog_ReplacesWorksheetWithComposableApparatus()
    {
        var catalog = SpatialLaboratoryExperimentCatalog.CreateDefault();

        Assert.Contains(catalog.Templates, x => x.Id == "ramp-and-mass");
        Assert.Contains(catalog.Templates, x => x.Id == "hydrogen-balloon");
        Assert.Contains(catalog.Templates, x => x.Id == "ballistics");
        Assert.Contains(catalog.Items, x => x.Id == "bunsen-burner");
        Assert.Contains(catalog.Items, x => x.Id == "hydrogen-spigot");
        Assert.Contains(catalog.Items, x => x.Id == "gravity-tray");
    }
}