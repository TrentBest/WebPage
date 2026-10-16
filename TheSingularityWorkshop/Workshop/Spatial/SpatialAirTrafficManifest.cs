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
