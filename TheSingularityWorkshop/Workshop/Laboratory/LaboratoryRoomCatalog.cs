using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Laboratory;

public sealed record LaboratoryInstrument(
    string Id,
    string Name,
    string Purpose);

public sealed record LaboratoryRoom(
    string Id,
    LaboratoryDiscipline Discipline,
    string Name,
    string Description,
    IReadOnlyList<LaboratoryInstrument> Instruments,
    IReadOnlyList<string> ExperimentIds);

/// <summary>
/// Diegetic floor plan for the Singularity Laboratory. Rooms describe scientific
/// capability; presentation layers decide how those rooms are manifested.
/// </summary>
public static class LaboratoryRoomCatalog
{
    public static IReadOnlyList<LaboratoryRoom> Rooms { get; } =
    [
        Room("mechanics-bay", LaboratoryDiscipline.Mechanics, "Mechanics Bay",
            "Force, motion, energy, momentum, and mechanical measurement.",
            [Instrument("force-frame", "Force Frame", "Measures applied force and displacement."),
             Instrument("motion-track", "Motion Track", "Measures position, velocity, and acceleration.")],
            "mechanics-newton"),

        Room("electromagnetics-bay", LaboratoryDiscipline.Electromagnetism, "Electromagnetics Bay",
            "Current, voltage, resistance, power, charge, and electrical energy.",
            [Instrument("power-bench", "Power Bench", "Measures current, voltage, and power."),
             Instrument("resistance-rig", "Resistance Rig", "Characterizes resistance and resistive geometry.")],
            "electromagnetism-power"),

        Room("thermodynamics-bay", LaboratoryDiscipline.Thermodynamics, "Thermodynamics Bay",
            "Heat transfer, calorimetry, thermal expansion, and thermal transport.",
            [Instrument("calorimeter", "Calorimeter", "Measures energy transferred as heat."),
             Instrument("thermal-plate", "Thermal Plate", "Measures conductive heat transfer.")],
            "thermodynamics-heating"),

        Room("fluid-dynamics-bay", LaboratoryDiscipline.FluidDynamics, "Fluid Dynamics Bay",
            "Pressure, flow, buoyancy, viscosity, and dimensionless flow regimes.",
            [Instrument("hydrostatic-column", "Hydrostatic Column", "Measures pressure with fluid depth."),
             Instrument("flow-loop", "Flow Loop", "Measures flow rate and velocity.")],
            "fluid-hydrostatic"),

        Room("materials-bay", LaboratoryDiscipline.Materials, "Materials Bay",
            "Mechanical properties and constitutive response of materials.",
            [Instrument("tensile-frame", "Tensile Frame", "Measures force, elongation, stress, and strain."),
             Instrument("material-database", "Material Property Console", "Loads sourced material properties.")],
            "materials-elasticity"),

        Room("fracture-bay", LaboratoryDiscipline.FractureMechanics, "Fracture Mechanics Bay",
            "Crack initiation, stress intensity, toughness, and fatigue growth.",
            [Instrument("fracture-rig", "Fracture Rig", "Applies controlled loading to cracked specimens."),
             Instrument("crack-monitor", "Crack Monitor", "Tracks crack geometry and growth.")],
            "fracture-mode-one"),

        Room("nuclear-bay", LaboratoryDiscipline.NuclearPhysics, "Nuclear Physics Bay",
            "Decay, activity, half-life, radiation measurement, and mass-energy.",
            [Instrument("decay-counter", "Decay Counter", "Measures event rates from radioactive decay."),
             Instrument("nuclear-console", "Nuclear Console", "Evaluates decay and mass-energy relationships.")],
            "nuclear-decay"),

        Room("chemistry-bay", LaboratoryDiscipline.Chemistry, "Chemistry Bay",
            "Elemental identity, composition, material properties, and characterization.",
            [Instrument("periodic-console", "Elemental Console", "Inspects canonical and fictional element identity."),
             Instrument("density-bench", "Density Bench", "Measures mass and volume to determine density.")],
            "chemistry-density"),

        Room("research-bay", LaboratoryDiscipline.Research, "Research Bay",
            "Open-ended hypothesis testing and experimental composition.",
            [Instrument("research-console", "Research Console", "Composes experimental inputs and observations."),
             Instrument("mass-energy-bench", "Mass-Energy Bench", "Explores mass-energy equivalence.")],
            "research-energy")
    ];

    public static LaboratoryRoom GetById(string id) =>
        Rooms.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.Ordinal))
        ?? throw new KeyNotFoundException($"Unknown laboratory room '{id}'.");

    private static LaboratoryRoom Room(
        string id,
        LaboratoryDiscipline discipline,
        string name,
        string description,
        IReadOnlyList<LaboratoryInstrument> instruments,
        params string[] experimentIds) =>
        new(id, discipline, name, description, instruments, experimentIds);

    private static LaboratoryInstrument Instrument(string id, string name, string purpose) =>
        new(id, name, purpose);
}
