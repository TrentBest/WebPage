using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// A concrete building type in the AEC ontology mall. The record is semantic inventory;
/// renderers consume it rather than inventing their own building taxonomy.
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

    /// <summary>Converts this catalog record to the compact runtime ontology coordinate.</summary>
    public OntologySignature ToOntologySignature()
        => SpatialAECOntologyCatalog.ToSignature(this);
}

/// <summary>
/// The AEC ontology mall: a searchable inventory of concrete building types.
/// The primary inventory is derived from <see cref="SpatialBuildingSpecificationCatalog"/>;
/// scenario-only fictional records are explicitly appended rather than masquerading as
/// ordinary building specifications.
/// </summary>
public static class SpatialAECOntologyCatalog
{
    private static readonly IReadOnlyList<SpatialAECBuildingType> Inventory =
        BuildInventory();

    /// <summary>Returns the complete building-type inventory.</summary>
    public static IReadOnlyList<SpatialAECBuildingType> ListBuildingTypes()
        => Inventory;

    /// <summary>Returns a concrete building type by stable catalog identity.</summary>
    public static bool TryGetBuildingType(
        string id,
        out SpatialAECBuildingType? buildingType)
    {
        buildingType = Inventory.FirstOrDefault(x =>
            string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
        return buildingType is not null;
    }

    /// <summary>
    /// Shops the inventory using the answers already supplied. Every option is backed by
    /// at least one concrete inventory record compatible with the selected path.
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
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>Finds a concrete catalog item matching a completed ontology path.</summary>
    public static SpatialAECBuildingType? Resolve(
        IReadOnlyList<string?> answers)
    {
        return Inventory
            .Where(x => answers.Count >= OntologySignature.LayerCount)
            .Where(x => x.Layers.Zip(
                answers,
                (catalogValue, answer) =>
                    string.Equals(
                        catalogValue,
                        answer,
                        StringComparison.OrdinalIgnoreCase))
                .All(match => match))
            .OrderBy(x => x.Id, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    /// <summary>Creates the nine-integer runtime coordinate for a catalog item.</summary>
    public static OntologySignature ToSignature(SpatialAECBuildingType buildingType)
    {
        ArgumentNullException.ThrowIfNull(buildingType);

        var values = buildingType.Layers
            .Select(SpatialOntologyToken.For)
            .ToArray();

        return new OntologySignature(
            values[0], values[1], values[2], values[3], values[4],
            values[5], values[6], values[7], values[8]);
    }

    private static IReadOnlyList<SpatialAECBuildingType> BuildInventory()
    {
        var canonical = BuildCanonicalBuildingTypes();

        var scenarios = new[]
        {
            new SpatialAECBuildingType(
                "aec.scenario.fiction.civic-future",
                "Futuristic Civic Research Center",
                "FICTION", "BUILT ENVIRONMENT", "FACILITY", "LABORATORY",
                "RESEARCH FACILITY", "SINGLE BUILDING", "SCIENCE",
                "APPLIED RESEARCH", "CUSTOM FACILITY"),

            new SpatialAECBuildingType(
                "aec.scenario.fiction.high-tech-research",
                "High-Tech Research Laboratory",
                "FICTION", "RESEARCH", "FACILITY", "LABORATORY",
                "HIGH-TECH LABORATORY", "SINGLE BUILDING", "SCIENCE",
                "ADVANCED RESEARCH", "SCI-FI HIGH-TECH LABORATORY"),

            new SpatialAECBuildingType(
                "aec.scenario.industrial.energy-production",
                "Energy Production Facility",
                "REALITY", "INDUSTRIAL", "INFRASTRUCTURE", "FACTORY",
                "PRODUCTION LABORATORY", "MULTI-BUILDING", "ENERGY",
                "DEVELOPMENT", "CUSTOM FACILITY"),

            new SpatialAECBuildingType(
                "aec.scenario.industrial.warehouse",
                "Distribution Warehouse",
                "REALITY", "INDUSTRIAL", "INFRASTRUCTURE", "WAREHOUSE",
                "PRODUCTION LABORATORY", "MULTI-BUILDING", "ENGINEERING",
                "DEVELOPMENT", "CUSTOM FACILITY")
        };

        var aliases = new[]
        {
            new SpatialAECBuildingType(
                "office.building",
                "Office Building",
                "REALITY", "BUILT ENVIRONMENT", "FACILITY", "OFFICE",
                "OFFICE BUILDING", "SINGLE BUILDING", "ENGINEERING",
                "DEVELOPMENT", "SMALL OFFICE")
        };

        return canonical
            .Concat(scenarios)
            .Concat(aliases)
            .GroupBy(x => x.Id, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();
    }

    private static IReadOnlyList<SpatialAECBuildingType> BuildCanonicalBuildingTypes()
    {
        var specifications = SpatialBuildingSpecificationCatalog.All;

        var profiles = specifications.Select(CreateProfile);

        return SpatialAECBuildingTypeFactory.Create(specifications, profiles);
    }

    private static SpatialAECBuildingOntologyProfile CreateProfile(
        SpatialBuildingSpecification specification)
    {
        if (specification.Purpose == SpatialBuildingPurpose.Research)
        {
            return new SpatialAECBuildingOntologyProfile(
                specification.Id,
                "REALITY",
                "RESEARCH",
                "FACILITY",
                "LABORATORY",
                "RESEARCH FACILITY",
                "VERTICAL FACILITY",
                "ENGINEERING",
                "DEVELOPMENT",
                "RESEARCH LABORATORY");
        }

        var domain = specification.Purpose switch
        {
            SpatialBuildingPurpose.CivicGovernment => "CIVIC",
            SpatialBuildingPurpose.Education => "BUILT ENVIRONMENT",
            SpatialBuildingPurpose.Industrial => "INDUSTRIAL",
            SpatialBuildingPurpose.Medical => "CIVIC",
            SpatialBuildingPurpose.Office => "BUILT ENVIRONMENT",
            SpatialBuildingPurpose.Retail => "BUILT ENVIRONMENT",
            SpatialBuildingPurpose.ResidentialMultifamily => "RESIDENTIAL",
            SpatialBuildingPurpose.ResidentialLuxury => "RESIDENTIAL",
            _ => "BUILT ENVIRONMENT"
        };

        var phylum = specification.Purpose switch
        {
            SpatialBuildingPurpose.CivicGovernment => "GOVERNMENT",
            SpatialBuildingPurpose.Education => "EDUCATION",
            SpatialBuildingPurpose.Industrial => "FACTORY",
            SpatialBuildingPurpose.Medical => "HOSPITAL",
            SpatialBuildingPurpose.Office => "OFFICE",
            SpatialBuildingPurpose.Retail => "RETAIL",
            SpatialBuildingPurpose.ResidentialMultifamily => "RESIDENTIAL",
            SpatialBuildingPurpose.ResidentialLuxury => "RESIDENTIAL",
            _ => "FACILITY"
        };

        var family = specification.Purpose switch
        {
            SpatialBuildingPurpose.CivicGovernment => "CIVIC",
            SpatialBuildingPurpose.Education => "SCIENCE",
            SpatialBuildingPurpose.Industrial => "ENGINEERING",
            SpatialBuildingPurpose.Medical => "MEDICAL",
            SpatialBuildingPurpose.Office => "ENGINEERING",
            SpatialBuildingPurpose.Retail => "COMMERCE",
            SpatialBuildingPurpose.ResidentialMultifamily => "RESIDENTIAL",
            SpatialBuildingPurpose.ResidentialLuxury => "RESIDENTIAL",
            _ => "ENGINEERING"
        };

        return SpatialAECBuildingTypeFactory.StandardProfile(
            specification,
            domain,
            phylum,
            family);
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
/// integer ontology coordinates. Human-readable catalog data stays at the authoring edge.
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
