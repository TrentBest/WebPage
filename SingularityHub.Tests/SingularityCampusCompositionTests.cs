using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityCampusCompositionTests
{
    [Fact(DisplayName = "Incremental Unit Test 36 — Campus composition exposes transit and ontology mall")]
    public void CampusContainsTransitAndOntologyMall()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.Contains(scene.Interactables, item => item.ExperienceId == "singularity-transit");
        Assert.Contains(SingularityCampusCatalog.Buildings, building => building.Id == "singularity-ontology-mall");
        Assert.Contains(SingularityCampusCatalog.TransitDestinations, destination => destination.SceneId == "singularity-ontology-mall");
        Assert.Equal(4, SingularityCampusCatalog.OntologyDepartments.Count);
    }

    [Fact(DisplayName = "Incremental Unit Test 37 — Campus keeps the avatar in open space")]
    public void CampusStartIsNotInsideABuilding()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.DoesNotContain(scene.Interactables, item => item.Bounds.Contains(scene.StartingPosition));
    }

    [Fact(DisplayName = "Incremental Unit Test 38 — Campus buildings are materially separated")]
    public void CampusHasSpatialSeparation()
    {
        var mansion = SingularityCampusCatalog.Buildings.Single(item => item.Id == "singularity-mansion").Bounds;
        var mall = SingularityCampusCatalog.Buildings.Single(item => item.Id == "singularity-ontology-mall").Bounds;

        Assert.True(mansion.X + mansion.Width < mall.X);
        Assert.True(mansion.Y + mansion.Height < mall.Y);
    }
}
