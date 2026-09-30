using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Owns the Workshop's presence boundary independently of the spatial renderer.
/// The first implementation is deterministic simulated presence; a connected
/// transport can replace the participant source without changing the Experience
/// or GUI contract.
/// </summary>
public sealed class WorkshopPresenceMicroBundle : IDisposable
{
    public const int BundleId = 2110;

    private readonly MicroBundle _lifecycle = new(BundleId, "WORKSHOP PRESENCE", new WebMicroBundleProvider());
    private readonly List<WorkshopPresenceParticipant> _participants =
    [
        new("visitor-01", "Workshop Visitor", 28, 34, WorkshopPresenceKind.Simulated),
        new("visitor-02", "Forge Explorer", 70, 58, WorkshopPresenceKind.Simulated),
        new("visitor-03", "Lab Researcher", 54, 78, WorkshopPresenceKind.Simulated),
        new("visitor-04", "Builder", 34, 66, WorkshopPresenceKind.Simulated)
    ];

    private bool _disposed;

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    /// <summary>Describes where the current participant snapshot came from.</summary>
    public WorkshopPresenceSource Source { get; private set; } = WorkshopPresenceSource.Simulated;

    /// <summary>Remote participants currently visible in the shared Workshop world.</summary>
    public IReadOnlyList<WorkshopPresenceParticipant> Participants => _participants;

    /// <summary>Replaces the current snapshot with a host-provided connected snapshot.</summary>
    public void SetConnectedParticipants(IEnumerable<WorkshopPresenceParticipant> participants)
    {
        if (_disposed) return;
        ArgumentNullException.ThrowIfNull(participants);

        _participants.Clear();
        _participants.AddRange(participants.Where(p => p.Kind == WorkshopPresenceKind.Connected));
        Source = WorkshopPresenceSource.Connected;
    }

    /// <summary>Restores deterministic local presence for the development/demo surface.</summary>
    public void ResetSimulation()
    {
        if (_disposed) return;

        _participants.Clear();
        _participants.AddRange(
        [
            new("visitor-01", "Workshop Visitor", 28, 34, WorkshopPresenceKind.Simulated),
            new("visitor-02", "Forge Explorer", 70, 58, WorkshopPresenceKind.Simulated),
            new("visitor-03", "Lab Researcher", 54, 78, WorkshopPresenceKind.Simulated),
            new("visitor-04", "Builder", 34, 66, WorkshopPresenceKind.Simulated)
        ]);
        Source = WorkshopPresenceSource.Simulated;
    }

    public void Update() => _lifecycle.Update();

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }
}

/// <summary>Describes the provenance of a presence snapshot.</summary>
public enum WorkshopPresenceSource
{
    Simulated,
    Connected
}

/// <summary>A visitor occupying the shared Workshop world.</summary>
public sealed record WorkshopPresenceParticipant(
    string Id,
    string DisplayName,
    double X,
    double Y,
    WorkshopPresenceKind Kind);

public enum WorkshopPresenceKind
{
    Simulated,
    Connected
}
