using Microsoft.AspNetCore.Components;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the first true room interior for the spatial Workshop prototype.
/// The room is intentionally expressed as a place: a table, conference hub,
/// seats, participant presence, and a physical-looking exit rather than an app panel.
/// </summary>
public static class ConferenceRoomGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff2cff";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static RenderFragment Build(
        object receiver,
        bool joined,
        Action joinConference,
        Action leaveConference,
        Action exitRoom)
        => builder =>
        {
            var room = WorkshopGui.Panel(receiver)
                .Style("position", "relative")
                .Style("height", "100vh")
                .Style("min-height", "0")
                .Style("overflow", "hidden")
                .Style("background", Ink)
                .Style("color", White)
                .Style("font-family", "Consolas, 'Courier New', monospace");

            room.Content(Floor(receiver, joined, joinConference, leaveConference, exitRoom));
            builder.AddContent(0, room.Build());
        };

    private static ElementBuilder Floor(
        object receiver,
        bool joined,
        Action joinConference,
        Action leaveConference,
        Action exitRoom)
    {
        var floor = WorkshopGui.Panel(receiver)
            .Style("position", "relative")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("overflow", "hidden")
            .Style("background", "radial-gradient(circle at 50% 48%, rgba(0,234,255,.06), transparent 45%), #020a12");

        floor.Content(BlueprintFloor(receiver));
        floor.Content(RoomTitle(receiver));
        floor.Content(Table(receiver));
        floor.Content(ConferenceHub(receiver, joined));
        floor.Content(Participants(receiver, joined));
        floor.Content(JoinPanel(receiver, joined, joinConference, leaveConference));
        floor.Content(ExitDoor(receiver, exitRoom));
        return floor;
    }

    private static ElementBuilder BlueprintFloor(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none");

        for (var i = 10; i < 100; i += 5)
        {
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", i).Attribute("y1", "0").Attribute("x2", i).Attribute("y2", "100")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".06").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "0").Attribute("y1", i).Attribute("x2", "100").Attribute("y2", i)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".06").Attribute("stroke-width", ".12"));
        }

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "8").Attribute("y", "8").Attribute("width", "84").Attribute("height", "84")
            .Attribute("fill", "none").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".55")
            .Attribute("stroke-width", ".45"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "12").Attribute("y", "12").Attribute("width", "76").Attribute("height", "76")
            .Attribute("fill", "none").Attribute("stroke", Magenta).Attribute("stroke-opacity", ".18")
            .Attribute("stroke-width", ".2"));
        return svg;
    }

    private static ElementBuilder RoomTitle(object receiver)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("top", "6%")
            .Style("transform", "translateX(-50%)").Style("align-items", "center")
            .Style("background", "transparent").Style("border", "0")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("color", Cyan).Style("font-size", ".5rem").Style("letter-spacing", ".3em")
                .Text("WORKSHOP // CONFERENCE ROOM"))
            .Content(WorkshopGui.Element(receiver, "strong")
                .Style("margin-top", ".3rem").Style("font-size", "clamp(1.3rem,3vw,2.4rem)")
                .Style("letter-spacing", ".08em").Text("YUP"));

    private static ElementBuilder Table(object receiver)
    {
        var table = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("top", "52%")
            .Style("width", "44%").Style("height", "28%")
            .Style("transform", "translate(-50%,-50%)")
            .Style("align-items", "center").Style("justify-content", "center")
            .Style("border", $"4px solid {Cyan}").Style("border-radius", "50%")
            .Style("background", "rgba(0,234,255,.025)")
            .Style("box-shadow", "0 0 35px rgba(0,234,255,.08), inset 0 0 35px rgba(0,234,255,.04)");
        table.Content(WorkshopGui.Element(receiver, "strong")
            .Style("font-size", ".8rem").Style("letter-spacing", ".25em").Style("color", White)
            .Text("TABLE"));
        table.Content(WorkshopGui.Element(receiver, "small")
            .Style("margin-top", ".35rem").Style("font-size", ".4rem").Style("letter-spacing", ".12em")
            .Style("color", "#58747e").Text("SHARED WORKSPACE"));
        return table;
    }

    private static ElementBuilder ConferenceHub(object receiver, bool joined)
    {
        var hub = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Style("position", "absolute").Style("left", "50%").Style("top", "52%")
            .Style("width", "16%").Style("height", "20%")
            .Style("transform", "translate(-50%,-50%)")
            .Style("pointer-events", "none");

        hub.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "50").Attribute("cy", "50").Attribute("r", "20")
            .Attribute("fill", "none").Attribute("stroke", joined ? Green : Magenta)
            .Attribute("stroke-width", "2").Attribute("stroke-opacity", ".8"));
        hub.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "50").Attribute("cy", "50").Attribute("r", "7")
            .Attribute("fill", joined ? Green : Magenta));
        hub.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 20 50 Q 50 18 80 50 Q 50 82 20 50")
            .Attribute("fill", "none").Attribute("stroke", Cyan).Attribute("stroke-width", "1.2")
            .Attribute("stroke-opacity", ".7"));
        return hub;
    }

    private static ElementBuilder Participants(object receiver, bool joined)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("top", "53%")
            .Style("width", "68%").Style("height", "50%")
            .Style("transform", "translate(-50%,-50%)")
            .Style("background", "transparent").Style("border", "0")
            .Style("pointer-events", "none");

        var seats = new[]
        {
            (50d, 4d, "YOU"),
            (50d, 96d, "VISITOR"),
            (5d, 50d, "REMOTE"),
            (95d, 50d, "REMOTE")
        };

        foreach (var (x, y, label) in seats)
        {
            var color = label == "YOU" && joined ? Green : label == "YOU" ? Magenta : Cyan;
            var seat = WorkshopGui.Panel(receiver)
                .Style("position", "absolute").Style("left", $"{x}%").Style("top", $"{y}%")
                .Style("transform", "translate(-50%,-50%)").Style("align-items", "center")
                .Style("background", "rgba(1,4,10,.82)").Style("border", $"1px solid {color}66")
                .Style("padding", ".35rem .5rem");
            seat.Content(WorkshopGui.Element(receiver, "span")
                .Style("font-size", ".45rem").Style("letter-spacing", ".12em").Style("color", color)
                .Text(label));
            panel.Child(seat);
        }

        return panel;
    }

    private static ElementBuilder JoinPanel(
        object receiver,
        bool joined,
        Action joinConference,
        Action leaveConference)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", "4%").Style("bottom", "7%")
            .Style("width", "240px").Style("padding", ".8rem")
            .Style("border", $"1px solid {(joined ? Green : Magenta)}66")
            .Style("background", "rgba(1,5,11,.9)");
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".48rem").Style("letter-spacing", ".16em")
            .Style("color", joined ? Green : Magenta)
            .Text(joined ? "CONFERENCE LINK // ACTIVE" : "CONFERENCE LINK // READY"));
        panel.Content(WorkshopGui.Element(receiver, "p")
            .Style("margin", ".5rem 0").Style("color", "#718e99")
            .Style("font-family", "system-ui, sans-serif").Style("font-size", ".72rem")
            .Text(joined
                ? "Your avatar is present in the room. A future presence service can replace the local participant slots with live visitors."
                : "Step into the Workshop conference and make your avatar available to the other participants."));
        panel.Content(WorkshopGui.Button(receiver)
            .Label(joined ? "LEAVE CONFERENCE" : "JOIN CONFERENCE")
            .Style("width", "100%").Style("padding", ".65rem")
            .Style("border", $"1px solid {(joined ? Green : Magenta)}99")
            .Style("border-radius", "0").Style("background", "transparent")
            .Style("color", joined ? Green : Magenta).Style("font-family", "inherit")
            .Style("font-size", ".5rem").Style("letter-spacing", ".12em")
            .Style("cursor", "pointer")
            .OnClick(joined ? leaveConference : joinConference));
        return panel;
    }

    private static ElementBuilder ExitDoor(object receiver, Action exitRoom)
    {
        var door = WorkshopGui.Button(receiver)
            .Label("EXIT / DOOR")
            .Style("position", "absolute").Style("left", "50%").Style("bottom", "2%")
            .Style("transform", "translateX(-50%)")
            .Style("padding", ".55rem 1.2rem")
            .Style("border", $"4px solid {Cyan}").Style("border-radius", "0")
            .Style("background", Ink).Style("color", Cyan)
            .Style("font-family", "inherit").Style("font-size", ".5rem")
            .Style("letter-spacing", ".16em").Style("cursor", "pointer")
            .AriaLabel("Exit the conference room and return to the Workshop map")
            .OnClick(exitRoom);
        return door;
    }
}
