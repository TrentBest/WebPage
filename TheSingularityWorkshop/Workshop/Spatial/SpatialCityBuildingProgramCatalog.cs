namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

public sealed record SpatialCityBuildingProgram(
    string StructureId,
    string SpecificationId,
    string PurposeStatement);

/// <summary>
/// Connects city massing to purpose-driven building specifications. The map
/// footprint is therefore a manifestation of a building program, not merely
/// decorative geometry.
/// </summary>
public static class SpatialCityBuildingProgramCatalog
{
    private static readonly IReadOnlyList<SpatialCityBuildingProgram> Programs =
    [
        new("city-civic-core", "civic.government", "Government and public civic services."),
        new("city-hospital", "medical.hospital", "Clinical, diagnostic, surgical, and recovery services."),
        new("city-university", "education.school", "Teaching, research, libraries, and laboratories."),
        new("city-market", "retail.market", "Public commerce, vendors, food, and services."),
        new("city-residential-north", "residential.multifamily.standard", "Multifamily housing and neighborhood services."),
        new("city-arena", "retail.market", "Public-event support and visitor commerce."),
        new("city-hotel", "residential.multifamily.standard", "Guest accommodation and visitor services."),
        new("city-airport", "office.corporate", "Terminal administration, services, and operations."),
        new("city-industrial", "industrial.fabrication", "Manufacturing, fabrication, robotics, and logistics."),
        new("city-data-center", "office.corporate", "Compute operations, networking, and technical workspaces."),
        new("city-transit-hub", "office.corporate", "Transit operations and passenger services."),
        new("city-residential-east", "residential.multifamily.standard", "Multifamily housing, schools, parks, and neighborhood services."),
        new("city-science-park", "office.corporate", "Research laboratories and technical support facilities."),
        new("city-stadium", "retail.market", "Large-event venue support and public commerce."),
        new("city-port", "industrial.fabrication", "Port administration, logistics, and marine operations."),
        new("city-museum", "office.corporate", "Archives, galleries, public exhibits, and administration."),
        new("city-utility", "industrial.fabrication", "Energy, water, waste, and district utility operations."),
        new("city-park-west", "retail.market", "Park support, recreation, gardens, and visitor services."),
        new("city-park-east", "retail.market", "Greenbelt support and public recreation.")
    ];

    public static IReadOnlyList<SpatialCityBuildingProgram> All => Programs;

    public static SpatialCityBuildingProgram Resolve(string structureId)
        => Programs.FirstOrDefault(x => string.Equals(x.StructureId, structureId, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No building program is registered for city structure '{structureId}'.");
}
