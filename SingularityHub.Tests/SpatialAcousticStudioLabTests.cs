using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialAcousticStudioLabTests
{
    [Fact(DisplayName = "Acoustic Studio Lab has stable source and listener identities")]
    public void DefaultManifestProvidesStableSpatialParticipants()
    {
        var studio = SpatialAcousticStudioManifest.CreateDefault();

        Assert.Equal("workshop.acoustic-studio", studio.Id);
        Assert.Equal(3, studio.Sources.Count);
        Assert.Equal(2, studio.Listeners.Count);
        Assert.All(studio.Sources, source =>
        {
            Assert.False(string.IsNullOrWhiteSpace(source.Id));
            Assert.InRange(source.PositionMeters.X, 0, studio.WidthMeters);
            Assert.InRange(source.PositionMeters.Y, 0, studio.HeightMeters);
        });
        Assert.All(studio.Listeners, listener =>
        {
            Assert.False(string.IsNullOrWhiteSpace(listener.Id));
            Assert.InRange(listener.PositionMeters.X, 0, studio.WidthMeters);
            Assert.InRange(listener.PositionMeters.Y, 0, studio.HeightMeters);
        });
    }

    [Fact(DisplayName = "Acoustic Studio Lab fixture values are explicitly authored and optional by pair")]
    public void FixtureObservationsDoNotPretendToCoverEveryPair()
    {
        var studio = SpatialAcousticStudioManifest.CreateDefault();

        Assert.NotNull(studio.FindFixtureObservation("source-vocal", "listener-control"));
        Assert.Null(studio.FindFixtureObservation("source-vocal", "listener-booth"));
        Assert.Equal("Vocal Source", studio.ResolveSource("source-vocal").Name);
        Assert.Equal("Control Listener", studio.ResolveListener("listener-control").Name);
        Assert.Throws<KeyNotFoundException>(() => studio.ResolveSource("missing-source"));
    }

    [Fact(DisplayName = "Acoustic Studio Lab is a real navigable Workshop campus place")]
    public void AcousticStudioIsRegisteredAndPlacedInScene()
    {
        var place = SpatialPlaceCatalog.Resolve("acoustic-studio-lab");
        var scene = SpatialWorkshopScene.CreateDefault();
        var interactable = Assert.Single(scene.Interactables, item => item.ExperienceId == "acoustic-studio-lab");

        Assert.Equal(SpatialDomain.Workshop, place.Domain);
        Assert.Equal("Acoustic Studio Lab", place.Name);
        Assert.Equal(place.Name, interactable.Name);
        Assert.Equal("acoustic-studio-lab", interactable.ExperienceId);
        Assert.Contains(SingularityCampusCatalog.Buildings, building => building.Id == "acoustic-studio-lab");
    }
}
