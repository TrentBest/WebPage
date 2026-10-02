using System;
using System.Linq;
using TheSingularityWorkshop.Workshop.Chemistry;
using TheSingularityWorkshop.Workshop.Laboratory;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityLaboratoryMicroBundleTests
{
    [Fact]
    public void Laboratory_Exposes_Diegetic_Science_Disciplines()
    {
        Assert.Equal(9, SingularityLaboratory.Experiments.Count);
        Assert.Contains(SingularityLaboratory.Experiments, x => x.Discipline == LaboratoryDiscipline.Mechanics);
        Assert.Contains(SingularityLaboratory.Experiments, x => x.Discipline == LaboratoryDiscipline.Thermodynamics);
        Assert.Contains(SingularityLaboratory.Experiments, x => x.Discipline == LaboratoryDiscipline.FluidDynamics);
        Assert.Contains(SingularityLaboratory.Experiments, x => x.Discipline == LaboratoryDiscipline.FractureMechanics);
        Assert.Contains(SingularityLaboratory.Experiments, x => x.Discipline == LaboratoryDiscipline.NuclearPhysics);
        Assert.Contains(SingularityLaboratory.Experiments, x => x.Discipline == LaboratoryDiscipline.Chemistry);
        Assert.Contains(SingularityLaboratory.Experiments, x => x.Discipline == LaboratoryDiscipline.Research);
    }

    [Fact]
    public void Laboratory_Experiments_Are_Executable_Verification_Surfaces()
    {
        var results = SingularityLaboratory.RunAll();

        Assert.Equal(9, results.Count);
        Assert.All(results, result => Assert.True(result.Passed, result.Evidence));
        Assert.Equal(results.Count, results.Select(result => result.ExperimentId).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Laboratory_MicroBundle_Exposes_And_Runs_The_Scientific_Benches()
    {
        using var laboratory = new SingularityLaboratoryMicroBundle();

        Assert.Equal(2216, laboratory.Id);
        Assert.Equal(9, laboratory.Experiments.Count);

        var fracture = laboratory.RunExperiment("fracture-mode-one");
        Assert.True(fracture.Passed);
        Assert.Equal(LaboratoryDiscipline.FractureMechanics, fracture.Discipline);

        var results = laboratory.RunAllExperiments();
        Assert.Equal(9, results.Count);
        Assert.All(results, result => Assert.True(result.Passed, result.Evidence));
        Assert.Equal(9, laboratory.LastResults.Count);
    }

    [Fact]
    public void Laboratory_Physics_Expansion_Covers_Thermal_Fluid_Fracture_And_Nuclear_Relationships()
    {
        Assert.Equal(18_000, PhysicsRelationships.HeatEnergyJoules(2, 900, 10));
        Assert.Equal(100, PhysicsRelationships.ConductiveHeatRateWatts(10, 2, 5, 1));
        Assert.Equal(0.001, PhysicsRelationships.ThermalExpansionM(0.00001, 10, 10));

        Assert.Equal(98_100, PhysicsRelationships.HydrostaticPressurePa(1000, 9.81, 10));
        Assert.Equal(250, PhysicsRelationships.DynamicPressurePa(10, 5));
        Assert.Equal(6, PhysicsRelationships.VolumetricFlowRateM3PerS(2, 3));
        Assert.Equal(100_000, PhysicsRelationships.ReynoldsNumber(1000, 2, 0.5, 0.01));
        Assert.Equal(9810, PhysicsRelationships.BuoyantForceNewtons(1000, 1, 9.81));

        Assert.Equal(0.01, PhysicsRelationships.VolumetricStrain(0.01, 1));
        Assert.Equal(10_000, PhysicsRelationships.BulkModulusPa(-100, 0.01));

        var critical = PhysicsRelationships.CriticalCrackLengthM(1_000_000, 1, 100_000_000);
        Assert.Equal(0.0000318309886, critical, 10);
        Assert.Equal(0.01, PhysicsRelationships.ParisCrackGrowthRateMPerCycle(0.000001, 100, 2), 12);

        Assert.Equal(Math.Log(2) / 10, PhysicsRelationships.DecayConstantFromHalfLifeSeconds(10), 12);
        Assert.Equal(500, PhysicsRelationships.DecayedQuantity(1000, Math.Log(2) / 10, 10), 10);
        Assert.Equal(100, PhysicsRelationships.ActivityBecquerels(1000, 0.1));
        Assert.Equal(89_875_517_873_681_764d, PhysicsRelationships.MassEnergyJoules(1), 1_000_000_000);
    }
    [Fact]
    public void Laboratory_Room_Catalog_Maps_Diegetic_Instruments_To_Executable_Experiments()
    {
        Assert.Equal(SingularityLaboratory.Experiments.Count, LaboratoryRoomCatalog.Rooms.Count);
        Assert.All(LaboratoryRoomCatalog.Rooms, room =>
        {
            Assert.NotEmpty(room.Instruments);
            Assert.NotEmpty(room.ExperimentIds);
            Assert.All(room.ExperimentIds, experimentId =>
                Assert.Contains(SingularityLaboratory.Experiments, experiment => experiment.Id == experimentId));
        });

        var fracture = LaboratoryRoomCatalog.GetById("fracture-bay");
        Assert.Equal(LaboratoryDiscipline.FractureMechanics, fracture.Discipline);
        Assert.Contains(fracture.Instruments, instrument => instrument.Id == "crack-monitor");
        Assert.Contains("fracture-mode-one", fracture.ExperimentIds);
    }
    [Fact]
    public void Laboratory_Gui_Builder_Manifests_The_Same_Rooms_And_Experiments()
    {
        var root = TheSingularityWorkshop.Workshop.Gui.SingularityLaboratoryGuiBuilder.Build();

        Assert.Equal("singularity-laboratory", root.Id);
        Assert.Equal(LaboratoryRoomCatalog.Rooms.Count, root.Children.Count(child => child.Id.StartsWith("room-", StringComparison.Ordinal)));

        var fracture = root.Find("room-fracture-bay");
        Assert.Contains("fracture-mode-one", fracture.Find("room-experiments-fracture-bay").Text);
        Assert.Contains("Crack Monitor", fracture.Find("room-instruments-fracture-bay").Text);
    }

    [Fact]
    public void Laboratory_MicroBundle_Provides_Empty_Rooms_That_Users_Can_Configure()
    {
        using var laboratory = new SingularityLaboratoryMicroBundle();

        Assert.Equal(3, laboratory.ConfigurableRooms.Count);
        Assert.All(laboratory.ConfigurableRooms, room =>
        {
            Assert.Null(room.Discipline);
            Assert.Empty(room.Instruments);
            Assert.Empty(room.Experiments);
        });

        var room = laboratory.ConfigurableRooms[0];
        room.Configure(
                "Materials Research Cell",
                LaboratoryDiscipline.Materials,
                "User-defined materials and fracture experiments.")
            .AddInstrument(new LaboratoryInstrument(
                "custom-tensile-rig",
                "Custom Tensile Rig",
                "User-configured tensile measurement."))
            .AddExperiment(new LaboratoryExperiment(
                "custom-elasticity",
                LaboratoryDiscipline.Materials,
                "Custom Elasticity Study",
                "User-defined constitutive experiment."));

        Assert.Equal(LaboratoryDiscipline.Materials, room.Discipline);
        Assert.Equal("Materials Research Cell", room.Name);
        Assert.Single(room.Instruments);
        Assert.Single(room.Experiments);
        Assert.Equal("custom-elasticity", room.Experiments[0].Id);
    }

    [Fact]
    public void Laboratory_Gui_Builder_Manifests_User_Configurable_Empty_Rooms()
    {
        using var laboratory = new SingularityLaboratoryMicroBundle();

        var root = TheSingularityWorkshop.Workshop.Gui.SingularityLaboratoryGuiBuilder.Build(laboratory.ConfigurableRooms);

        Assert.Equal(laboratory.ConfigurableRooms.Count, root.Children.Count(child => child.Id.StartsWith("configurable-room-", StringComparison.Ordinal)));
        var empty = root.Find("configurable-room-configurable-lab-01");
        Assert.Contains("EMPTY // READY FOR USER CONFIGURATION", empty.Find("configurable-room-state-configurable-lab-01").Text);
        Assert.Contains("NONE // USER ADDS", empty.Find("configurable-room-instruments-configurable-lab-01").Text);
        Assert.Contains("NONE // USER ADDS", empty.Find("configurable-room-experiments-configurable-lab-01").Text);
    }

    [Fact]
    public void Laboratory_Gui_Builder_Provides_Semantic_Configuration_Commands()
    {
        using var laboratory = new SingularityLaboratoryMicroBundle();

        var root = TheSingularityWorkshop.Workshop.Gui.SingularityLaboratoryGuiBuilder.Build(laboratory.ConfigurableRooms);
        var actions = root.Find("configurable-room-actions-configurable-lab-01");

        Assert.Equal("configure:configurable-lab-01:Materials", actions.Find("configure-materials-configurable-lab-01").Properties["command"]);
        Assert.Equal("configure:configurable-lab-01:Mechanics", actions.Find("configure-physics-configurable-lab-01").Properties["command"]);
        Assert.Equal("clear:configurable-lab-01", actions.Find("clear-room-configurable-lab-01").Properties["command"]);
    }

    [Fact]
    public void Laboratory_Gui_Builder_Reflects_A_User_Configured_Room()
    {
        using var laboratory = new SingularityLaboratoryMicroBundle();
        var room = laboratory.ConfigurableRooms[0]
            .Configure("Materials Research Cell", LaboratoryDiscipline.Materials, "User-defined materials research.")
            .AddInstrument(new LaboratoryInstrument("custom-tensile-rig", "Custom Tensile Rig", "Custom tensile measurement."));

        var root = TheSingularityWorkshop.Workshop.Gui.SingularityLaboratoryGuiBuilder.Build(laboratory.ConfigurableRooms);
        var configured = root.Find("configurable-room-configurable-lab-01");

        Assert.Contains("CONFIGURED // MATERIALS", configured.Find("configurable-room-state-configurable-lab-01").Text);
        Assert.Contains("Custom Tensile Rig", configured.Find("configurable-room-instruments-configurable-lab-01").Text);
    }

    [Fact]
    public void Laboratory_MicroBundle_Exposes_Reusable_Physics_Simulation_Benches()
    {
        using var laboratory = new SingularityLaboratoryMicroBundle();

        var collision = laboratory.ParticleCollider;
        collision.Configure(
            new ColliderParticle("a", 1, 1, 8),
            new ColliderParticle("b", 1, -1, 8));

        Assert.True(collision.Collide(16).ConservesEnergy);
        Assert.Equal(101_325, laboratory.Hydrodynamics.State.PressurePa);
        Assert.Equal(293.15, laboratory.Thermodynamics.State.TemperatureK);
        Assert.Equal(2, laboratory.Electromagnetism.State.CurrentAmps);
    }

}
