namespace TheSingularityWorkshop.Gui;

/// <summary>Converts spatial geometry primitives into the current SVG perception layer.</summary>
/// <remarks>The renderer is intentionally disposable: the geometry model is the durable seam.</remarks>
public static class SpatialGeometryGuiBuilder
{
    private const string White = "#f7f7ff";
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";

    public static ElementBuilder Build(object receiver, SpatialWorkshopScene scene, Func<string, Task> interact, double zoom = 1d)
    {
        var effectiveZoom = new SpatialCamera(50, 50, zoom).Clamped().Zoom;
        var svg = WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("left", "0").Style("top", "0")
            .Style("width", $"{SpatialCamera.WorldWidthVw * effectiveZoom:0.###}vw").Style("height", $"{SpatialCamera.WorldHeightVh * effectiveZoom:0.###}vh")
            .Style("pointer-events", "none").Attribute("aria-label", "Workshop schematic geometry");
        foreach (var building in SpatialGeometryEngine.FromInteractables(scene.Interactables)) svg.Child(BuildBuilding(receiver, building));
        svg.Child(BuildTransit(receiver, SpatialTransitManifest.CreateDefault()));
        svg.Child(CenterMarker(receiver));
        return svg;
    }

    private static ElementBuilder BuildBuilding(object receiver, SpatialBuildingGeometry building)
    {
        var b = building.Bounds; var group = WorkshopGui.Element(receiver, "g");
        group.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", b.X).Attribute("y", b.Y).Attribute("width", b.Width).Attribute("height", b.Height).Attribute("fill", building.Fill).Attribute("fill-opacity", ".92").Attribute("stroke", "none"));
        foreach (var wall in SpatialGeometryEngine.BuildWalls(building)) { group.Child(Line(receiver, wall.Outer, wall.Accent, ".42")); group.Child(Line(receiver, wall.Inner, wall.Accent, ".16")); }
        foreach (var opening in building.Openings) group.Child(Opening(receiver, building, opening));
        group.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", b.X + b.Width * .12).Attribute("y", b.Y + b.Height * .42).Attribute("width", b.Width * .76).Attribute("height", b.Height * .28).Attribute("fill", "none").Attribute("stroke", building.Accent).Attribute("stroke-opacity", ".16").Attribute("stroke-width", ".18"));
        group.Child(WorkshopGui.Element(receiver, "text").Attribute("x", b.X + b.Width / 2).Attribute("y", b.Y + b.Height / 2).Attribute("fill", White).Attribute("fill-opacity", ".82").Attribute("font-size", ".9").Attribute("font-family", "monospace").Attribute("text-anchor", "middle").Attribute("dominant-baseline", "middle").Text(building.Id.ToUpperInvariant()));
        return group;
    }

    private static ElementBuilder BuildTransit(object receiver, SpatialTransitManifest transit)
    {
        var group = WorkshopGui.Element(receiver, "g");
        for (var i = 0; i < transit.TrackCenterline.Count - 1; i++)
        {
            var a = transit.TrackCenterline[i]; var b = transit.TrackCenterline[i + 1];
            group.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", a.X).Attribute("y1", a.Y - .55).Attribute("x2", b.X).Attribute("y2", b.Y - .55).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".72").Attribute("stroke-width", ".28"));
            group.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", a.X).Attribute("y1", a.Y + .55).Attribute("x2", b.X).Attribute("y2", b.Y + .55).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".72").Attribute("stroke-width", ".28"));
        }
        var p = transit.PlatformBounds;
        group.Child(WorkshopGui.Element(receiver, "rect").Attribute("x", p.X).Attribute("y", p.Y).Attribute("width", p.Width).Attribute("height", p.Height).Attribute("fill", "#0b1820").Attribute("fill-opacity", ".94").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".65").Attribute("stroke-width", ".25"));
        group.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", p.X + .4).Attribute("y1", p.Y + .45).Attribute("x2", p.X + p.Width - .4).Attribute("y2", p.Y + .45).Attribute("stroke", Yellow).Attribute("stroke-opacity", ".9").Attribute("stroke-width", ".35"));
        group.Child(WorkshopGui.Element(receiver, "text").Attribute("x", p.X + p.Width / 2).Attribute("y", p.Y - .7).Attribute("fill", Cyan).Attribute("font-size", ".62").Attribute("font-family", "monospace").Attribute("text-anchor", "middle").Text("TRAM PLATFORM // WORKSHOP CENTRAL"));
        foreach (var passenger in transit.Passengers) group.Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", passenger.WaitingPosition.X).Attribute("cy", passenger.WaitingPosition.Y).Attribute("r", ".34").Attribute("fill", White).Attribute("fill-opacity", ".78"));
        return group;
    }

    private static ElementBuilder Opening(object receiver, SpatialBuildingGeometry building, SpatialOpening opening)
    {
        var b = building.Bounds; const double thickness = 1.5;
        return opening.Edge switch
        {
            SpatialGeometryEdge.Top => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X + opening.Start).Attribute("y", b.Y - thickness / 2).Attribute("width", opening.Length).Attribute("height", thickness).Attribute("fill", "#02060b"),
            SpatialGeometryEdge.Right => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X + b.Width - thickness / 2).Attribute("y", b.Y + opening.Start).Attribute("width", thickness).Attribute("height", opening.Length).Attribute("fill", "#02060b"),
            SpatialGeometryEdge.Bottom => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X + opening.Start).Attribute("y", b.Y + b.Height - thickness / 2).Attribute("width", opening.Length).Attribute("height", thickness).Attribute("fill", "#02060b"),
            _ => WorkshopGui.Element(receiver, "rect").Attribute("x", b.X - thickness / 2).Attribute("y", b.Y + opening.Start).Attribute("width", thickness).Attribute("height", opening.Length).Attribute("fill", "#02060b")
        };
    }

    private static ElementBuilder Line(object receiver, SpatialLine line, string stroke, string opacity)
        => WorkshopGui.Element(receiver, "line").Attribute("x1", line.Start.X).Attribute("y1", line.Start.Y).Attribute("x2", line.End.X).Attribute("y2", line.End.Y).Attribute("stroke", stroke).Attribute("stroke-opacity", opacity).Attribute("stroke-width", ".32").Attribute("vector-effect", "non-scaling-stroke");

    private static ElementBuilder CenterMarker(object receiver)
        => WorkshopGui.Element(receiver, "g").Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", 50).Attribute("cy", 50).Attribute("r", 1.2).Attribute("fill", "none").Attribute("stroke", White).Attribute("stroke-opacity", ".65").Attribute("stroke-width", ".2")).Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 46).Attribute("y1", 50).Attribute("x2", 54).Attribute("y2", 50).Attribute("stroke", White).Attribute("stroke-opacity", ".16").Attribute("stroke-width", ".12")).Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 50).Attribute("y1", 46).Attribute("x2", 50).Attribute("y2", 54).Attribute("stroke", White).Attribute("stroke-opacity", ".16").Attribute("stroke-width", ".12"));
}
