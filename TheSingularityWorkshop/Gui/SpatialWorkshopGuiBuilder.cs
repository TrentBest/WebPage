using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>Renderer-neutral spatial Workshop presentation built with recursive GUI primitives.</summary>
public static class SpatialWorkshopGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Magenta = "#ff38d1";

    public static ElementBuilder Build(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY,
        int detailLevel, Action<int> setDetailLevel, Action<KeyboardEventArgs> onKeyDown, Func<string, Task> interact)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden").Style("background", "#020a12")
            .Style("color", White).Style("font-family", "Consolas, 'Courier New', monospace")
            .Attribute("id", "workshop-spatial-experience");

        root.Content(Styles(receiver));
        root.Content(World(receiver, scene, avatarX, avatarY, detailLevel, onKeyDown, interact));
        root.Content(ViewBar(receiver, detailLevel, setDetailLevel));
        root.Content(ConstructionBanner(receiver));
        return root;
    }

    private static ElementBuilder World(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY,
        int detailLevel, Action<KeyboardEventArgs> onKeyDown, Func<string, Task> interact)
    {
        var world = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0").Style("overflow", "hidden")
            .Attribute("tabindex", "0")
            .AriaLabel("The Workshop spatial experience. You are inside the Forge.")
            .OnKeyDown(onKeyDown).PreventDefault("onkeydown");

        var camera = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("transform", $"translate(calc(50vw - {avatarX:0.##}vw), calc(50vh - {avatarY:0.##}vh))")
            .Style("transition", "transform .28s linear");

        camera.Content(Grid(receiver, detailLevel));
        camera.Content(FloorPlan(receiver));
        foreach (var vehicle in scene.ConstructionVehicles)
            camera.Content(Vehicle(receiver, vehicle));
        foreach (var item in scene.Interactables)
            camera.Content(Interactable(receiver, item, interact));

        world.Content(camera);
        world.Content(Avatar(receiver));
        return world;
    }

    private static ElementBuilder Grid(object receiver, int detailLevel)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%").Style("pointer-events", "none")
            .Style("opacity", detailLevel >= 32 ? ".48" : ".28");

        var spacing = detailLevel >= 48 ? 2.5 : detailLevel >= 24 ? 5 : 10;
        for (var i = 0; i <= 100 / spacing; i++)
        {
            var p = i * spacing;
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", p).Attribute("y1", "0").Attribute("x2", p).Attribute("y2", "100")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "0").Attribute("y1", p).Attribute("x2", "100").Attribute("y2", p)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder FloorPlan(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4%").Style("top", "8%")
            .Style("width", "92%").Style("height", "84%")
            .Style("border", $"1px solid {Cyan}33")
            .Style("box-shadow", $"inset 0 0 50px {Cyan}08")
            .Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("left", "1rem").Style("top", ".4rem")
                .Style("font-size", ".48rem").Style("letter-spacing", ".2em")
                .Style("color", $"{Cyan}99").Text("FORGE COMPLEX // LIVE FLOOR PLAN"));

    private static ElementBuilder Interactable(object receiver, SpatialInteractable item, Func<string, Task> interact)
    {
        var accent = item.Id switch
        {
            "image-tools" => Magenta,
            "npc-studio" => Yellow,
            "fsm-bench" => Cyan,
            "storage-bins" => Green,
            "blueprint-library" => White,
            _ => Cyan
        };
        var b = item.Bounds;
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"{b.X:0.##}%").Style("top", $"{b.Y:0.##}%")
            .Style("width", $"{b.Width:0.##}%").Style("height", $"{b.Height:0.##}%")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {accent}88")
            .Style("background", $"linear-gradient(135deg, {accent}12, transparent 60%)")
            .Style("cursor", "pointer")
            .AriaLabel(item.Name).Title($"{item.Name} // spatial interactable")
            .OnMouseEnter(() => item.OnHover(new NormalizedPointer(.5, .5)))
            .OnMouseMove(_ => item.OnHover(new NormalizedPointer(.5, .5)))
            .OnMouseLeave(item.OnHoverExit)
            .OnClick(() => interact(item.Id));

        if (item.IsBreathing)
            surface.Style("animation", "workshop-interactable-breathe 1.25s ease-in-out infinite");

        surface.Content(WorkshopGui.Element(receiver, "span")
            .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
            .Style("transform", "translate(-50%,-50%)")
            .Style("font-size", item.Id == "forge" ? ".8rem" : ".48rem")
            .Style("letter-spacing", ".1em").Style("white-space", "nowrap")
            .Style("pointer-events", "none").Text(item.Name));
        return surface;
    }

    private static ElementBuilder Vehicle(object receiver, ConstructionVehicle vehicle)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"{vehicle.Position.X:0.##}%").Style("top", $"{vehicle.Position.Y:0.##}%")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "4")
            .Style("pointer-events", "none")
            .Style("animation", $"workshop-vehicle-{vehicle.Kind.ToString().ToLowerInvariant()} 6s linear infinite")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("padding", ".2rem .3rem")
                .Style("border", $"1px solid {Yellow}66")
                .Style("background", "rgba(1,4,10,.82)")
                .Style("font-size", ".42rem").Style("letter-spacing", ".1em")
                .Text($"{vehicle.Kind.ToString().ToUpperInvariant()} // {vehicle.Name}"));

    private static ElementBuilder Avatar(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
            .Style("width", "14px").Style("height", "14px")
            .Style("border", $"1px solid {White}").Style("border-radius", "50%")
            .Style("box-shadow", $"0 0 12px {White}88")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "10")
            .Style("pointer-events", "none");

    private static ElementBuilder ViewBar(object receiver, int detailLevel, Action<int> setDetailLevel)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("right", "1rem").Style("top", "1rem")
            .Style("z-index", "30").Style("display", "flex").Style("gap", ".25rem")
            .Style("align-items", "center").Style("padding", ".35rem")
            .Style("border", $"1px solid {Cyan}55")
            .Style("background", "rgba(1,4,10,.86)")
            .Content(ViewButton(receiver, "−", () => setDetailLevel(detailLevel - 1)))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("padding", "0 .35rem").Style("font-size", ".45rem")
                .Style("letter-spacing", ".12em").Text($"DETAIL {detailLevel:00}/64"))
            .Content(ViewButton(receiver, "+", () => setDetailLevel(detailLevel + 1)))
            .Content(ViewButton(receiver, "STUD", () => setDetailLevel(64)));

    private static ElementBuilder ViewButton(object receiver, string text, Action action)
        => WorkshopGui.Element(receiver, "div")
            .Style("padding", ".2rem .3rem").Style("border", $"1px solid {Cyan}44")
            .Style("color", White).Style("cursor", "pointer").Style("user-select", "none")
            .Style("font-size", ".45rem").OnClick(action).Text(text);

    private static ElementBuilder ConstructionBanner(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("left", "1rem").Style("top", "1rem")
            .Style("z-index", "30").Style("padding", ".35rem .55rem")
            .Style("border", $"1px solid {Yellow}88")
            .Style("background", "rgba(1,4,10,.82)").Style("color", Yellow)
            .Style("font-size", ".5rem").Style("letter-spacing", ".18em")
            .Text("⚠ UNDER CONSTRUCTION // WORKSHOP COMPLEX EXPANDING");

    private static ElementBuilder Styles(object receiver)
        => WorkshopGui.Element(receiver, "style")
            .Text("@keyframes workshop-interactable-breathe{0%,100%{filter:brightness(1);transform:scale(1)}50%{filter:brightness(1.8);transform:scale(1.025)}}@keyframes workshop-vehicle-crane{0%,100%{translate:0 0}50%{translate:8vw 2vh}}@keyframes workshop-vehicle-rover{0%,100%{translate:0 0}50%{translate:-7vw 1vh}}@keyframes workshop-vehicle-lifter{0%,100%{translate:0 0}50%{translate:5vw -2vh}}");
}
