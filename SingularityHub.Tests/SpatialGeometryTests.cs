using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Incremental Unit Test 25 — spatial geometry is explicit data shared by rendering and navigation.</summary>
public sealed class SpatialGeometryTests
{
    [Fact(DisplayName = "Incremental Unit Test 25 — Workshop begins at the clear world origin")]
    public void WorkshopStartsAtWorldCenter()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        Assert.Equal(new SpatialPoint(50, 50), scene.StartingPosition);
        Assert.DoesNotContain(scene.Interactables, item => Contains(item.Bounds, scene.StartingPosition));
    }

    [Fact(DisplayName = "Incremental Unit Test 25 — schematic buildings expose paired wall lines")]
    public void BuildingsProducePairedWallGeometry()
    {
        var scene = SpatialWorkshopScene.CreateDefault();
        var forge = SpatialGeometryEngine.FromInteractables(scene.Interactables).Single(x => x.Id == "forge");
        var walls = SpatialGeometryEngine.BuildWalls(forge);

        Assert.Equal(4, walls.Count);
        Assert.All(walls, wall =>
        {
            Assert.NotEqual(wall.Outer.Start, wall.Outer.End);
            Assert.NotEqual(wall.Inner.Start, wall.Inner.End);
            Assert.Equal(forge.Accent, wall.Accent);
            Assert.Equal(forge.Fill, wall.Fill);
        });
    }

    [Fact(DisplayName = "Incremental Unit Test 25 — direct movement is rejected when it crosses a building")]
    public void BuildingBlocksDirectMovement()
    {
        var building = new SpatialBuildingGeometry("test", new SpatialBounds(40, 40, 20, 20), "#00eaff", "#101820");

        Assert.True(SpatialGeometryEngine.SegmentBlocked(new SpatialPoint(30, 50), new SpatialPoint(70, 50), building.Bounds));
        Assert.False(SpatialGeometryEngine.SegmentBlocked(new SpatialPoint(30, 30), new SpatialPoint(35, 35), building.Bounds));
    }

    [Fact(DisplayName = "Incremental Unit Test 25 — blocked movement can route around a structure")]
    public void BlockedMovementFindsDetour()
    {
        var building = new SpatialBuildingGeometry("test", new SpatialBounds(40, 40, 20, 20), "#00eaff", "#101820");
        var path = SpatialGeometryEngine.FindPath(
            new SpatialPoint(30, 50),
            new SpatialPoint(70, 50),
            [building]);

        Assert.NotEmpty(path);
        Assert.Equal(new SpatialPoint(70, 50), path[^1]);
        Assert.All(path, point => Assert.False(Contains(new SpatialBounds(38.5, 38.5, 23, 23), point)));
    }

    [Fact(DisplayName = "Incremental Unit Test 25 — interaction objects can expose multiple approach positions")]
    public void InteractableSupportsMultipleInteractionPositions()
    {
        var item = SpatialInteractable.CreateVehicle(
            "fighter",
            "Fighter Aircraft",
            new SpatialBounds(40, 40, 10, 6),
            [
                new SpatialInteractionPoint("pilot", "Pilot Position", new SpatialBounds(38, 41, 2, 2)),
                new SpatialInteractionPoint("systems", "Systems Position", new SpatialBounds(50, 41, 2, 2))
            ],
            "fighter");

        Assert.Equal(2, item.InteractionPoints.Count);
        Assert.Contains(item.InteractionPoints, point => point.Id == "pilot");
        Assert.Contains(item.InteractionPoints, point => point.Id == "systems");
        Assert.Equal(item.InteractionPoints[0].Bounds, item.InteractionPoint);
    }

    private static bool Contains(SpatialBounds bounds, SpatialPoint point)
        => point.X >= bounds.X && point.X <= bounds.X + bounds.Width && point.Y >= bounds.Y && point.Y <= bounds.Y + bounds.Height;
}
