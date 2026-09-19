namespace TheSingularityWorkshop.Gui;

/// <summary>Declarative world-scale composition surrounding the Workshop.</summary>
/// <remarks>
/// The Workshop is the first completed point of interest. Everything else is intentionally
/// represented as approximate, under-construction civic infrastructure until its own Experience exists.
/// </remarks>
public static class SingularityCityCatalog
{
    public static SpatialBounds WorldBounds => new(0, 0, 100, 100);
    public static SpatialBounds WorkshopDistrict => new(38, 38, 24, 24);

    public static IReadOnlyList<SpatialCityStructure> Structures =>
    [
        new("city-civic-core", "Civic Core", new SpatialBounds(44, 7, 12, 12), SpatialCityStructureKind.Civic, "Future municipal services, public records, courts, and civic gathering spaces."),
        new("city-hospital", "Medical Center", new SpatialBounds(65, 8, 14, 9), SpatialCityStructureKind.Medical, "Future hospital campus with emergency, diagnostic, surgical, and recovery facilities."),
        new("city-university", "University Quarter", new SpatialBounds(6, 10, 18, 12), SpatialCityStructureKind.Education, "Future university district for research, teaching, libraries, and laboratories."),
        new("city-market", "Public Market", new SpatialBounds(28, 8, 12, 8), SpatialCityStructureKind.Commerce, "Future market hall for food, goods, services, and local exchange."),
        new("city-residential-north", "North Residential Quarter", new SpatialBounds(81, 20, 13, 17), SpatialCityStructureKind.Residential, "Future mixed residential neighborhood with streets, courtyards, and neighborhood services."),
        new("city-arena", "Civic Arena", new SpatialBounds(5, 29, 15, 12), SpatialCityStructureKind.Culture, "Future arena for performances, competition, exhibitions, and large public events."),
        new("city-hotel", "Grand Hotel", new SpatialBounds(23, 27, 10, 12), SpatialCityStructureKind.Hospitality, "Future hotel and visitor center serving travelers arriving through the city network."),
        new("city-airport", "Regional Air Terminal", new SpatialBounds(79, 42, 17, 10), SpatialCityStructureKind.Transport, "Future regional aviation terminal and ground-transport interchange."),
        new("city-industrial", "Manufacturing District", new SpatialBounds(4, 47, 17, 15), SpatialCityStructureKind.Industry, "Future light manufacturing, fabrication, robotics, and logistics district."),
        new("city-data-center", "Data Campus", new SpatialBounds(23, 48, 11, 9), SpatialCityStructureKind.Technology, "Future high-density compute, storage, networking, and digital infrastructure campus."),
        new("city-transit-hub", "Intermodal Transit Hub", new SpatialBounds(67, 52, 14, 11), SpatialCityStructureKind.Transport, "Future rail, pedestrian, autonomous shuttle, and regional transit interchange."),
        new("city-residential-east", "East Residential Quarter", new SpatialBounds(84, 57, 12, 18), SpatialCityStructureKind.Residential, "Future residential neighborhood with apartments, townhomes, parks, and schools."),
        new("city-science-park", "Science Park", new SpatialBounds(5, 68, 18, 12), SpatialCityStructureKind.Science, "Future research campus for advanced materials, life sciences, physics, and engineering."),
        new("city-stadium", "Stadium District", new SpatialBounds(27, 68, 16, 13), SpatialCityStructureKind.Culture, "Future stadium and public-event district with plazas and support facilities."),
        new("city-port", "Deepwater Port", new SpatialBounds(75, 78, 20, 13), SpatialCityStructureKind.Port, "Future port for cargo, passenger vessels, marine industry, and waterfront commerce."),
        new("city-museum", "Museum District", new SpatialBounds(45, 68, 11, 9), SpatialCityStructureKind.Culture, "Future museums, galleries, archives, and public exhibition halls."),
        new("city-utility", "Utility Works", new SpatialBounds(58, 80, 13, 10), SpatialCityStructureKind.Utility, "Future water, energy, waste, and district utility infrastructure."),
        new("city-park-west", "Western Grand Park", new SpatialBounds(4, 83, 22, 10), SpatialCityStructureKind.Park, "Future metropolitan parkland with trails, gardens, recreation, and ecological reserve."),
        new("city-park-east", "Eastern Greenbelt", new SpatialBounds(81, 4, 15, 12), SpatialCityStructureKind.Park, "Future greenbelt separating the urban core from the outer development frontier.")
    ];
}

public readonly record struct SpatialCityStructure(
    string Id,
    string Name,
    SpatialBounds Bounds,
    SpatialCityStructureKind Kind,
    string FutureFunction);

public enum SpatialCityStructureKind
{
    Civic,
    Medical,
    Education,
    Commerce,
    Residential,
    Culture,
    Hospitality,
    Transport,
    Industry,
    Technology,
    Science,
    Port,
    Utility,
    Park
}
