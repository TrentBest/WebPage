using TheSingularityWorkshop.Workshop.Interaction;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Owns the visual manifestation of one wayfinding route.
/// The route remains domain data; this class is the current web/SVG renderer.
/// A future renderer can consume the same route and presentation intent without
/// changing the interactable or pathfinding contracts.
/// </summary>
public static class WorkshopMapLineRenderer
{
    public static ElementBuilder Build(
        object receiver,
        IReadOnlyList<SpatialWaypoint> route,
        double avatarX,
        double avatarY,
        InteractableMapPresentation presentation)
    {
        ArgumentNullException.ThrowIfNull(route);
        ArgumentNullException.ThrowIfNull(presentation);

        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("pointer-events", "none");

        var points = route.Select(point =>
            $"{50 + (point.X - avatarX) * .92:0.##},{50 + (point.Y - avatarY) * .92:0.##}");

        svg.Child(WorkshopGui.Element(receiver, "polyline")
            .Attribute("points", string.Join(" ", points))
            .Attribute("fill", "none")
            .Attribute("stroke", presentation.Accent)
            .Attribute("stroke-opacity", presentation.LineOpacity.ToString("0.##"))
            .Attribute("stroke-width", presentation.LineWidth.ToString("0.##"))
            .Attribute("stroke-linecap", "round")
            .Attribute("stroke-linejoin", "round"));

        var end = route[^1];
        var endX = 50 + (end.X - avatarX) * .92;
        var endY = 50 + (end.Y - avatarY) * .92;
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", endX.ToString("0.##"))
            .Attribute("cy", endY.ToString("0.##"))
            .Attribute("r", "1.8")
            .Attribute("fill", "#01040a")
            .Attribute("stroke", presentation.Accent)
            .Attribute("stroke-width", ".7"));

        return svg;
    }
}
