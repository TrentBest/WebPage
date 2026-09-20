using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Incremental Unit Test 05 — Explore becomes a spatial Workshop complex whose
/// structures surround a clear central origin rather than enclosing the visitor.
/// </summary>
public sealed class ExploreSpatialExperienceTests
{
    [Fact(DisplayName = "Incremental Unit Test 05 — Explore starts at the clear center of the Workshop complex")]
    public void Explore_StartsAtClearWorkshopCenterWithToolsViewControlsAndConstructionActivity()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.Equal("center", scene.StartingAreaId);
        Assert.Equal(50d, scene.StartingPosition.X);
        Assert.Equal(58d, scene.StartingPosition.Y);
        Assert.DoesNotContain(scene.Interactables, item => Contains(item.Bounds, scene.StartingPosition));

        Assert.Equal(
            [
                "aec-remote-office",
                "blueprint-library",
                "forge",
                "fsm-bench",
                "npc-studio",
                "singularity-lab",
                "singularity-software-office",
                "singularity-transit"
            ],
            scene.Interactables.Select(item => item.Id).OrderBy(id => id));

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
            new SpatialRectangularHitRegion(0, 0, 1, 1),
            "forge");

        Assert.False(interactable.IsBreathing);
        Assert.True(interactable.OnHover(new NormalizedPointer(0.5, 0.5)));
        Assert.True(interactable.IsBreathing);
        Assert.Equal(new NormalizedPointer(0.5, 0.5), interactable.LastHover);

        Assert.False(interactable.OnHover(new NormalizedPointer(1.1, 0.5)));
        Assert.True(interactable.IsBreathing);
    }

    private static bool Contains(SpatialBounds bounds, SpatialPoint point)
        => point.X >= bounds.X && point.X <= bounds.X + bounds.Width && point.Y >= bounds.Y && point.Y <= bounds.Y + bounds.Height;
}
