namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>Declarative solar-system-scale map for Explore. The map is schematic rather than physically scaled so traffic and destinations remain legible.</summary>
public sealed record SpatialSolarSystemManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialCelestialBody> Bodies,
    IReadOnlyList<SpatialOrbitalFacility> Facilities,
    IReadOnlyList<SpatialSpacecraftActivity> Activities)
{
    public static SpatialSolarSystemManifest CreateDefault()
        => new(
            "solar-system",
            "SOLAR SYSTEM",
            [
                new("sol", "SOL", SpatialCelestialBodyKind.Star, new SpatialPoint(50, 50), 8),
                new("mercury", "MERCURY", SpatialCelestialBodyKind.Planet, new SpatialPoint(56, 50), 1.2),
                new("venus", "VENUS", SpatialCelestialBodyKind.Planet, new SpatialPoint(62, 50), 1.8),
                new("earth", "EARTH", SpatialCelestialBodyKind.Planet, new SpatialPoint(69, 50), 2),
                new("mars", "MARS", SpatialCelestialBodyKind.Planet, new SpatialPoint(76, 50), 1.4),
                new("asteroid-belt", "ASTEROID BELT", SpatialCelestialBodyKind.Belt, new SpatialPoint(82, 50), 2),
                new("jupiter", "JUPITER", SpatialCelestialBodyKind.Planet, new SpatialPoint(12, 50), 4.5),
                new("saturn", "SATURN", SpatialCelestialBodyKind.Planet, new SpatialPoint(24, 50), 3.8),
                new("uranus", "URANUS", SpatialCelestialBodyKind.Planet, new SpatialPoint(34, 50), 2.5),
                new("neptune", "NEPTUNE", SpatialCelestialBodyKind.Planet, new SpatialPoint(42, 50), 2.4)
            ],
            [
                new("singularity-station", "SINGULARITY STATION", "earth", SpatialPointKind.Orbit, new SpatialPoint(69, 43), "Passenger transfer, orbital operations, and Workshop access."),
                new("singularity-orbital-shipyard", "SINGULARITY ORBITAL SHIPYARD", "earth", SpatialPointKind.Orbit, new SpatialPoint(69, 57), "Vessel construction, repair, and ship systems.")
            ],
            [
                new("freighter-01", "DAEDALUS FREIGHTER", SpatialSpacecraftKind.Freighter, "earth", "mars", 18, "Cargo run"),
                new("freighter-02", "OUTER RIM FREIGHTER", SpatialSpacecraftKind.Freighter, "mars", "jupiter", 11, "Bulk freight"),
                new("passenger-01", "SINGULARITY LINER", SpatialSpacecraftKind.PassengerShip, "earth", "mars", 26, "Passenger service"),
                new("passenger-02", "ORBITAL FERRY", SpatialSpacecraftKind.PassengerShip, "earth", "singularity-station", 7, "Station transfer"),
                new("yacht-01", "PERSONAL YACHT ALPHA", SpatialSpacecraftKind.PersonalYacht, "earth", "mars", 4, "Private voyage"),
                new("yacht-02", "PERSONAL YACHT BETA", SpatialSpacecraftKind.PersonalYacht, "earth", "asteroid-belt", 3, "Exploration"),
                new("shuttle-01", "WORKSHOP SHUTTLE", SpatialSpacecraftKind.Shuttle, "singularity-station", "singularity-orbital-shipyard", 2, "Shipyard transfer"),
                new("survey-01", "DEEP SPACE SURVEYOR", SpatialSpacecraftKind.SurveyCraft, "asteroid-belt", "jupiter", 6, "Survey mission")
            ]);

    public IReadOnlyList<SpatialSpacecraftActivity> ActivitiesOf(SpatialSpacecraftKind kind)
        => Activities.Where(x => x.Kind == kind).ToArray();
}

public readonly record struct SpatialCelestialBody(string Id, string Name, SpatialCelestialBodyKind Kind, SpatialPoint Position, double DisplayRadius);
public enum SpatialCelestialBodyKind { Star, Planet, DwarfPlanet, Moon, Belt }

public readonly record struct SpatialOrbitalFacility(
    string Id, string Name, string ParentBodyId, SpatialPointKind LocationKind, SpatialPoint Position, string Purpose);

public enum SpatialPointKind { Surface, Orbit, DeepSpace }

public readonly record struct SpatialSpacecraftActivity(
    string Id, string Name, SpatialSpacecraftKind Kind, string OriginId, string DestinationId, int CrewOrPassengers, string Activity);

public enum SpatialSpacecraftKind { Freighter, PassengerShip, PersonalYacht, Shuttle, SurveyCraft }
