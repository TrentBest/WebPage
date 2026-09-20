using SingularityHub.Abstractions;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialHolodeckMazeTests
{
    [Fact]
    public void LaboratoryHolodeckStartsInTwoDimensionalMode()
    {
        var maze = new SpatialHolodeckMazeModel();

        Assert.Equal(SpatialHolodeckMazeMode.TwoDimensional, maze.Mode);
        Assert.Equal(SpatialMazeCatalog.Showcase.Start, maze.PlayerPosition);
        Assert.False(maze.AgentsReleased);
    }

    [Fact]
    public void HolodeckCanToggleToFirstPersonAndBack()
    {
        var maze = new SpatialHolodeckMazeModel();

        maze.ToggleMode();
        Assert.Equal(SpatialHolodeckMazeMode.FirstPerson, maze.Mode);

        maze.ToggleMode();
        Assert.Equal(SpatialHolodeckMazeMode.TwoDimensional, maze.Mode);
    }

    [Fact]
    public void AgentsRemainLockedForThirtySecondsThenReleaseWithOppositeWallRules()
    {
        var maze = new SpatialHolodeckMazeModel();
        var start = DateTime.UtcNow;

        maze.Start(start);
        maze.Advance(start.AddSeconds(29.9));
        Assert.False(maze.AgentsReleased);

        maze.Advance(start.AddSeconds(30));
        Assert.True(maze.AgentsReleased);
        Assert.Equal(SpatialMazeWallRule.LeftHand, maze.Agents[0].Rule);
        Assert.Equal(SpatialMazeWallRule.RightHand, maze.Agents[1].Rule);
    }

    [Fact]
    public void CardReaderRequiresVoiceAccessAndSupportsClearanceUpgrade()
    {
        var access = new SpatialLaboratoryAccessModel();

        Assert.False(access.TryBadge());
        access.CallIntercom();
        Assert.False(access.TryBadge());

        access.UpgradeBadge("RESEARCH-13", SpatialSecurityClearance.Research);
        Assert.True(access.TryBadge());
        Assert.True(access.DoorOpen);
    }

    [Fact]
    public void AccessControlIsOntologyAddressable()
    {
        var access = new SpatialLaboratoryAccessModel();

        Assert.Equal(OntologySignature.LayerCount, 9);
        Assert.Equal(101, access.CardReader.Ontology[8]);
        Assert.NotEqual(access.CardReader.Ontology, access.Intercom.Ontology);
    }
}
