using System.Diagnostics;

namespace TheSingularityWorkshop.Gui;

/// <summary>Builds the Forge as a diegetic FSM fabrication floor.</summary>
public static class SpatialForgeGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff38d1";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";

    /// <summary>Builds the Forge interior, centered on the FSM creation workbench.</summary>
    public static ElementBuilder Build(object receiver, SpatialLinework structure, string avatarName, Action enterLineLab, Action exitForge)
    {
        var renderStart = Stopwatch.GetTimestamp();
        var rendered = new SpatialLineRenderer().Render(structure, SpatialLineStyleCatalog.Blueprint);
        var renderMicroseconds = Stopwatch.GetElapsedTime(renderStart).TotalMilliseconds * 1000d;

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden").Style("background", "#020812").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text("@keyframes forge-breathe{0%,100%{filter:brightness(1)}50%{filter:brightness(1.35)}}@keyframes ingot-pulse{0%,100%{opacity:.65}50%{opacity:1}}"));

        var floor = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0")
            .Style("background", "radial-gradient(circle at 50% 48%,#0a1c2a 0%,#030911 48%,#010308 100%)").Style("overflow", "hidden");

        floor.Content(Grid(receiver));
        floor.Content(Structure(receiver, rendered));

        // The Forge has one purpose: fabricate an FSM. Everything else feeds this bench.
        floor.Content(Station(receiver, 12, 17, 28, 18, "LINEWORK LAB", Cyan, "DRAW / SHAPE / VISUALIZE", enterLineLab));
        floor.Content(Station(receiver, 60, 17, 28, 18, "FSM INGOTS", Magenta, "ON ENTER / ON UPDATE / ON EXIT / TRANSITION"));
        floor.Content(Workbench(receiver));
        floor.Content(Station(receiver, 12, 69, 28, 14, "BLUEPRINT LIBRARY", Yellow, "SAVE / REUSE / SCAFFOLD"));
        floor.Content(Station(receiver, 60, 69, 28, 14, "PREVIEW BAY", White, "ASSEMBLE / PREVIEW / VERIFY"));
        floor.Content(Ingots(receiver));

        root.Content(floor);
        root.Content(Title(receiver, structure.Lines.Count, renderMicroseconds));
        root.Content(Avatar(receiver, avatarName));
        root.Content(WorkshopGui.Button(receiver).Label("← EXIT FORGE")
            .Style("position", "fixed").Style("right", "1rem").Style("bottom", "1rem").Style("z-index", "30")
            .Style("padding", ".55rem .75rem").Style("border", $"1px solid {Magenta}66").Style("background", "rgba(1,4,10,.88)")
            .Style("color", Magenta).Style("font-family", "inherit").Style("font-size", ".48rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(exitForge));
        return root;
    }

    private static ElementBuilder Workbench(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "25%").Style("top", "39%").Style("width", "50%").Style("height", "25%")
            .Style("box-sizing", "border-box").Style("border", $"2px solid {Magenta}bb")
            .Style("background", "linear-gradient(180deg,{Magenta}16,#03070d 80%)")
            .Style("box-shadow", $"0 0 45px {Magenta}25,inset 0 0 30px {Magenta}10").Style("animation", "forge-breathe 4s ease-in-out infinite")
            .Content(WorkshopGui.Element(receiver, "div").Style("padding", ".65rem").Style("color", Magenta).Style("font-size", ".65rem").Style("letter-spacing", ".22em").Text("FSM FABRICATION WORKBENCH"))
            .Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("justify-content", "center").Style("gap", "1rem").Style("margin-top", "2rem")
                .Content(WorkbenchSlot(receiver, "EMPTY STATE", Cyan))
                .Content(WorkbenchSlot(receiver, "EMPTY STATE", Cyan))
                .Content(WorkbenchSlot(receiver, "TRANSITION", Yellow)))
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("bottom", ".7rem").Style("left", "0").Style("right", "0").Style("text-align", "center")
                .Style("color", "#7895a1").Style("font-family", "system-ui,sans-serif").Style("font-size", ".5rem")
                .Text("ASSEMBLE STATES → LOAD INGOTS → NAME → PREVIEW"));

    private static ElementBuilder WorkbenchSlot(object receiver, string label, string accent)
        => WorkshopGui.Element(receiver, "div").Style("width", "22%").Style("height", "2.6rem")
            .Style("border", $"1px dashed {accent}88").Style("background", $"{accent}08")
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center")
            .Style("color", accent).Style("font-size", ".4rem").Style("letter-spacing", ".08em").Text(label);

    private static ElementBuilder Ingots(object receiver)
    {
        var rack = WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "43%").Style("top", "12%").Style("width", "14%").Style("height", "20%")
            .Style("display", "flex").Style("justify-content", "space-around").Style("align-items", "flex-end");
        foreach (var item in new[] { ("ON ENTER", Cyan), ("ON UPDATE", Yellow), ("ON EXIT", Magenta), ("TRANSITION", White) })
        {
            rack.Content(WorkshopGui.Element(receiver, "div").Style("width", "19%").Style("height", "70%").Style("border-radius", "35% 35% 12% 12%")
                .Style("border", $"1px solid {item.Item2}bb").Style("background", $"linear-gradient(180deg,{item.Item2}44,#05080d)")
                .Style("box-shadow", $"0 0 14px {item.Item2}33").Style("animation", "ingot-pulse 2.4s ease-in-out infinite")
                .Attribute("title", item.Item1));
        }
        return rack;
    }

    private static ElementBuilder Title(object receiver, int lineCount, double renderMicroseconds)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "50%").Style("top", "1rem")
            .Style("transform", "translateX(-50%)").Style("z-index", "30").Style("text-align", "center").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".65rem").Style("letter-spacing", ".3em").Text("THE FORGE"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("color", "#7895a1").Style("font-size", ".42rem").Style("letter-spacing", ".18em").Text("FSM FABRICATION FACILITY"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("color", Yellow).Style("font-size", ".38rem").Style("letter-spacing", ".12em").Text($"LINE RENDER // {lineCount:00} PRIMITIVES // {renderMicroseconds:0.0} μs"));

    private static ElementBuilder Grid(object receiver)
        => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("inset", "0").Style("background-size", "5vw 5vh")
            .Style("background-image", "linear-gradient(rgba(0,234,255,.07) 1px,transparent 1px),linear-gradient(90deg,rgba(0,234,255,.07) 1px,transparent 1px)").Style("opacity", ".7").Style("pointer-events", "none");

    private static ElementBuilder Structure(object receiver, IReadOnlyList<SpatialRenderedLine> rendered)
    {
        var svg = WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100vw").Style("height", "100vh").Style("pointer-events", "none");
        foreach (var line in rendered)
        {
            var points = string.Join(" ", line.Points.Select(point => $"{point.X:0.###},{point.Y:0.###}"));
            svg.Child(WorkshopGui.Element(receiver, "polyline").Attribute("points", points).Attribute("fill", "none")
                .Attribute("stroke", line.Style.Stroke).Attribute("stroke-width", line.Style.Width).Attribute("stroke-opacity", line.Style.Opacity)
                .Attribute("stroke-linecap", "round").Attribute("stroke-linejoin", "round"));
        }
        return svg;
    }

    private static ElementBuilder Station(object receiver, double left, double top, double width, double height, string label, string accent, string detail, Action? activate = null)
    {
        var station = WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", left + "%").Style("top", top + "%")
            .Style("width", width + "%").Style("height", height + "%").Style("box-sizing", "border-box").Style("border", $"1px solid {accent}88")
            .Style("background", $"linear-gradient(180deg,{accent}14,#03070d 75%)").Style("box-shadow", $"0 0 22px {accent}18,inset 0 0 20px {accent}08")
            .Style("animation", "forge-breathe 3.2s ease-in-out infinite").Style("cursor", activate is null ? "default" : "pointer");
        station.Content(WorkshopGui.Element(receiver, "div").Style("padding", ".6rem").Style("color", accent).Style("font-size", ".55rem").Style("letter-spacing", ".18em").Text(label));
        station.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "8%").Style("right", "8%").Style("top", "38%").Style("height", "30%")
            .Style("border", $"1px solid {accent}44").Style("background", $"{accent}08").Content(WorkshopGui.Element(receiver, "div").Style("padding", ".5rem")
                .Style("color", "#7895a1").Style("font-family", "system-ui,sans-serif").Style("font-size", ".55rem").Text(detail)));
        if (activate is not null)
            station.Content(WorkshopGui.Button(receiver).Label("ENTER STATION →").Style("position", "absolute").Style("left", "8%").Style("right", "8%").Style("bottom", "8%")
                .Style("padding", ".4rem").Style("border", $"1px solid {accent}66").Style("background", "transparent").Style("color", accent)
                .Style("font-family", "inherit").Style("font-size", ".42rem").Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(activate));
        return station;
    }

    private static ElementBuilder Avatar(object receiver, string name)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "50%").Style("top", "78%").Style("z-index", "25")
            .Style("transform", "translate(-50%,-50%)").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("width", "16px").Style("height", "16px").Style("border", $"1px solid {White}").Style("border-radius", "50%").Style("box-shadow", $"0 0 16px {White}99"))
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "50%").Style("top", "-1.2rem").Style("transform", "translateX(-50%)")
                .Style("padding", ".12rem .3rem").Style("border", "1px solid rgba(255,255,255,.18)").Style("background", "rgba(1,4,10,.84)")
                .Style("font-size", ".45rem").Style("letter-spacing", ".1em").Style("white-space", "nowrap").Text(name));
}
