namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Declarative contract for the Workshop remote AEC office and its intelligent drafting table.
/// </summary>
/// <remarks>
/// The drafting table treats a drawn line as semantic data rather than merely pixels:
/// the creator establishes arm geometry, angle, start and stop positions, then declares
/// what the line represents. The resulting intent can become a platform-neutral AEC command
/// or be routed to a downstream BIM/CAD application.
/// </remarks>
public sealed record SpatialAECRemoteOfficeManifest(
    string Id,
    string Name,
    string SectorPortalName,
    SpatialDraftingTableManifest DraftingTable,
    IReadOnlyList<SpatialAECDiscipline> Disciplines,
    IReadOnlyList<SpatialAECExternalTarget> ExternalTargets)
{
    public static SpatialAECRemoteOfficeManifest CreateDefault()
        => new(
            "aec-remote-office",
            "AEC REMOTE OFFICE",
            "AEC SECTOR PORTAL",
            new SpatialDraftingTableManifest(
                "intelligent-drafting-table",
                "INTELLIGENT DRAFTING TABLE",
                ["wall", "door", "window", "column", "beam", "concrete", "steel", "equipment", "duct", "pipe", "conduit", "electrical", "annotation"],
                ["arm-position", "angle", "start-point", "end-point", "semantic-type"],
                "semantic-line"),
            [
                new("architecture", "ARCHITECTURE", ["walls", "openings", "rooms", "assemblies"]),
                new("structural", "STRUCTURAL", ["concrete", "steel", "connections", "loads"]),
                new("mechanical", "MECHANICAL", ["duct", "equipment", "hvac"]),
                new("electrical", "ELECTRICAL", ["power", "lighting", "controls"]),
                new("plumbing", "PLUMBING", ["pipe", "fixtures", "systems"]),
                new("procurement", "PROCUREMENT", ["estimating", "suppliers", "fabrication", "logistics"])
            ],
            [
                new("revit", "REVIT", "external-application-command"),
                new("inventor", "INVENTOR", "external-application-command"),
                new("singularity-warehouse", "SINGULARITY WAREHOUSE", "platform-neutral-data")
            ]);

    public bool SupportsLineType(string type)
        => DraftingTable.SemanticLineTypes.Any(x => string.Equals(x, type, StringComparison.OrdinalIgnoreCase));

    public SpatialAECDiscipline? FindDiscipline(string id)
        => Disciplines.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
}

public readonly record struct SpatialDraftingTableManifest(string Id, string Name, IReadOnlyList<string> SemanticLineTypes, IReadOnlyList<string> Controls, string OutputConcept);
public readonly record struct SpatialAECDiscipline(string Id, string Name, IReadOnlyList<string> Concepts);
public readonly record struct SpatialAECExternalTarget(string Id, string Name, string Boundary);
