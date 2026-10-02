using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.Workshop.Chemistry;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public enum LaboratoryDiscipline
{
    Mechanics,
    Electromagnetism,
    Thermodynamics,
    FluidDynamics,
    Materials,
    FractureMechanics,
    NuclearPhysics,
    Chemistry,
    Research
}

public sealed record LaboratoryExperiment(
    string Id,
    LaboratoryDiscipline Discipline,
    string Name,
    string Description);

public sealed record LaboratoryResult(
    string ExperimentId,
    LaboratoryDiscipline Discipline,
    string Name,
    bool Passed,
    string Evidence);

/// <summary>
/// Diegetic scientific laboratory. Each experiment is a deterministic
/// verification surface for the physical relationships used by the Workshop.
/// </summary>
public static class SingularityLaboratory
{
    public static IReadOnlyList<LaboratoryExperiment> Experiments { get; } =
    [
        new("mechanics-newton", LaboratoryDiscipline.Mechanics, "Newton's Second Law", "Verify F = ma."),
        new("electromagnetism-power", LaboratoryDiscipline.Electromagnetism, "Electrical Power Bench", "Verify P = IV."),
        new("thermodynamics-heating", LaboratoryDiscipline.Thermodynamics, "Calorimetry Bench", "Verify Q = mcΔT."),
        new("fluid-hydrostatic", LaboratoryDiscipline.FluidDynamics, "Hydrostatic Column", "Verify p = ρgh."),
        new("materials-elasticity", LaboratoryDiscipline.Materials, "Tensile Test", "Verify σ = Eε."),
        new("fracture-mode-one", LaboratoryDiscipline.FractureMechanics, "Brittle Fracture Bench", "Verify K_I = Yσ√(πa)."),
        new("nuclear-decay", LaboratoryDiscipline.NuclearPhysics, "Decay Counter", "Verify exponential radioactive decay."),
        new("chemistry-density", LaboratoryDiscipline.Chemistry, "Material Density Bench", "Verify density from mass and volume."),
        new("research-energy", LaboratoryDiscipline.Research, "Mass-Energy Bench", "Verify E = mc².")
    ];

    public static LaboratoryResult Run(string experimentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(experimentId);

        return experimentId switch
        {
            "mechanics-newton" => Result("mechanics-newton", LaboratoryDiscipline.Mechanics, "Newton's Second Law",
                PhysicsRelationships.ForceNewtons(12, 3) == 36, "12 kg × 3 m/s² = 36 N"),
            "electromagnetism-power" => Result("electromagnetism-power", LaboratoryDiscipline.Electromagnetism, "Electrical Power Bench",
                PhysicsRelationships.ElectricalPowerWatts(5, 12) == 60, "5 A × 12 V = 60 W"),
            "thermodynamics-heating" => Result("thermodynamics-heating", LaboratoryDiscipline.Thermodynamics, "Calorimetry Bench",
                NearlyEqual(PhysicsRelationships.HeatEnergyJoules(2, 900, 10), 18_000), "2 kg × 900 J/(kg·K) × 10 K = 18,000 J"),
            "fluid-hydrostatic" => Result("fluid-hydrostatic", LaboratoryDiscipline.FluidDynamics, "Hydrostatic Column",
                NearlyEqual(PhysicsRelationships.HydrostaticPressurePa(1000, 9.81, 10), 98_100), "1000 kg/m³ × 9.81 m/s² × 10 m = 98,100 Pa"),
            "materials-elasticity" => Result("materials-elasticity", LaboratoryDiscipline.Materials, "Tensile Test",
                NearlyEqual(PhysicsRelationships.ElasticStressPa(200_000_000_000, 0.001), 200_000_000), "200 GPa × 0.001 = 200 MPa"),
            "fracture-mode-one" => Result("fracture-mode-one", LaboratoryDiscipline.FractureMechanics, "Brittle Fracture Bench",
                NearlyEqual(PhysicsRelationships.ModeIStressIntensityPaSqrtM(1, 100_000_000, 0.01), 17_724_538.5, 1),
                "Yσ√(πa) for Y=1, σ=100 MPa, a=10 mm"),
            "nuclear-decay" => Result("nuclear-decay", LaboratoryDiscipline.NuclearPhysics, "Decay Counter",
                NearlyEqual(PhysicsRelationships.DecayedQuantity(1000, PhysicsRelationships.DecayConstantFromHalfLifeSeconds(10), 10), 500),
                "One half-life leaves 50% of the initial quantity"),
            "chemistry-density" => Result("chemistry-density", LaboratoryDiscipline.Chemistry, "Material Density Bench",
                NearlyEqual(PhysicsRelationships.DensityKgPerM3(10, 0.01), 1000), "10 kg / 0.01 m³ = 1000 kg/m³"),
            "research-energy" => Result("research-energy", LaboratoryDiscipline.Research, "Mass-Energy Bench",
                NearlyEqual(PhysicsRelationships.MassEnergyJoules(1), 89_875_517_873_681_764d, 1_000_000_000),
                "1 kg × c² ≈ 8.99 × 10^16 J"),
            _ => throw new KeyNotFoundException($"Unknown laboratory experiment '{experimentId}'.")
        };
    }

    public static IReadOnlyList<LaboratoryResult> RunAll() =>
        Experiments.Select(x => Run(x.Id)).ToArray();

    private static LaboratoryResult Result(string id, LaboratoryDiscipline discipline, string name, bool passed, string evidence) =>
        new(id, discipline, name, passed, evidence);

    private static bool NearlyEqual(double actual, double expected, double tolerance = 0.000001) =>
        Math.Abs(actual - expected) <= tolerance;
}
