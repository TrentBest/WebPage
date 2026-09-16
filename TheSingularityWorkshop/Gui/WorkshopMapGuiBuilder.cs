using TheSingularityWorkshop.Workshop.Experience;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Recursive fluent GUI for the Workshop's spatial wayfinding map.
/// The map is a temporary spatial instrument placed in front of the visitor:
/// destinations become cards, cards belong to semantic threads, and SVG
/// threads visually connect the visitor to the direction of each destination.
/// </summary>
public static class WorkshopMapGuiBuilder
{
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder MapButton(object receiver, bool open, Action toggle)
        => WorkshopGui.Button(receiver)
            .Label(open ? "CLOSE MAP" : "WORKSHOP MAP")
            .Style("position", "fixed")
            .Style("right", "1rem")
            .Style("bottom", "1rem")
            .Style("z-index", "80")
            .Style("padding", ".55rem .75rem")
            .Style("border", "1px solid rgba(255,255,255,.35)")
            .Style("background", "rgba(1,4,10,.9)")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("font-size", ".46rem")
            .Style("letter-spacing", ".12em")
            .Style("cursor", "pointer")
            .AriaLabel(open ? "Close Workshop map" : "Open Workshop map")
            .OnClick(toggle);

    public static ElementBuilder Build(
        object receiver,
        IReadOnlyList<ExperienceSpace> destinations,
        double avatarX,
        double avatarY,
        Func<string, Task> selectDestination,
        Action close)
    {
        ArgumentNullException.ThrowIfNull(destinations);
        ArgumentNullException.ThrowIfNull(selectDestination);
        ArgumentNullException.ThrowIfNull(close);

        var root = WorkshopGui.Element(receiver, "section")
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("z-index", "90")
            .Style("display", "flex")
            .Style("align-items", "center")
            .Style("justify-content", "center")
            .Style("background", "rgba(0,0,0,.58)")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Attribute("role", "dialog")
            .Attribute("aria-modal", "true")
            .Attribute("aria-label", "Workshop map");

        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "relative")
            .Style("width", "min(920px, 96vw)")
            .Style("height", "min(680px, 88vh)")
            .Style("overflow", "hidden")
            .Style("box-sizing", "border-box")
            .Style("border", "1px solid rgba(255,255,255,.28)")
            .Style("background", "rgba(1,4,10,.96)")
            .Style("box-shadow", "0 0 90px rgba(0,0,0,.7)");

        panel.Content(Header(receiver, close));
        panel.Content(ThreadSvg(receiver, destinations, avatarX, avatarY));
        panel.Content(Visitor(receiver));

        foreach (var destination in destinations)
        {
            var (thread, accent) = ThreadFor(destination.Id);
            var dx = destination.X - avatarX;
            var dy = destination.Y - avatarY;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            var radial = Math.Clamp(27 + distance * .32, 30, 42);
            var left = 50 + Math.Cos(angle * Math.PI / 180) * radial;
            var top = 50 + Math.Sin(angle * Math.PI / 180) * radial;

            panel.Content(DestinationCard(receiver, destination, thread, accent, left, top, selectDestination));
        }

        panel.Content(Legend(receiver));
        root.Content(panel);
        return root;
    }

    private static ElementBuilder Header(object receiver, Action close)
    {
        var title = WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".62rem")
            .Style("letter-spacing", ".24em")
            .Style("color", White)
            .Text("WORKSHOP // SPATIAL MAP");

        var subtitle = WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".25rem")
            .Style("font-size", ".43rem")
            .Style("letter-spacing", ".12em")
            .Style("opacity", ".62")
            .Text("FOLLOW A THREAD // SELECT A DESTINATION");

        var copy = WorkshopGui.Element(receiver, "div")
            .Content(title)
            .Content(subtitle);

        var closeButton = WorkshopGui.Button(receiver)
            .Label("CLOSE MAP")
            .Style("padding", ".35rem .55rem")
            .Style("border", "1px solid rgba(255,255,255,.28)")
            .Style("background", "rgba(255,255,255,.04)")
            .Style("color", White)
            .Style("font-family", "inherit")
            .Style("font-size", ".45rem")
            .Style("letter-spacing", ".1em")
            .OnClick(close);

        return WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("left", "1rem")
            .Style("right", "1rem")
            .Style("top", ".8rem")
            .Style("z-index", "4")
            .Style("display", "flex")
            .Style("justify-content", "space-between")
            .Style("align-items", "center")
            .Content(copy)
            .Content(closeButton);
    }

    private static ElementBuilder ThreadSvg(object receiver, IReadOnlyList<ExperienceSpace> destinations, double avatarX, double avatarY)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("pointer-events", "none");

        foreach (var destination in destinations)
        {
            var (_, accent) = ThreadFor(destination.Id);
            var dx = destination.X - avatarX;
            var dy = destination.Y - avatarY;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            var scale = Math.Clamp(28 / Math.Max(distance, 1), .45, 1.0);
            var endX = 50 + dx * scale;
            var endY = 50 + dy * scale;

            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "50").Attribute("y1", "50")
                .Attribute("x2", endX.ToString("0.##")).Attribute("y2", endY.ToString("0.##"))
                .Attribute("stroke", accent)
                .Attribute("stroke-opacity", ".78")
                .Attribute("stroke-width", "1.1"));

            svg.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", endX.ToString("0.##"))
                .Attribute("cy", endY.ToString("0.##"))
                .Attribute("r", "1.6")
                .Attribute("fill", "none")
                .Attribute("stroke", accent)
                .Attribute("stroke-width", ".6"));
        }

        return svg;
    }

    private static ElementBuilder Visitor(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", "50%")
            .Style("top", "50%")
            .Style("width", "46px")
            .Style("height", "46px")
            .Style("transform", "translate(-50%,-50%)")
            .Style("z-index", "3")
            .Style("border", "1px solid white")
            .Style("border-radius", "50%")
            .Style("overflow", "hidden")
            .Style("box-shadow", "0 0 28px rgba(255,255,255,.22)")
            .Content(WorkshopGui.Image(receiver)
                .Attribute("src", "https://avatars.githubusercontent.com/u/16405167?v=4")
                .Attribute("alt", "Visitor"));

    private static ElementBuilder DestinationCard(object receiver, ExperienceSpace destination, string thread, string accent, double left, double top, Func<string, Task> selectDestination)
        => WorkshopGui.Button(receiver)
            .Style("position", "absolute")
            .Style("left", $"{left:0.##}%")
            .Style("top", $"{top:0.##}%")
            .Style("transform", "translate(-50%,-50%) rotate(-90deg)")
            .Style("transform-origin", "center")
            .Style("z-index", "5")
            .Style("min-width", "120px")
            .Style("max-width", "190px")
            .Style("padding", ".55rem .65rem")
            .Style("border", $"2px solid {accent}aa")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "inherit")
            .Style("font-size", ".48rem")
            .Style("letter-spacing", ".08em")
            .Style("text-align", "left")
            .Style("cursor", "pointer")
            .Style("box-shadow", $"0 0 22px {accent}22")
            .AriaLabel($"Follow {thread} thread to {destination.Name}")
            .OnClick(() => selectDestination(destination.Id))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", accent)
                .Style("font-size", ".4rem")
                .Style("letter-spacing", ".16em")
                .Text(thread))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".2rem")
                .Style("font-size", ".62rem")
                .Text(destination.Name));

    private static ElementBuilder Legend(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", "1rem")
            .Style("bottom", ".8rem")
            .Style("z-index", "4")
            .Style("font-size", ".4rem")
            .Style("letter-spacing", ".1em")
            .Style("opacity", ".7")
            .Text("THREADS ARE ROUTES THROUGH THE WORKSHOP // CARDS ARE DESTINATIONS");

    private static (string Thread, string Accent) ThreadFor(string id)
        => id switch
        {
            "engineering" => ("MACHINERY", "#00eaff"),
            "creation" => ("CREATION", "#52e05a"),
            "experience" => ("EXPERIENCE", "#ff70d9"),
            "architecture" => ("ARCHITECTURE", "#ffd34d"),
            "research" => ("RESEARCH", "#b58cff"),
            "space-elevator" => ("ORBITAL", "#ff9f43"),
            "unknown" => ("UNKNOWN", "#ff5577"),
            _ => ("WORKSHOP", "#ffffff")
        };
}
