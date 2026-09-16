using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Agents;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// MicroBundle for a construction vehicle. The vehicle is the authority for
/// what the machine is and what the machine can do; an operating Digitens
/// discovers this manifest instead of containing machine-specific knowledge.
/// Lifecycle remains owned by FSM_API through <see cref="MicroBundle"/>.
/// </summary>
public sealed class ConstructionVehicleMicroBundle : IDisposable, IVehicleCapabilitySource
{
    private readonly MicroBundle _lifecycle;
    private readonly Dictionary<string, string> _subsystemStatus = new(StringComparer.OrdinalIgnoreCase);
    private readonly IReadOnlyList<VehicleCapability> _capabilities;
    private bool _disposed;

    public ConstructionVehicleMicroBundle(int bundleId, string name, ConstructionMachineKind kind)
    {
        if (bundleId <= 0) throw new ArgumentOutOfRangeException(nameof(bundleId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A vehicle name is required.", nameof(name));

        _lifecycle = new MicroBundle(bundleId, $"CONSTRUCTION VEHICLE // {name.ToUpperInvariant()}", new WebMicroBundleProvider());
        Name = name;
        Kind = kind;
        _capabilities = BuildCapabilities(kind);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public string Name { get; }
    public ConstructionMachineKind Kind { get; }
    public VehicleOperatingStatus OperatingStatus { get; private set; } = VehicleOperatingStatus.Idle;
    public VehicleDriveStatus DriveStatus { get; private set; } = VehicleDriveStatus.Parked;
    public IReadOnlyDictionary<string, string> SubsystemStatus => _subsystemStatus;
    public IReadOnlyList<VehicleCapability> Capabilities => _capabilities;

    public bool CanPerform(string actionId)
        => !string.IsNullOrWhiteSpace(actionId) && _capabilities.SelectMany(c => c.Actions).Any(a => string.Equals(a.Id, actionId, StringComparison.OrdinalIgnoreCase));

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

    private static IReadOnlyList<VehicleCapability> BuildCapabilities(ConstructionMachineKind kind)
        => kind switch
        {
            ConstructionMachineKind.DumpTruck =>
            [
                Capability("haul", "Transport material", Action("load", "Load material", "bed"), Action("dump", "Empty the bed", "bed")),
                Capability("drive", "Move material between locations", Action("navigate", "Navigate to a destination"))
            ],
            ConstructionMachineKind.Backhoe =>
            [
                Capability("excavate", "Dig and remove material", Action("dig", "Lower and operate the bucket", "grounding", "boom", "stick", "bucket")),
                Capability("load", "Load excavated material", Action("scoop", "Scoop material into the bucket", "grounding", "boom", "stick", "bucket")),
                Capability("drive", "Move the machine between work sites", Action("navigate", "Navigate to a destination"))
            ],
            ConstructionMachineKind.Crane =>
            [
                Capability("lift", "Lift and position suspended loads", Action("lift", "Raise a load", "outrigger", "boom", "hook"), Action("lower", "Lower a load", "outrigger", "boom", "hook")),
                Capability("drive", "Move the crane between work sites", Action("navigate", "Navigate to a destination"))
            ],
            ConstructionMachineKind.Rover =>
            [
                Capability("survey", "Inspect and report on an area", Action("scan", "Scan the surrounding site", "sensor")),
                Capability("drive", "Move autonomously between locations", Action("navigate", "Navigate to a destination"))
            ],
            ConstructionMachineKind.Lifter =>
            [
                Capability("lift", "Raise and position material", Action("fork-lift", "Lift a pallet or load", "mast", "fork")),
                Capability("drive", "Move material between locations", Action("navigate", "Navigate to a destination"))
            ],
            _ => Array.Empty<VehicleCapability>()
        };

    private static VehicleCapability Capability(string id, string description, params VehicleAction[] actions)
        => new(id, description, actions);

    private static VehicleAction Action(string id, string description, params string[] requiredCapabilities)
        => new(id, description, requiredCapabilities);
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

/// <summary>Machine family used to select its subsystem vocabulary.</summary>
public enum ConstructionMachineKind
{
    DumpTruck,
    Backhoe,
    Crane,
    Rover,
    Lifter
}
