namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

public enum SpatialBuildingPurpose
{
    Unknown,
    ResidentialMultifamily,
    ResidentialLuxury,
    Office,
    Retail,
    CivicGovernment,
    Education,
    Medical,
    Industrial,
    Hospitality,
    Transport,
    Research,
    Recreation,
    Utility
}

public enum SpatialBuildingTier
{
    Low,
    Standard,
    Premium,
    Luxury
}

/// <summary>
/// A purpose-first specification for a building type. Geometry is derived from
/// purpose and tier rather than being an arbitrary visual rectangle.
/// </summary>
public sealed record SpatialBuildingSpecification(
    string Id,
    string Name,
    SpatialBuildingPurpose Purpose,
    SpatialBuildingTier Tier,
    double MinimumWidth,
    double MinimumDepth,
    double MaximumWidth,
    double MaximumDepth,
    int MinimumFloors,
    int MaximumFloors,
    int OccupantsPerFloor,
    string Description)
{
    public double MinimumFootprint => MinimumWidth * MinimumDepth;
    public double MaximumFootprint => MaximumWidth * MaximumDepth;

    public bool AcceptsFootprint(double width, double depth)
        => width >= MinimumWidth && depth >= MinimumDepth &&
           width <= MaximumWidth && depth <= MaximumDepth;

    public int EstimateOccupancy(int floors)
        => Math.Max(1, Math.Min(floors, MaximumFloors)) * OccupantsPerFloor;
}

/// <summary>
/// Canonical building specifications used by the city builder and future
/// building-authoring tools.
/// </summary>
public static class SpatialBuildingSpecificationCatalog
{
    private static readonly IReadOnlyList<SpatialBuildingSpecification> Specifications =
    [
        new(
            "residential.multifamily.low",
            "Low-End Multifamily",
            SpatialBuildingPurpose.ResidentialMultifamily,
            SpatialBuildingTier.Low,
            10, 10, 20, 20,
            2, 4, 8,
            "Compact multifamily housing. The canonical low-end unit block starts at a 10 x 10 footprint."),
        new(
            "residential.multifamily.standard",
            "Standard Multifamily",
            SpatialBuildingPurpose.ResidentialMultifamily,
            SpatialBuildingTier.Standard,
            16, 16, 32, 32,
            2, 8, 16,
            "Mid-market apartments with larger shared circulation and amenity space."),
        new(
            "residential.multifamily.luxury",
            "Luxury Multifamily",
            SpatialBuildingPurpose.ResidentialLuxury,
            SpatialBuildingTier.Luxury,
            24, 24, 60, 60,
            4, 20, 12,
            "Large residential development whose footprint, floors, amenities, and capacity expand with luxury."),
        new(
            "office.small",
            "Small Office",
            SpatialBuildingPurpose.Office,
            SpatialBuildingTier.Standard,
            10, 10, 20, 20,
            1, 4, 10,
            "Small office floorplate. Capacity scales with usable floorplate and floors."),
        new(
            "office.corporate",
            "Corporate Office",
            SpatialBuildingPurpose.Office,
            SpatialBuildingTier.Premium,
            20, 20, 60, 60,
            2, 30, 24,
            "Office building sized around the required floorplate rather than a fixed visual template."),
        new(
            "office.tower",
            "Office Tower",
            SpatialBuildingPurpose.Office,
            SpatialBuildingTier.Luxury,
            30, 30, 90, 90,
            8, 80, 32,
            "High-rise office specification for major employment districts."),
        new(
            "civic.government",
            "Civic Government Building",
            SpatialBuildingPurpose.CivicGovernment,
            SpatialBuildingTier.Premium,
            30, 20, 100, 80,
            2, 12, 20,
            "Administrative building whose required departments and public services determine its program."),
        new(
            "retail.market",
            "Market Hall",
            SpatialBuildingPurpose.Retail,
            SpatialBuildingTier.Standard,
            20, 12, 80, 40,
            1, 3, 18,
            "Commercial hall sized to the number of vendors and public circulation requirements."),
        new(
            "education.school",
            "School",
            SpatialBuildingPurpose.Education,
            SpatialBuildingTier.Standard,
            30, 20, 100, 80,
            1, 5, 30,
            "Educational facility sized to its student and staff population."),
        new(
            "industrial.fabrication",
            "Fabrication Facility",
            SpatialBuildingPurpose.Industrial,
            SpatialBuildingTier.Premium,
            40, 30, 160, 120,
            1, 8, 35,
            "Industrial building sized to fabrication lines, logistics, robotics, and service capacity."),
        new(
            "medical.hospital",
            "Hospital",
            SpatialBuildingPurpose.Medical,
            SpatialBuildingTier.Premium,
            50, 40, 160, 120,
            3, 15, 40,
            "Medical campus building sized by clinical departments and patient capacity."),
        new(
            "research.laboratory",
            "Research Laboratory",
            SpatialBuildingPurpose.Research,
            SpatialBuildingTier.Premium,
            30, 24, 90, 70,
            3, 18, 24,
            "Research facility sized for laboratories, controlled environments, engineering work, and technical support.")
    ];

    public static IReadOnlyList<SpatialBuildingSpecification> All => Specifications;

    public static SpatialBuildingSpecification Resolve(string id)
        => Specifications.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No building specification is registered for '{id}'.");

    public static bool TryResolve(string id, out SpatialBuildingSpecification specification)
    {
        specification = Specifications.FirstOrDefault(
            x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase))!;
        return specification is not null;
    }
}
