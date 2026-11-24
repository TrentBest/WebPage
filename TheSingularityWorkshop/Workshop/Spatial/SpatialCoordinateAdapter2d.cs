#if WORKSHOP_USE_REPOSITORY_SOURCES
using TheSingularityWorkshop.Physics.Spatial;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Explicit conversion boundary between legacy Workshop spatial records and Physics-owned
/// frame-aware coordinates. The caller must choose the frame; this adapter never guesses it.
/// </summary>
public static class SpatialCoordinateAdapter2d
{
    /// <summary>Wraps Workshop coordinates in the caller-selected Physics coordinate frame.</summary>
    public static Point2d ToPhysicsPoint(SpatialPoint point, CoordinateFrame2d frame)
        => new(frame, new Vector2d(point.X, point.Y));

    /// <summary>Converts a Physics point back only when its frame matches the expected frame.</summary>
    public static SpatialPoint ToWorkshopPoint(Point2d point, CoordinateFrame2d expectedFrame)
    {
        point.RequireSameFrame(new Point2d(expectedFrame, point.Coordinates));
        return new SpatialPoint(point.Coordinates.X, point.Coordinates.Y);
    }

    /// <summary>Applies an explicit Physics transform and returns a point in its destination frame.</summary>
    public static Point2d Transform(Point2d point, RigidTransform2d transform)
        => transform.Transform(point);
}
#endif
