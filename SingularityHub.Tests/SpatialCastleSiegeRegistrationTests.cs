using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Guards the stable spatial registration for the authored Castle Siege destination.</summary>
public sealed class SpatialCastleSiegeRegistrationTests
{
    [Fact]
    public void CastleSiegeResolvesAsASingularityCityPlace()
    {
        var place = SpatialPlaceCatalog.Resolve("castle-siege");

        Assert.Equal("city.castle-siege", place.Id);
        Assert.Equal("Castle Siege", place.Name);
        Assert.Equal(SpatialDomain.SingularityCity, place.Domain);
        Assert.Null(place.ToolId);
    }

    [Fact]
    public void DefaultSpatialSceneContainsTheRegisteredCastleSiegeInteractable()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var interactable = Assert.Single(
            scene.Interactables,
            item => item.ExperienceId == "castle-siege");

        Assert.Equal("Castle Siege", interactable.Name);
        Assert.Equal("castle-siege", interactable.Place?.SceneId);
    }
}
