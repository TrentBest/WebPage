namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>Named living Digiten offices and civic committee assignments.</summary>
/// <remarks>
/// Every civic office is held by a Digiten. The model intentionally keeps identity
/// separate from presentation so a persistent world can later hydrate these occupants
/// from server state when citizens arrive or leave.
/// </remarks>
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
        new(
            "digiten.mayor",
            "Mayor Mara",
            "government.mayor",
            "Singularity Mayor",
            ["Executive Priorities", "Planning & Zoning", "Public Works"]),

        new(
            "digiten.council-01",
            "Councilor Ivo",
            "government.council",
            "City Representative",
            ["Transit & Mobility", "Public Works"]),

        new(
            "digiten.council-02",
            "Councilor Nia",
            "government.council",
            "City Representative",
            ["Housing & Neighborhoods", "Planning & Zoning"]),

        new(
            "digiten.council-03",
            "Councilor Sol",
            "government.council",
            "City Representative",
            ["Science & Technology", "Digital Services"])
    ];

    public static SingularityCivicDigiten Resolve(string id)
        => Officials.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No civic Digiten is registered for '{id}'.");

    public static SingularityCivicDigiten Mayor
        => Resolve("digiten.mayor");
}

/// <summary>
/// Describes the persistent-world presence rule for living Digitens.
/// </summary>
public static class SingularityDigitenPresenceRules
{
    /// <summary>
    /// A connected user is represented by their avatar while present. The avatar is
    /// removed from the live population when the session ends; durable identity and
    /// durable civic office state remain separate concerns.
    /// </summary>
    public const string PresenceContract =
        "CONNECTED = VISIBLE AVATAR; DISCONNECTED = REMOVE LIVE AVATAR; PERSISTED WORLD STATE = RETAIN DURABLE ROLES AND WORLD DATA.";
}

/// <summary>A named room/feature in the Mayor's Mansion plan.</summary>
public sealed record SingularityMansionFeature(
    string Id,
    string Name,
    string Kind,
    double X,
    double Y,
    double Width,
    double Height,
    string Description);

/// <summary>
/// Semantic architecture for the Mayor's Mansion. The linework factory renders these
/// concepts, while the feature catalog gives the visitor something meaningful to enter.
/// </summary>
public static class SingularityMansionArchitecture
{
    public static IReadOnlyList<SingularityMansionFeature> Features =>
    [
        new("front-entry", "Grand Entrance", "Entry", 45, 84, 10, 8, "The ceremonial front door and arrival stair."),
        new("grand-stair", "Grand Stair & Landing", "Stair", 42, 53, 16, 19, "A broad stair rises into the central hall with a landing at the turn."),
        new("west-alcove", "West Gallery Alcove", "Alcove", 8, 36, 18, 12, "A quiet gallery recess for civic art and conversation."),
        new("east-alcove", "East Gallery Alcove", "Alcove", 74, 36, 18, 12, "A matching gallery recess overlooking the eastern court."),
        new("north-alcove", "Portrait Alcove", "Alcove", 40, 10, 20, 12, "Portraits of former Mayors and important city moments."),
        new("south-alcove", "City Charter Alcove", "Alcove", 40, 74, 20, 12, "The city's founding charter is presented here."),
        new("library", "Mayor's Library", "Room", 8, 54, 18, 12, "Books, plans, maps, and long-form city proposals."),
        new("map-room", "City Map Room", "Room", 74, 54, 18, 12, "A planning room for districts, zoning, permits, and infrastructure."),
        new("dining", "State Dining Room", "Room", 30, 6, 40, 12, "A formal room for hosted civic dinners and receptions.")
    ];

    public static SingularityMansionFeature Resolve(string id)
        => Features.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No mansion feature is registered for '{id}'.");

    public static IReadOnlyList<SingularityMansionFeature> Alcoves
        => Features.Where(x => string.Equals(x.Kind, "Alcove", StringComparison.OrdinalIgnoreCase)).ToArray();
}

/// <summary>Exterior/elevation presentation metadata for the Mayor's Mansion.</summary>
public static class SingularityMansionElevation
{
    public const string Title = "MAYOR'S MANSION // FRONT ELEVATION";

    public const string Description =
        "The front elevation reveals the ceremonial entrance, central stair, upper gallery, " +
        "roofline, windows, and civic seal as an architectural facade rather than a plan.";

    public const string ButlerGreeting =
        "Welcome to the mayor's mansion. If you'd like to meet with the mayor then follow with me...";

    public const string MeetingCancelledPrompt =
        "The mayor is unavailable. Would you prefer to take over the mayor's office and run this great city for some time?";
}
