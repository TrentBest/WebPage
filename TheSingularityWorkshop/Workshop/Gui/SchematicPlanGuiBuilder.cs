namespace TheSingularityWorkshop.Gui;

/// <summary>
/// SVG perception builder for <see cref="SchematicPlan"/>.
/// Architectural geometry is deliberately rendered as linework rather than
/// conventional GUI chrome: poche, openings, labels, and trade overlays remain
/// visible as the visitor changes scale.
/// </summary>
public static class SchematicPlanGuiBuilder
{
    private const string Ink = "#020a12";
    private const string Blueprint = "#00eaff";
    private const string White = "#ffffff";
    private const string Fire = "#ffb84d";

    public static ElementBuilder Build(object receiver, SchematicPlan plan, string accent = Blueprint)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", $"0 0 {plan.Width:0.##} {plan.Height:0.##}")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("role", "img")
            .Attribute("aria-label", $"{plan.Name} {plan.Scale} schematic")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("pointer-events", "none")
            .Style("overflow", "visible");

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "0")
            .Attribute("y", "0")
            .Attribute("width", plan.Width)
            .Attribute("height", plan.Height)
            .Attribute("fill", Ink)
            .Attribute("fill-opacity", ".72"));

        foreach (var wall in plan.Walls)
            AddWall(svg, receiver, wall, accent);

        foreach (var opening in plan.Openings)
            AddOpening(svg, receiver, opening, accent);

        foreach (var system in plan.Systems)
            AddSystem(svg, receiver, system);

        foreach (var label in plan.Labels)
            svg.Child(WorkshopGui.Element(receiver, "text")
                .Attribute("x", label.X)
                .Attribute("y", label.Y)
                .Attribute("fill", label.Kind == "CODE" ? White : accent)
                .Attribute("fill-opacity", label.Kind == "CODE" ? ".72" : ".9")
                .Attribute("font-family", "Consolas, 'Courier New', monospace")
                .Attribute("font-size", label.Size)
                .Attribute("letter-spacing", ".08em")
                .Attribute("text-anchor", "middle")
                .Attribute("dominant-baseline", "middle")
                .Text(label.Text));

        return svg;
    }

    /// <summary>
    /// Creates a simple building plan from the existing spatial landmark.
    /// The entrance is a real opening in the wall, not a button-shaped border.
    /// </summary>
    public static SchematicPlan ForBuilding(
        SpatialRoom room,
        double width,
        double height,
        double entranceWidth = 12)
    {
        var mid = width / 2d;
        var halfOpening = entranceWidth / 2d;

        return new SchematicPlan(
            room.Id,
            room.Name,
            SchematicScale.Building,
            width,
            height,
            [
                new(0, 0, width, 0),
                new(0, 0, 0, height),
                new(width, 0, width, height),
                new(0, height, mid - halfOpening, height),
                new(mid + halfOpening, height, width, height)
            ],
            [
                new(mid - halfOpening, height - 1.8, entranceWidth, 1.8, "DOOR")
            ],
            [
                new(room.Name.ToUpperInvariant(), mid, height * .18, room.Id == "engineering" ? 4 : 3.2, "BUILDING"),
                new(room.Kind, mid, height * .28, 1.8, "CODE")
            ],
            []);
    }

    private static void AddWall(ElementBuilder svg, object receiver, SchematicLine line, string accent)
    {
        var dx = line.X2 - line.X1;
        var dy = line.Y2 - line.Y1;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length < .001)
            return;

        var nx = -dy / length;
        var ny = dx / length;
        const double offset = .72;

        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", line.X1 + nx * offset)
            .Attribute("y1", line.Y1 + ny * offset)
            .Attribute("x2", line.X2 + nx * offset)
            .Attribute("y2", line.Y2 + ny * offset)
            .Attribute("stroke", accent)
            .Attribute("stroke-opacity", ".34")
            .Attribute("stroke-width", "1.25"));

        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", line.X1 - nx * offset)
            .Attribute("y1", line.Y1 - ny * offset)
            .Attribute("x2", line.X2 - nx * offset)
            .Attribute("y2", line.Y2 - ny * offset)
            .Attribute("stroke", accent)
            .Attribute("stroke-opacity", ".82")
            .Attribute("stroke-width", ".28"));
    }

    private static void AddOpening(ElementBuilder svg, object receiver, SchematicOpening opening, string accent)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", opening.X)
            .Attribute("y", opening.Y)
            .Attribute("width", opening.Width)
            .Attribute("height", opening.Height)
            .Attribute("fill", Ink)
            .Attribute("fill-opacity", ".98"));

        if (opening.Kind.Equals("DOOR", StringComparison.OrdinalIgnoreCase))
        {
            var hingeX = opening.X;
            var hingeY = opening.Y + opening.Height;
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", hingeX)
                .Attribute("y1", hingeY)
                .Attribute("x2", opening.X + opening.Width)
                .Attribute("y2", opening.Y)
                .Attribute("stroke", accent)
                .Attribute("stroke-opacity", ".65")
                .Attribute("stroke-width", ".22"));
        }
        else
        {
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", opening.X)
                .Attribute("y", opening.Y)
                .Attribute("width", opening.Width)
                .Attribute("height", opening.Height)
                .Attribute("fill", "none")
                .Attribute("stroke", accent)
                .Attribute("stroke-opacity", ".65")
                .Attribute("stroke-width", ".3"));
        }
    }

    private static void AddSystem(
        ElementBuilder svg,
        object receiver,
        SchematicSystemRoute system)
    {
        var stroke = system.Trade == SchematicTrade.FireProtection ? Fire : Blueprint;

        foreach (var segment in system.Segments)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", segment.X1)
                .Attribute("y1", segment.Y1)
                .Attribute("x2", segment.X2)
                .Attribute("y2", segment.Y2)
                .Attribute("stroke", stroke)
                .Attribute("stroke-opacity", ".7")
                .Attribute("stroke-width", ".34")
                .Attribute("stroke-dasharray", system.Trade == SchematicTrade.FireProtection ? "1.2 .8" : "none"));
    }
}
