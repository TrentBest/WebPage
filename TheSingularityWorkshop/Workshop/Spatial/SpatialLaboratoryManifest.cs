namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;

/// <summary>Declarative manifest for the Singularity Laboratory.</summary>
/// <remarks>
/// The laboratory is a composition of floors and research domains rather than hard-coded rooms.
/// A future GUI editor can author these records without changing the spatial renderer.
/// </remarks>
public sealed record SpatialLaboratoryManifest(
    string Id,
    string Name,
    SpatialBounds BuildingBounds,
    IReadOnlyList<SpatialLaboratoryFloor> Floors,
    IReadOnlyList<SpatialLaboratorySimulator> Simulators,
    IReadOnlyList<SpatialLaboratoryLesson> Lessons)
{
    public static SpatialLaboratoryManifest CreateDefault()
        => new(
            "singularity-lab",
            "SINGULARITY LAB",
            new SpatialBounds(22, 60, 34, 28),
            [
                new("gravity", "GRAVITY & SPACETIME", 0, "gravity"),
                new("energy", "ENERGY SYSTEMS", 1, "energy"),
                new("materials", "MATERIALS SCIENCE", 2, "materials"),
                new("atomics", "ATOMICS & CHEMISTRY", 3, "atomics"),
                new("quantum", "QUANTUM SYSTEMS", 4, "quantum"),
                new("plasma", "PLASMA & FIELD STUDIES", 5, "plasma"),
                new("fluids", "FLUIDS & HYDRAULICS", 6, "fluids"),
                new("cosmic", "COSMIC SCALE", 7, "cosmic"),
                new("simulation", "FSM PHYSICS SIMULATION", 8, "simulation")
            ],
            [
                new("gravity-well", "GRAVITY WELL", "MICRO", "Numerically explore attraction, acceleration, and field falloff.", ["gravity", "fsm-physics"]),
                new("antigravity", "ANTI-GRAVITY WORKBENCH", "MICRO", "A speculative sandbox for testing counter-field rules.", ["gravity", "fsm-physics"]),
                new("energy-lattice", "ENERGY LATTICE", "MOLECULAR", "Compose energy transfer experiments from reusable state machines.", ["energy", "fsm-physics"]),
                new("atom-forge", "ATOM FORGE", "ATOMIC", "Select elements and inspect their chemical data as simulation inputs.", ["atomics", "materials"]),
                new("material-stress", "MATERIAL STRESS CHAMBER", "MACRO", "Test material properties against controlled forces and energy.", ["materials", "gravity", "energy"]),
                new("field-chamber", "FIELD CHAMBER", "MACRO", "Combine field sources and observe their mathematical interaction.", ["quantum", "plasma"]),
                new("hydraulic-terrain", "HYDRAULIC TERRAIN", "TERRAIN", "Run water, pressure, erosion, drainage, and weathering experiments.", ["fluids", "energy"]),
                new("solar-system", "SOLAR SYSTEM SIMULATOR", "SOLAR SYSTEM", "Explore orbital and multi-body behavior.", ["cosmic", "gravity"]),
                new("galaxy", "GALAXY SIMULATOR", "GALAXY", "Study large-scale structure and gravitational emergence.", ["cosmic", "gravity"]),
                new("fsm-physics", "FSM PHYSICS ENGINE", "ARBITRARY", "Turn a declarative physics model into ordered FSM process groups.", ["simulation", "fsm-physics", "atom-data"])
            ],
            [
                new("gravity", "GRAVITY", ["mass", "acceleration", "orbits", "field strength"]),
                new("antigravity", "ANTI-GRAVITY", ["counter-fields", "lift", "stability", "speculation"]),
                new("energy", "ENERGY", ["transfer", "storage", "conversion", "conservation"]),
                new("materials", "MATERIALS", ["brittleness", "hardness", "ductility", "conductivity"]),
                new("atoms", "ATOMS", ["structure", "bonding", "energy", "interaction"]),
                new("fluids", "FLUIDS", ["flow", "pressure", "weathering", "watering"]),
                new("cosmos", "COSMOS", ["orbits", "gravity", "scale", "emergence"]),
                new("fsm", "FSM SIMULATION", ["state", "transition", "process order", "measurement"])
            ]);
}

/// <summary>A research floor in the laboratory tower.</summary>
public readonly record struct SpatialLaboratoryFloor(string Id, string Name, int Level, string DomainId);

/// <summary>A reusable simulator exposed by one or more laboratory floors.</summary>
public readonly record struct SpatialLaboratorySimulator(string Id, string Name, string Scale, string Purpose, IReadOnlyList<string> DomainIds);

/// <summary>A teaching or research topic discovered through interaction.</summary>
public readonly record struct SpatialLaboratoryLesson(string Id, string Name, IReadOnlyList<string> Concepts);
