#if WORKSHOP_USE_REPOSITORY_SOURCES
using TheSingularityWorkshop.Physics.Spatial;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialCoordinateAdapter2dTests
{
    [Fact]
    public void ToPhysicsPoint_UsesTheExplicitCallerFrameAndPreservesCoordinates()
    {
        var frame = new CoordinateFrame2d("aec.plan.level-01.xy.v1");
        var source = new SpatialPoint(12.5, 8.25);

        var result = SpatialCoordinateAdapter2d.ToPhysicsPoint(source, frame);

        Assert.Equal(frame, result.Frame);
        Assert.Equal(new Vector2d(12.5, 8.25), result.Coordinates);
    }

    [Fact]
    public void ToWorkshopPoint_RejectsAnUnexpectedFrame()
    {
        var actual = new CoordinateFrame2d("plan.level-01");
        var expected = new CoordinateFrame2d("plan.level-02");
        var point = new Point2d(actual, new Vector2d(4, 9));

        Assert.Throws<InvalidOperationException>(() =>
            SpatialCoordinateAdapter2d.ToWorkshopPoint(point, expected));
    }

    [Fact]
    public void ExplicitTransformChangesFrameAndCoordinates()
    {
        var planFrame = new CoordinateFrame2d("plan.local");
        var worldFrame = new CoordinateFrame2d("campus.world");
        var source = new Point2d(planFrame, new Vector2d(2, 3));
        var transform = new RigidTransform2d(planFrame, worldFrame, 0, new Vector2d(10, -2));

        var result = SpatialCoordinateAdapter2d.Transform(source, transform);

        Assert.Equal(worldFrame, result.Frame);
        Assert.Equal(new Vector2d(12, 1), result.Coordinates);
    }

    [Fact]
    public void ToPhysicsPoint_RejectsNonFiniteLegacyCoordinates()
    {
        var frame = new CoordinateFrame2d("plan.local");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SpatialCoordinateAdapter2d.ToPhysicsPoint(new SpatialPoint(double.NaN, 0), frame));
    }
}
#endif
