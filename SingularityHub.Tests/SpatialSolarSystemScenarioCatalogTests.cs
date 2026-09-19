using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialSolarSystemScenarioCatalogTests
{
    [Fact]
    public void Catalog_SwitchesBetweenAstronomicalFictionalAnd3DModels()
    {
        var catalog = SpatialSolarSystemScenarioCatalog.CreateDefault();

        Assert.Equal(SpatialSolarSystemModelKind.Astronomical, catalog.Find("solar-system")!.Kind);
        Assert.Equal(SpatialSolarSystemModelKind.FictionalStory, catalog.Find("avatar-story-system")!.Kind);
        Assert.Equal(SpatialSolarSystemModelKind.GameHomage, catalog.Find("asteroids-2d")!.Kind);
        Assert.Equal(SpatialSolarSystemModelKind.Experimental, catalog.Find("asteroids-3d")!.Kind);
    }
}