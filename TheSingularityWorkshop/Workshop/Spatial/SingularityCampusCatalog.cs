namespace TheSingularityWorkshop.Gui;

/// <summary>Declarative composition catalog for the first Singularity Workshop campus.</summary>
/// <remarks>
/// The catalog separates what exists in the campus from how the current SVG perception layer renders it.
/// These definitions are deliberately small data objects so future users can compose their own buildings.
/// </remarks>
public static class SingularityCampusCatalog
{
    /// <summary>Returns the campus buildings in world-space order.</summary>
    public static IReadOnlyList<SpatialBuildingSpec> Buildings =>
    [
        new("singularity-mansion", "Singularity Mansion", new SpatialBounds(2, 10, 30, 32), Columns: 12, Rows: 10, Detail: 1, Accent: "#ff3b8d", Fill: "#21101a", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Right, 12, 18), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Bottom, 11, 23)]),
        new("image-tools", "Image Workshop", new SpatialBounds(42, 8, 18, 14), Columns: 3, Rows: 2, Detail: 1, Accent: "#ff2bd6", Fill: "#251126", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 7, 11), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Right, 4, 9)]),
        new("forge", "The Forge", new SpatialBounds(72, 8, 18, 20), Columns: 4, Rows: 3, Detail: 1, Accent: "#ff3b8d", Fill: "#21101a", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 6, 12), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Left, 7, 13)]),
        new("blueprint-library", "Blueprint Library", new SpatialBounds(42, 30, 18, 12), Columns: 4, Rows: 2, Detail: 1, Accent: "#f7f7ff", Fill: "#17171f", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 7, 11), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Top, 4, 10)]),
        new("singularity-transit", "Singularity Transit", new SpatialBounds(68, 34, 24, 14), Columns: 6, Rows: 2, Detail: 1, Accent: "#00eaff", Fill: "#0d2026", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Bottom, 9, 15), new SpatialOpening(SpatialOpeningKind.Passage, SpatialGeometryEdge.Left, 5, 9)]),
        new("npc-studio", "NPC Studio", new SpatialBounds(4, 48, 18, 14), Columns: 3, Rows: 2, Detail: 1, Accent: "#ffe04a", Fill: "#26210c", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Top, 7, 11), new SpatialOpening(SpatialOpeningKind.Window, SpatialGeometryEdge.Right, 4, 10)]),
        new("fsm-bench", "FSM Workbench", new SpatialBounds(30, 46, 16, 10), Columns: 4, Rows: 2, Detail: 1, Accent: "#00eaff", Fill: "#0d2026", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Top, 6, 10)]),
        new("storage-bins", "Storage Bins", new SpatialBounds(90, 58, 7, 20), Columns: 2, Rows: 4, Detail: 1, Accent: "#52e05a", Fill: "#102115", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Left, 8, 12)]),
        new("singularity-ontology-mall", "Singularity Ontology Mall", new SpatialBounds(36, 62, 60, 30), Columns: 8, Rows: 4, Detail: 1, Accent: "#ffe04a", Fill: "#17150a", Openings: [new SpatialOpening(SpatialOpeningKind.Door, SpatialGeometryEdge.Top, 30, 38), new SpatialOpening(SpatialOpeningKind.Passage, SpatialGeometryEdge.Left, 12, 18), new SpatialOpening(SpatialOpeningKind.Passage, SpatialGeometryEdge.Right, 12, 18)])
    ];

    /// <summary>Departments exposed by the inverted ontology floor plan.</summary>
    public static IReadOnlyList<SpatialOntologyDepartment> OntologyDepartments =>
    [
        new("reality", "REALITY", 0),
        new("fiction", "FICTION", 1),
        new("abstraction", "ABSTRACTION", 2),
        new("utility", "UTILITY", 3)
    ];

    /// <summary>Destinations served by the campus transit station.</summary>
    public static IReadOnlyList<SpatialTransitDestination> TransitDestinations =>
    [
        new("ontology-mall", "SINGULARITY ONTOLOGY", "singularity-ontology-mall"),
        new("workshop", "THE WORKSHOP", "workshop"),
        new("forge", "THE FORGE", "forge"),
        new("mansion", "SINGULARITY MANSION", "mansion")
    ];
}

/// <summary>A department in the mall's inverted ontology hierarchy.</summary>
public readonly record struct SpatialOntologyDepartment(string Id, string Label, int Level);

/// <summary>A destination exposed by a spatial transit station.</summary>
public readonly record struct SpatialTransitDestination(string Id, string Label, string SceneId);
