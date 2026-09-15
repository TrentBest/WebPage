using TheSingularityWorkshop.Workshop.Interaction;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Workshop presentation for an interactable.
///
/// This is intentionally separate from the interaction domain: an interactable
/// declares presentation intent, while the Workshop decides how that intent is
/// visualized. The renderer is therefore replaceable without changing the
/// interaction contract.
/// </summary>
public static class InteractableSchematicGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder Build(
        object receiver,
        Interactable interactable,
        bool hovered)
    {
        ArgumentNullException.ThrowIfNull(interactable);

        var accent = Accent(interactable.Presentation.SchematicId);
        var active = hovered ? "1" : ".62";
        var glow = hovered
            ? $"0 0 28px {accent}55, inset 0 0 24px {accent}16"
            : $"0 0 12px {accent}18";

        var frame = WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {accent}{(hovered ? "dd" : "66")}")
            .Style("background", hovered ? $"rgba(0,28,38,.54)" : "rgba(0,14,22,.34)")
            .Style("box-shadow", glow)
            .Style("opacity", active)
            .Style("transform", hovered ? "scale(1.018)" : "scale(1)")
            .Style("transition", "transform .16s ease, opacity .16s ease, box-shadow .16s ease, background .16s ease")
            .Style("box-sizing", "border-box")
            .Style("pointer-events", "none");

        frame.Content(Schematic(receiver, interactable.Presentation.SchematicId, accent, hovered));

        if (hovered)
        {
            frame.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute")
                .Style("left", "50%")
                .Style("bottom", "5%")
                .Style("transform", "translateX(-50%)")
                .Style("padding", ".16rem .34rem")
                .Style("border", $"1px solid {accent}aa")
                .Style("background", "rgba(1,4,10,.9)")
                .Style("color", White)
                .Style("font-size", "clamp(.4rem, .8vw, .58rem)")
                .Style("letter-spacing", ".12em")
                .Style("white-space", "nowrap")
                .Text("HOVER // INSPECT"));
        }

        return frame;
    }

    private static ElementBuilder Schematic(
        object receiver,
        string schematicId,
        string accent,
        bool hovered)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 60")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute")
            .Style("inset", "5%")
            .Style("width", "90%")
            .Style("height", "90%")
            .Style("overflow", "visible")
            .Style("opacity", hovered ? ".9" : ".56");

        // Blueprint construction grid.
        for (var x = 0; x <= 100; x += 10)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", "0")
                .Attribute("x2", x).Attribute("y2", "60")
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".12")
                .Attribute("stroke-width", ".25"));

        for (var y = 0; y <= 60; y += 10)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "0").Attribute("y1", y)
                .Attribute("x2", "100").Attribute("y2", y)
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".12")
                .Attribute("stroke-width", ".25"));

        switch (schematicId)
        {
            case "research-facility":
                ResearchFacility(svg, receiver, accent);
                break;
            case "forge":
                Forge(svg, receiver, accent);
                break;
            case "creation-bay":
                CreationBay(svg, receiver, accent);
                break;
            default:
                DefaultBuilding(svg, receiver, accent);
                break;
        }

        return svg;
    }

    private static void ResearchFacility(ElementBuilder svg, object receiver, string accent)
    {
        Room(svg, receiver, accent, 5, 6, 90, 48);
        Room(svg, receiver, accent, 10, 11, 24, 18);
        Room(svg, receiver, accent, 38, 11, 24, 18);
        Room(svg, receiver, accent, 66, 11, 24, 18);
        Room(svg, receiver, accent, 10, 34, 24, 14);
        Room(svg, receiver, accent, 38, 34, 24, 14);
        Room(svg, receiver, accent, 66, 34, 24, 14);
        Line(svg, receiver, accent, 34, 20, 38, 20);
        Line(svg, receiver, accent, 62, 20, 66, 20);
        Line(svg, receiver, accent, 34, 41, 38, 41);
        Line(svg, receiver, accent, 62, 41, 66, 41);
        Crosshair(svg, receiver, accent, 50, 30);
    }

    private static void Forge(ElementBuilder svg, object receiver, string accent)
    {
        Room(svg, receiver, accent, 7, 7, 86, 46);
        Room(svg, receiver, accent, 13, 13, 22, 34);
        Room(svg, receiver, accent, 39, 13, 22, 34);
        Room(svg, receiver, accent, 65, 13, 22, 34);
        Line(svg, receiver, accent, 35, 30, 39, 30);
        Line(svg, receiver, accent, 61, 30, 65, 30);
        Crosshair(svg, receiver, accent, 50, 30);
    }

    private static void CreationBay(ElementBuilder svg, object receiver, string accent)
    {
        Room(svg, receiver, accent, 7, 7, 86, 46);
        Room(svg, receiver, accent, 13, 14, 20, 32);
        Room(svg, receiver, accent, 40, 14, 20, 32);
        Room(svg, receiver, accent, 67, 14, 20, 32);
        Line(svg, receiver, accent, 33, 30, 40, 30);
        Line(svg, receiver, accent, 60, 30, 67, 30);
        Crosshair(svg, receiver, accent, 50, 30);
    }

    private static void DefaultBuilding(ElementBuilder svg, object receiver, string accent)
    {
        Room(svg, receiver, accent, 8, 8, 84, 44);
        Room(svg, receiver, accent, 18, 18, 24, 24);
        Room(svg, receiver, accent, 58, 18, 24, 24);
        Line(svg, receiver, accent, 42, 30, 58, 30);
        Crosshair(svg, receiver, accent, 50, 30);
    }

    private static void Room(ElementBuilder svg, object receiver, string accent, int x, int y, int width, int height)
        => svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("width", width).Attribute("height", height)
            .Attribute("fill", "none")
            .Attribute("stroke", accent)
            .Attribute("stroke-opacity", ".72")
            .Attribute("stroke-width", ".6"));

    private static void Line(ElementBuilder svg, object receiver, string accent, int x1, int y1, int x2, int y2)
        => svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", x1).Attribute("y1", y1)
            .Attribute("x2", x2).Attribute("y2", y2)
            .Attribute("stroke", accent)
            .Attribute("stroke-opacity", ".9")
            .Attribute("stroke-width", ".55"));

    private static void Crosshair(ElementBuilder svg, object receiver, string accent, int x, int y)
    {
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x).Attribute("cy", y).Attribute("r", "2.2")
            .Attribute("fill", "none")
            .Attribute("stroke", accent).Attribute("stroke-width", ".55"));
        Line(svg, receiver, accent, x - 6, y, x - 2, y);
        Line(svg, receiver, accent, x + 2, y, x + 6, y);
        Line(svg, receiver, accent, x, y - 6, x, y - 2);
        Line(svg, receiver, accent, x, y + 2, x, y + 6);
    }

    private static string Accent(string schematicId)
        => schematicId switch
        {
            "forge" => Cyan,
            "creation-bay" => Green,
            "unknown" => Yellow,
            _ => Cyan
        };
}
