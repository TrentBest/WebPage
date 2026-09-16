using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.Interaction;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Spatial wayfinding instrument. This is an overlay, not a second Workshop
/// reality: the visitor stays at the center while destinations become radial
/// data pills and their actual pathfinding routes are drawn across the surface.
/// </summary>
public static class WorkshopMapGuiBuilder
{
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder MapButton(object receiver, bool open, Action toggle)
        => WorkshopGui.Button(receiver)
            .Label(open ? "CLOSE MAP" : "MAP")
            .Style("position", "fixed").Style("left", "50%").Style("top", "calc(50% + 42px)")
            .Style("transform", "translateX(-50%)").Style("z-index", "85")
            .Style("padding", ".42rem .8rem").Style("border", "1px solid rgba(255,255,255,.5)")
            .Style("background", "rgba(1,4,10,.94)").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace").Style("font-size", ".48rem")
            .Style("letter-spacing", ".16em").Style("cursor", "pointer")
            .Style("box-shadow", "0 0 20px rgba(255,255,255,.16)")
            .AriaLabel(open ? "Close Workshop map" : "Open Workshop map").OnClick(toggle);

    public static ElementBuilder Build(
        object receiver,
        IReadOnlyList<ExperienceSpace> destinations,
        IReadOnlyDictionary<string, IInteractable> interactables,
        double avatarX,
        double avatarY,
        IReadOnlyDictionary<string, IReadOnlyList<SpatialWaypoint>> routes,
        Func<string, Task> selectDestination,
        Action close)
    {
        ArgumentNullException.ThrowIfNull(destinations);
        ArgumentNullException.ThrowIfNull(interactables);
        ArgumentNullException.ThrowIfNull(routes);
        ArgumentNullException.ThrowIfNull(selectDestination);
        ArgumentNullException.ThrowIfNull(close);

        var root = WorkshopGui.Element(receiver, "section")
            .Style("position", "fixed").Style("inset", "0").Style("z-index", "90")
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Style("background", "rgba(0,0,0,.58)")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Attribute("role", "dialog").Attribute("aria-modal", "true")
            .Attribute("aria-label", "Workshop spatial wayfinding map");

        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "relative").Style("width", "min(920px, 96vw)")
            .Style("height", "min(680px, 88vh)").Style("overflow", "hidden")
            .Style("box-sizing", "border-box").Style("border", "1px solid rgba(255,255,255,.28)")
            .Style("background", "rgba(1,4,10,.96)").Style("box-shadow", "0 0 90px rgba(0,0,0,.7)");

        panel.Content(Header(receiver, close));
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("pointer-events", "none")
            .Content(RouteLines(receiver, destinations, interactables, avatarX, avatarY, routes)));
        panel.Content(Visitor(receiver));

        foreach (var destination in destinations)
        {
            if (!interactables.TryGetValue(destination.Id, out var interactable))
                continue;

            var presentation = interactable.Presentation.MapIntent;
            var dx = destination.X - avatarX;
            var dy = destination.Y - avatarY;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            var radial = Math.Clamp(27 + distance * .32, 30, 42);
            var left = 50 + Math.Cos(angle * Math.PI / 180) * radial;
            var top = 50 + Math.Sin(angle * Math.PI / 180) * radial;
            panel.Content(DataPill(receiver, destination, presentation, left, top, selectDestination));
        }

        panel.Content(Legend(receiver));
        root.Content(panel);
        return root;
    }

    private static ElementBuilder Header(object receiver, Action close)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "1rem").Style("right", "1rem")
            .Style("top", ".8rem").Style("z-index", "4")
            .Style("display", "flex").Style("justify-content", "space-between")
            .Style("align-items", "center")
            .Content(WorkshopGui.Element(receiver, "div")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("font-size", ".62rem").Style("letter-spacing", ".24em").Style("color", White)
                    .Text("WORKSHOP // WAYFINDING"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".25rem").Style("font-size", ".43rem")
                    .Style("letter-spacing", ".12em").Style("opacity", ".62")
                    .Text("DATA PILLS ARE DESTINATIONS // COLORED THREADS ARE THE WALKABLE ROUTES")))
            .Content(WorkshopGui.Button(receiver)
                .Label("CLOSE MAP").Style("padding", ".35rem .55rem")
                .Style("border", "1px solid rgba(255,255,255,.28)")
                .Style("background", "rgba(255,255,255,.04)").Style("color", White)
                .Style("font-family", "inherit").Style("font-size", ".45rem")
                .Style("letter-spacing", ".1em").OnClick(close));

    private static ElementBuilder RouteLines(
        object receiver,
        IReadOnlyList<ExperienceSpace> destinations,
        IReadOnlyDictionary<string, IInteractable> interactables,
        double avatarX,
        double avatarY,
        IReadOnlyDictionary<string, IReadOnlyList<SpatialWaypoint>> routes)
    {
        var root = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("pointer-events", "none");

        foreach (var destination in destinations)
        {
            if (!interactables.TryGetValue(destination.Id, out var interactable))
                continue;

            if (!routes.TryGetValue(destination.Id, out var route) || route.Count == 0)
                route = [new SpatialWaypoint(avatarX, avatarY), new SpatialWaypoint(destination.EntranceX, destination.EntranceY)];

            root.Content(WorkshopMapLineRenderer.Build(
                receiver, route, avatarX, avatarY, interactable.Presentation.MapIntent));
        }

        return root;
    }

    private static ElementBuilder Visitor(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
            .Style("width", "46px").Style("height", "46px")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "3")
            .Style("border", "1px solid white").Style("border-radius", "50%")
            .Style("overflow", "hidden").Style("box-shadow", "0 0 28px rgba(255,255,255,.22)")
            .Content(WorkshopGui.Image(receiver)
                .Attribute("src", "https://avatars.githubusercontent.com/u/16405167?v=4")
                .Attribute("alt", "Visitor").Style("width", "100%").Style("height", "100%")
                .Style("object-fit", "cover").Style("border-radius", "50%"));

    private static ElementBuilder DataPill(
        object receiver,
        ExperienceSpace destination,
        InteractableMapPresentation presentation,
        double left,
        double top,
        Func<string, Task> selectDestination)
        => WorkshopGui.Button(receiver)
            .Style("position", "absolute").Style("left", $"{left:0.##}%").Style("top", $"{top:0.##}%")
            .Style("transform", "translate(-50%,-50%) rotate(-90deg)").Style("transform-origin", "center")
            .Style("z-index", "5").Style("width", "180px").Style("height", "46px")
            .Style("padding", ".25rem .75rem").Style("border", $"2px solid {presentation.Accent}aa")
            .Style("border-radius", "999px").Style("background", Ink).Style("color", White)
            .Style("font-family", "inherit").Style("font-size", ".46rem")
            .Style("letter-spacing", ".08em").Style("text-align", "center")
            .Style("cursor", "pointer").Style("box-shadow", $"0 0 22px {presentation.Accent}22")
            .AriaLabel($"Follow {presentation.ThreadLabel} route to {destination.Name}")
            .OnClick(() => selectDestination(destination.Id))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("color", presentation.Accent).Style("letter-spacing", ".16em")
                .Text($"{presentation.ThreadLabel} // {destination.Name}"));

    private static ElementBuilder Legend(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "1rem").Style("bottom", ".8rem")
            .Style("z-index", "4").Style("font-size", ".4rem")
            .Style("letter-spacing", ".1em").Style("opacity", ".7")
            .Text("FOLLOW THE COLORED LINE // IT IS THE SAME PATH YOUR AVATAR WILL WALK");
}
