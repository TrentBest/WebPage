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
    public static ElementBuilder Build(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, int detailLevel, Action<int> setDetailLevel, Action<KeyboardEventArgs> onKeyDown, Func<MouseEventArgs, Task> onWorldClick, Func<string, Task> interact)
    {
        var root = WorkshopGui.Panel(receiver).Style("position", "fixed").Style("inset", "0").Style("width", "100vw").Style("height", "100vh").Style("overflow", "hidden").Style("background", "#020a12").Style("color", White).Style("font-family", "Consolas, 'Courier New', monospace").Attribute("id", "workshop-spatial-experience");
        root.Content(Styles(receiver));
        root.Content(World(receiver, scene, avatarX, avatarY, detailLevel, onKeyDown, onWorldClick, interact));
        root.Content(ViewBar(receiver, detailLevel, setDetailLevel));
        root.Content(ConstructionBanner(receiver));
        return root;
    }

    private static ElementBuilder World(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, int detailLevel, Action<KeyboardEventArgs> onKeyDown, Func<MouseEventArgs, Task> onWorldClick, Func<string, Task> interact)
    {
        var camera = new SpatialCamera(avatarX, avatarY);
        var world = WorkshopGui.Panel(receiver).Style("position", "absolute").Style("inset", "0").Style("overflow", "hidden").Style("outline", "none").Attribute("tabindex", "0").Attribute("autofocus", "autofocus").AriaLabel("The Workshop spatial experience. Click anywhere to move your avatar.").OnKeyDown(onKeyDown).OnClick(args => _ = onWorldClick(args)).PreventDefault("onkeydown");
        var worldPlane = WorkshopGui.Panel(receiver).Style("position", "absolute").Style("left", "0").Style("top", "0").Style("width", $"{SpatialCamera.WorldWidthVw:0.###}vw").Style("height", $"{SpatialCamera.WorldHeightVh:0.###}vh").Style("transform", $"translate({camera.OffsetVw:0.###}vw, {camera.OffsetVh:0.###}vh)").Style("transition", "transform .18s ease-in-out").Style("transform-origin", "0 0");
        worldPlane.Content(Grid(receiver, detailLevel));
        worldPlane.Content(FloorPlan(receiver));
        foreach (var vehicle in scene.ConstructionVehicles) worldPlane.Content(Vehicle(receiver, vehicle));
        foreach (var item in scene.Interactables) worldPlane.Content(Interactable(receiver, item, interact));
        world.Content(worldPlane);
        world.Content(Avatar(receiver));
        return world;
    }

    private static ElementBuilder Grid(object receiver, int detailLevel)
    {
        var svg = WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none").Style("position", "absolute").Style("left", "0").Style("top", "0").Style("width", $"{SpatialCamera.WorldWidthVw:0.###}vw").Style("height", $"{SpatialCamera.WorldHeightVh:0.###}vh").Style("pointer-events", "none").Style("opacity", detailLevel >= 32 ? ".48" : ".28");
        var spacing = detailLevel >= 48 ? 2.5 : detailLevel >= 24 ? 5 : 10;
        for (var i = 0; i <= 100 / spacing; i++)
        {
            var p = i * spacing;
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", p).Attribute("y1", "0").Attribute("x2", p).Attribute("y2", "100").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "0").Attribute("y1", p).Attribute("x2", "100").Attribute("y2", p).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".13").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder FloorPlan(object receiver) => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", $"{SpatialCamera.WorldToVw(4):0.###}vw").Style("top", $"{SpatialCamera.WorldToVh(8):0.###}vh").Style("width", $"{SpatialCamera.WorldToVw(92):0.###}vw").Style("height", $"{SpatialCamera.WorldToVh(84):0.###}vh").Style("box-sizing", "border-box").Style("border", $"1px solid {Cyan}33").Style("box-shadow", $"inset 0 0 50px {Cyan}08").Style("pointer-events", "none").Content(WorkshopGui.Element(receiver, "span").Style("position", "absolute").Style("left", "1rem").Style("top", ".4rem").Style("font-size", ".48rem").Style("letter-spacing", ".2em").Style("color", $"{Cyan}99").Text("WORKSHOP COMPLEX // SPATIAL MAP"));

    private static ElementBuilder Interactable(object receiver, SpatialInteractable item, Func<string, Task> interact)
    {
        var accent = item.Id switch { "image-tools" => Magenta, "npc-studio" => Yellow, "fsm-bench" => Cyan, "storage-bins" => Green, "blueprint-library" => White, _ => Cyan };
        var b = item.Bounds;
        var surface = WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", $"{SpatialCamera.WorldToVw(b.X):0.###}vw").Style("top", $"{SpatialCamera.WorldToVh(b.Y):0.###}vh").Style("width", $"{SpatialCamera.WorldToVw(b.Width):0.###}vw").Style("height", $"{SpatialCamera.WorldToVh(b.Height):0.###}vh").Style("box-sizing", "border-box").Style("border", $"1px solid {accent}aa").Style("background", $"linear-gradient(180deg, {accent}20, #05070c 28%, #03050a 100%)").Style("box-shadow", $"inset 0 -12px 0 rgba(0,0,0,.3), 0 4px 0 rgba(0,0,0,.45)").Style("cursor", "pointer").Style("overflow", "visible").AriaLabel(item.Name).Title($"{item.Name} // spatial building").OnMouseEnter(() => item.OnHover(new NormalizedPointer(.5, .5))).OnMouseMove(_ => item.OnHover(new NormalizedPointer(.5, .5))).OnMouseLeave(item.OnHoverExit).OnClick(() => interact(item.Id)).StopPropagation("onclick");
        if (item.IsBreathing) surface.Style("animation", "workshop-interactable-breathe 1.25s ease-in-out infinite");
        surface.Content(BuildingRoof(receiver, accent));
        surface.Content(BuildingDoor(receiver, accent));
        surface.Content(BuildingSign(receiver, item.Name, accent));
        return surface;
    }

    private static ElementBuilder BuildingRoof(object receiver, string accent) => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "3%").Style("top", "-10%").Style("width", "94%").Style("height", "24%").Style("background", $"linear-gradient(180deg, {accent}66, {accent}18)").Style("border", $"1px solid {accent}aa").Style("clip-path", "polygon(7% 0,93% 0,100% 100%,0 100%)").Style("pointer-events", "none");
    private static ElementBuilder BuildingDoor(object receiver, string accent) => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "44%").Style("bottom", "0").Style("width", "12%").Style("height", "28%").Style("min-width", "12px").Style("background", "#010205").Style("border", $"1px solid {accent}99").Style("box-shadow", $"0 0 8px {accent}33").Style("pointer-events", "none");
    private static ElementBuilder BuildingSign(object receiver, string name, string accent) => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "50%").Style("top", "40%").Style("transform", "translate(-50%,-50%)").Style("padding", ".2rem .4rem").Style("border", $"1px solid {accent}88").Style("background", "rgba(1,3,7,.88)").Style("color", White).Style("font-size", ".55rem").Style("letter-spacing", ".08em").Style("white-space", "nowrap").Style("pointer-events", "none").Text(name);
    private static ElementBuilder Vehicle(object receiver, ConstructionVehicle vehicle) => WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", $"{SpatialCamera.WorldToVw(vehicle.Position.X):0.###}vw").Style("top", $"{SpatialCamera.WorldToVh(vehicle.Position.Y):0.###}vh").Style("transform", "translate(-50%,-50%)").Style("z-index", "4").Style("animation", $"workshop-vehicle-{vehicle.Kind.ToString().ToLowerInvariant()} 6s linear infinite").Content(ConstructionVehicleGuiBuilder.Build(receiver, vehicle));
    private static ElementBuilder Avatar(object receiver) => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "50%").Style("top", "50%").Style("width", "14px").Style("height", "14px").Style("border", $"1px solid {White}").Style("border-radius", "50%").Style("box-shadow", $"0 0 12px {White}88").Style("transform", "translate(-50%,-50%)").Style("z-index", "10").Style("pointer-events", "none");
    private static ElementBuilder ViewBar(object receiver, int detailLevel, Action<int> setDetailLevel) => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("right", "1rem").Style("top", "1rem").Style("z-index", "30").Style("display", "flex").Style("gap", ".25rem").Style("align-items", "center").Style("padding", ".35rem").Style("border", $"1px solid {Cyan}55").Style("background", "rgba(1,4,10,.86)").Content(ViewButton(receiver, "−", () => setDetailLevel(detailLevel - 1))).Content(WorkshopGui.Element(receiver, "span").Style("padding", "0 .35rem").Style("font-size", ".45rem").Style("letter-spacing", ".12em").Text($"DETAIL {detailLevel:00}/64")).Content(ViewButton(receiver, "+", () => setDetailLevel(detailLevel + 1))).Content(ViewButton(receiver, "STUD", () => setDetailLevel(64)));
    private static ElementBuilder ViewButton(object receiver, string text, Action action) => WorkshopGui.Element(receiver, "div").Style("padding", ".2rem .3rem").Style("border", $"1px solid {Cyan}44").Style("color", White).Style("cursor", "pointer").Style("user-select", "none").Style("font-size", ".45rem").OnClick(action).Text(text);
    private static ElementBuilder ConstructionBanner(object receiver) => WorkshopGui.Element(receiver, "div").Style("position", "fixed").Style("left", "1rem").Style("top", "1rem").Style("z-index", "30").Style("padding", ".35rem .55rem").Style("border", $"1px solid {Yellow}88").Style("background", "rgba(1,4,10,.82)").Style("color", Yellow).Style("font-size", ".5rem").Style("letter-spacing", ".18em").Text("⚠ UNDER CONSTRUCTION // WORKSHOP COMPLEX EXPANDING");
    private static ElementBuilder Styles(object receiver) => WorkshopGui.Element(receiver, "style").Text("@keyframes workshop-interactable-breathe{0%,100%{filter:brightness(1);transform:scale(1)}50%{filter:brightness(1.8);transform:scale(1.025)}}@keyframes workshop-vehicle-crane{0%,100%{translate:0 0}50%{translate:8vw 2vh}}@keyframes workshop-vehicle-rover{0%,100%{translate:0 0}50%{translate:-7vw 1vh}}@keyframes workshop-vehicle-lifter{0%,100%{translate:0 0}50%{translate:5vw -2vh}}@keyframes workshop-vehicle-backhoe{0%,100%{translate:0 0}50%{translate:6vw -1vh}}");
}
