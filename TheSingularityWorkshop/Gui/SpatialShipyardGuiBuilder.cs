using System;
using System.Linq;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the Singularity Shipyard as a place rather than as a generic application panel.
/// The vessel population is deliberately schematic: docks, slips, cranes, workshops,
/// harbor traffic, sailboats, motorboats, and ferries are all spatial constructs.
/// </summary>
public static class SpatialShipyardGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffe04a";
    private const string Magenta = "#ff3b8d";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";
    private const string Water = "#061b26";

    public static ElementBuilder Build(
        object receiver,
        SpatialShipyardManifest manifest,
        SpatialShipDesignManifest design,
        string visitorName,
        Action exitRoom)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "relative")
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Harbor(receiver));
        root.Content(Title(receiver, manifest, visitorName));
        root.Content(DockLegend(receiver, manifest));
        root.Content(Exit(receiver, exitRoom));
        return root;
    }

    private static ElementBuilder Harbor(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 120 80")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-label", "Schematic view of the Singularity Shipyard")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("pointer-events", "none");

        // Water and quay.
        svg.Child(Rect(receiver, 0, 0, 120, 80, Water, Cyan, 1, .15));
        svg.Child(Rect(receiver, 4, 5, 72, 70, "none", Cyan, .7, .45));
        svg.Child(Line(receiver, 4, 22, 76, 22, Cyan, .65, .55));
        svg.Child(Line(receiver, 4, 53, 76, 53, Cyan, .65, .55));

        // Land-side production buildings.
        svg.Child(Building(receiver, 8, 8, 18, 10, "HULL FABRICATION"));
        svg.Child(Building(receiver, 30, 8, 18, 10, "ENGINEERING"));
        svg.Child(Building(receiver, 52, 8, 18, 10, "OUTFITTING"));

        // Dry docks / slips.
        svg.Child(Slip(receiver, 10, 27, 20, 20, "DRY DOCK 01"));
        svg.Child(Slip(receiver, 34, 27, 20, 20, "DRY DOCK 02"));
        svg.Child(Slip(receiver, 58, 27, 14, 20, "REFIT"));
        svg.Child(Quay(receiver, 8, 56, 64, 10));

        // Yard systems.
        svg.Child(Crane(receiver, 25, 20));
        svg.Child(Crane(receiver, 55, 20));
        svg.Child(Crane(receiver, 68, 48));
        svg.Child(Route(receiver, 7, 70, 72, 70, Yellow));
        svg.Child(Route(receiver, 7, 73, 72, 73, Magenta));

        // Harbor traffic. These are deliberately animated as world activity,
        // not UI controls.
        svg.Child(Sailboat(receiver, 92, 17, 1, 24));
        svg.Child(Motorboat(receiver, 103, 35, -1, 18));
        svg.Child(Ferry(receiver, 92, 55, 1, 30));

        // Water-side service markers.
        svg.Child(DockMarker(receiver, 82, 14, "FERRY"));
        svg.Child(DockMarker(receiver, 82, 30, "SERVICE"));
        svg.Child(DockMarker(receiver, 82, 46, "MARINA"));
        svg.Child(DockMarker(receiver, 82, 62, "HARBOR"));

        return svg;
    }

    private static ElementBuilder Title(object receiver, SpatialShipyardManifest manifest, string visitorName)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("left", "2rem")
            .Style("top", "1.25rem")
            .Style("z-index", "10")
            .Style("padding", ".65rem .9rem")
            .Style("border", $"1px solid {Yellow}88")
            .Style("background", "rgba(1,4,10,.9)")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Yellow)
                .Style("font-size", ".55rem")
                .Style("letter-spacing", ".22em")
                .Text(manifest.Name))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".3rem")
                .Style("font-size", ".42rem")
                .Style("letter-spacing", ".12em")
                .Style("color", Cyan)
                .Text($"{manifest.RelationshipToStation} // VISITOR {visitorName}"));

    private static ElementBuilder DockLegend(object receiver, SpatialShipyardManifest manifest)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("right", "2rem")
            .Style("top", "1.25rem")
            .Style("z-index", "10")
            .Style("width", "240px")
            .Style("padding", ".7rem")
            .Style("border", $"1px solid {Cyan}66")
            .Style("background", "rgba(1,4,10,.92)");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan)
            .Style("font-size", ".45rem")
            .Style("letter-spacing", ".16em")
            .Text("YARD SERVICES"));

        foreach (var service in manifest.Services)
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".42rem")
                .Style("font-size", ".42rem")
                .Style("letter-spacing", ".08em")
                .Style("color", White)
                .Text($"{service.Name} // {service.Description}"));
        }

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".7rem")
            .Style("padding-top", ".5rem")
            .Style("border-top", $"1px solid {Cyan}33")
            .Style("font-size", ".4rem")
            .Style("line-height", "1.5")
            .Style("color", "#91aeb8")
            .Text("Vessel construction is a Workshop capability presented here as a living city place."));

        return panel;
    }

    private static ElementBuilder Exit(object receiver, Action exitRoom)
        => WorkshopGui.Button(receiver)
            .Label("RETURN TO SINGULARITY CITY")
            .Style("position", "absolute")
            .Style("left", "2rem")
            .Style("bottom", "1.5rem")
            .Style("z-index", "20")
            .Style("padding", ".6rem 1rem")
            .Style("border", $"1px solid {Cyan}99")
            .Style("border-radius", "0")
            .Style("background", Ink)
            .Style("color", Cyan)
            .Style("font-family", "inherit")
            .Style("font-size", ".48rem")
            .Style("letter-spacing", ".12em")
            .Style("cursor", "pointer")
            .OnClick(exitRoom);

    private static ElementBuilder Building(object receiver, double x, double y, double width, double height, string label)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(Rect(receiver, x, y, width, height, "rgba(10,24,30,.9)", Cyan, .5, 1));
        g.Child(Rect(receiver, x + 1.5, y + 1.5, width - 3, height - 3, "none", Cyan, .35, 1));
        g.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x + width / 2)
            .Attribute("y", y + height / 2 + 1)
            .Attribute("text-anchor", "middle")
            .Attribute("fill", White)
            .Attribute("font-size", "2")
            .Attribute("letter-spacing", ".35")
            .Text(label));
        return g;
    }

    private static ElementBuilder Slip(object receiver, double x, double y, double width, double height, string label)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(Rect(receiver, x, y, width, height, "rgba(1,4,10,.4)", Yellow, .45, 1));
        g.Child(Line(receiver, x + 2, y + 2, x + 2, y + height - 2, Yellow, .35, .4));
        g.Child(Line(receiver, x + width - 2, y + 2, x + width - 2, y + height - 2, Yellow, .35, .4));
        g.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x + width / 2)
            .Attribute("y", y + height - 2)
            .Attribute("text-anchor", "middle")
            .Attribute("fill", Yellow)
            .Attribute("font-size", "1.7")
            .Text(label));
        return g;
    }

    private static ElementBuilder Quay(object receiver, double x, double y, double width, double height)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(Rect(receiver, x, y, width, height, "rgba(30,30,18,.75)", Yellow, .5, 1));
        for (var i = 1; i < 8; i++)
            g.Child(Line(receiver, x + i * width / 8, y + 1, x + i * width / 8, y + height - 1, Yellow, .18, .5));
        g.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x + width / 2)
            .Attribute("y", y + height / 2 + 1)
            .Attribute("text-anchor", "middle")
            .Attribute("fill", Yellow)
            .Attribute("font-size", "2")
            .Attribute("letter-spacing", ".4")
            .Text("ACTIVE QUAY // LOADING // FUEL // SERVICE"));
        return g;
    }

    private static ElementBuilder Crane(object receiver, double x, double y)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(Line(receiver, x, y, x, y + 10, Magenta, .7, .8));
        g.Child(Line(receiver, x, y, x + 7, y, Magenta, .7, .8));
        g.Child(Line(receiver, x + 7, y, x + 7, y + 6, Magenta, .35, .8));
        g.Child(Line(receiver, x + 7, y + 6, x + 9, y + 8, Magenta, .35, .8));
        g.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x).Attribute("cy", y).Attribute("r", "1.3")
            .Attribute("fill", "none").Attribute("stroke", Magenta).Attribute("stroke-width", ".4"));
        return g;
    }

    private static ElementBuilder Route(object receiver, double x1, double y1, double x2, double y2, string stroke)
        => Line(receiver, x1, y1, x2, y2, stroke, .35, .65, "6 2");

    private static ElementBuilder DockMarker(object receiver, double x, double y, string label)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(Rect(receiver, x, y, 28, 8, "rgba(1,4,10,.65)", Cyan, .35, 1));
        g.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x + 14).Attribute("y", y + 5)
            .Attribute("text-anchor", "middle")
            .Attribute("fill", Cyan).Attribute("font-size", "1.8")
            .Text(label));
        return g;
    }

    private static ElementBuilder Sailboat(object receiver, double x, double y, int direction, double duration)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(WorkshopGui.Element(receiver, "animateTransform")
            .Attribute("attributeName", "transform")
            .Attribute("type", "translate")
            .Attribute("values", $"{-direction * 24} 0;{direction * 8} 0;{-direction * 24} 0")
            .Attribute("dur", $"{duration}s")
            .Attribute("repeatCount", "indefinite"));
        g.Child(Line(receiver, x, y, x, y + 7, White, .5, .8));
        g.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", $"M {x} {y} L {x + direction * 7} {y + 4} L {x} {y + 7} Z")
            .Attribute("fill", "none").Attribute("stroke", White).Attribute("stroke-width", ".5"));
        g.Child(Line(receiver, x - 4, y + 8, x + 5, y + 8, Cyan, .7, .8));
        return g;
    }

    private static ElementBuilder Motorboat(object receiver, double x, double y, int direction, double duration)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(WorkshopGui.Element(receiver, "animateTransform")
            .Attribute("attributeName", "transform")
            .Attribute("type", "translate")
            .Attribute("values", $"{-direction * 18} 0;{direction * 8} 0;{-direction * 18} 0")
            .Attribute("dur", $"{duration}s")
            .Attribute("repeatCount", "indefinite"));
        g.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", $"M {x - 5} {y} L {x + 5} {y} L {x + 3} {y + 3} L {x - 4} {y + 3} Z")
            .Attribute("fill", "none").Attribute("stroke", Yellow).Attribute("stroke-width", ".7"));
        g.Child(Line(receiver, x - 7, y + 5, x + 7, y + 5, Cyan, .4, .8));
        return g;
    }

    private static ElementBuilder Ferry(object receiver, double x, double y, int direction, double duration)
    {
        var g = WorkshopGui.Element(receiver, "g");
        g.Child(WorkshopGui.Element(receiver, "animateTransform")
            .Attribute("attributeName", "transform")
            .Attribute("type", "translate")
            .Attribute("values", $"{-direction * 12} 0;{direction * 10} 0;{-direction * 12} 0")
            .Attribute("dur", $"{duration}s")
            .Attribute("repeatCount", "indefinite"));
        g.Child(Rect(receiver, x - 7, y, 14, 4, "none", Magenta, .7, 1));
        g.Child(Rect(receiver, x - 5, y - 3, 10, 3, "none", Magenta, .45, 1));
        g.Child(Line(receiver, x - 10, y + 7, x + 10, y + 7, Cyan, .55, .8));
        g.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y + 2.7)
            .Attribute("text-anchor", "middle")
            .Attribute("fill", Magenta).Attribute("font-size", "1.4")
            .Text("FERRY"));
        return g;
    }

    private static ElementBuilder Rect(object receiver, double x, double y, double width, double height, string fill, string stroke = "none", double strokeWidth = 1, double opacity = 1)
        => WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("width", width).Attribute("height", height)
            .Attribute("fill", fill)
            .Attribute("stroke", stroke)
            .Attribute("stroke-width", strokeWidth)
            .Attribute("fill-opacity", opacity);

    private static ElementBuilder Line(object receiver, double x1, double y1, double x2, double y2, string stroke, double width, double opacity, string? dash = null)
    {
        var line = WorkshopGui.Element(receiver, "line")
            .Attribute("x1", x1).Attribute("y1", y1)
            .Attribute("x2", x2).Attribute("y2", y2)
            .Attribute("stroke", stroke)
            .Attribute("stroke-width", width)
            .Attribute("stroke-opacity", opacity);
        if (dash is not null)
            line.Attribute("stroke-dasharray", dash);
        return line;
    }
}
