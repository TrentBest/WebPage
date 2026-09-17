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
        Assert.Contains(scene.Interactables, item => item.ExperienceId == "singularity-ontology-mall");
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
        var scene = SpatialWorkshopScene.CreateDefault();
        var mansion = scene.Interactables.Single(item => item.Id == "singularity-mansion");
        var mall = scene.Interactables.Single(item => item.Id == "singularity-ontology-mall");

        Assert.True(mansion.Bounds.X + mansion.Bounds.Width < mall.Bounds.X);
        Assert.True(mansion.Bounds.Y + mansion.Bounds.Height < mall.Bounds.Y);
    }
}
