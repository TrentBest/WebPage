namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Declarative foundation for the Singularity Taxi service: a Crazy Taxi-inspired
/// transportation Experience set inside a large science-fiction city.
/// </summary>
/// <remarks>
/// The service separates the passenger contract from the vehicle fleet and from
/// the city itself. A passenger may ride through the city for the full visual
/// journey or skip directly to the destination. The ride is therefore an
/// Experience, not a loading screen.
/// </remarks>
public sealed record SpatialTaxiManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialTaxiVehicleModel> Fleet,
    IReadOnlyList<SpatialTaxiDestination> Destinations,
    IReadOnlyList<SpatialTaxiPassenger> Passengers,
    SpatialTaxiRideRules Rules)
{
    public static SpatialTaxiManifest CreateDefault()
        => new(
            "singularity-taxi",
            "SINGULARITY TAXI",
            [
                new("hover-cab", "HOVER CAB", "CITY", 1, 1),
                new("aero-limo", "AERO LIMO", "CITY", 2, 2),
                new("orbital-shuttle", "ORBITAL SHUTTLE", "CITY → STATION", 3, 4),
                new("neon-runner", "NEON RUNNER", "CITY", 1, 1)
            ],
            [
                new("city-center", "SINGULARITY CITY CENTER", new SpatialPoint(50, 50)),
                new("capitol", "SINGULARITY CAPITOL", new SpatialPoint(62, 17)),
                new("space-elevator", "SPACE ELEVATOR", new SpatialPoint(50, 78)),
                new("station", "SINGULARITY STATION", new SpatialPoint(76, 17)),
                new("ocean-terminal", "OCEAN TERMINAL", new SpatialPoint(8, 76))
            ],
            [new("demo-passenger-01", "VISITOR", "city-center", "station")],
            new SpatialTaxiRideRules(true, true, true, true));

    public SpatialTaxiDestination? FindDestination(string id)
        => Destinations.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

    public SpatialTaxiPassenger? FindPassenger(string id)
        => Passengers.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
}

public readonly record struct SpatialTaxiVehicleModel(string Id, string Name, string OperatingDomain, int PassengerCapacity, int VisualTier);
public readonly record struct SpatialTaxiDestination(string Id, string Name, SpatialPoint Position);
public readonly record struct SpatialTaxiPassenger(string Id, string DisplayName, string OriginDestinationId, string DestinationId);
public readonly record struct SpatialTaxiRideRules(bool RideIsOptional, bool InstantDestinationAvailable, bool PassengerMaySkipRide, bool RideCountsAsExperience);
