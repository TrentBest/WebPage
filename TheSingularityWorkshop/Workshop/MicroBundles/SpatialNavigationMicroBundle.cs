using System;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Provides the spatial locomotion capability consumed by an Experience.
/// The presentation layer never owns the visitor's position; it renders the
/// coordinates exposed by this MicroBundle.
/// </summary>
public sealed class SpatialNavigationMicroBundle : IDisposable
{
    public const int BundleId = 2101;
    public const double WorldCenterX = 50;
    public const double WorldCenterY = 50;
    public const double DefaultMovementStep = 1.5;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public SpatialNavigationMicroBundle(double startX = WorldCenterX, double startY = WorldCenterY)
    {
        _lifecycle = new MicroBundle(BundleId, "SPATIAL NAVIGATION", new WebMicroBundleProvider());
        X = ClampX(startX);
        Y = ClampY(startY);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public double X { get; private set; }
    public double Y { get; private set; }

    public void Move(double dx, double dy)
    {
        if (_disposed) return;
        X = ClampX(X + dx);
        Y = ClampY(Y + dy);
    }

    public void MoveTo(double x, double y)
    {
        if (_disposed) return;
        X = ClampX(x);
        Y = ClampY(y);
    }

    public void Approach(double x, double y, double offsetY = 6)
    {
        if (_disposed) return;
        MoveTo(x, y + offsetY);
    }

    public void ApproachEntrance(SpatialRoom room, double offset = 0)
    {
        if (_disposed) return;
        MoveTo(room.EntranceX, room.EntranceY + offset);
    }

    public void MoveToExit(SpatialRoom room)
    {
        if (_disposed) return;
        MoveTo(room.ExitX, room.ExitY);
    }

    public void Update() => _lifecycle.Update();

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }

    private static double ClampX(double value) => Math.Clamp(value, 4, 96);
    private static double ClampY(double value) => Math.Clamp(value, 8, 92);
}
