namespace TheSingularityWorkshop.Gui;

/// <summary>Builds the first-contact Workshop map used to teach spatial navigation.</summary>
/// <remarks>
/// The map is intentionally a fire-escape/mall-directory abstraction: the visitor sees the
/// whole complex, chooses a destination by its representation, and then returns to the live
/// world while the avatar travels to that destination.
/// </remarks>
public static class SpatialWorkshopMapGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Green = "#52e05a";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";

    public static ElementBuilder Build(
        object receiver,
        SpatialWorkshopScene scene,
        double avatarX,
        double avatarY,
        string moniker,
        Func<string, Task> selectDestination,
        Action close)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("z-index", "200")
            .Style("display", "flex")
            .Style("align-items", "center")
            .Style("justify-content", "center")
            .Style("padding", "2rem")
            .Style("box-sizing", "border-box")
            .Style("background", "rgba(1,4,10,.94)")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text("@keyframes workshop-map-moniker{0%,100%{transform:translateY(0);text-shadow:0 0 8px #00eaff55}50%{transform:translateY(-3px);text-shadow:0 0 24px #00eaffaa}}@keyframes workshop-map-pulse{0%,100%{opacity:.62}50%{opacity:1}}"));

        var panel = WorkshopGui.Panel(receiver)
            .Style("width", "min(1040px,94vw)")
            .Style("max-height", "92vh")
            .Style("overflow", "auto")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {Cyan}99")
            .Style("background", "rgba(2,9,18,.98)")
            .Style("box-shadow", $"0 0 90px {Cyan}18,inset 0 0 50px {Cyan}08")
            .Style("padding", "1.2rem");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("text-align", "center")
            .Style("color", Cyan)
            .Style("font-size", "clamp(.8rem,2vw,1.25rem)")
            .Style("letter-spacing", ".34em")
            .Style("animation", "workshop-map-moniker 2.8s ease-in-out infinite")
            .Text(moniker.ToUpperInvariant()));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".45rem")
            .Style("text-align", "center")
            .Style("color", Yellow)
            .Style("font-size", ".5rem")
            .Style("letter-spacing", ".26em")
            .Text("WELCOME TO THE WORKSHOP // CAMPUS DIRECTORY"));

        panel.Content(Map(receiver, scene, avatarX, avatarY, selectDestination));
        panel.Content(Legend(receiver, scene));
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("display", "flex")
            .Style("justify-content", "space-between")
            .Style("align-items", "center")
            .Style("gap", "1rem")
            .Style("margin-top", ".8rem")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("color", "#a8bbc4")
                .Style("font-size", ".5rem")
                .Style("letter-spacing", ".12em")
                .Text("SELECT A BUILDING. THE MAP CLOSES. YOUR AVATAR WALKS THERE."))
            .Content(WorkshopGui.Button(receiver)
                .Label("CLOSE MAP")
                .Style("padding", ".45rem .7rem")
                .Style("border", $"1px solid {Magenta}66")
                .Style("background", "rgba(20,4,18,.8)")
                .Style("color", Magenta)
                .Style("font-family", "inherit")
                .Style("font-size", ".45rem")
                .Style("letter-spacing", ".12em")
                .Style("cursor", "pointer")
                .OnClick(close)));

        root.Content(panel);
        return root;
    }

    private static ElementBuilder Map(object receiver, SpatialWorkshopScene scene, double avatarX, double avatarY, Func<string, Task> selectDestination)
    {
        var map = WorkshopGui.Panel(receiver)
            .Style("position", "relative")
            .Style("width", "100%")
            .Style("aspect-ratio", "1.15")
            .Style("margin-top", "1.1rem")
            .Style("border", $"1px solid {Cyan}44")
            .Style("background", "#01060d")
            .Style("overflow", "hidden");

        map.Content(WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0").Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none")
            .Content(Grid(receiver)));

        foreach (var item in scene.Interactables)
        {
            var bounds = item.Bounds;
            var width = Math.Max(bounds.Width, 5d);
            var height = Math.Max(bounds.Height, 4d);
            var button = WorkshopGui.Button(receiver)
                .Style("position", "absolute")
                .Style("left", $"{bounds.X:0.##}%")
                .Style("top", $"{bounds.Y:0.##}%")
                .Style("width", $"{width:0.##}%")
                .Style("height", $"{height:0.##}%")
                .Style("box-sizing", "border-box")
                .Style("padding", ".1rem")
                .Style("border", $"1px solid {Accent(item.ExperienceId)}aa")
                .Style("background", "rgba(0,18,28,.55)")
                .Style("color", White)
                .Style("font-family", "inherit")
                .Style("font-size", ".4rem")
                .Style("letter-spacing", ".05em")
                .Style("cursor", "pointer")
                .Style("z-index", "3")
                .AriaLabel($"Navigate to {item.Name}")
                .Title($"Select {item.Name}")
                .OnClick(() => selectDestination(item.Id))
                .StopPropagation("onclick")
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("display", "block")
                    .Style("overflow", "hidden")
                    .Style("text-overflow", "ellipsis")
                    .Style("white-space", "nowrap")
                    .Text(item.Name));
            map.Content(button);
        }

        map.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"{avatarX:0.##}%")
            .Style("top", $"{avatarY:0.##}%")
            .Style("width", "10px")
            .Style("height", "10px")
            .Style("transform", "translate(-50%,-50%)")
            .Style("border", $"1px solid {White}")
            .Style("border-radius", "50%")
            .Style("box-shadow", $"0 0 14px {White}aa")
            .Style("animation", "workshop-map-pulse 1.6s ease-in-out infinite")
            .Style("z-index", "6")
            .Style("pointer-events", "none"));

        return map;
    }

    private static ElementBuilder Grid(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "g");
        for (var i = 0; i <= 20; i++)
        {
            var p = i * 5;
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", p).Attribute("y1", 0).Attribute("x2", p).Attribute("y2", 100).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", 0).Attribute("y1", p).Attribute("x2", 100).Attribute("y2", p).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".1").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder Legend(object receiver, SpatialWorkshopScene scene)
    {
        var legend = WorkshopGui.Element(receiver, "div")
            .Style("display", "grid")
            .Style("grid-template-columns", "repeat(auto-fit,minmax(170px,1fr))")
            .Style("gap", ".35rem")
            .Style("margin-top", ".7rem");
        foreach (var item in scene.Interactables)
        {
            legend.Content(WorkshopGui.Element(receiver, "div")
                .Style("padding", ".3rem .4rem")
                .Style("border-left", $"2px solid {Accent(item.ExperienceId)}")
                .Style("background", "rgba(255,255,255,.025)")
                .Style("color", "#b8c9d0")
                .Style("font-size", ".42rem")
                .Style("letter-spacing", ".08em")
                .Text(item.Name));
        }
        return legend;
    }

    private static string Accent(string experienceId) => experienceId switch
    {
        "mansion" => Magenta,
        "forge" => Cyan,
        "singularity-transit" => Cyan,
        "singularity-ontology-mall" => Yellow,
        "image-workshop" => Magenta,
        _ => Green
    };
}
