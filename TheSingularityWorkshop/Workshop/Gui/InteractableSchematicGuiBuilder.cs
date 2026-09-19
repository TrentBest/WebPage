using TheSingularityWorkshop.Workshop.Interaction;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Renders an interactable as a small architectural drawing rather than a
/// decorated button. The world surface owns interaction; this builder owns
/// only the visual language of the structure.
/// </summary>
public static class InteractableSchematicGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Blueprint = "#03121b";

    public static ElementBuilder Build(object receiver, IInteractable interactable, bool hovered)
    {
        ArgumentNullException.ThrowIfNull(interactable);

        var accent = Accent(interactable.Presentation.SchematicId);
        var frame = WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("box-sizing", "border-box")
            .Style("overflow", "visible")
            .Style("pointer-events", "none")
            .Style("background", hovered ? $"{Blueprint}ee" : $"{Blueprint}bb")
            .Style("transition", "background .16s ease, opacity .16s ease")
            .Style("opacity", hovered ? "1" : ".9");

        frame.Content(Schematic(receiver, interactable.Presentation.SchematicId, accent, hovered));
        frame.Content(BlueprintHeader(receiver, interactable, accent, hovered));
        return frame;
    }

    private static ElementBuilder BlueprintHeader(object receiver, IInteractable interactable, string accent, bool hovered)
    {
        var header = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", "4%")
            .Style("right", "4%")
            .Style("bottom", "4%")
            .Style("display", "flex")
            .Style("justify-content", "space-between")
            .Style("align-items", "end")
            .Style("gap", ".5rem")
            .Style("pointer-events", "none")
            .Style("font-size", "clamp(.34rem, .7vw, .52rem)")
            .Style("letter-spacing", ".1em")
            .Style("color", White);

        header.Content(WorkshopGui.Element(receiver, "div")
            .Style("border-top", $"1px solid {accent}88")
            .Style("padding-top", ".18rem")
            .Style("min-width", "0")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", accent).Text(interactable.Presentation.SchematicId.ToUpperInvariant()))
            .Content(WorkshopGui.Element(receiver, "div").Style("opacity", ".7").Text(hovered ? "ACTIVE / INSPECT" : "SCHEMATIC / PLAN")));

        header.Content(WorkshopGui.Element(receiver, "div")
            .Style("border", $"1px solid {accent}66")
            .Style("padding", ".15rem .25rem")
            .Style("background", "rgba(1,4,10,.82)")
            .Text("A-101  ·  1:100"));

        return header;
    }

    private static ElementBuilder Schematic(object receiver, string schematicId, string accent, bool hovered)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 60")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute")
            .Style("inset", "3%")
            .Style("width", "94%")
            .Style("height", "94%")
            .Style("overflow", "visible")
            .Style("pointer-events", "none");

        SheetBorder(svg, receiver, accent);
        Grid(svg, receiver, accent);
        NorthArrow(svg, receiver, accent);

        switch (schematicId)
        {
            case "research-facility": ResearchFacility(svg, receiver, accent); break;
            case "forge": Forge(svg, receiver, accent); break;
            case "creation-bay": CreationBay(svg, receiver, accent); break;
            case "architecture-shop": ArchitectureShop(svg, receiver, accent); break;
            case "space-elevator": SpaceElevator(svg, receiver, accent); break;
            case "tram-terminal": TramTerminal(svg, receiver, accent); break;
            case "spaceship": Spaceship(svg, receiver, accent); break;
            default: DefaultBuilding(svg, receiver, accent); break;
        }

        InteractionPulse(svg, receiver, accent, hovered);
        return svg;
    }

    private static void SheetBorder(ElementBuilder svg, object receiver, string accent)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "1").Attribute("y", "1").Attribute("width", "98").Attribute("height", "58")
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-opacity", ".5").Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "3").Attribute("y", "3").Attribute("width", "94").Attribute("height", "54")
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-opacity", ".18").Attribute("stroke-width", ".2"));
    }

    private static void Grid(ElementBuilder svg, object receiver, string accent)
    {
        for (var x = 10; x < 100; x += 10)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", "4").Attribute("x2", x).Attribute("y2", "56")
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".18"));
        for (var y = 10; y < 60; y += 10)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "4").Attribute("y1", y).Attribute("x2", "96").Attribute("y2", y)
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".18"));
    }

    private static void NorthArrow(ElementBuilder svg, object receiver, string accent)
    {
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "7").Attribute("y", "8")
            .Attribute("fill", accent).Attribute("font-size", "3").Text("N"));
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M7 10 L5.5 14 L7 13 L8.5 14 Z")
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-width", ".35"));
    }

    private static void ResearchFacility(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "7,8 93,8 93,52 7,52 7,8");
        Room(svg, receiver, accent, 11, 13, 20, 14, "MATERIALS");
        Room(svg, receiver, accent, 40, 13, 20, 14, "CHEMISTRY");
        Room(svg, receiver, accent, 69, 13, 20, 14, "ANALYSIS");
        Room(svg, receiver, accent, 11, 33, 20, 14, "STORAGE");
        Room(svg, receiver, accent, 40, 33, 20, 14, "CONTROL");
        Room(svg, receiver, accent, 69, 33, 20, 14, "DETECTOR");
        Door(svg, receiver, accent, 50, 52, 5);
        Circle(svg, receiver, accent, 50, 30, 9);
        Circle(svg, receiver, accent, 50, 30, 6);
        Crosshair(svg, receiver, accent, 50, 30);
        Dimension(svg, receiver, accent, 7, 4, 93, 4, "86m");
        Dimension(svg, receiver, accent, 95, 8, 95, 52, "44m");
    }

    private static void Forge(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "7,8 93,8 93,52 7,52 7,8");
        Room(svg, receiver, accent, 11, 13, 18, 34, "STATE");
        Room(svg, receiver, accent, 35, 13, 30, 18, "FSM CORE");
        Room(svg, receiver, accent, 71, 13, 18, 14, "BUILD");
        Room(svg, receiver, accent, 71, 33, 18, 14, "TEST");
        Line(svg, receiver, accent, 29, 30, 35, 30);
        Line(svg, receiver, accent, 65, 22, 71, 22);
        Line(svg, receiver, accent, 65, 40, 71, 40);
        Door(svg, receiver, accent, 50, 52, 5);
        Crosshair(svg, receiver, accent, 50, 30);
        Dimension(svg, receiver, accent, 7, 4, 93, 4, "86m");
        Dimension(svg, receiver, accent, 95, 8, 95, 52, "44m");
    }

    private static void CreationBay(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "7,8 93,8 93,52 7,52 7,8");
        Room(svg, receiver, accent, 11, 13, 20, 34, "CAPTURE");
        Room(svg, receiver, accent, 40, 13, 20, 34, "COMPOSE");
        Room(svg, receiver, accent, 69, 13, 20, 34, "PUBLISH");
        Line(svg, receiver, accent, 31, 30, 40, 30);
        Line(svg, receiver, accent, 60, 30, 69, 30);
        Door(svg, receiver, accent, 50, 52, 5);
        Crosshair(svg, receiver, accent, 50, 30);
        Dimension(svg, receiver, accent, 7, 4, 93, 4, "86m");
    }

    private static void ArchitectureShop(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "7,8 93,8 93,52 7,52 7,8");
        Room(svg, receiver, accent, 11, 13, 35, 34, "PLAN ROOM");
        Room(svg, receiver, accent, 54, 13, 35, 14, "MODEL");
        Room(svg, receiver, accent, 54, 33, 15, 14, "STAIRS");
        Room(svg, receiver, accent, 74, 33, 15, 14, "LIFT");
        Stair(svg, receiver, accent, 61, 40);
        Elevator(svg, receiver, accent, 81.5, 40);
        Door(svg, receiver, accent, 50, 52, 5);
        Dimension(svg, receiver, accent, 7, 4, 93, 4, "86m");
        Dimension(svg, receiver, accent, 95, 8, 95, 52, "44m");
    }

    private static void SpaceElevator(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "14,8 86,8 86,52 14,52 14,8");
        Room(svg, receiver, accent, 19, 13, 24, 14, "PASSENGER");
        Room(svg, receiver, accent, 57, 13, 24, 14, "FREIGHT");
        Room(svg, receiver, accent, 19, 33, 62, 14, "CLIMBER CONCOURSE");
        Line(svg, receiver, accent, 43, 20, 57, 20);
        Circle(svg, receiver, accent, 50, 40, 8);
        Circle(svg, receiver, accent, 50, 40, 4);
        Line(svg, receiver, accent, 50, 32, 50, 12);
        Door(svg, receiver, accent, 50, 52, 7);
        Dimension(svg, receiver, accent, 14, 4, 86, 4, "72m");
        Dimension(svg, receiver, accent, 88, 8, 88, 52, "44m");
    }

    private static void TramTerminal(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "8,12 92,12 92,48 8,48 8,12");
        Room(svg, receiver, accent, 13, 17, 28, 26, "CONCOURSE");
        Room(svg, receiver, accent, 59, 17, 28, 26, "PLATFORM");
        Line(svg, receiver, accent, 47, 17, 47, 43);
        Line(svg, receiver, accent, 53, 17, 53, 43);
        Line(svg, receiver, accent, 50, 17, 50, 43);
        Door(svg, receiver, accent, 50, 48, 6);
        Dimension(svg, receiver, accent, 8, 8, 92, 8, "84m");
    }

    private static void Spaceship(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "7,8 93,8 93,52 7,52 7,8");
        Room(svg, receiver, accent, 11, 13, 22, 34, "AIRLOCK");
        Room(svg, receiver, accent, 39, 13, 22, 34, "COCKPIT");
        Room(svg, receiver, accent, 67, 13, 22, 14, "CARGO");
        Room(svg, receiver, accent, 67, 33, 22, 14, "SYSTEMS");
        Circle(svg, receiver, accent, 50, 30, 7);
        Crosshair(svg, receiver, accent, 50, 30);
        Line(svg, receiver, accent, 33, 30, 39, 30);
        Line(svg, receiver, accent, 61, 20, 67, 20);
        Line(svg, receiver, accent, 61, 40, 67, 40);
        Door(svg, receiver, accent, 50, 52, 5);
        Dimension(svg, receiver, accent, 7, 4, 93, 4, "86m");
    }

    private static void DefaultBuilding(ElementBuilder svg, object receiver, string accent)
    {
        Wall(svg, receiver, accent, "8,8 92,8 92,52 8,52 8,8");
        Room(svg, receiver, accent, 13, 13, 29, 34, "PRIMARY");
        Room(svg, receiver, accent, 58, 13, 29, 34, "SECONDARY");
        Door(svg, receiver, accent, 50, 52, 6);
        Line(svg, receiver, accent, 42, 30, 58, 30);
        Crosshair(svg, receiver, accent, 50, 30);
        Dimension(svg, receiver, accent, 8, 4, 92, 4, "84m");
    }

    private static void Wall(ElementBuilder svg, object receiver, string accent, string points)
        => svg.Child(WorkshopGui.Element(receiver, "polyline")
            .Attribute("points", points).Attribute("fill", "none").Attribute("stroke", accent)
            .Attribute("stroke-opacity", ".95").Attribute("stroke-width", "1.2"));

    private static void Room(ElementBuilder svg, object receiver, string accent, int x, int y, int width, int height, string label)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y).Attribute("width", width).Attribute("height", height)
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-opacity", ".68").Attribute("stroke-width", ".55"));
        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x + width / 2).Attribute("y", y + height / 2 + 1)
            .Attribute("text-anchor", "middle").Attribute("fill", accent).Attribute("fill-opacity", ".75")
            .Attribute("font-size", "2.7").Text(label));
    }

    private static void Door(ElementBuilder svg, object receiver, string accent, double x, double y, double width)
    {
        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", x - width / 2).Attribute("y1", y).Attribute("x2", x + width / 2).Attribute("y2", y)
            .Attribute("stroke", Blueprint).Attribute("stroke-width", "2.4"));
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", $"M{x - width / 2:0.##} {y:0.##} A {width:0.##} {width:0.##} 0 0 1 {x + width / 2:0.##} {y - width:0.##}")
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-opacity", ".65").Attribute("stroke-width", ".4"));
    }

    private static void Stair(ElementBuilder svg, object receiver, string accent, double x, double y)
    {
        for (var i = 0; i < 6; i++)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x - 5).Attribute("y1", y - 5 + i * 2)
                .Attribute("x2", x + 5).Attribute("y2", y - 5 + i * 2)
                .Attribute("stroke", accent).Attribute("stroke-opacity", ".75").Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", x).Attribute("y", y + 7)
            .Attribute("text-anchor", "middle").Attribute("fill", accent).Attribute("font-size", "2.5").Text("UP"));
    }

    private static void Elevator(ElementBuilder svg, object receiver, string accent, double x, double y)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x - 5).Attribute("y", y - 5).Attribute("width", "10").Attribute("height", "10")
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-width", ".6"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", x).Attribute("y", y + 1)
            .Attribute("text-anchor", "middle").Attribute("fill", accent).Attribute("font-size", "2.6").Text("L"));
    }

    private static void Circle(ElementBuilder svg, object receiver, string accent, double x, double y, double radius)
        => svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x).Attribute("cy", y).Attribute("r", radius)
            .Attribute("fill", "none").Attribute("stroke", accent).Attribute("stroke-opacity", ".85").Attribute("stroke-width", ".6"));

    private static void Crosshair(ElementBuilder svg, object receiver, string accent, double x, double y)
    {
        Line(svg, receiver, accent, x - 6, y, x - 2, y);
        Line(svg, receiver, accent, x + 2, y, x + 6, y);
        Line(svg, receiver, accent, x, y - 6, x, y - 2);
        Line(svg, receiver, accent, x, y + 2, x, y + 6);
    }

    private static void Line(ElementBuilder svg, object receiver, string accent, double x1, double y1, double x2, double y2)
        => svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", x1).Attribute("y1", y1).Attribute("x2", x2).Attribute("y2", y2)
            .Attribute("stroke", accent).Attribute("stroke-opacity", ".82").Attribute("stroke-width", ".5"));

    private static void Dimension(ElementBuilder svg, object receiver, string accent, double x1, double y1, double x2, double y2, string label)
    {
        Line(svg, receiver, accent, x1, y1, x2, y2);
        svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", x1).Attribute("y1", y1 - 1.5).Attribute("x2", x1).Attribute("y2", y1 + 1.5).Attribute("stroke", accent).Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", x2).Attribute("y1", y2 - 1.5).Attribute("x2", x2).Attribute("y2", y2 + 1.5).Attribute("stroke", accent).Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", (x1 + x2) / 2).Attribute("y", y1 - 1).Attribute("text-anchor", "middle").Attribute("fill", accent).Attribute("font-size", "2.5").Text(label));
    }

    private static void InteractionPulse(ElementBuilder svg, object receiver, string accent, bool hovered)
    {
        var pulse = WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "50").Attribute("cy", "30").Attribute("r", hovered ? "12" : "1")
            .Attribute("fill", "none").Attribute("stroke", accent)
            .Attribute("stroke-width", hovered ? ".7" : ".2")
            .Attribute("stroke-opacity", hovered ? ".8" : ".15");

        if (hovered)
            pulse.Child(WorkshopGui.Element(receiver, "animate")
                .Attribute("attributeName", "r").Attribute("values", "10;13;10")
                .Attribute("dur", "1.5s").Attribute("repeatCount", "indefinite"));
        svg.Child(pulse);
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
