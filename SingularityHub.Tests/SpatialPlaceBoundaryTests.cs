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
            var place = item.Place ?? throw new InvalidOperationException($"Interactable '{item.Name}' has no authoritative place.");
            Assert.Equal(item.ExperienceId, place.SceneId);
            Assert.Equal(item.Name, place.Name);
            Assert.NotEqual(default, place.EntryPoint);
        });
    }

    [Fact]
    public void WorkshopFootprintsDoNotStructurallyOverlap()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        for (var i = 0; i < scene.Interactables.Count; i++)
        {
            for (var j = i + 1; j < scene.Interactables.Count; j++)
            {
                var a = scene.Interactables[i].Bounds;
                var b = scene.Interactables[j].Bounds;
                Assert.False(
                    a.X < b.X + b.Width && b.X < a.X + a.Width &&
                    a.Y < b.Y + b.Height && b.Y < a.Y + a.Height,
                    $"'{scene.Interactables[i].Name}' overlaps '{scene.Interactables[j].Name}'.");
            }
        }
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

        Assert.DoesNotContain(scene.Interactables, x => x.Id == "singularity-shipyard");
        Assert.Equal(new SpatialPoint(50, 72), place.EntryPoint);
        Assert.Equal("singularity-shipyard", place.SceneId);
    }

    [Fact]
    public void RouterMaterializesThePlaceAtItsDeclaredInteriorEntry()
    {
        var place = SpatialPlaceCatalog.Resolve("singularity-shipyard");
        var router = new WorkshopRouter();

        router.RegisterPlace(place);
        Assert.Equal(place.EntryPoint, router.GetEntryPoint(place.SceneId));

        var firstVisit = router.Enter(place, new SpatialPoint(90, 18));

        Assert.True(firstVisit);
        Assert.Equal(place.SceneId, router.CurrentSceneId);
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
