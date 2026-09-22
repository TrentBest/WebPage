namespace TheSingularityWorkshop.Workshop.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines the distance bands at which visual content may be rendered at different rates.
/// The user-facing perception point receives the finest update budget; distant content is
/// allowed to remain buffered until movement or elapsed time makes another update worthwhile.
/// </summary>
public sealed class RenderingPerceptionModel
{
    private readonly IReadOnlyList<RenderingPerceptionBand> _bands;

    /// <summary>Creates the Workshop's initial event-horizon bands.</summary>
    public RenderingPerceptionModel()
    {
        _bands =
        [
            new("CONTACT", 0d, 1d, TimeSpan.FromMilliseconds(16)),
            new("REACH", 1d, 2d, TimeSpan.FromMilliseconds(33)),
            new("CLOSE", 2d, 4d, TimeSpan.FromMilliseconds(66)),
            new("MEDIUM", 4d, 8d, TimeSpan.FromMilliseconds(132)),
            new("FAR", 8d, 16d, TimeSpan.FromMilliseconds(264)),
            new("HORIZON", 16d, double.PositiveInfinity, TimeSpan.FromMilliseconds(528))
        ];
    }

    /// <summary>Gets the configured perception bands from nearest to farthest.</summary>
    public IReadOnlyList<RenderingPerceptionBand> Bands => _bands;

    /// <summary>Finds the event-horizon band containing the supplied distance.</summary>
    public RenderingPerceptionBand Resolve(double distance)
    {
        if (double.IsNaN(distance) || distance < 0d)
            throw new ArgumentOutOfRangeException(nameof(distance));

        return _bands.First(band => distance >= band.MinimumDistance && distance < band.MaximumDistance);
    }

    /// <summary>
    /// Determines whether a buffered distant representation needs another update.
    /// Distance travelled is included so a moving observer can invalidate a horizon buffer
    /// without forcing every far object to update every frame.
    /// </summary>
    public bool ShouldRefresh(double distance, TimeSpan sinceLastUpdate, double distanceTravelled)
    {
        var band = Resolve(distance);
        return sinceLastUpdate >= band.UpdateInterval || distanceTravelled >= band.ReframeDistance;
    }
}

/// <summary>One event-horizon band in the rendering perception model.</summary>
public readonly record struct RenderingPerceptionBand(
    string Name,
    double MinimumDistance,
    double MaximumDistance,
    TimeSpan UpdateInterval)
{
    /// <summary>Gets the observer movement that invalidates a buffered representation.</summary>
    public double ReframeDistance =>
        double.IsPositiveInfinity(MaximumDistance)
            ? 8d
            : Math.Max(.25d, (MaximumDistance - MinimumDistance) * .5d);
}
