using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Heartbeat tests for the compositional Singularity Laboratory and open Universe.</summary>
public sealed class SpatialLaboratoryAndUniverseExperienceTests
{
    [Fact]
    public void Laboratory_Is_A_MultiFloor_Composition()
    {
        var lab = SpatialLaboratoryManifest.CreateDefault();

        Assert.Equal("singularity-lab", lab.Id);
        Assert.True(lab.Floors.Count >= 8);
        Assert.Contains(lab.Floors, x => x.Name.Contains("GRAVITY"));
        Assert.Contains(lab.Floors, x => x.Name.Contains("ENERGY"));
        Assert.Contains(lab.Floors, x => x.Name.Contains("MATERIALS"));
        Assert.Contains(lab.Floors, x => x.Name.Contains("ATOMICS"));
        Assert.Contains(lab.Floors, x => x.Name.Contains("FSM"));
        Assert.Contains(lab.Simulators, x => x.Id == "atom-forge");
        Assert.Contains(lab.Simulators, x => x.Id == "fsm-physics");
    }

    [Fact]
    public void Campus_Exposes_The_Laboratory_As_A_Real_Interactable()
    {
        var scene = SpatialWorkshopScene.CreateDefault();

        var lab = Assert.Single(scene.Interactables.Where(x => x.Id == "singularity-lab"));
        Assert.Equal("singularity-lab", lab.ExperienceId);
        Assert.True(lab.Bounds.Width > 0);
        Assert.True(lab.Bounds.Height > 0);
    }

    [Fact]
    public void SingularityUniverse_Reserves_Empty_Space_For_Creation()
    {
        var universe = SpatialUniverseManifest.SingularityUniverse;

        Assert.NotEmpty(universe.OpenParcels);
        Assert.Contains(universe.OpenParcels, x => x.Id == "north-expanse");
        Assert.Contains(universe.OpenParcels, x => x.Id == "deep-space");
    }
}
