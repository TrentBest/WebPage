using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class ExperienceSpaceSpatialTests
{
    [Fact]
    public void ExperienceSpace_Footprint_UsesTopLeftCoordinates()
    {
        var space = new ExperienceSpace(
            "forge",
            "The Forge",
            "FSM WORKSHOP",
            66,
            18,
            78,
            40,
            78,
            40,
            "",
            "FORGE",
            "FSM",
            24,
            18);

        Assert.Equal(78, space.Footprint.CenterX);
        Assert.Equal(27, space.Footprint.CenterY);
        Assert.Equal(24, space.Footprint.Width);
        Assert.Equal(18, space.Footprint.Height);
    }

    [Fact]
    public void ExperienceSpace_Entrance_RemainsOutsideItsWalkabilityFootprint()
    {
        var space = new ExperienceSpace(
            "research",
            "Research Laboratory",
            "PARTICLE PHYSICS",
            43,
            44,
            53,
            64,
            53,
            64,
            "",
            "RESEARCH",
            "Chemistry",
            20,
            16);

        var obstacle = space.Footprint;
        var clearOfObstacle =
            Math.Abs(space.EntranceX - obstacle.CenterX) > obstacle.Width / 2 + PathfindingMicroBundle.Clearance ||
            Math.Abs(space.EntranceY - obstacle.CenterY) > obstacle.Height / 2 + PathfindingMicroBundle.Clearance;

        Assert.True(clearOfObstacle);
    }

    [Fact]
    public void Pathfinding_CanReach_Workshop_Forge_Entrance()
    {
        using var pathfinding = new PathfindingMicroBundle();
        var forge = new ExperienceSpace(
            "engineering",
            "The Forge",
            "FSM WORKSHOP",
            66,
            18,
            78,
            40,
            78,
            40,
            "",
            "FORGE",
            "FSM",
            24,
            18);

        Assert.True(pathfinding.TryPlan(
            50,
            50,
            forge.EntranceX,
            forge.EntranceY,
            [forge.Footprint]));

        Assert.Equal(forge.EntranceX, pathfinding.CurrentPath[^1].X);
        Assert.Equal(forge.EntranceY, pathfinding.CurrentPath[^1].Y);
    }
}