namespace TheSingularityWorkshop.Gui;

/// <summary>Visualizes a generated AEC concept as a navigable spatial massing model.</summary>
public static class SpatialAECGeneratedStructureGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";

    public static ElementBuilder Build(
        object receiver,
        SpatialAECGeneratedStructure structure,
        string avatarName,
        Action enterOffice,
        Action regenerate,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", "#02080f").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Massing(receiver, structure));
        root.Content(Brief(receiver, structure, avatarName, enterOffice, regenerate));
        root.Content(Exit(receiver, exit));
        return root;
    }

    private static ElementBuilder Massing(object receiver, SpatialAECGeneratedStructure structure)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh");

        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "8").Attribute("fill", Yellow).Attribute("font-size", "2").Attribute("text-anchor", "middle").Text(structure.BuildingType));

        var colors = new[] { Cyan, Magenta, Yellow, "#7df9ff", "#52e05a", "#ff8a3d" };
        var columns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(structure.Rooms.Count)));
        for (var i = 0; i < structure.Rooms.Count; i++)
        {
            var room = structure.Rooms[i];
            var col = i % columns;
            var row = i / columns;
            var x = 25 + col * 16;
            var y = 24 + row * 18;
            var width = Math.Max(10, Math.Min(15, room.Width / Math.Max(1, structure.ApproximateSideLength) * 28));
            var height = Math.Max(10, Math.Min(15, room.Depth / Math.Max(1, structure.ApproximateSideLength) * 28));

            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", x).Attribute("y", y).Attribute("width", width).Attribute("height", height)
                .Attribute("fill", $"{colors[i % colors.Length]}10")
                .Attribute("stroke", colors[i % colors.Length]).Attribute("stroke-opacity", ".65").Attribute("stroke-width", ".35"));
            svg.Child(WorkshopGui.Element(receiver, "text")
                .Attribute("x", x + width / 2d).Attribute("y", y + height / 2d)
                .Attribute("fill", White).Attribute("font-size", ".95").Attribute("text-anchor", "middle")
                .Text(room.Name));
        }

        for (var floor = 1; floor < structure.Floors; floor++)
        {
            var y = 21 - floor * .8;
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "18").Attribute("y1", y).Attribute("x2", "82").Attribute("y2", y)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".18").Attribute("stroke-width", ".25"));
        }

        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "50").Attribute("y", "93").Attribute("fill", "#7895a1").Attribute("font-size", "1.1").Attribute("text-anchor", "middle")
            .Text($"{structure.SquareUnits:0} SQUARE UNITS // {structure.Floors} FLOORS // {structure.ApproximateSideLength:0.#} UNIT FOOTPRINT"));

        return svg;
    }

    private static ElementBuilder Brief(object receiver, SpatialAECGeneratedStructure structure, string avatarName, Action enterOffice, Action regenerate)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", "4%").Style("top", "12%")
            .Style("width", "min(25rem,34vw)").Style("padding", ".9rem")
            .Style("border", $"1px solid {Cyan}55").Style("background", "rgba(1,6,12,.92)")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".45rem").Style("letter-spacing", ".16em").Text("GENERATIVE AEC // FIRST PASS"))
            .Content(WorkshopGui.Element(receiver, "p").Style("font-family", "system-ui,sans-serif").Style("font-size", ".75rem").Style("color", "#9ab0b8").Text($"The Workshop has assembled a navigable concept for {avatarName}. It is not the final building; it is a spatial hypothesis you can now inspect, reject, refine, or author directly."))
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".38rem").Style("margin-top", ".5rem").Text($"ONTOLOGY // {structure.OntologySummary}"))
            .Content(WorkshopGui.Button(receiver).Label("ENTER GENERATED STRUCTURE / AUTHOR")
                .Style("margin-top", ".7rem").Style("width", "100%").Style("padding", ".65rem")
                .Style("border", $"1px solid {Cyan}88").Style("background", "rgba(0,234,255,.05)")
                .Style("color", Cyan).Style("font-family", "inherit").Style("cursor", "pointer").OnClick(enterOffice))
            .Content(WorkshopGui.Button(receiver).Label("REGENERATE FROM INTENT")
                .Style("margin-top", ".35rem").Style("width", "100%").Style("padding", ".45rem")
                .Style("border", "1px solid rgba(255,211,77,.35)").Style("background", "transparent")
                .Style("color", Yellow).Style("font-family", "inherit").Style("cursor", "pointer").OnClick(regenerate));
}
