using System;
using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>Provides spatial locomotion independently of the perception layer.</summary>
public sealed class SpatialNavigationMicroBundle(double startX = 50, double startY = 50) : IDisposable
{
    public const int BundleId = 2101;
    public const double WorldCenterX = 50;
    public const double WorldCenterY = 50;

    private readonly MicroBundle _lifecycle = new(BundleId, "SPATIAL NAVIGATION", new WebMicroBundleProvider());
    private bool _disposed;

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public double X { get; private set; } = ClampX(startX);
    public double Y { get; private set; } = ClampY(startY);

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

    /// <summary>Moves the avatar directly to a world-space entrance selected by the scene.</summary>
    public void MoveToEntrance(SpatialPoint entrance)
    {
        if (_disposed) return;
        MoveTo(entrance.X, entrance.Y);
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
