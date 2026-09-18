namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative boundary for the Workshop's future physics implementation.
/// </summary>
/// <remarks>
/// The deliberately provisional name is part of the architecture: TBD Physics Engine.
/// The Workshop can describe experiments, materials, fields, particles, structures, and
/// observations before committing the runtime to one numerical implementation.
///
/// This boundary does not contain hazardous procedures or pretend that a physics engine
/// already exists. It is a composition contract for future providers.
/// </remarks>
public sealed record TbdPhysicsEngineManifest(
    string Id,
    string Name,
    IReadOnlyList<TbdPhysicsScale> Scales,
    IReadOnlyList<string> Capabilities)
{
    public static TbdPhysicsEngineManifest Default { get; } = new(
        "tbd-physics-engine",
        "TBD PHYSICS ENGINE",
        [
            new("micro", "MICRO"),
            new("atomic", "ATOMIC"),
            new("molecular", "MOLECULAR"),
            new("human", "HUMAN"),
            new("planetary", "PLANETARY"),
            new("stellar", "STELLAR"),
            new("galactic", "GALACTIC"),
            new("universal", "UNIVERSAL")
        ],
        [
            "particle interaction",
            "material properties",
            "fluid behavior",
            "field behavior",
            "structural behavior",
            "orbital behavior",
            "multi-scale visualization",
            "measurement and observation"
        ]);
}

/// <summary>A scale at which the future physics provider can expose a model.</summary>
public readonly record struct TbdPhysicsScale(string Id, string Name);
