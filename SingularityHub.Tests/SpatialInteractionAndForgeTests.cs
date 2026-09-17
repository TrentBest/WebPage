using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialInteractionAndForgeTests
{
    [Fact(DisplayName = "Incremental Unit Test 28 — Bounding box rejects points before shape evaluation")]
    public void InteractableBoundingBoxRejectsOutsidePoint()
    {
        var interactable = new SpatialInteractable(
            "triangle",
            "Triangle",
            new SpatialBounds(10, 10, 20, 20),
            new SpatialBounds(18, 30, 4, 3),
            new SpatialPolygonHitRegion([
                new NormalizedPointer(.5, 0),
                new NormalizedPointer(1, 1),
                new NormalizedPointer(0, 1)
            ]),
            "triangle-room");

        Assert.False(interactable.HitTest(new SpatialPoint(31, 20)));
    }

    [Fact(DisplayName = "Incremental Unit Test 28 — Non-rectangular interactable accepts a point inside its shape")]
    public void PolygonInteractableAcceptsInteriorPoint()
    {
        var interactable = new SpatialInteractable(
            "triangle",
            "Triangle",
            new SpatialBounds(10, 10, 20, 20),
            new SpatialBounds(18, 30, 4, 3),
            new SpatialPolygonHitRegion([
                new NormalizedPointer(.5, 0),
                new NormalizedPointer(1, 1),
                new NormalizedPointer(0, 1)
            ]),
            "triangle-room");

        Assert.True(interactable.HitTest(new SpatialPoint(20, 25)));
        Assert.False(interactable.HitTest(new SpatialPoint(12, 12)));
    }

    [Fact(DisplayName = "Incremental Unit Test 28 — Spatial navigation exposes an explicit entrance destination")]
    public void NavigationMovesToEntrance()
    {
        using var navigation = new SpatialNavigationMicroBundle();

        navigation.MoveToEntrance(new SpatialPoint(73, 42));

        Assert.Equal(new SpatialPoint(73, 42), new SpatialPoint(navigation.X, navigation.Y));
    }

    [Fact(DisplayName = "Incremental Unit Test 28 — WorkshopRouter identifies the first visit")]
    public void RouterReportsFirstVisitOnce()
    {
        var router = new WorkshopRouter();

        Assert.True(router.Enter("forge", new SpatialPoint(50, 50)));
        Assert.False(router.Enter("forge", new SpatialPoint(50, 50)));
        Assert.True(router.HasVisited("forge"));
    }
}
