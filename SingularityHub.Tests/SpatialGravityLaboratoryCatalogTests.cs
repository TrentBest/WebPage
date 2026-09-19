using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialGravityLaboratoryCatalogTests
{
    [Fact]
    public void GravityLab_ProvidesColoredBodiesAndFieldSamples()
    {
        var catalog = SpatialGravityBodyCatalog.CreateDefault();

        Assert.Contains(catalog.Bodies, x => x.Id == "earth" && x.Color == "#4da6ff");
        Assert.Contains(catalog.Bodies, x => x.Id == "moon");
        var heatmap = catalog.Heatmap("earth", "moon");

        Assert.Equal(64, heatmap.Count);
        Assert.Contains(heatmap, x => x.GravityInteractionActive);
        Assert.All(heatmap, x => Assert.InRange(x.Intensity, 0, 1));
    }
}