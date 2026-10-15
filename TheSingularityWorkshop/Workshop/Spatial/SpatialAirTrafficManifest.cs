namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Deterministic, renderer-independent routes for the city's aerial traffic layer.
/// </summary>
/// <remarks>
/// This is a repeatable presentation/model reference, not collision avoidance, flight
/// dynamics, dispatch, or an autonomous vehicle simulation. A host supplies elapsed
/// simulation time; consumers can inspect each vehicle's route and altitude band.
/// Coordinates use the same normalized city-plan space as other spatial manifests.
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
                new(
                    "air-vehicle-001",
                    "hover-cab",
                    "northbound-corridor",
                    new SpatialPoint(8, 22),
                    new SpatialPoint(92, 30),
                    altitudeBand: 1,
                    travelSeconds: 42,
                    phaseSeconds: 0),
                new(
                    "air-vehicle-002",
                    "aero-limo",
                    "eastbound-corridor",
                    new SpatialPoint(18, 14),
                    new SpatialPoint(82, 20),
                    altitudeBand: 2,
                    travelSeconds: 56,
                    phaseSeconds: 9),
                new(
                    "air-vehicle-003",
                    "neon-runner",
                    "westbound-corridor",
                    new SpatialPoint(94, 43),
                    new SpatialPoint(6, 49),
                    altitudeBand: 1,
                    travelSeconds: 36,
                    phaseSeconds: 17),
                new(
                    "air-vehicle-004",
                    "orbital-shuttle",
                    "high-transit-corridor",
                    new SpatialPoint(12, 70),
                    new SpatialPoint(88, 76),
                    altitudeBand: 3,
                    travelSeconds: 72,
                    phaseSeconds: 25)
            ]);

    /// <summary>Returns a route by stable vehicle identity, ignoring identifier casing.</summary>
    public SpatialAirTrafficVehicle? FindVehicle(string id)
        => Vehicles.FirstOrDefault(vehicle =>
            string.Equals(vehicle.Id, id, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Produces a repeatable position snapshot for all declared vehicles at the supplied
    /// elapsed time. This method does not start a clock or mutate the manifest.
    /// </summary>
    public IReadOnlyList<SpatialAirTrafficSnapshot> SnapshotAt(double elapsedSeconds)
    {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), "Elapsed time must be finite and non-negative.");

        return Vehicles
            .Select(vehicle => vehicle.SnapshotAt(elapsedSeconds))
            .ToArray();
    }
}

/// <summary>
/// A declared aerial route. AltitudeBand is a discrete presentation lane, not a physical altitude.
/// </summary>
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

        var cycleSeconds = TravelSeconds * 2;
        var cyclePosition = (elapsedSeconds + PhaseSeconds) % cycleSeconds;
        var progress = cyclePosition <= TravelSeconds
            ? cyclePosition / TravelSeconds
            : 2 - (cyclePosition / TravelSeconds);

        var position = new SpatialPoint(
            Origin.X + ((Destination.X - Origin.X) * progress),
            Origin.Y + ((Destination.Y - Origin.Y) * progress));

        return new SpatialAirTrafficSnapshot(Id, ModelId, LaneId, AltitudeBand, progress, position);
    }
}

/// <summary>A disposable aerial position sample derived from a declared route and time.</summary>
public readonly record struct SpatialAirTrafficSnapshot(
    string VehicleId,
    string ModelId,
    string LaneId,
    int AltitudeBand,
    double RouteProgress,
    SpatialPoint Position);
