using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialConstructionAndMapTests
{
    [Fact(DisplayName = "Incremental Unit Test 51 — Construction real estate encloses every work vehicle")]
    public void ConstructionSiteEnclosesVehicles()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.All(scene.ConstructionVehicles, vehicle => Assert.True(scene.ConstructionSite.Bounds.Contains(vehicle.Position)));
        Assert.True(scene.ConstructionSite.Bounds.Width > scene.ConstructionSite.FoundationBounds.Width);
        Assert.True(scene.ConstructionSite.Bounds.Height > scene.ConstructionSite.FoundationBounds.Height);
    }

    [Fact(DisplayName = "Incremental Unit Test 52 — Campus spacing separates the construction site from the ontology mall")]
    public void ConstructionSiteAndMallAreSeparate()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var lab = scene.Interactables.Single(item => item.Id == "singularity-lab");
        var site = scene.ConstructionSite.Bounds;

        Assert.True(site.X > lab.Bounds.X + lab.Bounds.Width);
    }
}
