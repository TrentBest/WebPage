namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the spatial perception layer for an interior scene.
/// The visitor remains an avatar inside the scene; the builder does not replace
/// the world with a conventional application surface.
/// </summary>
public static class SpatialInteriorGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";

    /// <summary>Builds a first-person spatial approximation of an interior scene.</summary>
    public static ElementBuilder Build(object receiver, string title, string description, string avatarName, Action exitScene)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", "#020812")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text(
            "@keyframes spatial-interior-breathe{0%,100%{filter:brightness(1)}50%{filter:brightness(1.55)}}"));

        var floor = WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("background", "linear-gradient(180deg,#071522 0%,#03070d 62%,#02040a 100%)")
            .Style("overflow", "hidden");

        floor.Content(Grid(receiver));
        floor.Content(ForgeStation(receiver, 18, 26, 22, 13, "STATE LAB", Cyan));
        floor.Content(ForgeStation(receiver, 60, 23, 22, 13, "FSM WORKBENCH", Magenta));
        floor.Content(ForgeStation(receiver, 19, 62, 24, 14, "MICROBUNDLE STOCK", Green));
        floor.Content(ForgeStation(receiver, 60, 62, 23, 14, "RUNTIME BAY", Yellow));
        floor.Content(ForgeDoor(receiver));
        root.Content(floor);

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed")
            .Style("left", "1rem")
            .Style("top", "1rem")
            .Style("z-index", "30")
            .Style("max-width", "34rem")
            .Style("padding", ".55rem .7rem")
            .Style("border", $"1px solid {Cyan}55")
            .Style("background", "rgba(1,4,10,.88)")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".48rem").Style("letter-spacing", ".18em").Text("SPATIAL SCENE // " + title.ToUpperInvariant()))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("color", "#8ba8b2").Style("font-family", "system-ui,sans-serif").Style("font-size", ".68rem").Text(description)));

        root.Content(Avatar(receiver, avatarName));
        root.Content(WorkshopGui.Button(receiver)
            .Label("← LEAVE SCENE")
            .Style("position", "fixed")
            .Style("right", "1rem")
            .Style("bottom", "1rem")
            .Style("z-index", "30")
            .Style("padding", ".55rem .75rem")
            .Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.88)")
            .Style("color", Magenta)
            .Style("font-family", "inherit")
            .Style("font-size", ".48rem")
            .Style("letter-spacing", ".12em")
            .Style("cursor", "pointer")
            .OnClick(exitScene));

        return root;
    }

    private static ElementBuilder Grid(object receiver)
    {
        var grid = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("background-size", "6vw 6vh")
            .Style("background-image", "linear-gradient(rgba(0,234,255,.08) 1px, transparent 1px),linear-gradient(90deg,rgba(0,234,255,.08) 1px,transparent 1px)")
            .Style("opacity", ".6")
            .Style("pointer-events", "none");
        return grid;
    }

    private static ElementBuilder ForgeStation(object receiver, double left, double top, double width, double height, string label, string accent)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", left + "%")
            .Style("top", top + "%")
            .Style("width", width + "%")
            .Style("height", height + "%")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {accent}88")
            .Style("background", $"linear-gradient(180deg,{accent}18,#03070d 72%)")
            .Style("box-shadow", $"0 0 18px {accent}12,inset 0 0 18px {accent}08")
            .Style("animation", "spatial-interior-breathe 3s ease-in-out infinite")
            .Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", ".5rem").Style("top", ".4rem").Style("color", accent).Style("font-size", ".48rem").Style("letter-spacing", ".15em").Text(label))
            .Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", "8%").Style("right", "8%").Style("bottom", "16%").Style("height", "18%").Style("border", $"1px solid {accent}44").Style("background", $"{accent}0a"));

    private static ElementBuilder ForgeDoor(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", "46%")
            .Style("bottom", "0")
            .Style("width", "8%")
            .Style("height", "16%")
            .Style("border", $"1px solid {Cyan}88")
            .Style("background", "#010205")
            .Style("box-shadow", $"0 0 20px {Cyan}22")
            .Style("pointer-events", "none");

    private static ElementBuilder Avatar(object receiver, string name)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed")
            .Style("left", "50%")
            .Style("top", "50%")
            .Style("z-index", "20")
            .Style("transform", "translate(-50%,-50%)")
            .Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("width", "14px")
                .Style("height", "14px")
                .Style("border", $"1px solid {White}")
                .Style("border-radius", "50%")
                .Style("box-shadow", $"0 0 14px {White}88"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute")
                .Style("left", "50%")
                .Style("top", "-1.1rem")
                .Style("transform", "translateX(-50%)")
                .Style("padding", ".12rem .3rem")
                .Style("border", "1px solid rgba(255,255,255,.18)")
                .Style("background", "rgba(1,4,10,.84)")
                .Style("font-size", ".45rem")
                .Style("letter-spacing", ".1em")
                .Style("white-space", "nowrap")
                .Text(name));
}
