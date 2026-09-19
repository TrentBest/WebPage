using System;
using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Breadcrumbs for the spatial pointer contract: click intent, arrival, interaction, and scene entry.</summary>
public sealed class SpatialPointerInteractionTests
{
    [Fact(DisplayName = "Incremental Unit Test 21 — a spatial building exposes a declared arrival point and fires OnInteraction only when invoked")]
    public void SpatialInteractable_InteractionLifecycleIsArrivalDriven()
    {
        var forge = SpatialWorkshopScene.CreateDefault().Interactables.Single(item => item.Id == "forge");
        var fired = 0;
        forge.Interaction += () => fired++;

        Assert.Equal(new SpatialBounds(78, 41, 4, 3), forge.InteractionPoint);
        Assert.Equal(0, forge.InteractionCount);
        Assert.Equal(0, fired);

        forge.OnInteraction();

        Assert.Equal(1, forge.InteractionCount);
        Assert.Equal(1, fired);
    }

    [Fact(DisplayName = "Incremental Unit Test 22 — an empty-space click target remains distinct from an interactable")]
    public void SpatialInteractable_ClickHitRegionRemainsIndependentFromArrivalPoint()
    {
        var office = SpatialWorkshopScene.CreateDefault().Interactables.Single(item => item.Id == "aec-remote-office");

        Assert.True(office.OnClick(new NormalizedPointer(.5, .5)));
        Assert.False(office.OnClick(new NormalizedPointer(-.1, -.1)));
        Assert.Equal(0, office.InteractionCount);
        Assert.NotEqual(office.Bounds.X, office.InteractionPoint.X);
    }

    [Fact(DisplayName = "Incremental Unit Test 23 — spatial interactables declare the scene they enter")]
    public void SpatialInteractable_DeclaresExperienceScene()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.Equal("forge", scene.Interactables.Single(item => item.Id == "forge").ExperienceId);
        Assert.Equal("library", scene.Interactables.Single(item => item.Id == "blueprint-library").ExperienceId);
    }
}
