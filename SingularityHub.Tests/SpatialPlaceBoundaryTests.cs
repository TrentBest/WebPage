using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialPlaceBoundaryTests
{
    [Fact]
    public void EveryExploreInteractableResolvesToOneAuthoritativePlace()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.Equal(scene.Interactables.Count, scene.Interactables.Select(x => x.Place.Id).Distinct().Count());
        Assert.All(scene.Interactables, item =>
        {
            Assert.Equal(item.ExperienceId, item.Place.SceneId);
            Assert.Equal(item.Name, item.Place.Name);
            Assert.NotEqual(default, item.Place.EntryPoint);
        });
    }

    [Fact]
    public void WorkshopAndCityPlacesRemainExplicitlySeparated()
    {
        Assert.All(SpatialPlaceCatalog.WorkshopPlaces, place =>
            Assert.Equal(SpatialDomain.Workshop, place.Domain));

        Assert.All(SpatialPlaceCatalog.CityPlaces, place =>
            Assert.Equal(SpatialDomain.SingularityCity, place.Domain));

        var shipyard = SpatialPlaceCatalog.Resolve("singularity-shipyard");
        Assert.Equal("Singularity Shipyard", shipyard.Name);
        Assert.Equal(SpatialDomain.SingularityCity, shipyard.Domain);
        Assert.Equal("vehicle-builder", shipyard.ToolId);
    }

    [Fact]
    public void ShipyardUsesItsOwnSceneIdentityAndEntryPoint()
    {
        var place = SpatialPlaceCatalog.Resolve("singularity-shipyard");
        var scene = SpatialWorkshopScene.CreateDefault();
        var interactable = scene.Interactables.Single(x => x.Id == "singularity-shipyard");

        Assert.Same(place, interactable.Place);
        Assert.Equal(place.EntryPoint, interactable.InteractionPoints[0].Bounds is var bounds
            ? new SpatialPoint(bounds.X + bounds.Width / 2d, bounds.Y + bounds.Height / 2d)
            : default);
        Assert.Equal("singularity-shipyard", interactable.ExperienceId);
    }

    [Fact]
    public void ShipyardManifestRepresentsAWorkingYard()
    {
        var manifest = SpatialShipyardManifest.CreateDefault();

        Assert.Equal("SINGULARITY SHIPYARD", manifest.Name);
        Assert.Contains(manifest.Services, x => x.Id == "design");
        Assert.Contains(manifest.Services, x => x.Id == "construction");
        Assert.Contains(manifest.Services, x => x.Id == "resource");
        Assert.Contains(manifest.BuildRules, x => x.Id == "capability-driven");
    }
}
