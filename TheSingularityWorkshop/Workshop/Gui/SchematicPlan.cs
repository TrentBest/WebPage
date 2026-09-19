namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Describes the drawing level used to perceive a spatial construct.
/// The same universe can therefore be redrawn as a site, building,
/// department, or room without changing the underlying interaction model.
/// </summary>
public enum SchematicScale
{
    Site,
    Building,
    Department,
    Room
}

/// <summary>
/// Building-service disciplines that may be drawn over architectural geometry.
/// These are intentionally semantic; a GUI builder decides how each trade is perceived.
/// </summary>
public enum SchematicTrade
{
    Electrical,
    Plumbing,
    Hvac,
    FireProtection,
    Data,
    Security
}

/// <summary>
/// A line segment in schematic/world coordinates.
/// </summary>
public sealed record SchematicLine(
    double X1,
    double Y1,
    double X2,
    double Y2);

/// <summary>
/// A rectangular opening in a wall, such as a door or window.
/// </summary>
public sealed record SchematicOpening(
    double X,
    double Y,
    double Width,
    double Height,
    string Kind = "DOOR");

/// <summary>
/// A semantic label attached to a region of a plan.
/// </summary>
public sealed record SchematicLabel(
    string Text,
    double X,
    double Y,
    double Size = 3,
    string Kind = "ROOM");

/// <summary>
/// A semantic route for a building service/trade.
/// </summary>
public sealed record SchematicSystemRoute(
    SchematicTrade Trade,
    IReadOnlyList<SchematicLine> Segments);

/// <summary>
/// Blueprint-level geometry for a spatial construct.
/// It contains no HTML, CSS, SVG, or renderer-specific state.
/// </summary>
public sealed record SchematicPlan(
    string Id,
    string Name,
    SchematicScale Scale,
    double Width,
    double Height,
    IReadOnlyList<SchematicLine> Walls,
    IReadOnlyList<SchematicOpening> Openings,
    IReadOnlyList<SchematicLabel> Labels,
    IReadOnlyList<SchematicSystemRoute> Systems);
