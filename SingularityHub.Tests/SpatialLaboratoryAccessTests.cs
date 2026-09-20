using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialLaboratoryAccessTests
{
    [Fact]
    public void Laboratory_IsRegistered_With_A_Dedicated_Entrance_And_Reachable_Path()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var lab = Assert.Single(scene.Interactables, item => item.Id == "singularity-lab");

        var obstacles = SpatialGeometryEngine.FromInteractables(scene.Interactables);
        var path = SpatialGeometryEngine.FindPath(
            scene.StartingPosition,
            new SpatialPoint(
                lab.InteractionPoint.X + lab.InteractionPoint.Width / 2d,
                lab.InteractionPoint.Y + lab.InteractionPoint.Height / 2d),
            obstacles);

        Assert.Equal("singularity-lab", lab.ExperienceId);
        Assert.Equal("Singularity Laboratory", lab.Name);
        Assert.NotEmpty(path);
    }

    [Fact]
    public void Laboratory_Security_Requires_The_Documented_Visitor_Sequence()
    {
        using var lab = new SpatialSingularityLabModel();

        Assert.Equal(SpatialSingularityLabStage.ApproachSecurity, lab.Stage);

        lab.EnterSecurityLine();
        Assert.Equal(SpatialSingularityLabStage.SecurityQueue, lab.Stage);

        lab.PlaceBelongings();
        Assert.Equal(SpatialSingularityLabStage.SecurityScreening, lab.Stage);

        lab.CompleteScreening();
        Assert.Equal(SpatialSingularityLabStage.SecurityComplete, lab.Stage);

        lab.EnterElevator();
        Assert.Equal(SpatialSingularityLabStage.ElevatorElevation, lab.Stage);

        lab.ScanId("VISITOR");
        lab.ScanFingerprint();
        lab.ScanRetina();
        Assert.Equal(SpatialSingularityLabStage.FloorSelection, lab.Stage);

        lab.SelectFloor(1);
        lab.ResolveFloorChallenge();
        Assert.Equal(SpatialSingularityLabStage.FloorOpen, lab.Stage);
    }
}
