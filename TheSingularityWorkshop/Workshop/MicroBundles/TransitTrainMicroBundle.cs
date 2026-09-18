using System;
using System.Collections.Generic;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Data-bearing train MicroBundle for the Workshop transit sandbox.
/// The physical model is renderer-neutral: a host can present the same train as
/// a blueprint, an HO-scale model, a first-person cab, or a full-size train.
/// </summary>
public sealed class TransitTrainMicroBundle : IDisposable
{
    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public TransitTrainMicroBundle(int bundleId, TransitTrainSpecification specification)
    {
        if (bundleId <= 0) throw new ArgumentOutOfRangeException(nameof(bundleId));
        ArgumentNullException.ThrowIfNull(specification);

        _lifecycle = new MicroBundle(bundleId, $"TRANSIT TRAIN // {specification.Id.ToUpperInvariant()}", new WebMicroBundleProvider());
        Specification = specification;
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public TransitTrainSpecification Specification { get; }
    public TransitTrainOperatingState State { get; private set; } = TransitTrainOperatingState.Stabled;
    public double RouteProgress { get; private set; }

    public void SetState(TransitTrainOperatingState state)
    {
        if (!_disposed) State = state;
    }

    public void SetRouteProgress(double progress)
    {
        if (!_disposed) RouteProgress = Math.Clamp(progress, 0d, 1d);
    }

    public void Update() => _lifecycle.Update();

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }
}

/// <summary>Renderer-neutral specification of a physical trainset.</summary>
public sealed record TransitTrainSpecification(
    string Id,
    string Name,
    string Operator,
    int CarCount,
    double PrototypeCarLengthMeters,
    double PrototypeCarWidthMeters,
    double PrototypeCarHeightMeters,
    IReadOnlyList<TransitTrainCar> Cars,
    IReadOnlyList<string> Capabilities)
{
    /// <summary>NMRA HO scale ratio. 1 model unit represents 87.1 prototype units.</summary>
    public const double HoScaleRatio = 87.1;

    /// <summary>Standard HO track gauge from the NMRA standard.</summary>
    public const double HoTrackGaugeMillimeters = 16.54;

    public double ModelCarLengthMillimeters => PrototypeCarLengthMeters * 1000d / HoScaleRatio;
    public double ModelCarWidthMillimeters => PrototypeCarWidthMeters * 1000d / HoScaleRatio;
    public double ModelCarHeightMillimeters => PrototypeCarHeightMeters * 1000d / HoScaleRatio;

    public static TransitTrainSpecification WorkshopHoEmu()
        => new(
            "workshop-ho-emu",
            "WORKSHOP HO ELECTRIC MULTIPLE UNIT",
            "SINGULARITY TRANSIT",
            8,
            25.9,
            3.2,
            4.0,
            [
                new TransitTrainCar(1, "CAB", true, true),
                new TransitTrainCar(2, "PASSENGER", false, false),
                new TransitTrainCar(3, "PASSENGER", false, false),
                new TransitTrainCar(4, "PASSENGER", false, false),
                new TransitTrainCar(5, "PASSENGER", false, false),
                new TransitTrainCar(6, "PASSENGER", false, false),
                new TransitTrainCar(7, "PASSENGER", false, false),
                new TransitTrainCar(8, "CAB", true, true)
            ],
            ["passenger-service", "cab-control", "door-control", "dispatch", "camera-view", "museum-display"]);
}

public readonly record struct TransitTrainCar(int Number, string Kind, bool Powered, bool Cab);

public enum TransitTrainOperatingState
{
    Stabled,
    Boarding,
    Departing,
    EnRoute,
    Arriving,
    Unloading,
    MuseumDisplay
}
