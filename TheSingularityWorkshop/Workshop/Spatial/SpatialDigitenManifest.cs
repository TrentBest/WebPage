namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative model for Digitens: simulated inhabitants who make the spatial
/// environment feel inhabited rather than merely rendered.
/// </summary>
/// <remarks>
/// A Digiten is not a decorative NPC. Its role, needs, schedule, relationships,
/// employment, services, and current activity are data that can be arbitrated into
/// whatever manifestation an Experience provides.
/// </remarks>
public sealed record SpatialDigitenManifest(
    IReadOnlyList<SpatialDigitenRole> Roles,
    IReadOnlyList<SpatialCivicService> Services,
    IReadOnlyList<SpatialDailyActivity> Activities,
    IReadOnlyList<SpatialDigitenRoutine> Routines)
{
    /// <summary>Creates the first civilian-life vocabulary.</summary>
    public static SpatialDigitenManifest CreateDefault()
        => new(
            [
                new("resident", "RESIDENT", "Lives in the city and participates in ordinary civic life."),
                new("mayor", "MAYOR", "Coordinates city policy and represents the civic administration."),
                new("council-member", "CITY COUNCIL", "Participates in civic deliberation and local decisions."),
                new("hvac-technician", "HVAC TECHNICIAN", "Inspects, repairs, and maintains building climate systems."),
                new("police-officer", "POLICE OFFICER", "Responds to incidents and performs public-safety duties."),
                new("firefighter", "FIREFIGHTER", "Responds to emergencies and protects people and property."),
                new("paramedic", "PARAMEDIC", "Provides emergency medical response."),
                new("builder", "BUILDER", "Constructs and repairs physical infrastructure."),
                new("merchant", "MERCHANT", "Operates a shop and participates in local commerce."),
                new("teacher", "TEACHER", "Provides education and participates in school life."),
                new("engineer", "ENGINEER", "Designs, diagnoses, and maintains technical systems."),
                new("transit-operator", "TRANSIT OPERATOR", "Moves people and goods through the city's transportation network.")
            ],
            [
                new("civic-government", "CIVIC GOVERNMENT", "governance"),
                new("public-safety", "PUBLIC SAFETY", "safety"),
                new("fire-response", "FIRE RESPONSE", "emergency"),
                new("emergency-medical", "EMERGENCY MEDICAL", "health"),
                new("building-maintenance", "BUILDING MAINTENANCE", "infrastructure"),
                new("transit", "PUBLIC TRANSIT", "transport"),
                new("commerce", "COMMERCE", "economy"),
                new("education", "EDUCATION", "education")
            ],
            [
                new("work", "WORK", "Attend employment or perform an assigned task."),
                new("travel", "TRAVEL", "Move between home, work, services, and social locations."),
                new("shop", "SHOP", "Acquire goods or services."),
                new("socialize", "SOCIALIZE", "Spend time with other Digitens."),
                new("maintain", "MAINTAIN", "Repair or inspect a physical or digital asset."),
                new("govern", "GOVERN", "Participate in civic administration."),
                new("rest", "REST", "Recover at a home or personal space."),
                new("respond", "RESPOND", "React to an event or service request.")
            ],
            [
                new("morning", "MORNING ROUTINE", ["rest", "travel", "work"]),
                new("workday", "WORKDAY", ["work", "shop", "travel", "socialize"]),
                new("evening", "EVENING ROUTINE", ["travel", "shop", "socialize", "rest"]),
                new("on-call", "ON CALL", ["rest", "respond", "travel", "maintain"])
            ]);

    /// <summary>Finds a role by stable identifier.</summary>
    public SpatialDigitenRole? FindRole(string id)
        => Roles.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Finds a civic service by stable identifier.</summary>
    public SpatialCivicService? FindService(string id)
        => Services.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Finds a routine by stable identifier.</summary>
    public SpatialDigitenRoutine? FindRoutine(string id)
        => Routines.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
}

/// <summary>A reusable social/economic role occupied by a Digiten.</summary>
public readonly record struct SpatialDigitenRole(string Id, string Name, string Purpose);

/// <summary>A service that Digitens can provide to the environment.</summary>
public readonly record struct SpatialCivicService(string Id, string Name, string Domain);

/// <summary>A common activity that can become an FSM state or scheduled action.</summary>
public readonly record struct SpatialDailyActivity(string Id, string Name, string Purpose);

/// <summary>A routine composed from reusable activities.</summary>
public readonly record struct SpatialDigitenRoutine(string Id, string Name, IReadOnlyList<string> ActivityIds);
