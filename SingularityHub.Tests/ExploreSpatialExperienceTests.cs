using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Incremental Unit Test 05 — the Explore experience becomes a spatial tool complex.
///
/// This test intentionally names the next architectural step before its renderer is
/// complete. The test is the ledger entry for the work: Explore begins inside the
/// Forge rather than presenting the Forge as a button, the complex contains tools,
/// view controls, and construction activity, and spatial interaction remains data.
/// </summary>
public sealed class ExploreSpatialExperienceTests
{
    [Fact(DisplayName = "Incremental Unit Test 05 — Explore starts inside the Forge tool complex")]
    public void Explore_StartsInsideForgeWithToolsViewControlsAndConstructionActivity()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.Equal("engineering", scene.StartingAreaId);
        Assert.Equal(73d, scene.StartingPosition.X);
        Assert.Equal(42.5d, scene.StartingPosition.Y);

        Assert.Contains(scene.Interactables, item => item.Id == "forge");
        Assert.Contains(scene.Interactables, item => item.Id == "image-tools");
        Assert.Contains(scene.Interactables, item => item.Id == "npc-studio");
        Assert.Contains(scene.Interactables, item => item.Id == "fsm-bench");

        Assert.Equal(64, scene.View.DetailLevels.Count);
        Assert.Contains(scene.View.DetailLevels, level => level == SpatialDetailLevel.Device);
        Assert.Contains(scene.View.DetailLevels, level => level == SpatialDetailLevel.Stud);

        Assert.Contains(scene.ConstructionVehicles, vehicle => vehicle.Id == "crane-01");
        Assert.Contains(scene.ConstructionVehicles, vehicle => vehicle.Id == "rover-01");
    }

    [Fact(DisplayName = "Incremental Unit Test 05 — spatial hover is normalized and semantic")]
    public void SpatialInteractable_HoverUsesNormalizedCoordinateAndBreathesOnlyInsideHitRegion()
    {
        var interactable = new SpatialInteractable(
            "forge",
            "The Forge",
            new SpatialBounds(20, 20, 30, 25),
            new SpatialBounds(30, 32, 10, 8),
            new SpatialRectangularHitRegion(0, 0, 1, 1));

        Assert.False(interactable.IsBreathing);
        Assert.True(interactable.OnHover(new NormalizedPointer(0.5, 0.5)));
        Assert.True(interactable.IsBreathing);
        Assert.Equal(new NormalizedPointer(0.5, 0.5), interactable.LastHover);

        Assert.False(interactable.OnHover(new NormalizedPointer(1.1, 0.5)));
        Assert.True(interactable.IsBreathing);
    }
}
