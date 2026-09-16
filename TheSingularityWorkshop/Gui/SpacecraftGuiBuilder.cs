using TheSingularityWorkshop.Workshop.Interaction;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Diegetic cockpit manifestation for a user-operated Workshop spacecraft.
/// This is intentionally a control surface inside the Experience rather than
/// a conventional application dashboard: the visitor is sitting in a vessel.
/// </summary>
public static class SpacecraftGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        string flightMode,
        double throttle,
        string destination,
        Action<string> setFlightMode,
        Action<double> setThrottle,
        Action<string> setDestination,
        Action launch,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("overflow", "auto")
            .Style("background", "#01040a")
            .Style("color", "#ffffff")
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("padding", "1rem")
            .Style("box-sizing", "border-box");

        root.Content(Header(receiver, flightMode, destination, exit));
        root.Content(Cockpit(receiver, flightMode, throttle, destination, setFlightMode, setThrottle, setDestination, launch));
        root.Content(Systems(receiver));
        root.Content(Manifest(receiver));
        return root;
    }

    private static ElementBuilder Header(object receiver, string mode, string destination, Action exit)
        => WorkshopGui.Panel(receiver)
            .Style("max-width", "1400px")
            .Style("margin", "0 auto .8rem")
            .Style("display", "flex")
            .Style("justify-content", "space-between")
            .Style("gap", "1rem")
            .Content(WorkshopGui.Element(receiver, "div")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("font-size", "1.15rem")
                    .Style("letter-spacing", ".2em")
                    .Text("SINGULARITY SPACESHIP"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".35rem")
                    .Style("font-size", ".5rem")
                    .Style("letter-spacing", ".12em")
                    .Style("opacity", ".68")
                    .Text($"VESSEL CONTROL // MODE {mode.ToUpperInvariant()} // DESTINATION {destination.ToUpperInvariant()}")))
            .Content(WorkshopGui.Button(receiver)
                .Label("EXIT SHIP")
                .Style("padding", ".5rem .8rem")
                .Style("background", "rgba(255,255,255,.04)")
                .Style("border", "1px solid rgba(255,255,255,.2)")
                .Style("color", "white")
                .Style("font-family", "inherit")
                .OnClick(exit));

    private static ElementBuilder Cockpit(
        object receiver,
        string mode,
        double throttle,
        string destination,
        Action<string> setFlightMode,
        Action<double> setThrottle,
        Action<string> setDestination,
        Action launch)
    {
        var panel = Section(receiver, "COCKPIT // DIEGETIC CONTROL SURFACE", "THE SHIP IS THE TOOL // THE CONTROLS ARE PART OF THE VESSEL");
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 1400 720")
            .Style("width", "100%")
            .Style("height", "min(68vh, 720px)")
            .Style("background", "rgba(2,7,15,.9)")
            .Style("border", "1px solid rgba(82,224,255,.28)");

        svg.Child(WorkshopGui.Element(receiver, "polygon")
            .Attribute("points", "160,600 300,250 700,80 1100,250 1240,600")
            .Attribute("fill", "rgba(82,224,255,.025)")
            .Attribute("stroke", "#52e0ff")
            .Attribute("stroke-opacity", ".7")
            .Attribute("stroke-width", "4"));
        svg.Child(WorkshopGui.Element(receiver, "polygon")
            .Attribute("points", "310,560 390,330 700,180 1010,330 1090,560")
            .Attribute("fill", "rgba(255,112,217,.025)")
            .Attribute("stroke", "#ff70d9")
            .Attribute("stroke-opacity", ".55")
            .Attribute("stroke-width", "3"));

        Label(svg, receiver, 700, 265, "FORWARD VIEW", "white", "middle", "18");
        Label(svg, receiver, 700, 290, destination.ToUpperInvariant(), "#52e0ff", "middle", "13");
        Label(svg, receiver, 700, 325, $"THROTTLE {throttle:0}%", "#ffd34d", "middle", "12");
        Label(svg, receiver, 700, 350, $"FLIGHT MODE // {mode.ToUpperInvariant()}", "rgba(255,255,255,.65)", "middle", "10");

        Control(svg, receiver, 330, 450, "DOCKED", "DOCK", mode == "docked", "#52e0ff", () => setFlightMode("docked"));
        Control(svg, receiver, 520, 450, "READY", "READY", mode == "ready", "#52e05a", () => setFlightMode("ready"));
        Control(svg, receiver, 710, 450, "FLIGHT", "FLY", mode == "flight", "#ff70d9", () => setFlightMode("flight"));
        Control(svg, receiver, 900, 450, "LANDING", "LAND", mode == "landing", "#ffd34d", () => setFlightMode("landing"));

        var destinationButton = WorkshopGui.Button(receiver)
            .Label($"DESTINATION // {destination.ToUpperInvariant()}")
            .Style("margin-top", ".7rem")
            .Style("padding", ".6rem")
            .Style("width", "100%")
            .Style("background", "rgba(82,224,255,.04)")
            .Style("border", "1px solid rgba(82,224,255,.45)")
            .Style("color", "#52e0ff")
            .Style("font-family", "inherit")
            .OnClick(() => setDestination(destination == "Orbital Station" ? "Singularity City" : "Orbital Station"));

        panel.Content(svg);
        panel.Content(destinationButton);
        panel.Content(WorkshopGui.Button(receiver)
            .Label("THROTTLE +25%")
            .Style("margin-top", ".5rem")
            .Style("padding", ".55rem")
            .Style("background", "rgba(255,211,77,.04)")
            .Style("border", "1px solid rgba(255,211,77,.45)")
            .Style("color", "#ffd34d")
            .Style("font-family", "inherit")
            .OnClick(() => setThrottle(Math.Min(100, throttle + 25))));
        panel.Content(WorkshopGui.Button(receiver)
            .Label("INITIATE FLIGHT")
            .Style("margin-top", ".5rem")
            .Style("padding", ".7rem")
            .Style("width", "100%")
            .Style("background", "rgba(255,112,217,.08)")
            .Style("border", "2px solid rgba(255,112,217,.6)")
            .Style("color", "#ffffff")
            .Style("font-family", "inherit")
            .Style("letter-spacing", ".14em")
            .OnClick(launch));
        return panel;
    }

    private static ElementBuilder Systems(object receiver)
    {
        var panel = Section(receiver, "VESSEL SYSTEMS", "SYSTEMS ARE INTERACTABLE CAPABILITIES // NOT DECORATION");
        foreach (var system in new[] { "REACTOR", "LIFE SUPPORT", "NAVIGATION", "COMMUNICATIONS", "ATTITUDE CONTROL", "DOCKING" })
        {
            panel.Content(WorkshopGui.Button(receiver)
                .Label($"SYSTEM // {system}")
                .Style("display", "inline-block")
                .Style("margin", ".3rem")
                .Style("padding", ".5rem .7rem")
                .Style("background", "rgba(82,224,255,.025)")
                .Style("border", "1px solid rgba(82,224,255,.3)")
                .Style("color", "#ffffff")
                .Style("font-family", "inherit"));
        }
        return panel;
    }

    private static ElementBuilder Manifest(object receiver)
        => Section(receiver, "SHIP MANIFEST", "FREIGHT // CREW // PLAYER-CREATED EQUIPMENT // FUTURE MICRO-BUNDLES")
            .Content(WorkshopGui.Element(receiver, "p")
                .Style("margin", ".7rem 0")
                .Style("line-height", "1.5")
                .Style("opacity", ".78")
                .Text("This vessel is a first Workshop vehicle. Later, the same Experience boundary can host ship construction, cargo, navigation, crew, docking, and player-created modules without turning the universe into a collection of disconnected pages."));

    private static ElementBuilder Section(object receiver, string title, string subtitle)
        => WorkshopGui.Panel(receiver)
            .Style("max-width", "1400px")
            .Style("margin", "0 auto .9rem")
            .Style("padding", ".8rem")
            .Style("border", "1px solid rgba(255,255,255,.12)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".62rem")
                .Style("letter-spacing", ".18em")
                .Text(title))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".3rem")
                .Style("font-size", ".46rem")
                .Style("letter-spacing", ".08em")
                .Style("opacity", ".58")
                .Text(subtitle));

    private static void Label(ElementBuilder svg, object receiver, double x, double y, string text, string fill, string anchor, string size)
        => svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("fill", fill).Attribute("font-size", size)
            .Attribute("text-anchor", anchor)
            .Attribute("font-family", "Consolas, 'Courier New', monospace")
            .Text(text));

    private static void Control(ElementBuilder svg, object receiver, double x, double y, string label, string buttonText, bool active, string accent, Action click)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x - 75).Attribute("y", y - 24)
            .Attribute("width", "150").Attribute("height", "48")
            .Attribute("fill", active ? "rgba(255,255,255,.08)" : "rgba(1,4,10,.9)")
            .Attribute("stroke", accent).Attribute("stroke-opacity", active ? ".95" : ".42")
            .Attribute("stroke-width", active ? "3" : "1"));
        Label(svg, receiver, x, y + 5, label, active ? accent : "rgba(255,255,255,.65)", "middle", "12");
    }
}
