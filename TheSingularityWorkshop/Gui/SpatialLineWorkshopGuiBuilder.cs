using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>Builds the Forge linework room from reusable spatial line styles and effects.</summary>
public static class SpatialLineWorkshopGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff38d1";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";

    /// <summary>Builds a room in which visitors can create reusable styled line primitives.</summary>
    public static ElementBuilder Build(object receiver, SpatialLinework linework, SpatialLineStyle style, bool awaitingEnd, Action<MouseEventArgs> drawPoint, Action clear, Action<SpatialLineStyle> setStyle, Action exitScene, string avatarName)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden").Style("background", "#020812").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text("@keyframes line-room-breathe{0%,100%{opacity:.72}50%{opacity:1}}"));

        var canvas = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("cursor", awaitingEnd ? "crosshair" : "cell")
            .AriaLabel("Linework drawing surface. Click a start point, then click an end point.")
            .OnClick(drawPoint).StopPropagation("onclick");

        for (var i = 0; i <= 100; i += 5)
        {
            canvas.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", i).Attribute("y1", 0).Attribute("x2", i).Attribute("y2", 100).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".08").Attribute("stroke-width", ".12"));
            canvas.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 0).Attribute("y1", i).Attribute("x2", 100).Attribute("y2", i).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".08").Attribute("stroke-width", ".12"));
        }

        foreach (var rendered in new SpatialLineRenderer().Render(linework, style))
        {
            var points = string.Join(" ", rendered.Points.Select(point => $"{point.X:0.###},{point.Y:0.###}"));
            canvas.Child(WorkshopGui.Element(receiver, "polyline").Attribute("points", points).Attribute("fill", "none").Attribute("stroke", rendered.Style.Stroke).Attribute("stroke-width", rendered.Style.Width).Attribute("stroke-opacity", rendered.Style.Opacity).Attribute("stroke-linecap", "round").Attribute("stroke-linejoin", "round"));
        }

        root.Content(canvas);
        root.Content(Avatar(receiver, avatarName));
        root.Content(Panel(receiver, style, awaitingEnd, clear, setStyle, exitScene, linework.Lines.Count));
        return root;
    }

    private static ElementBuilder Panel(object receiver, SpatialLineStyle style, bool awaitingEnd, Action clear, Action<SpatialLineStyle> setStyle, Action exitScene, int count)
    {
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1rem").Style("top", "1rem").Style("z-index", "30")
            .Style("width", "min(24rem,calc(100vw - 2rem))").Style("padding", ".7rem")
            .Style("border", $"1px solid {Cyan}66").Style("background", "rgba(1,4,10,.9)")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".5rem").Style("letter-spacing", ".18em").Text("THE FORGE // LINEWORK LAB"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("font-size", ".65rem").Style("color", "#b7c8cf").Style("font-family", "system-ui,sans-serif").Text(awaitingEnd ? "DRAWING: click the endpoint." : "DRAW: click a start point, then an endpoint."));

        var styles = WorkshopGui.Element(receiver, "div").Style("margin-top", ".55rem").Style("display", "flex").Style("gap", ".35rem").Style("flex-wrap", "wrap");
        foreach (var candidate in SpatialLineStyleCatalog.All)
            styles.Content(ActionButton(receiver, candidate.Id.ToUpperInvariant(), candidate.Id == style.Id, () => setStyle(candidate)));
        panel.Content(styles);
        panel.Content(ActionButton(receiver, "CLEAR", false, clear));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("font-size", ".45rem").Style("letter-spacing", ".12em").Style("color", Yellow).Text($"STYLE {style.Id.ToUpperInvariant()} // LINES {count:00}"));
        panel.Content(WorkshopGui.Button(receiver).Label("← LEAVE FORGE").Style("margin-top", ".65rem").Style("padding", ".45rem .6rem").Style("border", $"1px solid {Magenta}66").Style("background", "transparent").Style("color", Magenta).Style("font-family", "inherit").Style("cursor", "pointer").OnClick(exitScene));
        return panel;
    }

    private static ElementBuilder ActionButton(object receiver, string label, bool active, Action action)
        => WorkshopGui.Button(receiver).Label(label).Style("padding", ".35rem .5rem").Style("border", $"1px solid {(active ? Cyan : "#ffffff")}55").Style("background", active ? $"{Cyan}18" : "transparent").Style("color", active ? Cyan : White).Style("font-family", "inherit").Style("font-size", ".45rem").Style("cursor", "pointer").OnClick(action);

    private static ElementBuilder Avatar(object receiver, string name)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "50%").Style("top", "50%").Style("z-index", "20").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("width", "14px").Style("height", "14px").Style("border", $"1px solid {White}").Style("border-radius", "50%").Style("box-shadow", $"0 0 14px {White}88").Style("animation", "line-room-breathe 2.5s ease-in-out infinite"))
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "50%").Style("top", "-1.1rem").Style("transform", "translateX(-50%)").Style("font-size", ".45rem").Style("white-space", "nowrap").Text(name));
}
