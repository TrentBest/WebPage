namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Describes the structural/code basis that governs a generated AEC concept.
/// Fictional buildings use an explicit fictional code profile derived from a named
/// real-world code family rather than silently abandoning structural discipline.
/// </summary>
public sealed record SpatialAECCodeProfile(
    string Id,
    string Name,
    string Basis,
    string Edition,
    string StructuralSystem,
    int DefaultOccupancyLoad,
    int DefaultLiveLoad,
    int DefaultFloorToFloor,
    string SafetyNote);

/// <summary>
/// Supplies a pre-established code profile for every canonical building type.
/// This is a modeling basis, not a jurisdictional certification engine.
/// </summary>
public static class SpatialAECCodeCatalog
{
    private static readonly IReadOnlyDictionary<SpatialBuildingPurpose, SpatialAECCodeProfile> Profiles =
        new Dictionary<SpatialBuildingPurpose, SpatialAECCodeProfile>
        {
            [SpatialBuildingPurpose.ResidentialMultifamily] = Create("residential", "Residential Building Code Basis", "IBC/IRC-derived residential principles", 40, 40, 10, "Egress, fire separation, occupancy and structural loads remain explicit."),
            [SpatialBuildingPurpose.ResidentialLuxury] = Create("residential-luxury", "Residential High-Amenity Code Basis", "IBC-derived residential principles", 40, 50, 10, "Higher amenity density does not remove egress, fire or structural requirements."),
            [SpatialBuildingPurpose.Office] = Create("office", "Business Occupancy Code Basis", "IBC-derived business occupancy principles", 100, 50, 12, "Core, egress, fire protection and structural systems are first-class plan data."),
            [SpatialBuildingPurpose.Retail] = Create("retail", "Mercantile Code Basis", "IBC-derived mercantile occupancy principles", 100, 60, 14, "Public circulation, egress and fire separation are modeled with the program."),
            [SpatialBuildingPurpose.CivicGovernment] = Create("civic", "Civic Assembly/Business Code Basis", "IBC-derived civic occupancy principles", 100, 60, 14, "Public access, controlled areas and life-safety circulation remain explicit."),
            [SpatialBuildingPurpose.Education] = Create("education", "Educational Occupancy Code Basis", "IBC-derived educational occupancy principles", 100, 60, 14, "Classroom egress, assembly areas and protected circulation are modeled."),
            [SpatialBuildingPurpose.Medical] = Create("medical", "Institutional/Healthcare Code Basis", "IBC-derived institutional occupancy principles", 100, 80, 14, "Clinical circulation, protected egress and service infrastructure remain explicit."),
            [SpatialBuildingPurpose.Industrial] = Create("industrial", "Industrial Code Basis", "IBC-derived industrial occupancy principles", 100, 75, 18, "Equipment, service clearances, egress and structural loads are modeled together."),
            [SpatialBuildingPurpose.Research] = Create("research", "Research Laboratory Code Basis", "IBC-derived laboratory occupancy principles", 100, 75, 14, "Laboratory hazards, controlled circulation, egress and structural systems are explicit."),
            [SpatialBuildingPurpose.Hospitality] = Create("hospitality", "Hospitality Code Basis", "IBC-derived transient occupancy principles", 100, 50, 12, "Guest circulation, egress and fire separation remain explicit."),
            [SpatialBuildingPurpose.Transport] = Create("transport", "Transportation Facility Code Basis", "IBC-derived transportation occupancy principles", 100, 100, 18, "Large-span circulation, public egress and service zones remain explicit."),
            [SpatialBuildingPurpose.Recreation] = Create("recreation", "Recreation/Assembly Code Basis", "IBC-derived assembly occupancy principles", 100, 100, 18, "Assembly load, egress width and public circulation are explicit."),
            [SpatialBuildingPurpose.Utility] = Create("utility", "Utility Facility Code Basis", "IBC-derived utility/industrial principles", 100, 100, 18, "Equipment access, service clearances and life safety remain explicit."),
            [SpatialBuildingPurpose.Unknown] = Create("generic", "Generic Building Code Basis", "IBC-derived general building principles", 100, 50, 12, "Unknown programs must establish a code basis before detailed generation.")
        };

    /// <summary>Returns the pre-established code profile for a building specification.</summary>
    public static SpatialAECCodeProfile For(SpatialBuildingSpecification specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        var profile = Profiles.TryGetValue(specification.Purpose, out var value)
            ? value
            : Profiles[SpatialBuildingPurpose.Unknown];

        return profile;
    }

    private static SpatialAECCodeProfile Create(string id, string name, string basis, int occupancyLoad, int liveLoad, int floorToFloor, string safetyNote)
        => new(id, name, basis, "SIMULATION-BASIS-1.0", "Conventional structural frame", occupancyLoad, liveLoad, floorToFloor, safetyNote);
}
