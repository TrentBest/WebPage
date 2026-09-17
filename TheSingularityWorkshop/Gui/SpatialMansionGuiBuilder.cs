using System.Diagnostics;

namespace TheSingularityWorkshop.Gui;

/// <summary>Renders the Singularity Mansion as a linework stress environment.</summary>
public static class SpatialMansionGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff38d1";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";

    /// <summary>Builds a viewport around the entire mansion and reports rendering workload.</summary>
    public static ElementBuilder Build(object receiver, SpatialLinework mansion, string avatarName, Action exit)
    {
        var start = Stopwatch.GetTimestamp();
        var rendered = new SpatialLineRenderer().Render(mansion, SpatialLineStyleCatalog.Blueprint);
        var microseconds = Stopwatch.GetElapsedTime(start).TotalMilliseconds * 1000d;

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "auto")
            .Style("background", "#01050b").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        var viewport = WorkshopGui.Element(receiver, "div")
            .Style("position", "relative").Style("width", "150vw").Style("height", "150vh")
            .Style("min-width", "1200px").Style("min-height", "900px");

        viewport.Content(Structure(receiver, rendered));
        viewport.Content(Label(receiver, 50, 50, "SINGULARITY MANSION", Cyan));
        viewport.Content(Label(receiver, 50, 53, $"LINEWORK STRESS ENVIRONMENT // {mansion.Lines.Count:N0} LINES // {microseconds:0.0} μs", Yellow));
        viewport.Content(Label(receiver, 50, 57, "ORTHOGONAL VIEW CANDIDATE // GPU-ADJACENCY EXPERIMENT", Magenta));
        viewport.Content(Label(receiver, 50, 61, $"AVATAR // {avatarName}", White));
        root.Content(viewport);

        root.Content(WorkshopGui.Button(receiver).Label("← EXIT MANSION")
            .Style("position", "fixed").Style("right", "1rem").Style("bottom", "1rem").Style("z-index", "30")
            .Style("padding", ".55rem .75rem").Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.9)").Style("color", Magenta)
            .Style("font-family", "inherit").Style("font-size", ".48rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(exit));

        return root;
    }

    private static ElementBuilder Structure(object receiver, IReadOnlyList<SpatialRenderedLine> rendered)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%").Style("pointer-events", "none");

        foreach (var line in rendered)
        {
            var points = string.Join(" ", line.Points.Select(point => $"{point.X:0.###},{point.Y:0.###}"));
            svg.Child(WorkshopGui.Element(receiver, "polyline")
                .Attribute("points", points).Attribute("fill", "none")
                .Attribute("stroke", line.Style.Stroke).Attribute("stroke-width", line.Style.Width)
                .Attribute("stroke-opacity", line.Style.Opacity)
                .Attribute("stroke-linecap", "round").Attribute("stroke-linejoin", "round"));
        }

        return svg;
    }

    private static ElementBuilder Label(object receiver, double left, double top, string text, string accent)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", left + "%").Style("top", top + "%")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "5")
            .Style("padding", ".3rem .5rem").Style("border", $"1px solid {accent}66")
            .Style("background", "rgba(1,4,10,.8)").Style("color", accent)
            .Style("font-size", ".5rem").Style("letter-spacing", ".16em")
            .Style("white-space", "nowrap").Style("pointer-events", "none").Text(text);
}
