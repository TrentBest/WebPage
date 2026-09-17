namespace TheSingularityWorkshop.Gui;

/// <summary>Declarative manifest for the future Singularity Lab spatial experience.</summary>
/// <remarks>
/// The lab is intentionally described as data: floors, simulation domains, teaching subjects,
/// and hydraulic terrain capabilities can grow without creating a new renderer class per lab.
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
                new("ground", "MATERIALS & ATOMS", 0),
                new("physics", "PHYSICS SIMULATOR", 1),
                new("collider", "COLLIDER HALL", 2),
                new("hydraulic", "HYDRAULIC TERRAIN LAB", 3),
                new("cosmic", "COSMIC SCALE SIMULATORS", 4)
            ],
            [
                new("hadron-small", "HADRON COLLIDER // BENCH", "MICRO", "atom-scale collision studies"),
                new("hadron-planetary", "HADRON COLLIDER // PLANETARY", "PLANETARY", "planet-scale collision and field studies"),
                new("solar-system", "SOLAR SYSTEM SIMULATOR", "SOLAR SYSTEM", "orbital and multi-body experiments"),
                new("galaxy", "GALAXY SIMULATOR", "GALAXY", "large-scale structure and gravitational studies"),
                new("hydraulic", "HYDRAULIC TERRAIN", "TERRAIN", "water, weathering, erosion, and drainage experiments")
            ],
            [
                new("materials", "MATERIALS", ["brittleness", "hardness", "ductility", "conductivity"]),
                new("atoms", "ATOMS", ["structure", "bonding", "energy", "interaction"]),
                new("fluids", "FLUIDS", ["flow", "pressure", "weathering", "watering"]),
                new("cosmos", "COSMOS", ["orbits", "gravity", "scale", "emergence"])
            ]);
}

/// <summary>A navigable lab floor represented entirely by manifest data.</summary>
public readonly record struct SpatialLaboratoryFloor(string Id, string Name, int Level);

/// <summary>A simulator exposed by a laboratory floor.</summary>
public readonly record struct SpatialLaboratorySimulator(string Id, string Name, string Scale, string Purpose);

/// <summary>A teaching topic discovered through interaction rather than a classroom page.</summary>
public readonly record struct SpatialLaboratoryLesson(string Id, string Name, IReadOnlyList<string> Concepts);
