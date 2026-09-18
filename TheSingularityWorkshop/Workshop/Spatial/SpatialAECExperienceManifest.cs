namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative AEC simulation surface connecting models, behaviors, and operations.
/// </summary>
/// <remarks>
/// The model remains separate from its behavior package. A purchased behavior MicroBundle
/// can turn otherwise static building content into an active operational Experience.
/// </remarks>
public sealed record SpatialAECExperienceManifest(
    IReadOnlyList<SpatialAECBehaviorDomain> BehaviorDomains,
    IReadOnlyList<SpatialAECUseCase> UseCases,
    IReadOnlyList<string> MonetizationHooks)
{
    public static SpatialAECExperienceManifest CreateDefault()
        => new(
            [
                new("electrical", "ELECTRICAL", ["light switches", "disconnects", "power distribution"]),
                new("hvac", "HVAC", ["rooftop units", "installation", "uninstallation", "replacement tenancy", "maintenance"]),
                new("healthcare", "HEALTHCARE", ["patient flow", "helipad transfer", "organ transfer", "waiting room", "clinical routing"]),
                new("procurement", "PROCUREMENT", ["purchasing", "estimating", "supplier coordination"]),
                new("prefabrication", "PREFABRICATION", ["model extraction", "fabrication planning", "assembly"]),
                new("operations", "OPERATIONS", ["occupancy", "maintenance", "actors", "service calls"])
            ],
            [
                new("building-assembly", "BUILDING ASSEMBLY", "Compose a building from reusable ontology assets."),
                new("behavior-package", "BEHAVIOR PACKAGE", "Attach reusable behaviors as a MicroBundle."),
                new("background-maintenance", "BACKGROUND MAINTENANCE", "Allow maintenance events to occur without explicit user invocation."),
                new("marketplace-sale", "ONTOLOGY MALL SALE", "Publish reusable structures and behavior packages for virtual sale.")
            ],
            ["model-sale", "behavior-package-sale", "experience-license"]);
}

/// <summary>A behavior family that can animate a building model.</summary>
public readonly record struct SpatialAECBehaviorDomain(string Id, string Name, IReadOnlyList<string> Behaviors);

/// <summary>A reusable AEC use case exposed by the simulation.</summary>
public readonly record struct SpatialAECUseCase(string Id, string Name, string Purpose);
