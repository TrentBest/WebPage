namespace TheSingularityWorkshop.Gui;

public static partial class SpatialWorkshopMapGuiBuilder
{
    public static ElementBuilder Build(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, string moniker, Func<string, Task> selectDestination, Action close)
    {
        var root = WorkshopGui.Panel(receiver).Style("position", "fixed").Style("inset", "0").Style("z-index", "200")
            .Style("display", "flex").Style("align-items", "center").Style("justify-content", "center").Style("padding", "2rem")
            .Style("box-sizing", "border-box").Style("background", "rgba(1,4,10,.95)").Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");
        root.Content(WorkshopGui.Element(receiver, "style").Text("@keyframes workshop-map-moniker{0%,100%{transform:translateY(0);text-shadow:0 0 8px #00eaff55}50%{transform:translateY(-3px);text-shadow:0 0 24px #00eaffaa}}@keyframes workshop-map-pulse{0%,100%{opacity:.62}50%{opacity:1}}"));
        var panel = WorkshopGui.Panel(receiver).Style("width", "min(1180px,96vw)").Style("max-height", "94vh").Style("overflow", "auto")
            .Style("box-sizing", "border-box").Style("border", $"1px solid {Cyan}99").Style("background", "rgba(2,9,18,.99)")
            .Style("box-shadow", $"0 0 90px {Cyan}18,inset 0 0 50px {Cyan}08").Style("padding", "1.2rem");
        panel.Content(WorkshopGui.Element(receiver, "div").Style("text-align", "center").Style("color", Cyan).Style("font-size", "clamp(.8rem,2vw,1.25rem)").Style("letter-spacing", ".34em").Style("animation", "workshop-map-moniker 2.8s ease-in-out infinite").Text(moniker.ToUpperInvariant()));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("text-align", "center").Style("color", Yellow).Style("font-size", ".52rem").Style("letter-spacing", ".26em").Text("WORKSHOP CAMPUS DIRECTORY // ORIENT YOURSELF BEFORE YOU WALK"));
        var layout = WorkshopGui.Element(receiver, "div").Style("display", "grid").Style("grid-template-columns", "minmax(0,3fr) minmax(250px,1fr)").Style("gap", "1rem").Style("margin-top", "1.1rem");
        layout.Content(Map(receiver, scene, avatarX, avatarY, selectDestination));
        layout.Content(Directory(receiver, scene, selectDestination));
        panel.Content(layout);
        panel.Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("justify-content", "space-between").Style("align-items", "center").Style("gap", "1rem").Style("margin-top", ".8rem")
            .Content(WorkshopGui.Element(receiver, "span").Style("color", "#a8bbc4").Style("font-size", ".5rem").Style("letter-spacing", ".12em").Text("SELECT THE STRUCTURE OR DIRECTORY ENTRY. THE MAP CLOSES AND YOUR AVATAR WALKS THERE."))
            .Content(WorkshopGui.Button(receiver).Label("CLOSE MAP").Style("padding", ".45rem .7rem").Style("border", $"1px solid {Magenta}66").Style("background", "rgba(20,4,18,.8)").Style("color", Magenta).Style("font-family", "inherit").Style("font-size", ".45rem").Style("cursor", "pointer").OnClick(close)));
        root.Content(panel);
        return root;
    }

    private static ElementBuilder Map(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, Func<string, Task> selectDestination)
    {
        var map = WorkshopGui.Panel(receiver).Style("position", "relative").Style("width", "100%").Style("aspect-ratio", "1 / 1").Style("border", $"1px solid {Cyan}55").Style("background", "#01060d").Style("overflow", "hidden");
        map.Content(WorkshopGui.Element(receiver, "svg").Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "xMidYMid meet").Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%").Style("pointer-events", "none").Content(Grid(receiver)).Content(Compass(receiver)).Content(ScaleBar(receiver)));
        foreach (var item in scene.Interactables)
        {
            var b = item.Bounds; var accent = Accent(item.ExperienceId);
            map.Content(WorkshopGui.Button(receiver).Style("position", "absolute").Style("left", $"{b.X:0.##}%").Style("top", $"{b.Y:0.##}%").Style("width", $"{Math.Max(b.Width, 2.5d):0.##}%").Style("height", $"{Math.Max(b.Height, 2.5d):0.##}%")
                .Style("box-sizing", "border-box").Style("padding", "0").Style("border", $"2px solid {accent}cc").Style("background", Fill(accent)).Style("color", White).Style("font-family", "inherit").Style("cursor", "pointer").Style("z-index", "3")
                .AriaLabel($"Navigate to {item.Name}").Title($"Select {item.Name}").OnClick(() => selectDestination(item.Id)).StopPropagation("onclick")
                .Content(WorkshopGui.Element(receiver, "span").Style("display", "flex").Style("width", "100%").Style("height", "100%").Style("align-items", "center").Style("justify-content", "center").Style("padding", ".15rem").Style("box-sizing", "border-box").Style("font-size", "clamp(.42rem,.58vw,.68rem)").Style("font-weight", "700").Style("line-height", "1.1").Style("text-align", "center").Style("text-shadow", "0 1px 2px #000").Text(item.Name)));
        }
        map.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", $"{avatarX:0.##}%").Style("top", $"{avatarY:0.##}%").Style("width", "13px").Style("height", "13px").Style("transform", "translate(-50%,-50%)").Style("border", $"2px solid {White}").Style("border-radius", "50%").Style("box-shadow", $"0 0 18px {White}aa").Style("animation", "workshop-map-pulse 1.6s ease-in-out infinite").Style("z-index", "6").Style("pointer-events", "none"));
        map.Content(WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("left", $"{avatarX:0.##}%").Style("top", $"{avatarY + 2.4:0.##}%").Style("transform", "translateX(-50%)").Style("z-index", "6").Style("pointer-events", "none").Style("color", White).Style("font-size", ".42rem").Style("font-weight", "700").Text("YOU ARE HERE"));
        return map;
    }

    private static ElementBuilder Directory(object receiver, SpatialWorkshopScene scene, Func<string, Task> selectDestination)
    {
        var directory = WorkshopGui.Element(receiver, "div").Style("border", $"1px solid {Cyan}33").Style("background", "rgba(0,10,18,.72)").Style("padding", ".65rem").Style("overflow", "auto");
        directory.Content(WorkshopGui.Element(receiver, "div").Style("color", Yellow).Style("font-size", ".55rem").Style("font-weight", "700").Style("letter-spacing", ".16em").Text("BUILDING DIRECTORY"));
        directory.Content(WorkshopGui.Element(receiver, "div").Style("margin", ".35rem 0 .6rem").Style("color", "#8fa7b2").Style("font-size", ".43rem").Style("line-height", "1.5").Text("Footprints retain campus scale. Use the directory when a structure becomes too small to read on the map."));
        foreach (var item in scene.Interactables)
        {
            var accent = Accent(item.ExperienceId);
            directory.Content(WorkshopGui.Button(receiver).Label(item.Name.ToUpperInvariant()).Style("display", "block").Style("width", "100%").Style("margin", ".22rem 0").Style("padding", ".45rem .5rem").Style("text-align", "left").Style("border", $"1px solid {accent}66").Style("border-left", $"4px solid {accent}").Style("background", "rgba(255,255,255,.025)").Style("color", White).Style("font-family", "inherit").Style("font-size", ".48rem").Style("font-weight", "700").Style("letter-spacing", ".07em").Style("cursor", "pointer").OnClick(() => selectDestination(item.Id)));
        }
        return directory;
    }

    private static ElementBuilder Grid(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "g");
        for (var i = 0; i <= 20; i++) { var p = i * 5; svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", p).Attribute("y1", 0).Attribute("x2", p).Attribute("y2", 100).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".12")); svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 0).Attribute("y1", p).Attribute("x2", 100).Attribute("y2", p).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".12")); }
        return svg;
    }

    private static ElementBuilder Compass(object receiver) => WorkshopGui.Element(receiver, "g").Content(WorkshopGui.Element(receiver, "text").Attribute("x", 94).Attribute("y", 6).Attribute("fill", White).Attribute("font-size", "3").Attribute("font-weight", "700").Attribute("text-anchor", "middle").Text("N")).Content(WorkshopGui.Element(receiver, "line").Attribute("x1", 94).Attribute("y1", 7).Attribute("x2", 94).Attribute("y2", 11).Attribute("stroke", White).Attribute("stroke-width", ".35"));
    private static ElementBuilder ScaleBar(object receiver) => WorkshopGui.Element(receiver, "g").Content(WorkshopGui.Element(receiver, "line").Attribute("x1", 6).Attribute("y1", 94).Attribute("x2", 26).Attribute("y2", 94).Attribute("stroke", Yellow).Attribute("stroke-width", ".45")).Content(WorkshopGui.Element(receiver, "text").Attribute("x", 16).Attribute("y", 92).Attribute("fill", Yellow).Attribute("font-size", "2.1").Attribute("text-anchor", "middle").Text("20 WORLD UNITS"));
    private static string Fill(string accent) => accent switch { Cyan => "rgba(0,234,255,.10)", Yellow => "rgba(255,211,77,.10)", Magenta => "rgba(255,56,209,.10)", _ => "rgba(82,224,90,.10)" };
    private static string Accent(string experienceId) => experienceId switch { "mansion" => Magenta, "forge" => Cyan, "singularity-transit" => Cyan, "singularity-ontology-mall" => Yellow, "image-workshop" => Magenta, _ => Green };
}
