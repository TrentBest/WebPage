using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialZoomTests
{
    [Fact(DisplayName = "Incremental Unit Test 48 — Spatial camera zoom preserves avatar centering")]
    public void ZoomPreservesAvatarCentering()
    {
        var camera = new SpatialCamera(50, 50, 2d);

        Assert.Equal(50d - 50d * SpatialCamera.WorldUnitWidthVw * 2d, camera.OffsetVw);
        Assert.Equal(50d - 50d * SpatialCamera.WorldUnitHeightVh * 2d, camera.OffsetVh);
    }

    [Fact(DisplayName = "Incremental Unit Test 49 — Spatial camera clamps perception zoom")]
    public void ZoomIsClampedToSupportedRange()
    {
        Assert.Equal(SpatialCamera.MinZoom, new SpatialCamera(50, 50, 0.01).Clamped().Zoom);
        Assert.Equal(SpatialCamera.MaxZoom, new SpatialCamera(50, 50, 99).Clamped().Zoom);
    }

    [Fact(DisplayName = "Incremental Unit Test 50 — Floorplans explicitly expose scroll zoom policy")]
    public void FloorplansExposeZoomPolicy()
    {
        Assert.True(SpatialViewSettings.CreateDefault().ZoomEnabled);
        Assert.False(SpatialViewSettings.CreateDefault(false).ZoomEnabled);
    }
}
