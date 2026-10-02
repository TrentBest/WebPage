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
}
