using System;
using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Presents the construction site as its own nested Experience.
/// The exterior world is intentionally absent; planned facilities are rendered as
/// progressively detailed data, not runtime functionality.
/// </summary>
public static class SpatialConstructionExperienceGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Muted = "#9fb2bd";

    public static ElementBuilder Build(
        object receiver,
        SpatialConstructionExperience experience,
        string userName,
        double avatarX,
        double avatarY,
        double zoom,
        Action<double> setZoom,
        Action<KeyboardEventArgs> onKeyDown,
        Func<MouseEventArgs, Task> onWorldClick,
        Action exit)
    {
        var camera = new SpatialCamera(avatarX, avatarY, zoom).Clamped();
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "#02070d").Style("color", White)
            .Attribute("tabindex", "0").Attribute("autofocus", "autofocus")
            .AriaLabel("Construction site guided tour")
            .OnKeyDown(onKeyDown)
            .OnWheel(args => setZoom(ZoomFromWheel(zoom, args.DeltaY))).PreventDefault("onwheel");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes construction-ghost{0%,100%{opacity:.28;filter:brightness(1)}50%{opacity:.58;filter:brightness(1.7)}}"));

        var plane = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "0").Style("top", "0")
            .Style("width", $"{SpatialCamera.WorldWidthVw * zoom:0.###}vw")
            .Style("height", $"{SpatialCamera.WorldHeightVh * zoom:0.###}vh")
            .Style("transform", $"translate({camera.OffsetVw:0.###}vw,{camera.OffsetVh:0.###}vh)")
            .Style("transform-origin", "0 0");

        plane.Content(Grid(receiver, zoom));
        foreach (var feature in experience.Features)
            plane.Content(Feature(receiver, feature, zoom));
        plane.Content(Foreman(receiver, experience.ForemanIntroduction, zoom));
        plane.Content(Avatar(receiver));
        root.Content(plane);
        root.Content(Hud(receiver, experience, userName, zoom, setZoom, exit));
        return root;
    }

    private static ElementBuilder Grid(object receiver, double zoom)
        => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("inset", "0")
            .Style("background-image", $"linear-gradient({Cyan}12 1px,transparent 1px),linear-gradient(90deg,{Cyan}12 1px,transparent 1px)")
            .Style("background-size", $"{SpatialCamera.WorldToVw(5, zoom):0.###}vw {SpatialCamera.WorldToVh(5, zoom):0.###}vh")
            .Style("pointer-events", "none");

    private static ElementBuilder Feature(object receiver, SpatialPlannedFeature feature, double zoom)
    {
        var b = feature.Bounds;
        var opacity = feature.Active ? ".92" : ".3";
        var border = feature.Active ? "2px solid" : "1px dashed";
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"{SpatialCamera.WorldToVw(b.X, zoom):0.###}vw")
            .Style("top", $"{SpatialCamera.WorldToVh(b.Y, zoom):0.###}vh")
            .Style("width", $"{SpatialCamera.WorldToVw(b.Width, zoom):0.###}vw")
            .Style("height", $"{SpatialCamera.WorldToVh(b.Height, zoom):0.###}vh")
            .Style("box-sizing", "border-box")
            .Style("border", $"{border} {(feature.Active ? Cyan : Yellow)}")
            .Style("background", feature.Active ? $"{Cyan}10" : $"{Yellow}04")
            .Style("opacity", opacity)
            .Style("animation", feature.Active ? "none" : "construction-ghost 2.2s ease-in-out infinite")
            .Style("pointer-events", "none");

        surface.Content(WorkshopGui.Element(receiver, "div")
            .Style("padding", ".35rem").Style("font-size", ".42rem").Style("letter-spacing", ".12em")
            .Style("color", feature.Active ? Cyan : Yellow)
            .Text($"{feature.Name} // {(feature.Active ? "ACTIVE" : "PLANNED")}"));

        surface.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", ".35rem").Style("bottom", ".35rem")
            .Style("max-width", "90%").Style("font-family", "system-ui,sans-serif")
            .Style("font-size", ".58rem").Style("line-height", "1.35").Style("color", Muted)
            .Text(feature.Purpose));

        return surface;
    }

    private static ElementBuilder Foreman(object receiver, string message, double zoom)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{SpatialCamera.WorldToVw(4, zoom):0.###}vw")
            .Style("top", $"{SpatialCamera.WorldToVh(5, zoom):0.###}vh").Style("z-index", "20")
            .Style("display", "flex").Style("align-items", "flex-start").Style("gap", ".65rem")
            .Content(ForemanSketch(receiver))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", "1rem").Style("max-width", "min(440px,48vw)")
                .Style("padding", ".65rem .8rem").Style("border", $"1px solid {Yellow}66")
                .Style("background", "rgba(1,4,10,.9)").Style("color", White)
                .Style("font-family", "system-ui,sans-serif").Style("font-size", ".72rem")
                .Style("line-height", "1.45").Text(message));

    private static ElementBuilder ForemanSketch(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 120 150")
            .Style("width", "72px").Style("height", "92px").Style("overflow", "visible");
        svg.Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", "60").Attribute("cy", "38").Attribute("r", "18").Attribute("fill", "none").Attribute("stroke", Yellow).Attribute("stroke-width", "3"));
        svg.Child(WorkshopGui.Element(receiver, "path").Attribute("d", "M38 32 L82 32 L73 22 L47 22 Z").Attribute("fill", $"{Yellow}20").Attribute("stroke", Yellow).Attribute("stroke-width", "2"));
        svg.Child(WorkshopGui.Element(receiver, "path").Attribute("d", "M45 60 L75 60 L88 105 L70 105 L60 82 L50 105 L32 105 Z").Attribute("fill", $"{Yellow}10").Attribute("stroke", White).Attribute("stroke-width", "2"));
        svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "45").Attribute("y1", "68").Attribute("x2", "18").Attribute("y2", "90").Attribute("stroke", White).Attribute("stroke-width", "2"));
        svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "75").Attribute("y1", "68").Attribute("x2", "103").Attribute("y2", "84").Attribute("stroke", White).Attribute("stroke-width", "2"));
        svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "50").Attribute("y1", "105").Attribute("x2", "38").Attribute("y2", "137").Attribute("stroke", White).Attribute("stroke-width", "2"));
        svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "70").Attribute("y1", "105").Attribute("x2", "82").Attribute("y2", "137").Attribute("stroke", White).Attribute("stroke-width", "2"));
        svg.Child(WorkshopGui.Element(receiver, "text").Attribute("x", "60").Attribute("y", "148").Attribute("fill", Yellow).Attribute("font-size", "8").Attribute("text-anchor", "middle").Text("FOREMAN"));
        return svg;
    }

    private static ElementBuilder Avatar(object receiver)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "50%").Style("top", "50%")
            .Style("width", "14px").Style("height", "14px").Style("border", $"1px solid {White}")
            .Style("border-radius", "50%").Style("box-shadow", $"0 0 12px {White}88")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "30").Style("pointer-events", "none");

    private static ElementBuilder Hud(object receiver, SpatialConstructionExperience experience, string userName, double zoom, Action<double> setZoom, Action exit)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("inset", "0").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("right", "1rem").Style("top", "1rem")
                .Style("pointer-events", "auto").Style("padding", ".45rem .6rem").Style("border", $"1px solid {Cyan}55")
                .Style("background", "rgba(1,4,10,.9)").Style("font-size", ".48rem").Style("letter-spacing", ".14em")
                .Text($"{experience.Title} // {userName} // ZOOM {zoom:0.00}X"))
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "1rem").Style("bottom", "1rem")
                .Style("pointer-events", "auto").Style("max-width", "520px").Style("padding", ".5rem .65rem")
                .Style("border", $"1px solid {Cyan}44").Style("background", "rgba(1,4,10,.9)")
                .Style("font-family", "system-ui,sans-serif").Style("font-size", ".62rem").Style("line-height", "1.4")
                .Text("Ghost structures are definitions, not active functionality. Zoom out to see departments; zoom in to inspect planned spaces."))
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("right", "1rem").Style("bottom", "1rem")
                .Style("pointer-events", "auto").Style("padding", ".3rem .5rem").Style("border", $"1px solid {Yellow}66")
                .Style("background", "rgba(1,4,10,.9)").Style("color", Yellow).Style("cursor", "pointer")
                .OnClick(exit).Text("← LEAVE TOUR"));

    private static double ZoomFromWheel(double currentZoom, double deltaY)
        => Math.Clamp(currentZoom * (deltaY < 0 ? 1.15 : 1 / 1.15), SpatialCamera.MinZoom, SpatialCamera.MaxZoom);
}
