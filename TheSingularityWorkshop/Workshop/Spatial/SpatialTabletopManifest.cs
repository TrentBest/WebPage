namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Declarative vocabulary for tabletop Experiences. The Workshop supplies the
/// shared environment and presentation mechanics; the Game Master and players
/// supply the actual game content and rules.
/// </summary>
/// <remarks>
/// This model deliberately does not encode any particular published game's rules,
/// characters, setting, artwork, names, or proprietary terminology.
/// </remarks>
public sealed record SpatialTabletopManifest(
    IReadOnlyList<SpatialTabletopUtility> Utilities,
    IReadOnlyList<SpatialTabletopSurface> Surfaces,
    IReadOnlyList<SpatialTabletopSeat> Seats)
{
    /// <summary>Creates the initial neutral tabletop utility catalog.</summary>
    public static SpatialTabletopManifest CreateDefault()
        => new(
            [
                new("gm-control", "GAME MASTER CONTROL", "Private controls for placing, revealing, moving, and annotating table state."),
                new("tile-placement", "TILE PLACEMENT", "Place, rotate, remove, and replace user-provided tiles or terrain."),
                new("piece-placement", "PIECE PLACEMENT", "Place and move user-provided pieces, tokens, markers, and miniatures."),
                new("fog-of-knowledge", "FOG OF KNOWLEDGE", "Control which table objects are visible to each seat."),
                new("seat-view", "SEAT VIEW", "Render the table from the perspective assigned to a player or Game Master."),
                new("avatar", "TABLE AVATAR", "Represent a player with an avatar positioned around the shared table."),
                new("annotation", "TABLE ANNOTATION", "Allow notes, markers, measurements, and temporary spatial references."),
                new("snapshot", "TABLE SNAPSHOT", "Capture and restore a presentation state without imposing game rules."),
                new("initiative-board", "TURN BOARD", "Provide an optional neutral ordering surface for creator-defined sequencing."),
                new("custom-rule-panel", "CUSTOM RULE PANEL", "Expose creator-defined instructions and controls without embedding a ruleset.")
            ],
            [
                new("tabletop", "TABLETOP", "Shared surface for tiles, pieces, markers, and views."),
                new("gm-screen", "GAME MASTER SCREEN", "Private surface for the Game Master to observe and control the Experience."),
                new("player-seat", "PLAYER SEAT", "Seat-bound view of the shared table.")
            ],
            [
                new("game-master", "GAME MASTER", "Full creator-controlled table view."),
                new("player-1", "PLAYER 1", "Player perspective."),
                new("player-2", "PLAYER 2", "Player perspective."),
                new("player-3", "PLAYER 3", "Player perspective."),
                new("player-4", "PLAYER 4", "Player perspective.")
            ]);

    /// <summary>Finds a utility by stable identifier.</summary>
    public SpatialTabletopUtility? FindUtility(string id)
        => Utilities.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Finds a surface by stable identifier.</summary>
    public SpatialTabletopSurface? FindSurface(string id)
        => Surfaces.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Finds a seat by stable identifier.</summary>
    public SpatialTabletopSeat? FindSeat(string id)
        => Seats.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
}

/// <summary>A neutral capability supplied by the tabletop environment.</summary>
public readonly record struct SpatialTabletopUtility(string Id, string Name, string Purpose);

/// <summary>A spatial surface supplied by the tabletop environment.</summary>
public readonly record struct SpatialTabletopSurface(string Id, string Name, string Purpose);

/// <summary>A perspective assigned to a participant.</summary>
public readonly record struct SpatialTabletopSeat(string Id, string Name, string Purpose);
