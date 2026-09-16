using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Incremental Unit Test 06 — spatial objects own hit testing and click semantics.
/// The renderer may change; these assertions describe the world contract it must preserve.
/// </summary>
public sealed class SpatialInteractionTests
{
    [Fact(DisplayName = "Incremental Unit Test 06 — interactables reject pointer and click misses")]
    public void Interactable_HitRegionGuardsHoverAndClick()
    {
        var item = new SpatialInteractable(
            "outlet",
            "Power Outlet",
            new SpatialBounds(10, 10, 8, 8),
            new SpatialBounds(13, 14, 2, 2),
            new SpatialRectangularHitRegion(.25, .25, .5, .5),
            "power-outlet");

        Assert.False(item.OnHover(new NormalizedPointer(.1, .1)));
        Assert.False(item.IsBreathing);
        Assert.False(item.OnClick(new NormalizedPointer(.1, .1)));

        Assert.True(item.OnHover(new NormalizedPointer(.5, .5)));
        Assert.True(item.IsBreathing);
        Assert.True(item.OnClick(new NormalizedPointer(.5, .5)));

        item.OnHoverExit();
        Assert.False(item.IsBreathing);
    }
}
