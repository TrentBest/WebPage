using System;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Provides the Workshop conference-room capability.
/// Presence, membership, and future remote participants belong here; the room
/// builder only presents the capability and invokes these operations.
/// </summary>
public sealed class ConferenceMicroBundle : IDisposable
{
    public const int BundleId = 2104;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public ConferenceMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "WORKSHOP CONFERENCE", new WebMicroBundleProvider());
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public bool Joined { get; private set; }

    /// <summary>Current local participant count; remote presence can extend this without changing the room.</summary>
    public int ParticipantCount => Joined ? 1 : 0;

    public void Join()
    {
        if (_disposed) return;
        Joined = true;
    }

    public void Leave()
    {
        if (_disposed) return;
        Joined = false;
    }

    public void Update() => _lifecycle.Update();

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        Joined = false;
        _lifecycle.Dispose();
        _disposed = true;
    }
}
