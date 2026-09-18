namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative foundation for the Workshop's expanding civilization simulation.
/// </summary>
/// <remarks>
/// This manifest deliberately separates the environment's structure from the capabilities
/// users choose to install into it. A ship can therefore change how its pilot interacts
/// with space without requiring the solar-system environment itself to change.
/// </remarks>
public sealed record SpatialCivilizationManifest(
    IReadOnlyList<SpatialCivilizationDestination> Destinations,
    IReadOnlyList<SpatialSimulationDomain> SimulationDomains,
    IReadOnlyList<SpatialEconomyActivity> EconomyActivities,
    IReadOnlyList<SpatialUserCapability> UserCapabilities)
{
    /// <summary>Creates the first compositional civilization surface.</summary>
    public static SpatialCivilizationManifest CreateDefault()
        => new(
            [
                new("singularity-station", "SINGULARITY STATION", "space"),
                new("singularity-shipyard", "SINGULARITY SHIPYARD", "space"),
                new("luna-colony", "LUNA COLONY", "moon"),
                new("mars-colony", "MARS COLONY", "mars"),
                new("singularity-city", "SINGULARITY CITY", "city"),
                new("singularity-capitol", "SINGULARITY CAPITOL", "government"),
                new("ocean-terminal", "OCEAN TERMINAL", "ocean"),
                new("ocean-shipyard", "OCEAN SHIPYARD", "ocean"),
                new("singularity-military", "SINGULARITY MILITARY", "defense"),
                new("singularity-island", "SINGULARITY ISLAND", "island")
            ],
            [
                new("space-transportation-tycoon", "SPACE TRANSPORTATION TYCOON", "Build, mine, manufacture, rent, transport, and operate space assets."),
                new("city-builder", "SINGULARITY CITY BUILDER", "Develop a city through infrastructure, occupancy, services, and simulated citizens."),
                new("ocean-transportation-tycoon", "OCEAN TRANSPORTATION TYCOON", "Build and operate vessels, ports, routes, and marine infrastructure."),
                new("ship-design", "SHIP DESIGN STUDIO", "Compose vessels from real-world and fictional components."),
                new("aec-simulation", "AEC OPERATIONS SIMULATION", "Turn building models into operational environments with reusable behaviors."),
                new("stagecraft", "DIGITAL STAGECRAFT", "Construct only the physical and digital representation required by a scene.")
            ],
            [
                new("employment", "VIRTUAL JOBS", "Earn", "Users perform simulated work for compensation."),
                new("company", "VIRTUAL COMPANIES", "Earn/Spend", "Users can found companies, hire users, assign work, and purchase simulated outputs."),
                new("rent", "VIRTUAL RENT", "Spend", "Users can rent offices, workspaces, and other persistent facilities."),
                new("market", "SIMULATED MARKET", "Earn/Spend", "Manufactured goods can be purchased by users and simulated citizens."),
                new("ship-rental", "SHIP RENTAL", "Spend", "Users can rent available vessels without owning them."),
                new("casino", "ISLAND CASINO", "Spend", "Free-to-play virtual wagering experiences using virtual currency only."),
                new("banking", "VIRTUAL BANKING", "Earn/Spend", "Hold, transfer, and account for virtual currency.")
            ],
            [
                new("shipyard-design", "SHIP DESIGN", "Design a vessel and submit its construction plan."),
                new("ship-operation", "SHIP OPERATION", "Operate vessels according to their selected capability profile."),
                new("private-hangout", "PRIVATE SANDBOX", "Signed-in users receive a private space for unconstrained creation."),
                new("company-owner", "COMPANY OWNERSHIP", "Found and operate a simulated company."),
                new("aec-office", "AEC OFFICE", "Use virtual offices as automation and coordination surfaces."),
                new("military-service", "MILITARY SERVICE", "Enter the simulated military domain and participate in its defined roles.")
            ]);

    /// <summary>Returns a named destination.</summary>
    public SpatialCivilizationDestination? FindDestination(string id)
        => Destinations.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Returns the domains attached to a simulation family.</summary>
    public IReadOnlyList<SpatialSimulationDomain> DomainsFor(string family)
        => [.. SimulationDomains.Where(x => string.Equals(x.Id, family, StringComparison.OrdinalIgnoreCase))];
}

/// <summary>A destination in the growing Workshop civilization.</summary>
public readonly record struct SpatialCivilizationDestination(string Id, string Name, string Domain);

/// <summary>A simulation domain that can become an Experience or nested Experience.</summary>
public readonly record struct SpatialSimulationDomain(string Id, string Name, string Purpose);

/// <summary>A virtual-economic activity exposed by the civilization.</summary>
public readonly record struct SpatialEconomyActivity(string Id, string Name, string Flow, string Purpose);

/// <summary>A capability a signed-in user can attach to their personal Workshop identity.</summary>
public readonly record struct SpatialUserCapability(string Id, string Name, string Purpose);
