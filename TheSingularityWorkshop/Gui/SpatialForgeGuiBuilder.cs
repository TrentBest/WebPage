using System.Diagnostics;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the Forge as a spatial interior rather than replacing the Workshop
/// with an application surface. The building itself is rendered from the same
/// SpatialLinework substrate used by the Linework Lab.
/// </summary>
public static class SpatialForgeGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff38d1";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";

    /// <summary>Builds the Forge interior and its first functional station.</summary>
    public static ElementBuilder Build(
        object receiver,
        SpatialLinework structure,
        string avatarName,
        Action enterLineLab,
        Action exitForge)
    {
        var renderStart = Stopwatch.GetTimestamp();
        var rendered = new SpatialLineRenderer().Render(structure, SpatialLineStyleCatalog.Blueprint);
        var renderMicroseconds = Stopwatch.GetElapsedTime(renderStart).TotalMilliseconds * 1000d;

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", "#020812")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes forge-breathe{0%,100%{filter:brightness(1)}50%{filter:brightness(1.35)}}"));

        var floor = WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("background", "radial-gradient(circle at 50% 45%,#0a1c2a 0%,#030911 48%,#010308 100%)")
            .Style("overflow", "hidden");

        floor.Content(Grid(receiver));
        floor.Content(Structure(receiver, rendered));
        floor.Content(Station(receiver, 14, 20, 30, 23, "LINEWORK LAB", Cyan, "CREATE / EDIT / RENDER", enterLineLab));
        floor.Content(Station(receiver, 56, 20, 30, 23, "FSM BENCH", Magenta, "STATE / LIFECYCLE / EXECUTION"));
        floor.Content(Station(receiver, 14, 55, 30, 23, "MICROBUNDLE BAY", Yellow, "COMPOSE / ARBITRATE / PACKAGE"));
        floor.Content(Station(receiver, 56, 55, 30, 23, "RUNTIME CORE", White, "BOOT / EXPERIENCE / OBSERVE"));
        root.Content(floor);

        root.Content(Title(receiver, structure.Lines.Count, renderMicroseconds));
        root.Content(Avatar(receiver, avatarName));
        root.Content(WorkshopGui.Button(receiver)
            .Label("← EXIT FORGE")
            .Style("position", "fixed")
            .Style("right", "1rem")
            .Style("bottom", "1rem")
            .Style("z-index", "30")
            .Style("padding", ".55rem .75rem")
            .Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.88)")
            .Style("color", Magenta)
            .Style("font-family", "inherit")
            .Style("font-size", ".48rem")
            .Style("letter-spacing", ".12em")
            .Style("cursor", "pointer")
            .OnClick(exitForge));

        return root;
    }

    private static ElementBuilder Title(object receiver, int lineCount, double renderMicroseconds)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed")
            .Style("left", "50%")
            .Style("top", "1rem")
            .Style("transform", "translateX(-50%)")
            .Style("z-index", "30")
            .Style("text-align", "center")
            .Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Cyan)
                .Style("font-size", ".65rem")
                .Style("letter-spacing", ".3em")
                .Text("THE FORGE"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".25rem")
                .Style("color", "#7895a1")
                .Style("font-size", ".42rem")
                .Style("letter-spacing", ".18em")
                .Text("FABRICATION / STATE / COMPOSITION / RUNTIME"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".35rem")
                .Style("color", Yellow)
                .Style("font-size", ".38rem")
                .Style("letter-spacing", ".12em")
                .Text($"LINE RENDER // {lineCount:00} PRIMITIVES // {renderMicroseconds:0.0} μs"));

    private static ElementBuilder Grid(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("background-size", "5vw 5vh")
            .Style("background-image", "linear-gradient(rgba(0,234,255,.07) 1px,transparent 1px),linear-gradient(90deg,rgba(0,234,255,.07) 1px,transparent 1px)")
            .Style("opacity", ".7")
            .Style("pointer-events", "none");

    private static ElementBuilder Structure(object receiver, IReadOnlyList<SpatialRenderedLine> rendered)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("pointer-events", "none");

        foreach (var line in rendered)
        {
            var points = string.Join(" ", line.Points.Select(point => $"{point.X:0.###},{point.Y:0.###}"));
            svg.Child(WorkshopGui.Element(receiver, "polyline")
                .Attribute("points", points)
                .Attribute("fill", "none")
                .Attribute("stroke", line.Style.Stroke)
                .Attribute("stroke-width", line.Style.Width)
                .Attribute("stroke-opacity", line.Style.Opacity)
                .Attribute("stroke-linecap", "round")
                .Attribute("stroke-linejoin", "round"));
        }

        return svg;
    }

    private static ElementBuilder Station(object receiver, double left, double top, double width, double height, string label, string accent, string detail, Action? activate = null)
    {
        var station = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", left + "%")
            .Style("top", top + "%")
            .Style("width", width + "%")
            .Style("height", height + "%")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {accent}88")
            .Style("background", $"linear-gradient(180deg,{accent}14,#03070d 75%)")
            .Style("box-shadow", $"0 0 22px {accent}18,inset 0 0 20px {accent}08")
            .Style("animation", "forge-breathe 3.2s ease-in-out infinite")
            .Style("cursor", activate is null ? "default" : "pointer");

        station.Content(WorkshopGui.Element(receiver, "div")
            .Style("padding", ".6rem")
            .Style("color", accent)
            .Style("font-size", ".55rem")
            .Style("letter-spacing", ".18em")
            .Text(label));
        station.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", "8%")
            .Style("right", "8%")
            .Style("top", "38%")
            .Style("height", "30%")
            .Style("border", $"1px solid {accent}44")
            .Style("background", $"{accent}08")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("padding", ".5rem")
                .Style("color", "#7895a1")
                .Style("font-family", "system-ui,sans-serif")
                .Style("font-size", ".55rem")
                .Text(detail)));

        if (activate is not null)
        {
            station.Content(WorkshopGui.Button(receiver)
                .Label("ENTER STATION →")
                .Style("position", "absolute")
                .Style("left", "8%")
                .Style("right", "8%")
                .Style("bottom", "8%")
                .Style("padding", ".4rem")
                .Style("border", $"1px solid {accent}66")
                .Style("background", "transparent")
                .Style("color", accent)
                .Style("font-family", "inherit")
                .Style("font-size", ".42rem")
                .Style("letter-spacing", ".12em")
                .Style("cursor", "pointer")
                .OnClick(activate));
        }

        return station;
    }

    private static ElementBuilder Avatar(object receiver, string name)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed")
            .Style("left", "50%")
            .Style("top", "78%")
            .Style("z-index", "25")
            .Style("transform", "translate(-50%,-50%)")
            .Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("width", "16px")
                .Style("height", "16px")
                .Style("border", $"1px solid {White}")
                .Style("border-radius", "50%")
                .Style("box-shadow", $"0 0 16px {White}99"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute")
                .Style("left", "50%")
                .Style("top", "-1.2rem")
                .Style("transform", "translateX(-50%)")
                .Style("padding", ".12rem .3rem")
                .Style("border", "1px solid rgba(255,255,255,.18)")
                .Style("background", "rgba(1,4,10,.84)")
                .Style("font-size", ".45rem")
                .Style("letter-spacing", ".1em")
                .Style("white-space", "nowrap")
                .Text(name));
}
