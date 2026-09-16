using System;
using System.Collections.Generic;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// MicroBundle for a construction vehicle. A vehicle is a live data actor;
/// its visual manifestation is merely one consumer of this state.
/// </summary>
public sealed class ConstructionVehicleMicroBundle : IDisposable
{
    private readonly MicroBundle _lifecycle;
    private readonly Dictionary<string, string> _subsystemStatus = new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;

    public ConstructionVehicleMicroBundle(int bundleId, string name, ConstructionVehicleKind kind)
    {
        if (bundleId <= 0) throw new ArgumentOutOfRangeException(nameof(bundleId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A vehicle name is required.", nameof(name));

        _lifecycle = new MicroBundle(bundleId, $"CONSTRUCTION VEHICLE // {name.ToUpperInvariant()}", new WebMicroBundleProvider());
        Name = name;
        Kind = kind;
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public string Name { get; }
    public ConstructionVehicleKind Kind { get; }
    public VehicleOperatingStatus OperatingStatus { get; private set; } = VehicleOperatingStatus.Idle;
    public VehicleDriveStatus DriveStatus { get; private set; } = VehicleDriveStatus.Parked;
    public IReadOnlyDictionary<string, string> SubsystemStatus => _subsystemStatus;

    public void SetOperatingStatus(VehicleOperatingStatus status)
    {
        if (!_disposed) OperatingStatus = status;
    }

    public void SetDriveStatus(VehicleDriveStatus status)
    {
        if (!_disposed) DriveStatus = status;
    }

    public void SetSubsystemStatus(string subsystem, string status)
    {
        if (_disposed) return;
        if (string.IsNullOrWhiteSpace(subsystem)) throw new ArgumentException("A subsystem name is required.", nameof(subsystem));
        if (string.IsNullOrWhiteSpace(status)) throw new ArgumentException("A subsystem status is required.", nameof(status));
        _subsystemStatus[subsystem] = status;
    }

    public void Update() => _lifecycle.Update();

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }
}

/// <summary>Primary work state, independent of locomotion.</summary>
public enum VehicleOperatingStatus
{
    Empty,
    Full,
    Emptying,
    Loading,
    Operating,
    Maintenance,
    Idle
}

/// <summary>Locomotion state, independent of primary and secondary work.</summary>
public enum VehicleDriveStatus
{
    Parked,
    Driving,
    Arriving,
    Departing
}

/// <summary>Machine family. Each family can expose its own subsystem vocabulary.</summary>
public enum ConstructionVehicleKind
{
    DumpTruck,
    Backhoe,
    Crane,
    Rover,
    Lifter
}
