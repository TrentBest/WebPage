namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Converts spatial geometry primitives into the current SVG perception layer.
/// The renderer is intentionally disposable: the geometry model is the durable seam.
/// </summary>
public static class SpatialGeometryGuiBuilder
{
    private const string White = "#f7f7ff";

    /// <summary>Builds the complete Workshop schematic from scene geometry.</summary>
    public static ElementBuilder Build(object receiver, SpatialWorkshopScene scene, Func<string, Task> interact)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute")
            .Style("left", "0")
            .Style("top", "0")
            .Style("width", $"{SpatialCamera.WorldWidthVw:0.###}vw")
            .Style("height", $"{SpatialCamera.WorldHeightVh:0.###}vh")
            .Style("pointer-events", "none")
            .Attribute("aria-label", "Workshop schematic geometry");

        foreach (var building in SpatialGeometryEngine.FromInteractables(scene.Interactables))
            svg.Child(BuildBuilding(receiver, building));

        svg.Child(CenterMarker(receiver));
        return svg;
    }

    private static ElementBuilder BuildBuilding(object receiver, SpatialBuildingGeometry building)
    {
        var b = building.Bounds;
        var group = WorkshopGui.Element(receiver, "g");

        group.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", b.X).Attribute("y", b.Y)
            .Attribute("width", b.Width).Attribute("height", b.Height)
            .Attribute("fill", building.Fill)
            .Attribute("fill-opacity", ".92")
            .Attribute("stroke", "none"));

        foreach (var wall in SpatialGeometryEngine.BuildWalls(building))
        {
            group.Child(Line(receiver, wall.Outer, wall.Accent, ".42"));
            group.Child(Line(receiver, wall.Inner, wall.Accent, ".16"));
        }

        foreach (var opening in building.Openings)
            group.Child(Opening(receiver, building, opening));

        group.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", b.X + b.Width * .12).Attribute("y", b.Y + b.Height * .42)
            .Attribute("width", b.Width * .76).Attribute("height", b.Height * .28)
            .Attribute("fill", "none")
            .Attribute("stroke", building.Accent)
            .Attribute("stroke-opacity", ".16")
            .Attribute("stroke-width", ".18"));

        group.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", b.X + b.Width / 2).Attribute("y", b.Y + b.Height / 2)
            .Attribute("fill", White)
            .Attribute("fill-opacity", ".82")
            .Attribute("font-size", ".9")
            .Attribute("font-family", "monospace")
            .Attribute("text-anchor", "middle")
            .Attribute("dominant-baseline", "middle")
            .Text(building.Id.ToUpperInvariant()));

        return group;
    }

    private static ElementBuilder Opening(object receiver, SpatialBuildingGeometry building, SpatialOpening opening)
    {
        var b = building.Bounds;
        var thickness = 1.5;
        return opening.Edge switch
        {
            SpatialGeometryEdge.Top => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X + opening.Start).Attribute("y", b.Y - thickness / 2).Attribute("width", opening.Length).Attribute("height", thickness).Attribute("fill", "#02060b"),
            SpatialGeometryEdge.Right => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X + b.Width - thickness / 2).Attribute("y", b.Y + opening.Start).Attribute("width", thickness).Attribute("height", opening.Length).Attribute("fill", "#02060b"),
            SpatialGeometryEdge.Bottom => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X + opening.Start).Attribute("y", b.Y + b.Height - thickness / 2).Attribute("width", opening.Length).Attribute("height", thickness).Attribute("fill", "#02060b"),
            _ => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X - thickness / 2).Attribute("y", b.Y + opening.Start).Attribute("width", thickness).Attribute("height", opening.Length).Attribute("fill", "#02060b")
        };
    }

    private static ElementBuilder Line(object receiver, SpatialLine line, string stroke, string opacity)
        => WorkshopGui.Element(receiver, "line")
            .Attribute("x1", line.Start.X).Attribute("y1", line.Start.Y)
            .Attribute("x2", line.End.X).Attribute("y2", line.End.Y)
            .Attribute("stroke", stroke)
            .Attribute("stroke-opacity", opacity)
            .Attribute("stroke-width", ".32")
            .Attribute("vector-effect", "non-scaling-stroke");

    private static ElementBuilder CenterMarker(object receiver)
        => WorkshopGui.Element(receiver, "g")
            .Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", 50).Attribute("cy", 50).Attribute("r", 1.2).Attribute("fill", "none").Attribute("stroke", White).Attribute("stroke-opacity", ".65").Attribute("stroke-width", ".2"))
            .Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 46).Attribute("y1", 50).Attribute("x2", 54).Attribute("y2", 50).Attribute("stroke", White).Attribute("stroke-opacity", ".16").Attribute("stroke-width", ".12"))
            .Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 50).Attribute("y1", 46).Attribute("x2", 50).Attribute("y2", 54).Attribute("stroke", White).Attribute("stroke-opacity", ".16").Attribute("stroke-width", ".12"));
}
