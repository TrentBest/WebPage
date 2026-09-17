using System.Diagnostics;
using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>Renders the Singularity Mansion as a camera-driven linework stress environment.</summary>
public static class SpatialMansionGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff38d1";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";

    /// <summary>
    /// Builds an avatar-centered perception layer around the mansion and exposes density controls
    /// so the current line renderer can be pushed without changing the underlying world footprint.
    /// </summary>
    public static ElementBuilder Build(
        object receiver,
        SpatialLinework mansion,
        string avatarName,
        double avatarX,
        double avatarY,
        int density,
        Action<MouseEventArgs> onWorldClick,
        Action<KeyboardEventArgs> onKeyDown,
        Action<int> setDensity,
        Action exit,
        bool showWelcome,
        Action dismissWelcome)
    {
        var renderStart = Stopwatch.GetTimestamp();
        var rendered = new SpatialLineRenderer().Render(mansion, SpatialLineStyleCatalog.Blueprint);
        var renderMicroseconds = Stopwatch.GetElapsedTime(renderStart).TotalMilliseconds * 1000d;

        var textureStart = Stopwatch.GetTimestamp();
        var texture = mansion.TextureBuffer;
        var textureMicroseconds = Stopwatch.GetElapsedTime(textureStart).TotalMilliseconds * 1000d;
        var gpuPayloadBytes = texture.Width * 4 * sizeof(float);

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "#01050b").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .TabIndex(0)
            .OnKeyDown(onKeyDown);

        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", ViewBox(avatarX, avatarY))
            .Attribute("preserveAspectRatio", "xMidYMid meet")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("background", "#01050b")
            .OnClick(onWorldClick);

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "-1000").Attribute("y", "-1000")
            .Attribute("width", "2000").Attribute("height", "2000")
            .Attribute("fill", "transparent"));

        foreach (var line in rendered)
        {
            var points = string.Join(" ", line.Points.Select(point => $"{point.X:0.###},{point.Y:0.###}"));
            svg.Child(WorkshopGui.Element(receiver, "polyline")
                .Attribute("points", points).Attribute("fill", "none")
                .Attribute("stroke", line.Style.Stroke).Attribute("stroke-width", line.Style.Width)
                .Attribute("stroke-opacity", line.Style.Opacity)
                .Attribute("stroke-linecap", "round").Attribute("stroke-linejoin", "round")
                .Style("pointer-events", "none"));
        }

        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", avatarX).Attribute("cy", avatarY).Attribute("r", ".8")
            .Attribute("fill", White).Attribute("stroke", Cyan).Attribute("stroke-width", ".18")
            .Style("filter", "drop-shadow(0 0 4px #00eaff)").Style("pointer-events", "none"));

        root.Content(svg);
        root.Content(Hud(receiver, avatarName, density, mansion.Lines.Count, renderMicroseconds, textureMicroseconds, gpuPayloadBytes));
        root.Content(DensityControls(receiver, density, setDensity));
        root.Content(WorkshopGui.Button(receiver).Label("← EXIT MANSION")
            .Style("position", "fixed").Style("right", "1rem").Style("bottom", "1rem").Style("z-index", "30")
            .Style("padding", ".55rem .75rem").Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.9)").Style("color", Magenta)
            .Style("font-family", "inherit").Style("font-size", ".48rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(exit));

        if (showWelcome)
            root.Content(Welcome(receiver, dismissWelcome));

        return root;
    }

    /// <summary>Temporary first-visit orientation explaining why the Mansion exists.</summary>
    private static ElementBuilder Welcome(object receiver, Action dismiss)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "50%").Style("top", "50%").Style("transform", "translate(-50%,-50%)")
            .Style("z-index", "100").Style("width", "min(680px,84vw)").Style("padding", "1.5rem")
            .Style("box-sizing", "border-box").Style("border", $"1px solid {Cyan}aa")
            .Style("background", "rgba(2,8,18,.97)").Style("box-shadow", $"0 0 70px {Cyan}22,inset 0 0 35px {Cyan}08")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".7rem").Style("letter-spacing", ".28em").Text("WELCOME TO THE SINGULARITY MANSION"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".8rem").Style("color", Yellow).Style("font-size", ".5rem").Style("letter-spacing", ".18em").Text("LINE RENDERING STRESS LAB"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", "1rem").Style("color", White).Style("font-family", "system-ui,sans-serif").Style("font-size", ".9rem").Text("This mansion is deliberately excessive. Its job is to draw lots of ornate lines — lots of them."))
            .Content(WorkshopGui.Element(receiver, "ol").Style("margin", "1rem 0").Style("padding-left", "1.3rem").Style("color", "#b8c9d0").Style("font-family", "system-ui,sans-serif").Style("font-size", ".7rem").Style("line-height", "1.7")
                .Content(WorkshopGui.Element(receiver, "li").Text("Walk around the line-rendered mansion and watch the world move around you."))
                .Content(WorkshopGui.Element(receiver, "li").Text("Increase the density when you want to press the renderer harder."))
                .Content(WorkshopGui.Element(receiver, "li").Text("Watch the line count, render conversion, RGBA texel build, and GPU payload measurements."))
                .Content(WorkshopGui.Element(receiver, "li").Text("We are looking for the point where the current architecture reaches its practical limits."))
                .Content(WorkshopGui.Element(receiver, "li").Text("Those measurements will tell us where to make the next jump: warmer data shelves, tighter buffers, GPU adjacency, and eventually an orthogonal 3D view.")))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".8rem").Style("color", Magenta).Style("font-size", ".46rem").Style("letter-spacing", ".14em").Text("THIS IS A MEASURING INSTRUMENT DISGUISED AS A MANSION."))
            .Content(WorkshopGui.Button(receiver).Label("UNDERSTOOD — LET'S STRESS THE LINES")
                .Style("margin-top", "1.1rem").Style("padding", ".65rem 1rem").Style("border", $"1px solid {Cyan}88").Style("background", $"{Cyan}10")
                .Style("color", Cyan).Style("font-family", "inherit").Style("font-size", ".5rem").Style("letter-spacing", ".14em").Style("cursor", "pointer").OnClick(dismiss));

    private static ElementBuilder Hud(object receiver, string avatarName, int density, int lineCount, double renderMicroseconds, double textureMicroseconds, int gpuPayloadBytes)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1rem").Style("top", "1rem").Style("z-index", "30")
            .Style("padding", ".55rem .7rem").Style("border", $"1px solid {Cyan}66")
            .Style("background", "rgba(1,4,10,.84)").Style("color", White)
            .Style("font-size", ".48rem").Style("line-height", "1.65")
            .Style("letter-spacing", ".1em").Style("pointer-events", "none")
            .Text($"SINGULARITY MANSION // LINE STRESS LAB\nAVATAR // {avatarName}\nDENSITY // {density}X\nLINES // {lineCount:N0}\nRENDER CONVERSION // {renderMicroseconds:0.0} μs\nRGBA TEXEL BUILD // {textureMicroseconds:0.0} μs\nGPU PAYLOAD // {gpuPayloadBytes:N0} BYTES");

    private static ElementBuilder DensityControls(object receiver, int density, Action<int> setDensity)
    {
        var panel = WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1rem").Style("bottom", "1rem").Style("z-index", "30")
            .Style("display", "flex").Style("gap", ".25rem").Style("align-items", "center")
            .Style("padding", ".35rem").Style("border", $"1px solid {Yellow}55")
            .Style("background", "rgba(1,4,10,.84)");

        panel.Child(WorkshopGui.Element(receiver, "span")
            .Style("padding", ".3rem .35rem").Style("color", Yellow)
            .Style("font-size", ".42rem").Style("letter-spacing", ".1em")
            .Text("PRESS THE RENDERER"));

        foreach (var level in new[] { 1, 2, 4, 8, 16 })
        {
            panel.Child(WorkshopGui.Button(receiver).Label($"{level}X")
                .Style("padding", ".3rem .42rem")
                .Style("border", $"1px solid {(level == density ? Cyan : White)}55")
                .Style("background", "rgba(1,4,10,.75)")
                .Style("color", level == density ? Cyan : White)
                .Style("font-family", "inherit").Style("font-size", ".42rem")
                .Style("cursor", "pointer")
                .OnClick(() => setDensity(level)));
        }

        return panel;
    }

    private static string ViewBox(double avatarX, double avatarY)
        => $"{avatarX - 33.333:0.###} {avatarY - 33.333:0.###} 66.666 66.666";
}
