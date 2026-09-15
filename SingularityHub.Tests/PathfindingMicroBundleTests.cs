using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Incremental architectural tests for the Workshop pathfinding capability.
/// The tests deliberately exercise the bundle without rendering the world so
/// pathfinding remains a domain capability rather than GUI behavior.
/// </summary>
public sealed class PathfindingMicroBundleTests
{
    [Fact]
    public void Pathfinding_UsesItsOwnStableMicroBundleIdentity()
    {
        using var pathfinding = new PathfindingMicroBundle();

        Assert.Equal((ulong)PathfindingMicroBundle.BundleId, pathfinding.Id);
    }

    [Fact]
    public void Pathfinding_RoutesAroundSuiteFootprint()
    {
        using var pathfinding = new PathfindingMicroBundle();
        var forge = new SpatialRoom(
            "engineering", "The Forge", "FSM WORKSHOP",
            73, 30,
            73, 46,
            73, 46,
            "", "FORGE", null,
            30, 25);

        Assert.True(pathfinding.TryPlan(
            50, 50,
            forge.EntranceX, forge.EntranceY,
            [forge.Footprint]));

        Assert.NotEmpty(pathfinding.CurrentPath);
        Assert.Equal(forge.EntranceX, pathfinding.CurrentPath[^1].X);
        Assert.Equal(forge.EntranceY, pathfinding.CurrentPath[^1].Y);

        Assert.All(pathfinding.CurrentPath, point =>
            Assert.True(
                Math.Abs(point.X - forge.X) > forge.Width / 2 + PathfindingMicroBundle.Clearance ||
                Math.Abs(point.Y - forge.Y) > forge.Height / 2 + PathfindingMicroBundle.Clearance));
    }

    [Fact]
    public void Pathfinding_ConsumesWaypointsInOrder()
    {
        using var pathfinding = new PathfindingMicroBundle();
        var obstacle = new SpatialObstacle("suite", 60, 50, 20, 20);

        Assert.True(pathfinding.TryPlan(50, 50, 80, 50, [obstacle]));

        var consumed = new List<SpatialWaypoint>();
        while (pathfinding.TryTakeNext(out var waypoint))
            consumed.Add(waypoint);

        Assert.NotEmpty(consumed);
        Assert.Equal(80, consumed[^1].X);
        Assert.Equal(50, consumed[^1].Y);
        Assert.False(pathfinding.IsWalking);
    }
}
