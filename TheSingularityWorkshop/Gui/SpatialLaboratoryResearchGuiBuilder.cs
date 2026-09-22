namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Presents the hidden rendering research level inside the Singularity Laboratory.
/// The room is an instrument gallery: each physical station asks the renderer to
/// represent a different scale or visual problem rather than presenting a menu of tools.
/// </summary>
public static class SpatialLaboratoryResearchGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Muted = "#8ea7b2";
    private const string Ink = "#01050a";

    private static readonly IReadOnlyList<ResearchStation> Stations =
    [
        new("micro-detail", "MICRO DETAIL", "Face against the wall. Spend the budget on cracks, stains, dust, insects, and every surface event inside reach.", "detail", "frame"),
        new("tectonic-mass", "TECTONIC MASS", "A world assembled as material first. Continents, ridges, cuts, and enormous landforms emerge from deterministic terrain noise.", "massive", "orbit"),
        new("weathering", "WEATHERING", "Let time become geometry. Repeated erosion and removal expose new surfaces without inventing a separate render object.", "massive", "follow"),
        new("far-horizon", "FAR HORIZON", "Push the camera outward without surrendering the horizon. Distance becomes a rendering-rate problem, not an excuse for fog.", "massive", "frame"),
        new("cosmic-zoom", "COSMIC ZOOM", "The long experiment: universe, cluster, galaxy, system, planet, surface, and finally the Workshop moniker.", "massive", "orbit")
    ];

    /// <summary>Builds the research gallery and its rendering instrument.</summary>
    public static ElementBuilder Build(
        object receiver,
        string userName,
        string selectedStationId,
        bool threeDimensional,
        Action<string> selectStation,
        Action toggleDimension,
        Action<string> setDimension,
        Action<string> setCamera,
        Action exit)
    {
        var station = Stations.FirstOrDefault(x => x.Id == selectedStationId) ?? Stations[0];

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(ResearchCanvas(receiver));
        root.Content(Header(receiver, station, userName, threeDimensional));
        root.Content(StationGallery(receiver, station, selectStation));
        root.Content(RenderControls(receiver, threeDimensional, toggleDimension, setDimension, setCamera, station));
        root.Content(WorkshopGui.Button(receiver).Label("← LEAVE RESEARCH LEVEL")
            .Style("position", "fixed").Style("right", "1.25rem").Style("bottom", "1.1rem")
            .Style("z-index", "80").Style("padding", ".7rem 1rem")
            .Style("border", $"1px solid {Magenta}77").Style("background", "rgba(1,5,10,.92)")
            .Style("color", Magenta).Style("font-family", "inherit").Style("font-size", "1rem")
            .Style("letter-spacing", ".08em").Style("cursor", "pointer").OnClick(exit));

        return root;
    }

    private static ElementBuilder ResearchCanvas(object receiver)
        => WorkshopGui.Element(receiver, "canvas")
            .Attribute("id", "laboratory-rendering-canvas")
            .Attribute("aria-label", "Laboratory rendering research instrument")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("display", "block");

    private static ElementBuilder Header(object receiver, ResearchStation station, string userName, bool threeDimensional)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1.2rem").Style("top", "1rem").Style("z-index", "50")
            .Style("width", "min(760px,72vw)").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("letter-spacing", ".2em").Style("color", Cyan).Text("SINGULARITY LABORATORY // RESEARCH LEVEL"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", "clamp(1.4rem,3vw,3rem)").Style("font-weight", "800").Style("letter-spacing", ".08em").Style("color", White).Text(station.Name))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("max-width", "720px").Style("font-family", "system-ui,sans-serif").Style("font-size", "clamp(.72rem,1vw,1rem)").Style("line-height", "1.45").Style("color", Muted).Text(station.Description))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".5rem").Style("font-size", ".55rem").Style("letter-spacing", ".16em").Style("color", threeDimensional ? Magenta : Yellow).Text(threeDimensional ? $"VISITOR {userName.ToUpperInvariant()} // 3D VOLUME ACTIVE" : $"VISITOR {userName.ToUpperInvariant()} // DIMENSIONALITY CONTAINED"));

    private static ElementBuilder StationGallery(object receiver, ResearchStation selected, Action<string> selectStation)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("left", "1.2rem").Style("bottom", "1.1rem").Style("z-index", "55")
            .Style("width", "min(430px,42vw)").Style("max-height", "54vh").Style("overflow-y", "auto")
            .Style("padding", ".7rem").Style("border", $"1px solid {Cyan}33")
            .Style("background", "rgba(1,5,10,.82)").Style("backdrop-filter", "blur(8px)");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".55rem").Style("letter-spacing", ".16em").Style("color", Yellow).Text("RESEARCH INSTRUMENTS // DISCOVERED"));

        foreach (var station in Stations)
        {
            var active = station.Id == selected.Id;
            panel.Content(WorkshopGui.Button(receiver).Label($"{(active ? "●" : "○")}  {station.Name}")
                .Style("display", "block").Style("width", "100%").Style("margin-top", ".35rem")
                .Style("padding", ".65rem .7rem").Style("text-align", "left")
                .Style("border", $"1px solid {(active ? Magenta : Cyan)}{(active ? "aa" : "30")}")
                .Style("background", active ? "rgba(255,56,209,.09)" : "rgba(0,234,255,.025)")
                .Style("color", active ? White : Muted).Style("font-family", "inherit")
                .Style("font-size", "clamp(.7rem,.9vw,.9rem)").Style("letter-spacing", ".06em")
                .Style("cursor", "pointer").OnClick(() => selectStation(station.Id)));
        }

        return panel;
    }

    private static ElementBuilder RenderControls(
        object receiver,
        bool threeDimensional,
        Action toggleDimension,
        Action<string> setDimension,
        Action<string> setCamera,
        ResearchStation station)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("right", "1.2rem").Style("top", "1rem").Style("z-index", "55")
            .Style("width", "min(310px,28vw)").Style("padding", ".8rem")
            .Style("border", $"1px solid {Magenta}55").Style("background", "rgba(1,5,10,.84)").Style("backdrop-filter", "blur(8px)");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".55rem").Style("letter-spacing", ".15em").Style("color", Cyan).Text("RENDERING INSTRUMENT"));
        panel.Content(WorkshopGui.Button(receiver).Label(threeDimensional ? "3D // RETRACT DIMENSION" : "SURPRISE // EXTEND INTO 3D")
            .Style("width", "100%").Style("margin-top", ".55rem").Style("padding", ".8rem")
            .Style("border", $"2px solid {(threeDimensional ? Magenta : Yellow)}")
            .Style("background", threeDimensional ? "rgba(255,56,209,.12)" : "rgba(255,211,77,.08)")
            .Style("color", threeDimensional ? Magenta : Yellow).Style("font-family", "inherit")
            .Style("font-size", "clamp(.8rem,1vw,1rem)").Style("font-weight", "800")
            .Style("letter-spacing", ".08em").Style("cursor", "pointer").OnClick(toggleDimension));

        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".7rem").Style("font-size", ".5rem").Style("color", Muted).Text("DIMENSION"));
        panel.Content(ControlRow(receiver, [("2D", "0"), ("ISO", ".5"), ("3D", "1")], setDimension));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".65rem").Style("font-size", ".5rem").Style("color", Muted).Text("CAMERA BEHAVIOR"));
        panel.Content(ControlRow(receiver, [("FRAME", "frame"), ("FOLLOW", "follow"), ("ORBIT", "orbit")], setCamera));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".7rem").Style("font-size", ".48rem").Style("line-height", "1.5").Style("color", Muted)
            .Text($"DATA SCALE // {station.Specimen.ToUpperInvariant()}  ·  CAMERA // {station.Camera.ToUpperInvariant()}"));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".5rem").Style("letter-spacing", ".1em").Style("color", Green).Text("DATA → SURFACE → GPU"));
        return panel;
    }

    private static ElementBuilder ControlRow(object receiver, (string Label, string Value)[] controls, Action<string> action)
    {
        var row = WorkshopGui.Element(receiver, "div").Style("display", "grid").Style("grid-template-columns", $"repeat({controls.Length},1fr)").Style("gap", ".3rem").Style("margin-top", ".3rem");
        foreach (var control in controls)
            row.Child(WorkshopGui.Button(receiver).Label(control.Label)
                .Style("padding", ".5rem .2rem").Style("border", $"1px solid {Cyan}35")
                .Style("background", "rgba(0,234,255,.035)").Style("color", White)
                .Style("font-family", "inherit").Style("font-size", ".7rem").Style("cursor", "pointer")
                .OnClick(() => action(control.Value)));
        return row;
    }

    private readonly record struct ResearchStation(string Id, string Name, string Description, string Specimen, string Camera);
}
