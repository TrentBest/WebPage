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
    /// <summary>The deliberately provisional physics boundary used by laboratory simulations.</summary>
    public static TbdPhysicsEngineManifest PhysicsEngine => TbdPhysicsEngineManifest.Default;

    /// <summary>Creates the default multi-floor research tower manifest.</summary>
    public static SpatialLaboratoryManifest CreateDefault()
        => new(
            "singularity-lab",
            "SINGULARITY LAB",
            new SpatialBounds(22, 60, 34, 28),
            [
                new("security", "SECURITY GATE", -1, "security"),
                new("director", "DIRECTOR'S OFFICE", 0, "administration"),
                new("gravity", "GRAVITY & SPACETIME", 1, "gravity"),
                new("energy", "ENERGY SYSTEMS", 2, "energy"),
                new("materials", "MATERIALS SCIENCE", 3, "materials"),
                new("atomics", "ATOMICS & CHEMISTRY", 4, "atomics"),
                new("quantum", "QUANTUM SYSTEMS", 5, "quantum"),
                new("plasma", "PLASMA & FIELD STUDIES", 6, "plasma"),
                new("fluids", "FLUIDS & HYDRAULICS", 7, "fluids"),
                new("cosmic", "COSMIC SCALE", 8, "cosmic"),
                new("simulation", "FSM PHYSICS SIMULATION", 9, "simulation")
            ],
            [
                new("particle-collider", "PARTICLE COLLIDER", "MICRO → SOLAR SYSTEM", "Compose collision experiments across scale: tiny, small, medium, large, hadron-collider, extra-large, and eventually astronomical systems.", ["atomics", "energy", "gravity", "fsm-physics"]),
                new("gravity-well", "GRAVITY WELL", "MICRO", "Numerically explore attraction, acceleration, and field falloff.", ["gravity", "fsm-physics"]),
                new("antigravity", "ANTI-GRAVITY WORKBENCH", "MICRO", "A speculative sandbox for testing counter-field rules.", ["gravity", "fsm-physics"]),
                new("energy-lattice", "ENERGY LATTICE", "MOLECULAR", "Compose energy transfer experiments from reusable state machines.", ["energy", "fsm-physics"]),
                new("atom-forge", "ATOM FORGE", "ATOMIC", "Select elements and inspect their chemical data as simulation inputs.", ["atomics", "materials"]),
                new("material-stress", "MATERIAL STRESS CHAMBER", "MACRO", "Test material properties against controlled forces and energy.", ["materials", "gravity", "energy"]),
                new("field-chamber", "FIELD CHAMBER", "MACRO", "Combine field sources and observe their mathematical interaction.", ["quantum", "plasma"]),
                new("hydraulic-terrain", "HYDRAULIC TERRAIN", "TERRAIN", "Run water, pressure, erosion, drainage, and weathering experiments.", ["fluids", "energy"]),
                new("solar-system", "SOLAR SYSTEM SIMULATOR", "SOLAR SYSTEM", "Explore orbital and multi-body behavior.", ["cosmic", "gravity"]),
                new("galaxy", "GALAXY SIMULATOR", "GALAXY", "Study large-scale structure and gravitational emergence.", ["cosmic", "gravity"]),
                new("fsm-physics", "FSM PHYSICS ENGINE", "ARBITRARY", "Turn a declarative physics model into ordered FSM process groups.", ["simulation", "fsm-physics", "atom-data"]),
                new("data-chamber", "RESEARCH DATA CHAMBER", "OFFICE", "A researcher-controlled workspace for whiteboards, presentation boards, simulated computers, and experiment records.", ["simulation", "research-data"])
            ],
            [
                new("gravity", "GRAVITY", ["mass", "acceleration", "orbits", "field strength"]),
                new("antigravity", "ANTI-GRAVITY", ["counter-fields", "lift", "stability", "speculation"]),
                new("energy", "ENERGY", ["transfer", "storage", "conversion", "conservation"]),
                new("materials", "MATERIALS", ["brittleness", "hardness", "ductility", "conductivity"]),
                new("atoms", "ATOMS", ["structure", "bonding", "energy", "interaction"]),
                new("fluids", "FLUIDS", ["flow", "pressure", "weathering", "watering"]),
                new("cosmos", "COSMOS", ["orbits", "gravity", "scale", "emergence"]),
                new("fsm", "FSM SIMULATION", ["state", "transition", "process order", "measurement"]),
                new("research-data", "RESEARCH DATA", ["observations", "datasets", "provenance", "permissions"]),
                new("education", "EDUCATION", ["lessons", "questions", "answers", "reuse", "assessment"]),
                new("administration", "LAB ADMINISTRATION", ["projects", "requisitions", "occupancy", "operations"]),
                new("security", "LAB SECURITY", ["identity", "access", "data handling", "safety"])
            ]);
}

/// <summary>A research floor in the laboratory tower.</summary>
public readonly record struct SpatialLaboratoryFloor(string Id, string Name, int Level, string DomainId);

/// <summary>A reusable simulator exposed by one or more laboratory floors.</summary>
public readonly record struct SpatialLaboratorySimulator(string Id, string Name, string Scale, string Purpose, IReadOnlyList<string> DomainIds);

/// <summary>A teaching or research topic discovered through interaction.</summary>
public readonly record struct SpatialLaboratoryLesson(string Id, string Name, IReadOnlyList<string> Concepts);
