namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the AEC Remote Office lobby as a place of orientation rather than a tool palette.
/// The city model communicates that the office is a force multiplier for defining structures.
/// </summary>
public static class SpatialAECRemoteOfficeLobbyGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver,
        string avatarName,
        Action enterConference,
        Action exitLobby)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(CityModel(receiver));
        root.Content(Receptionist(receiver, avatarName, enterConference));
        root.Content(Exit(receiver, exitLobby));
        return root;
    }

    private static ElementBuilder CityModel(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("background", "radial-gradient(circle at 50% 42%, #102633 0, #02080f 55%)");

        for (var i = 8; i < 100; i += 6)
        {
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", i).Attribute("y1", "62").Attribute("x2", 50).Attribute("y2", 100)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".07").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", 0).Attribute("y1", i).Attribute("x2", 100).Attribute("y2", i)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".045").Attribute("stroke-width", ".12"));
        }

        // Singularity City massing model: definitive enough to reason about places,
        // deliberately schematic enough to remain authored data rather than finished CAD.
        var buildings = new[]
        {
            (12, 60, 12, 22, "RESEARCH"),
            (28, 50, 15, 32, "CIVIC"),
            (48, 42, 18, 40, "WORKSHOP"),
            (70, 55, 15, 27, "TRANSIT"),
            (84, 66, 9, 16, "RESIDENTIAL")
        };

        foreach (var (x, y, width, height, label) in buildings)
        {
            svg.Child(WorkshopGui.Element(receiver, "polygon")
                .Attribute("points", $"{x},{y + 5} {x + width},{y} {x + width},{y + height} {x},{y + height + 5}")
                .Attribute("fill", "rgba(0,234,255,.025)")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".35").Attribute("stroke-width", ".3"));
            svg.Child(WorkshopGui.Element(receiver, "text")
                .Attribute("x", x + width / 2d).Attribute("y", y + height / 2d)
                .Attribute("fill", Cyan).Attribute("font-size", "1.6").Attribute("text-anchor", "middle")
                .Text(label));
        }

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "50").Attribute("y", "10")
            .Attribute("fill", Yellow).Attribute("font-size", "2.1").Attribute("text-anchor", "middle")
            .Text("SINGULARITY CITY // AEC REMOTE OFFICE"));

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "50").Attribute("y", "14")
            .Attribute("fill", "#7895a1").Attribute("font-size", "1.15").Attribute("text-anchor", "middle")
            .Text("CITY MODEL // SPATIAL CONTEXT // STRUCTURES ARE CAPTURED HERE"));

        return svg;
    }

    private static ElementBuilder Receptionist(object receiver, string avatarName, Action enterConference)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "5%").Style("bottom", "8%")
            .Style("width", "min(28rem,90vw)").Style("padding", "1rem")
            .Style("border", $"1px solid {Cyan}66")
            .Style("background", "rgba(1,6,12,.9)")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", Cyan).Style("font-size", ".48rem").Style("letter-spacing", ".2em").Text("RECEPTION // AEC REMOTE OFFICE"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("font-family", "system-ui,sans-serif").Style("font-size", "1rem").Text($"Welcome, {avatarName}."))
            .Content(WorkshopGui.Element(receiver, "p").Style("color", "#9ab0b8").Style("font-family", "system-ui,sans-serif").Style("font-size", ".78rem").Text("You do not need to draw this building. Tell us what you intend to create. The Workshop will turn that intent into spatial structure; you can enter the model and author it yourself when you want to."))
            .Content(WorkshopGui.Element(receiver, "p").Style("color", White).Style("font-family", "system-ui,sans-serif").Style("font-size", ".78rem").Text("Our team leads are waiting in the conference room."))
            .Content(WorkshopGui.Button(receiver).Label("FOLLOW THE RECEPTIONIST → CONFERENCE ROOM")
                .Style("margin-top", ".45rem").Style("padding", ".65rem .8rem")
                .Style("border", $"1px solid {Yellow}88").Style("background", "rgba(255,211,77,.05)")
                .Style("color", Yellow).Style("font-family", "inherit").Style("cursor", "pointer")
                .OnClick(enterConference));

    private static ElementBuilder Exit(object receiver, Action exitLobby)
        => WorkshopGui.Button(receiver).Label("← RETURN TO CITY")
            .Style("position", "absolute").Style("right", "2%").Style("bottom", "2%")
            .Style("padding", ".5rem .7rem").Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.8)").Style("color", Magenta)
            .Style("font-family", "inherit").Style("cursor", "pointer").OnClick(exitLobby);
}
