namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>Living Digiten occupants assigned to Singularity City government.</summary>
/// <remarks>All civic officials are Digitens; player avatars are also Digitens when connected.</remarks>
public sealed record SingularityCivicDigiten(
    string Id,
    string Name,
    string OfficeId,
    string Role,
    IReadOnlyList<string> CommitteeAssignments);

public static class SingularityCivicDigitenCatalog
{
    public static IReadOnlyList<SingularityCivicDigiten> Officials =>
    [
        new("digiten.mayor", "Mayor Mara", "government.mayor", "Singularity Mayor", ["Executive Priorities", "Planning & Zoning", "Public Works"]),
        new("digiten.council-01", "Councilor Ivo", "government.council", "City Representative", ["Transit & Mobility", "Public Works"]),
        new("digiten.council-02", "Councilor Nia", "government.council", "City Representative", ["Housing & Neighborhoods", "Planning & Zoning"]),
        new("digiten.council-03", "Councilor Sol", "government.council", "City Representative", ["Science & Technology", "Digital Services"]),
        new("digiten.clerk", "Clerk Ada", "government.clerk", "City Clerk", ["Civic Records", "Elections"]),
        new("digiten.treasury", "Treasurer Ren", "government.treasury", "City Treasury", ["Budget & Funding", "Public Assets"]),
        new("digiten.planning", "Planner Kai", "government.planning", "Planning Commission", ["Planning & Zoning", "Permitting"]),
        new("digiten.public-works", "Director Vale", "government.public-works", "Public Works Authority", ["Infrastructure", "Construction"]),
        new("digiten.housing", "Director Mira", "government.housing", "Housing Authority", ["Housing & Neighborhoods", "Residential Supply"]),
        new("digiten.transit", "Director Pax", "government.transit", "Transit Authority", ["Transit & Mobility", "Infrastructure"]),
        new("digiten.digital-services", "Director Echo", "government.digital-services", "Digital Services Office", ["Digital Services", "Civic Identity"])
    ];

    public static SingularityCivicDigiten Resolve(string id)
        => Officials.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No civic Digiten is registered for '{id}'.");

    public static SingularityCivicDigiten Mayor => Resolve("digiten.mayor");
}

/// <summary>Defines the live-presence boundary for a persistent Digiten world.</summary>
public static class SingularityDigitenPresenceRules
{
    public const string PresenceContract =
        "CONNECTED = VISIBLE AVATAR; DISCONNECTED = REMOVE LIVE AVATAR; PERSISTED WORLD STATE = RETAIN DURABLE ROLES AND WORLD DATA.";
}

/// <summary>A semantic feature in the Mayor's Mansion plan.</summary>
public sealed record SingularityMansionFeature(
    string Id, string Name, string Kind,
    double X, double Y, double Width, double Height, string Description);

public static class SingularityMansionArchitecture
{
    public static IReadOnlyList<SingularityMansionFeature> Features =>
    [
        new("front-entry", "Grand Entrance", "Entry", 45, 84, 10, 8, "Ceremonial front door and arrival stair."),
        new("grand-stair", "Grand Stair & Landing", "Stair", 42, 53, 16, 19, "Broad stair rising into the central hall with a landing."),
        new("west-alcove", "West Gallery Alcove", "Alcove", 8, 36, 18, 12, "Gallery recess for civic art and conversation."),
        new("east-alcove", "East Gallery Alcove", "Alcove", 74, 36, 18, 12, "Gallery recess overlooking the eastern court."),
        new("north-alcove", "Portrait Alcove", "Alcove", 40, 10, 20, 12, "Portraits of former Mayors and important city moments."),
        new("south-alcove", "City Charter Alcove", "Alcove", 40, 74, 20, 12, "The city's founding charter."),
        new("library", "Mayor's Library", "Room", 8, 54, 18, 12, "Books, plans, maps, and city proposals."),
        new("map-room", "City Map Room", "Room", 74, 54, 18, 12, "Districts, zoning, permits, and infrastructure planning."),
        new("dining", "State Dining Room", "Room", 30, 6, 40, 12, "Formal civic dinners and receptions.")
    ];

    public static SingularityMansionFeature Resolve(string id)
        => Features.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No mansion feature is registered for '{id}'.");

    public static IReadOnlyList<SingularityMansionFeature> Alcoves
        => Features.Where(x => string.Equals(x.Kind, "Alcove", StringComparison.OrdinalIgnoreCase)).ToArray();
}

public static class SingularityMansionElevation
{
    public const string Title = "MAYOR'S MANSION // FRONT ELEVATION";
    public const string ButlerGreeting = "Welcome to the mayor's mansion. If you'd like to meet with the mayor then follow with me...";
    public const string MeetingCancelledPrompt = "The mayor is unavailable. Would you prefer to take over the mayor's office and run this great city for some time?";
}
