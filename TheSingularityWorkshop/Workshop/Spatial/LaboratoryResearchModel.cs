namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Describes a researcher's persistent digital laboratory workspace.
/// </summary>
/// <remarks>
/// A laboratory is deliberately not modeled as a finite room. The physical manifestation
/// can remain human-scale while the researcher's computational workspace expands without
/// a predefined spatial limit.
/// </remarks>
public sealed record LaboratoryResearchWorkspace(
    string Id,
    string ResearcherId,
    string DisplayName,
    IReadOnlyList<LaboratoryInstrument> Instruments,
    IReadOnlyList<LaboratoryDataRecord> Records,
    IReadOnlyList<LaboratoryVisualization> Visualizations)
{
    public static LaboratoryResearchWorkspace CreateFor(
        string researcherId,
        string displayName)
        => new(
            researcherId + "-lab",
            researcherId,
            displayName,
            [],
            [],
            []);
}

/// <summary>An instrument requested by a researcher inside a digital laboratory.</summary>
public readonly record struct LaboratoryInstrument(
    string Id,
    string Name,
    string Category,
    string Source);

/// <summary>Research data captured in a laboratory workspace.</summary>
/// <remarks>
/// Values remain opaque to the spatial layer. A future Data Warehouse/DataShelf provider
/// can attach typed datasets, provenance, units, uncertainty, and permissions.
/// </remarks>
public readonly record struct LaboratoryDataRecord(
    string Id,
    string Label,
    string DataReference,
    string ProvenanceReference);

/// <summary>A visualization embedded in a researcher's workspace.</summary>
public readonly record struct LaboratoryVisualization(
    string Id,
    string Name,
    string VisualizationType,
    string SourceReference);

/// <summary>
/// Declarative request for an instrument or visualization.
/// </summary>
public readonly record struct LaboratoryRequisition(
    string ResearcherId,
    string ItemType,
    string RequestedName,
    string Purpose,
    bool RequiresDirectorateApproval);
