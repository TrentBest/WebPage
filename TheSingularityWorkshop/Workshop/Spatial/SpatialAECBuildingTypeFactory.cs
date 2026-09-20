using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Explicit semantic mapping from a canonical building specification into the nine-layer
/// AEC ontology. The specification remains the source of physical/program data; this
/// profile supplies the ontology vocabulary that cannot be inferred safely from geometry.
/// </summary>
public sealed record SpatialAECBuildingOntologyProfile(
    string SpecificationId,
    string Paradigm,
    string Domain,
    string Kingdom,
    string Phylum,
    string Class,
    string Order,
    string Family,
    string Genus,
    string Species);

/// <summary>
/// Builds ontology-mall inventory from canonical building specifications.
/// This is the seam between real building data and the nine-layer interrogation.
/// </summary>
public static class SpatialAECBuildingTypeFactory
{
    /// <summary>
    /// Converts specifications into concrete ontology records using explicit profiles.
    /// A specification without a profile is rejected rather than silently receiving
    /// invented ontology values.
    /// </summary>
    public static IReadOnlyList<SpatialAECBuildingType> Create(
        IEnumerable<SpatialBuildingSpecification> specifications,
        IEnumerable<SpatialAECBuildingOntologyProfile> profiles)
    {
        ArgumentNullException.ThrowIfNull(specifications);
        ArgumentNullException.ThrowIfNull(profiles);

        var profileBySpecification = profiles.ToDictionary(
            x => x.SpecificationId,
            StringComparer.OrdinalIgnoreCase);

        var result = new List<SpatialAECBuildingType>();

        foreach (var specification in specifications)
        {
            if (!profileBySpecification.TryGetValue(specification.Id, out var profile))
                throw new InvalidOperationException(
                    $"No AEC ontology profile exists for building specification '{specification.Id}'.");

            result.Add(new SpatialAECBuildingType(
                specification.Id,
                specification.Name,
                profile.Paradigm,
                profile.Domain,
                profile.Kingdom,
                profile.Phylum,
                profile.Class,
                profile.Order,
                profile.Family,
                profile.Genus,
                profile.Species));
        }

        return result;
    }

    /// <summary>
    /// Creates a profile from the canonical purpose/tier data when the vocabulary is
    /// deliberately standardized. This helper is intended for future catalog expansion;
    /// callers should prefer explicit profiles when a domain requires finer distinctions.
    /// </summary>
    public static SpatialAECBuildingOntologyProfile StandardProfile(
        SpatialBuildingSpecification specification,
        string domain,
        string phylum,
        string family,
        string genus = "DEVELOPMENT")
    {
        ArgumentNullException.ThrowIfNull(specification);
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);
        ArgumentException.ThrowIfNullOrWhiteSpace(phylum);
        ArgumentException.ThrowIfNullOrWhiteSpace(family);
        ArgumentException.ThrowIfNullOrWhiteSpace(genus);

        var order = specification.MaximumFloors >= 8
            ? "VERTICAL FACILITY"
            : "SINGLE BUILDING";

        var @class = specification.Purpose switch
        {
            SpatialBuildingPurpose.ResidentialMultifamily or SpatialBuildingPurpose.ResidentialLuxury
                => "RESIDENTIAL BUILDING",
            SpatialBuildingPurpose.Office
                => "OFFICE BUILDING",
            SpatialBuildingPurpose.Retail
                => "COMMERCIAL BUILDING",
            SpatialBuildingPurpose.CivicGovernment
                => "CIVIC BUILDING",
            SpatialBuildingPurpose.Education
                => "EDUCATIONAL BUILDING",
            SpatialBuildingPurpose.Medical
                => "MEDICAL FACILITY",
            SpatialBuildingPurpose.Industrial
                => "INDUSTRIAL FACILITY",
            SpatialBuildingPurpose.Hospitality
                => "HOSPITALITY BUILDING",
            SpatialBuildingPurpose.Transport
                => "TRANSPORT FACILITY",
            SpatialBuildingPurpose.Research
                => "RESEARCH FACILITY",
            SpatialBuildingPurpose.Recreation
                => "RECREATION FACILITY",
            SpatialBuildingPurpose.Utility
                => "UTILITY FACILITY",
            _ => "GENERAL FACILITY"
        };

        return new SpatialAECBuildingOntologyProfile(
            specification.Id,
            "REALITY",
            domain.ToUpperInvariant(),
            "FACILITY",
            phylum.ToUpperInvariant(),
            @class,
            order,
            family.ToUpperInvariant(),
            genus.ToUpperInvariant(),
            specification.Name.ToUpperInvariant());
    }
}
