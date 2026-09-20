using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// A concrete authored AEC building-type record that can be discovered through the
/// ontology catalog. The record is semantic inventory; a renderer decides how it is shown.
/// </summary>
public sealed record SpatialAECBuildingType(
    string Id,
    string Name,
    string Paradigm,
    string Domain,
    string Kingdom,
    string Phylum,
    string Class,
    string Order,
    string Family,
    string Genus,
    string Species)
{
    /// <summary>Returns the nine human-readable ontology values in layer order.</summary>
    public IReadOnlyList<string> Layers =>
    [
        Paradigm, Domain, Kingdom, Phylum, Class,
        Order, Family, Genus, Species
    ];

    /// <summary>Converts this authored record to the compact runtime ontology coordinate.</summary>
    public OntologySignature ToOntologySignature()
        => SpatialAECOntologyCatalog.ToSignature(this);
}

/// <summary>
/// The AEC ontology mall: a deterministic inventory of concrete building types.
/// Shopping the mall means narrowing real catalog inventory as intent becomes specific,
/// rather than presenting an unrelated list of pseudo-options at every layer.
/// </summary>
public static class SpatialAECOntologyCatalog
{
    private static readonly IReadOnlyList<SpatialAECBuildingType> Inventory =
    [
        new(
            "aec.research.vertical.engineering-development",
            "Research Laboratory",
            "REALITY", "RESEARCH", "FACILITY", "LABORATORY", "RESEARCH FACILITY",
            "VERTICAL FACILITY", "ENGINEERING", "DEVELOPMENT", "RESEARCH LABORATORY"),

        new(
            "aec.fiction.built.civic-future",
            "Futuristic Civic Research Center",
            "FICTION", "BUILT ENVIRONMENT", "FACILITY", "LABORATORY", "RESEARCH FACILITY",
            "SINGLE BUILDING", "SCIENCE", "APPLIED RESEARCH", "CUSTOM FACILITY"),

        new(
            "aec.research.single.science-advanced",
            "High-Tech Research Laboratory",
            "FICTION", "RESEARCH", "FACILITY", "LABORATORY", "HIGH-TECH LABORATORY",
            "SINGLE BUILDING", "SCIENCE", "ADVANCED RESEARCH", "SCI-FI HIGH-TECH LABORATORY"),

        new(
            "aec.built.single.architecture-office",
            "Office Building",
            "REALITY", "BUILT ENVIRONMENT", "FACILITY", "OFFICE", "OFFICE BUILDING",
            "SINGLE BUILDING", "ENGINEERING", "DEVELOPMENT", "CUSTOM FACILITY"),

        new(
            "aec.built.vertical.science-research",
            "Research Facility",
            "REALITY", "BUILT ENVIRONMENT", "FACILITY", "LABORATORY", "RESEARCH FACILITY",
            "VERTICAL FACILITY", "SCIENCE", "APPLIED RESEARCH", "RESEARCH LABORATORY"),

        new(
            "aec.industrial.single.engineering-manufacturing",
            "Manufacturing Plant",
            "REALITY", "INDUSTRIAL", "FACILITY", "FACTORY", "PRODUCTION LABORATORY",
            "SINGLE BUILDING", "ENGINEERING", "DEVELOPMENT", "CUSTOM FACILITY"),

        new(
            "aec.industrial.multi.energy-production",
            "Energy Production Facility",
            "REALITY", "INDUSTRIAL", "INFRASTRUCTURE", "FACTORY", "PRODUCTION LABORATORY",
            "MULTI-BUILDING", "ENERGY", "DEVELOPMENT", "CUSTOM FACILITY"),

        new(
            "aec.civic.single.medical-hospital",
            "Hospital",
            "REALITY", "CIVIC", "FACILITY", "HOSPITAL", "RESEARCH FACILITY",
            "VERTICAL FACILITY", "MEDICAL", "APPLIED RESEARCH", "CUSTOM FACILITY"),

        new(
            "aec.civic.single.public-library",
            "Public Library",
            "REALITY", "CIVIC", "WORKPLACE", "OFFICE", "RESEARCH FACILITY",
            "SINGLE BUILDING", "SCIENCE", "APPLIED RESEARCH", "CUSTOM FACILITY"),

        new(
            "aec.residential.vertical.apartment",
            "Apartment Building",
            "REALITY", "RESIDENTIAL", "FACILITY", "OFFICE", "RESEARCH FACILITY",
            "VERTICAL FACILITY", "ENGINEERING", "DEVELOPMENT", "CUSTOM FACILITY"),

        new(
            "aec.built.campus.education",
            "University Campus",
            "REALITY", "BUILT ENVIRONMENT", "CAMPUS", "OFFICE", "RESEARCH FACILITY",
            "CAMPUS FACILITY", "SCIENCE", "APPLIED RESEARCH", "CUSTOM FACILITY"),

        new(
            "aec.industrial.multi.warehouse",
            "Distribution Warehouse",
            "REALITY", "INDUSTRIAL", "INFRASTRUCTURE", "WAREHOUSE", "PRODUCTION LABORATORY",
            "MULTI-BUILDING", "ENGINEERING", "DEVELOPMENT", "CUSTOM FACILITY"),

    ];

    /// <summary>Returns the complete immutable catalog inventory.</summary>
    public static IReadOnlyList<SpatialAECBuildingType> ListBuildingTypes()
        => Inventory;

    /// <summary>Returns a concrete building type by stable catalog identity.</summary>
    public static bool TryGetBuildingType(string id, out SpatialAECBuildingType? buildingType)
    {
        buildingType = Inventory.FirstOrDefault(x =>
            string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
        return buildingType is not null;
    }

    /// <summary>
    /// Shops the catalog using the answers already supplied. Each returned option exists
    /// on at least one concrete catalog record compatible with the selected path.
    /// </summary>
    public static IReadOnlyList<string> OptionsForLayer(
        int layer,
        IReadOnlyList<string?> answers)
    {
        if (layer < 0 || layer >= OntologySignature.LayerCount)
            return Array.Empty<string>();

        return Inventory
            .Where(x => MatchesPriorLayers(x, layer, answers))
            .Select(x => x.Layers[layer])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>Finds the most specific catalog item matching a completed ontology path.</summary>
    public static SpatialAECBuildingType? Resolve(
        IReadOnlyList<string?> answers)
    {
        return Inventory
            .Where(x => answers.Count >= OntologySignature.LayerCount)
            .Where(x => x.Layers.Zip(answers, (catalogValue, answer) =>
                string.Equals(catalogValue, answer, StringComparison.OrdinalIgnoreCase))
                .All(match => match))
            .OrderBy(x => x.Id, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    /// <summary>Creates the nine-integer runtime coordinate for an authored catalog item.</summary>
    public static OntologySignature ToSignature(SpatialAECBuildingType buildingType)
    {
        var values = buildingType.Layers
            .Select(SpatialOntologyToken.For)
            .ToArray();

        return new OntologySignature(
            values[0], values[1], values[2], values[3], values[4],
            values[5], values[6], values[7], values[8]);
    }

    private static bool MatchesPriorLayers(
        SpatialAECBuildingType buildingType,
        int layer,
        IReadOnlyList<string?> answers)
    {
        for (var i = 0; i < layer; i++)
        {
            if (i >= answers.Count || string.IsNullOrWhiteSpace(answers[i]))
                continue;

            if (!string.Equals(
                    buildingType.Layers[i],
                    answers[i],
                    StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
}

/// <summary>
/// Deterministic authoring-time token registry used to flatten catalog values into
/// integer ontology coordinates. The catalog remains human-readable; runtime identity
/// is the integer signature.
/// </summary>
public static class SpatialOntologyToken
{
    /// <summary>Returns the deterministic token for an ontology value.</summary>
    public static int For(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        unchecked
        {
            uint hash = 5381;
            foreach (var character in value.ToUpperInvariant())
                hash = ((hash << 5) + hash) ^ character;

            return hash == 0 ? 1 : (int)(hash & 0x7fffffff);
        }
    }
}
