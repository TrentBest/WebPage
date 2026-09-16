using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class ExperienceSpaceSpatialTests
{
    [Fact]
    public void ExperienceSpace_Footprint_UsesTopLeftCoordinates()
    {
        var space = new ExperienceSpace("forge", "The Forge", "FSM WORKSHOP", 66, 18, 78, 40, 78, 40,
            "", "FORGE", "FSM", 24, 18);

        Assert.Equal(78, space.Footprint.CenterX);
        Assert.Equal(27, space.Footprint.CenterY);
        Assert.Equal(24, space.Footprint.Width);
        Assert.Equal(18, space.Footprint.Height);
    }

    [Fact]
    public void ExperienceSpace_Entrance_RemainsOutsideItsWalkabilityFootprint()
    {
        var space = new ExperienceSpace("research", "Research Laboratory", "PARTICLE PHYSICS", 43, 44, 53, 64, 53, 64,
            "", "RESEARCH", "Chemistry", 20, 16);

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
        var forge = new ExperienceSpace("engineering", "The Forge", "FSM WORKSHOP", 66, 18, 78, 40, 78, 40,
            "", "FORGE", "FSM", 24, 18);

        Assert.True(pathfinding.TryPlan(50, 50, forge.EntranceX, forge.EntranceY, [forge.Footprint]));
        Assert.Equal(forge.EntranceX, pathfinding.CurrentPath[^1].X);
        Assert.Equal(forge.EntranceY, pathfinding.CurrentPath[^1].Y);
    }

    [Fact]
    public void Pathfinding_RouteContainsSurfaceTurns_WhenArchitectureBlocksDirectTravel()
    {
        using var pathfinding = new PathfindingMicroBundle();
        var blockingSuite = new ExperienceSpace("blocking", "Blocking Suite", "TEST", 60, 30, 72, 55, 72, 55,
            "", "TEST", "TEST", 20, 18);

        Assert.True(pathfinding.TryPlan(50, 50, 90, 55, [blockingSuite.Footprint]));
        Assert.True(pathfinding.CurrentPath.Count >= 3);
        Assert.Contains(pathfinding.CurrentPath, point => Math.Abs(point.X - 50) > 1 && Math.Abs(point.Y - 50) > 1);
    }

    [Fact]
    public void WorkshopSuiteLayout_DoesNotContainOverlappingFootprints()
    {
        var rooms = new[]
        {
            new ExperienceSpace("experience", "Experience Deck", "", 18, 18, 29, 40, 29, 40, "", "", "", 22, 18),
            new ExperienceSpace("engineering", "The Forge", "", 66, 18, 78, 40, 78, 40, "", "", "", 24, 18),
            new ExperienceSpace("research", "Research Laboratory", "", 43, 44, 53, 64, 53, 64, "", "", "", 20, 16),
            new ExperienceSpace("creation", "Creation Bay", "", 30, 72, 42, 90, 42, 90, "", "", "", 24, 18),
            new ExperienceSpace("architecture", "Architectural Shop", "", 4, 70, 15, 92, 15, 92, "", "", "", 22, 20),
            new ExperienceSpace("space-elevator", "Space Elevator", "", 80, 45, 86, 68, 86, 68, "", "", "", 12, 42),
            new ExperienceSpace("unknown", "Unknown Annex", "", 64, 74, 70, 92, 70, 92, "", "", "", 12, 18)
        };

        for (var i = 0; i < rooms.Length; i++)
        for (var j = i + 1; j < rooms.Length; j++)
        {
            var a = rooms[i].Footprint;
            var b = rooms[j].Footprint;
            var overlapX = Math.Abs(a.CenterX - b.CenterX) < (a.Width + b.Width) / 2;
            var overlapY = Math.Abs(a.CenterY - b.CenterY) < (a.Height + b.Height) / 2;
            Assert.False(overlapX && overlapY, $"{rooms[i].Id} overlaps {rooms[j].Id}.");
        }
    }
}
