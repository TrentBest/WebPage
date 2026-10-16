namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>Deterministic, renderer-independent routes for the city's aerial traffic layer.</summary>
/// <remarks>
/// This is a repeatable route/presentation model, not collision avoidance, flight dynamics,
/// dispatch, or an autonomous vehicle simulation. A host supplies elapsed simulation time.
/// Coordinates use the normalized city-plan space shared by other spatial manifests.
/// </remarks>
public sealed record SpatialAirTrafficManifest(
    string Id,
    string Name,
    IReadOnlyList<SpatialAirTrafficVehicle> Vehicles)
{
    public static SpatialAirTrafficManifest CreateDefault()
        => new(
            "singularity-air-traffic",
            "SINGULARITY AERIAL TRAFFIC",
            [
                new("air-vehicle-001", "hover-cab", "northbound-corridor",
                    new SpatialPoint(8, 22), new SpatialPoint(92, 30), 1, 42, 0),
                new("air-vehicle-002", "aero-limo", "eastbound-corridor",
                    new SpatialPoint(18, 14), new SpatialPoint(82, 20), 2, 56, 9),
                new("air-vehicle-003", "neon-runner", "westbound-corridor",
                    new SpatialPoint(94, 43), new SpatialPoint(6, 49), 1, 36, 17),
                new("air-vehicle-004", "orbital-shuttle", "high-transit-corridor",
                    new SpatialPoint(12, 70), new SpatialPoint(88, 76), 3, 72, 25)
            ]);

    /// <summary>
    /// Creates the denser city-scale sky traffic used by the living-world map.
    /// The compact default remains available for focused tests and small scenes.
    /// These are deterministic presentation routes, not flight-control simulation.
    /// </summary>
    public static SpatialAirTrafficManifest CreateCityTraffic()
    {
        var baseline = CreateDefault();
        return baseline with
        {
            Vehicles = baseline.Vehicles.Concat(
            [
                new SpatialAirTrafficVehicle("air-vehicle-005", "hover-cab", "central-express-lane",
                    new SpatialPoint(4, 34), new SpatialPoint(96, 38), 2, 48, 4),
                new SpatialAirTrafficVehicle("air-vehicle-006", "aero-limo", "upper-east-corridor",
                    new SpatialPoint(90, 8), new SpatialPoint(10, 16), 3, 64, 13),
                new SpatialAirTrafficVehicle("air-vehicle-007", "neon-runner", "market-shuttle-lane",
                    new SpatialPoint(12, 24), new SpatialPoint(78, 28), 1, 34, 7),
                new SpatialAirTrafficVehicle("air-vehicle-008", "orbital-shuttle", "station-approach-lane",
                    new SpatialPoint(8, 82), new SpatialPoint(92, 72), 4, 80, 31),
                new SpatialAirTrafficVehicle("air-vehicle-009", "hover-cab", "port-connector-lane",
                    new SpatialPoint(94, 88), new SpatialPoint(26, 58), 2, 58, 19),
                new SpatialAirTrafficVehicle("air-vehicle-010", "aero-limo", "university-connector-lane",
                    new SpatialPoint(6, 6), new SpatialPoint(70, 44), 3, 66, 22),
                new SpatialAirTrafficVehicle("air-vehicle-011", "neon-runner", "arena-event-lane",
                    new SpatialPoint(96, 62), new SpatialPoint(42, 84), 1, 38, 11),
                new SpatialAirTrafficVehicle("air-vehicle-012", "orbital-shuttle", "intermodal-high-lane",
                    new SpatialPoint(22, 92), new SpatialPoint(84, 34), 4, 76, 37)
            ]).ToArray()
        };
    }

    /// <summary>Resolves a vehicle by stable identity, ignoring identifier casing.</summary>
    public SpatialAirTrafficVehicle? FindVehicle(string id)
        => Vehicles.FirstOrDefault(vehicle =>
            string.Equals(vehicle.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Produces repeatable position snapshots at elapsed time without starting a clock
    /// or mutating the manifest.
    /// </summary>
    public IReadOnlyList<SpatialAirTrafficSnapshot> SnapshotAt(double elapsedSeconds)
    {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), "Elapsed time must be finite and non-negative.");

        return Vehicles.Select(vehicle => vehicle.SnapshotAt(elapsedSeconds)).ToArray();
    }
}

/// <summary>A declared aerial route; altitude band is a discrete presentation lane, not a physical height.</summary>
public sealed record SpatialAirTrafficVehicle(
    string Id,
    string ModelId,
    string LaneId,
    SpatialPoint Origin,
    SpatialPoint Destination,
    int AltitudeBand,
    double TravelSeconds,
    double PhaseSeconds)
{
    /// <summary>Evaluates a deterministic out-and-back route without changing vehicle state.</summary>
    public SpatialAirTrafficSnapshot SnapshotAt(double elapsedSeconds)
    {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), "Elapsed time must be finite and non-negative.");
        if (string.IsNullOrWhiteSpace(Id))
            throw new InvalidOperationException("Aerial vehicles require a stable identity.");
        if (string.IsNullOrWhiteSpace(ModelId))
            throw new InvalidOperationException($"Aerial vehicle '{Id}' requires a model identity.");
        if (string.IsNullOrWhiteSpace(LaneId))
            throw new InvalidOperationException($"Aerial vehicle '{Id}' requires an altitude-lane identity.");
        if (AltitudeBand < 1)
            throw new InvalidOperationException($"Aerial vehicle '{Id}' must use an altitude band above ground traffic.");
        if (!double.IsFinite(TravelSeconds) || TravelSeconds <= 0)
            throw new InvalidOperationException($"Aerial vehicle '{Id}' requires a positive finite travel duration.");
        if (!double.IsFinite(PhaseSeconds) || PhaseSeconds < 0)
            throw new InvalidOperationException($"Aerial vehicle '{Id}' requires a finite non-negative phase offset.");

        var cyclePosition = (elapsedSeconds + PhaseSeconds) % (TravelSeconds * 2);
        var progress = cyclePosition <= TravelSeconds
            ? cyclePosition / TravelSeconds
            : 2 - (cyclePosition / TravelSeconds);
        var position = new SpatialPoint(
            Origin.X + ((Destination.X - Origin.X) * progress),
            Origin.Y + ((Destination.Y - Origin.Y) * progress));

        return new SpatialAirTrafficSnapshot(Id, ModelId, LaneId, AltitudeBand, progress, position);
    }
}

/// <summary>A disposable position sample derived from a declared route and elapsed time.</summary>
public readonly record struct SpatialAirTrafficSnapshot(
    string VehicleId,
    string ModelId,
    string LaneId,
    int AltitudeBand,
    double RouteProgress,
    SpatialPoint Position);
