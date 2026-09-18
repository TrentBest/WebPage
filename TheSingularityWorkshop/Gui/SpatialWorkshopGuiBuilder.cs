using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>Renderer-neutral spatial Workshop presentation built from recursive GUI primitives.</summary>
public static class SpatialWorkshopGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Magenta = "#ff38d1";

    /// <summary>Builds the avatar-centered Workshop world and its interaction surface.</summary>
    public static ElementBuilder Build(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, int detailLevel, Action<int> setDetailLevel, Action<KeyboardEventArgs> onKeyDown, Func<MouseEventArgs, Task> onWorldClick, Func<string, Task> interact, double zoom = 1d, Action<double>? setZoom = null, Action? showMap = null, Func<Task>? enterConstructionSite = null, bool constructionTourActive = false, string? constructionTourMessage = null, bool constructionWelcomeVisible = false, Action? requestConstructionTour = null, Action? exitConstructionSite = null)
    {
        var effectiveZoom = new SpatialCamera(avatarX, avatarY, zoom).Clamped().Zoom;
        var zoomEnabled = scene.View.ZoomEnabled && setZoom is not null;
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden").Style("background", "#020a12").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace").Attribute("id", "workshop-spatial-experience");
        root.Content(Styles(receiver));
        root.Content(World(receiver, scene, avatarX, avatarY, detailLevel, effectiveZoom, zoomEnabled, onKeyDown, onWorldClick, interact, setZoom, enterConstructionSite, constructionTourActive, constructionTourMessage));
        root.Content(ViewBar(receiver, detailLevel, setDetailLevel, effectiveZoom, zoomEnabled, setZoom, showMap));
        root.Content(ConstructionBanner(receiver));
        if (constructionWelcomeVisible)
            root.Content(ConstructionHandover(receiver, scene.ConstructionSite, requestConstructionTour, exitConstructionSite));
        return root;
    }

    private static ElementBuilder World(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, int detailLevel, double zoom, bool zoomEnabled, Action<KeyboardEventArgs> onKeyDown, Func<MouseEventArgs, Task> onWorldClick, Func<string, Task> interact, Action<double>? setZoom, Func<Task>? enterConstructionSite, bool constructionTourActive, string? constructionTourMessage)
    {
        var camera = new SpatialCamera(avatarX, avatarY, zoom).Clamped();
        var world = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0").Style("overflow", "hidden").Style("outline", "none")
            .Attribute("tabindex", "0").Attribute("autofocus", "autofocus")
            .AriaLabel("The Workshop spatial experience. Click anywhere to move your avatar.")
            .OnKeyDown(onKeyDown).OnClick(args => _ = onWorldClick(args)).PreventDefault("onkeydown");

        if (zoomEnabled && setZoom is not null)
            world.OnWheel(args => setZoom(ZoomFromWheel(zoom, args.DeltaY))).PreventDefault("onwheel");

        var worldPlane = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "0").Style("top", "0")
            .Style("width", $"{SpatialCamera.WorldWidthVw * zoom:0.###}vw")
            .Style("height", $"{SpatialCamera.WorldHeightVh * zoom:0.###}vh")
            .Style("transform", $"translate({camera.OffsetVw:0.###}vw, {camera.OffsetVh:0.###}vh)")
            .Style("transition", "transform .18s ease-in-out").Style("transform-origin", "0 0");
        worldPlane.Content(Grid(receiver, detailLevel, zoom));
        worldPlane.Content(FloorPlan(receiver, zoom));
        worldPlane.Content(SpatialGeometryGuiBuilder.Build(receiver, scene, interact, zoom));
        worldPlane.Content(ConstructionSite(receiver, scene.ConstructionSite, zoom, enterConstructionSite));
        foreach (var vehicle in scene.ConstructionVehicles) worldPlane.Content(Vehicle(receiver, vehicle, zoom));
        foreach (var item in scene.Interactables) worldPlane.Content(Interactable(receiver, item, interact, zoom));
        world.Content(worldPlane);
        world.Content(Avatar(receiver));
        if (constructionTourActive)
            world.Content(Foreman(receiver, constructionTourMessage ?? "Stay close. I will show you what is being built."));
        return world;
    }

    private static double ZoomFromWheel(double currentZoom, double deltaY)
    {
        var factor = deltaY < 0 ? 1.12 : 1 / 1.12;
        return Math.Clamp(currentZoom * factor, SpatialCamera.MinZoom, SpatialCamera.MaxZoom);
    }

    private static ElementBuilder Grid(object receiver, int detailLevel, double zoom)
    {
        var siteSurface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{SpatialCamera.WorldToVw(b.X, zoom):0.###}vw").Style("top", $"{SpatialCamera.WorldToVh(b.Y, zoom):0.###}vh")
            .Style("width", $"{SpatialCamera.WorldToVw(b.Width, zoom):0.###}vw").Style("height", $"{SpatialCamera.WorldToVh(b.Height, zoom):0.###}vh")
            .Style("z-index", "7").Style("cursor", enterConstructionSite is null ? "default" : "pointer")
            .Style("background", $"{Yellow}05").Style("border", $"1px dashed {Yellow}22").AriaLabel("Construction site")
            .Title("WORKSHOP EXPANSION SITE // enter construction site");
        if (enterConstructionSite is not null)
            siteSurface.OnClick(() => _ = enterConstructionSite()).StopPropagation("onclick");

        var svg = WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("left", "0").Style("top", "0")
            .Style("width", $"{SpatialCamera.WorldWidthVw * zoom:0.###}vw").Style("height", $"{SpatialCamera.WorldHeightVh * zoom:0.###}vh")
            .Style("pointer-events", "none").Style("opacity", detailLevel >= 32 ? ".48" : ".28");
        var spacing = detailLevel >= 48 ? 2.5 : detailLevel >= 24 ? 5 : 10;
        for (var i = 0; i <= 100 / spacing; i++)
        {
            var p = i * spacing;
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", p).Attribute("y1", "0").Attribute("x2", p).Attribute("y2", "100").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "0").Attribute("y1", p).Attribute("x2", "100").Attribute("y2", p).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder FloorPlan(object receiver, double zoom)
        => WorkshopGui.Element(receiver, "div").Style("position", "absolute")
            .Style("left", $"{SpatialCamera.WorldToVw(4, zoom):0.###}vw").Style("top", $"{SpatialCamera.WorldToVh(8, zoom):0.###}vh")
            .Style("width", $"{SpatialCamera.WorldToVw(92, zoom):0.###}vw").Style("height", $"{SpatialCamera.WorldToVh(84, zoom):0.###}vh")
            .Style("box-sizing", "border-box").Style("border", $"1px solid {Cyan}33")
            .Style("box-shadow", $"inset 0 0 50px {Cyan}08").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "span").Style("position", "absolute").Style("left", "1rem").Style("top", ".4rem")
                .Style("font-size", ".48rem").Style("letter-spacing", ".2em").Style("color", $"{Cyan}99")
                .Text("WORKSHOP COMPLEX // SPATIAL SCHEMATIC"));

    private static ElementBuilder ConstructionSite(object receiver, SpatialConstructionSite site, double zoom, Func<Task>? enterConstructionSite)
    {
        var b = site.Bounds;
        var f = site.FoundationBounds;
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("left", "0").Style("top", "0")
            .Style("width", $"{SpatialCamera.WorldWidthVw * zoom:0.###}vw").Style("height", $"{SpatialCamera.WorldHeightVh * zoom:0.###}vh")
            .Style("pointer-events", "none").Style("z-index", "3");

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", b.X).Attribute("y", b.Y).Attribute("width", b.Width).Attribute("height", b.Height)
            .Attribute("fill", "none").Attribute("stroke", Yellow).Attribute("stroke-opacity", ".8")
            .Attribute("stroke-width", ".28").Attribute("stroke-dasharray", "1.2 .8"));

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", f.X).Attribute("y", f.Y).Attribute("width", f.Width).Attribute("height", f.Height)
            .Attribute("fill", "#ffd34d").Attribute("fill-opacity", ".035")
            .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".42")
            .Attribute("stroke-width", ".2"));

        for (var x = b.X + 2; x < b.X + b.Width; x += 2)
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", x).Attribute("y1", b.Y - .5).Attribute("x2", x).Attribute("y2", b.Y + .8).Attribute("stroke", Yellow).Attribute("stroke-opacity", ".65").Attribute("stroke-width", ".22"));
        var frameCount = Math.Max(1, (int)Math.Round(site.Progress * 4));
        for (var frame = 0; frame < frameCount; frame++)
        {
            var x = f.X + (frame + 1) * f.Width / (frameCount + 1);
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", x).Attribute("y1", f.Y).Attribute("x2", x).Attribute("y2", f.Y + f.Height).Attribute("stroke", Yellow).Attribute("stroke-opacity", ".32").Attribute("stroke-width", ".3"));
        }

        for (var y = b.Y + 2; y < b.Y + b.Height; y += 2)
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", b.X - .5).Attribute("y1", y).Attribute("x2", b.X + .8).Attribute("y2", y).Attribute("stroke", Yellow).Attribute("stroke-opacity", ".65").Attribute("stroke-width", ".22"));

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", b.X + b.Width / 2).Attribute("y", b.Y - 1.2)
            .Attribute("fill", Yellow).Attribute("font-size", ".7").Attribute("font-family", "monospace")
            .Attribute("text-anchor", "middle").Text($"{site.Name} // {site.Phase.ToString().ToUpperInvariant()} // {site.Progress:P0}"));
        siteSurface.Content(svg);
        siteSurface.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "50%").Style("top", "50%").Style("transform", "translate(-50%,-50%)").Style("padding", ".3rem .5rem").Style("border", $"1px solid {Yellow}66").Style("background", "rgba(1,4,10,.86)").Style("color", Yellow).Style("font-size", ".42rem").Style("letter-spacing", ".1em").Style("text-align", "center").Style("pointer-events", "none").Text($"{site.FuturePurpose} // ENTER SITE"));
        return siteSurface;
    }

    private static ElementBuilder Interactable(object receiver, SpatialInteractable item, Func<string, Task> interact, double zoom)
    {
        var b = item.Bounds;
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"{SpatialCamera.WorldToVw(b.X, zoom):0.###}vw")
            .Style("top", $"{SpatialCamera.WorldToVh(b.Y, zoom):0.###}vh")
            .Style("width", $"{SpatialCamera.WorldToVw(b.Width, zoom):0.###}vw")
            .Style("height", $"{SpatialCamera.WorldToVh(b.Height, zoom):0.###}vh")
            .Style("box-sizing", "border-box").Style("background", "transparent").Style("cursor", "pointer")
            .Style("z-index", "6").Style("overflow", "visible").AriaLabel(item.Name)
            .Title($"{item.Name} // schematic structure")
            .OnMouseEnter(() => item.OnHover(new NormalizedPointer(.5, .5)))
            .OnMouseMove(_ => item.OnHover(new NormalizedPointer(.5, .5)))
            .OnMouseLeave(item.OnHoverExit).OnClick(() => interact(item.Id)).StopPropagation("onclick");
        if (item.IsBreathing) surface.Style("animation", "workshop-interactable-breathe 1.25s ease-in-out infinite");
        surface.Content(BuildingSign(receiver, item.Name));
        return surface;
    }

    private static ElementBuilder BuildingSign(object receiver, string name)
        => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "50%").Style("top", "40%")
            .Style("transform", "translate(-50%,-50%)").Style("padding", ".2rem .4rem")
            .Style("border", "1px solid rgba(247,247,255,.24)").Style("background", "rgba(1,3,7,.82)")
            .Style("color", White).Style("font-size", ".55rem").Style("letter-spacing", ".08em")
            .Style("white-space", "nowrap").Style("pointer-events", "none").Text(name);

    private static ElementBuilder Vehicle(object receiver, ConstructionVehicle vehicle, double zoom)
        => WorkshopGui.Element(receiver, "div").Style("position", "absolute")
            .Style("left", $"{SpatialCamera.WorldToVw(vehicle.Position.X, zoom):0.###}vw")
            .Style("top", $"{SpatialCamera.WorldToVh(vehicle.Position.Y, zoom):0.###}vh")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "4")
            .Style("animation", $"workshop-vehicle-{vehicle.Kind.ToString().ToLowerInvariant()} 6s linear infinite")
            .Content(ConstructionVehicleGuiBuilder.Build(receiver, vehicle));

    private static ElementBuilder ConstructionHandover(object receiver, SpatialConstructionSite site, Action? requestTour, Action? exitSite)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("inset", "0").Style("z-index", "100").Style("display", "flex").Style("align-items", "center").Style("justify-content", "center").Style("background", "rgba(0,0,0,.58)")
            .Content(WorkshopGui.Element(receiver, "div").Style("width", "min(620px,84vw)").Style("padding", "1.3rem").Style("box-sizing", "border-box").Style("border", $"1px solid {Yellow}aa").Style("background", "rgba(2,8,18,.97)").Style("box-shadow", $"0 0 60px {Yellow}18")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".58rem").Style("letter-spacing", ".22em").Text("CONSTRUCTION SITE // FOREMAN"))
                .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".9rem").Style("color", White).Style("font-family", "system-ui,sans-serif").Style("font-size", "1rem").Text("Hold up. This site is currently under construction."))
                .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".6rem").Style("color", "#b8c9d0").Style("font-family", "system-ui,sans-serif").Style("font-size", ".72rem").Style("line-height", "1.55").Text($"Once complete, it will be {site.FuturePurpose.ToLowerInvariant()} The site is currently at {site.Progress:P0} of its planned build."))
                .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".75rem").Style("padding", ".55rem").Style("border-left", $"2px solid {Yellow}88").Style("color", Yellow).Style("font-family", "system-ui,sans-serif").Style("font-size", ".68rem").Text("You can look around from the perimeter, or request a guided tour from the foreman."))
                .Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("gap", ".5rem").Style("margin-top", "1rem")
                    .Content(WorkshopGui.Button(receiver).Label("REQUEST TOUR →").Style("padding", ".55rem .8rem").Style("border", $"1px solid {Yellow}88").Style("background", $"{Yellow}10").Style("color", Yellow).Style("font-family", "inherit").Style("font-size", ".46rem").Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(requestTour ?? (() => { })))
                    .Content(WorkshopGui.Button(receiver).Label("STAY OUTSIDE").Style("padding", ".55rem .8rem").Style("border", "1px solid #ffffff33").Style("background", "transparent").Style("color", White).Style("font-family", "inherit").Style("font-size", ".46rem").Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(exitSite ?? (() => { }))));

    private static ElementBuilder Foreman(object receiver, string message)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "calc(50% + 42px)").Style("top", "50%").Style("z-index", "80").Style("transform", "translateY(-50%)").Style("max-width", "min(360px,42vw)").Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("align-items", "center").Style("gap", ".45rem")
                .Content(WorkshopGui.Element(receiver, "div").Style("width", "20px").Style("height", "24px").Style("border", $"1px solid {Yellow}aa").Style("background", $"{Yellow}18").Style("clip-path", "polygon(50% 0,90% 25%,82% 100%,18% 100%,10% 25%)"))
                .Content(WorkshopGui.Element(receiver, "div").Style("padding", ".45rem .6rem").Style("border", $"1px solid {Yellow}66").Style("background", "rgba(1,4,10,.92)").Style("color", White).Style("font-family", "system-ui,sans-serif").Style("font-size", ".68rem").Text(message)));

    private static ElementBuilder Avatar(object receiver)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "50%").Style("top", "50%")
            .Style("width", "14px").Style("height", "14px").Style("border", $"1px solid {White}")
            .Style("border-radius", "50%").Style("box-shadow", $"0 0 12px {White}88")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "10").Style("pointer-events", "none");

    private static ElementBuilder ViewBar(object receiver, int detailLevel, Action<int> setDetailLevel, double zoom, bool zoomEnabled, Action<double>? setZoom, Action? showMap)
    {
        var bar = WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("right", "1rem").Style("top", "1rem")
            .Style("z-index", "30").Style("display", "flex").Style("gap", ".25rem").Style("align-items", "center")
            .Style("padding", ".35rem").Style("border", $"1px solid {Cyan}55").Style("background", "rgba(1,4,10,.86)");
        bar.Content(ViewButton(receiver, "−", () => setDetailLevel(detailLevel - 1)));
        bar.Content(WorkshopGui.Element(receiver, "span").Style("padding", "0 .35rem").Style("font-size", ".45rem").Style("letter-spacing", ".12em").Text($"DETAIL {detailLevel:00}/64"));
        bar.Content(ViewButton(receiver, "+", () => setDetailLevel(detailLevel + 1)));
        bar.Content(ViewButton(receiver, "STUD", () => setDetailLevel(64)));
        if (zoomEnabled && setZoom is not null)
        {
            bar.Content(WorkshopGui.Element(receiver, "span").Style("margin-left", ".35rem").Style("padding", "0 .35rem").Style("font-size", ".45rem").Style("letter-spacing", ".12em").Style("color", Yellow).Text($"ZOOM {zoom:0.00}X"));
            bar.Content(ViewButton(receiver, "RESET", () => setZoom(1d)));
        }
        if (showMap is not null)
            bar.Content(ViewButton(receiver, "MAP", showMap));
        return bar;
    }

    private static ElementBuilder ViewButton(object receiver, string text, Action action)
        => WorkshopGui.Element(receiver, "div").Style("padding", ".2rem .3rem").Style("border", $"1px solid {Cyan}44")
            .Style("color", White).Style("cursor", "pointer").Style("user-select", "none").Style("font-size", ".45rem").OnClick(action).Text(text);

    private static ElementBuilder ConstructionBanner(object receiver)
        => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "1rem").Style("top", "1rem")
            .Style("z-index", "30").Style("padding", ".35rem .55rem").Style("border", $"1px solid {Yellow}88")
            .Style("background", "rgba(1,4,10,.82)").Style("color", Yellow).Style("font-size", ".5rem")
            .Style("letter-spacing", ".18em").Text("⚠ UNDER CONSTRUCTION // SCHEMATIC MODE");

    private static ElementBuilder Styles(object receiver)
        => WorkshopGui.Element(receiver, "style").Text("@keyframes workshop-interactable-breathe{0%,100%{filter:brightness(1);transform:scale(1)}50%{filter:brightness(1.8);transform:scale(1.025)}}@keyframes workshop-vehicle-crane{0%,100%{translate:0 0}50%{translate:8vw 2vh}}@keyframes workshop-vehicle-rover{0%,100%{translate:0 0}50%{translate:-7vw 1vh}}@keyframes workshop-vehicle-lifter{0%,100%{translate:0 0}50%{translate:5vw -2vh}}@keyframes workshop-vehicle-backhoe{0%,100%{translate:0 0}50%{translate:6vw -1vh}}");
}
