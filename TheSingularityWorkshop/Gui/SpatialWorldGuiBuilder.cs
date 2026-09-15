using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Infrastructure.Hub;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Viewport-first Workshop world builder.
/// The world is a camera surface, not a scrolling document. Interaction is
/// expressed as spatial intent: walk, approach, inspect, and enter.
/// </summary>
public static class SpatialWorldGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff2cff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder Build(
        object receiver,
        HubRuntime runtime,
        IReadOnlyList<SpatialRoom> rooms,
        string? selectedRoomId,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Action<double, double> moveAvatar,
        Action<double, double> moveAvatarTo,
        Action<string> approachRoom,
        Action enterSelectedRoom,
        Action unlockUnknown)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Style("box-sizing", "border-box");

        root.Content(World(receiver, runtime, rooms, selectedRoomId, unknownUnlocked,
            avatarX, avatarY, onKeyDown, moveAvatar, moveAvatarTo, approachRoom));
        root.Content(Inspector(receiver, rooms, selectedRoomId, unknownUnlocked, enterSelectedRoom, unlockUnknown));
        return root;
    }

    private static ElementBuilder World(
        object receiver,
        HubRuntime runtime,
        IReadOnlyList<SpatialRoom> rooms,
        string? selectedRoomId,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Action<double, double> moveAvatar,
        Action<double, double> moveAvatarTo,
        Action<string> approachRoom)
    {
        var world = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("inset", "0")
            .Style("overflow", "hidden").Style("background", "#020a12")
            .Style("border", "0").Style("box-sizing", "border-box")
            .Attribute("id", "workshop-spatial-map")
            .Attribute("tabindex", "0")
            .AriaLabel("Workshop world. Click a place to walk. Use WASD or arrow keys.")
            .OnKeyDown(onKeyDown).PreventDefault("onkeydown");

        world.Content(Grid(receiver));
        world.Content(WalkSurface(receiver, moveAvatarTo));

        foreach (var room in rooms)
            world.Content(Building(receiver, room, selectedRoomId == room.Id,
                room.Id == "unknown" && !unknownUnlocked, approachRoom));

        world.Content(Avatar(receiver, avatarX, avatarY));
        world.Content(Controls(receiver, moveAvatar));
        world.Content(Readout(receiver, avatarX, avatarY, runtime));
        return world;
    }

    /// <summary>
    /// A dense invisible walk surface. Each cell is a real GUI interaction,
    /// rather than a viewport click hack. The cell resolution can later be
    /// replaced by a true navigation mesh without changing the Experience API.
    /// </summary>
    private static ElementBuilder WalkSurface(object receiver, Action<double, double> moveAvatarTo)
    {
        const int cells = 24;
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("z-index", "2").Style("display", "grid")
            .Style("grid-template-columns", $"repeat({cells}, 1fr)")
            .Style("grid-template-rows", $"repeat({cells}, 1fr)")
            .Style("pointer-events", "none");

        for (var row = 0; row < cells; row++)
        for (var column = 0; column < cells; column++)
        {
            var x = (column + .5) / cells * 100;
            var y = (row + .5) / cells * 100;
            surface.Content(WorkshopGui.Button(receiver)
                .Style("display", "block")
                .Style("width", "100%").Style("height", "100%")
                .Style("min-width", "0").Style("min-height", "0")
                .Style("margin", "0").Style("padding", "0")
                .Style("border", "0").Style("outline", "none")
                .Style("background", "transparent")
                .Style("pointer-events", "auto")
                .Style("cursor", "crosshair")
                .AriaLabel($"Walk to {x:0.#}, {y:0.#}")
                .OnClick(() => moveAvatarTo(x, y)));
        }

        return surface;
    }

    private static ElementBuilder Grid(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%")
            .Style("pointer-events", "none").Style("opacity", ".42");

        for (var i = 0; i <= 20; i++)
        {
            var p = i * 5;
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", p).Attribute("y1", "0").Attribute("x2", p).Attribute("y2", "100")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".12").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "0").Attribute("y1", p).Attribute("x2", "100").Attribute("y2", p)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".12").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder Building(object receiver, SpatialRoom room, bool selected, bool locked, Action<string> approachRoom)
    {
        if (room.Id == "yup")
            return Yup(receiver, room, selected, approachRoom);

        var accent = locked ? Yellow : selected ? Magenta : room.Id == "engineering" ? Cyan : Green;
        var size = room.Id switch
        {
            "engineering" => (30d, 25d),
            "experience" => (27d, 22d),
            "creation" => (31d, 23d),
            _ => (24d, 20d)
        };

        return WorkshopGui.Button(receiver)
            .PositionAt(room.X, room.Y)
            .Style("z-index", "5").Style("width", $"{size.Item1}%").Style("height", $"{size.Item2}%")
            .Style("padding", "0").Style("border", $"2px solid {accent}99")
            .Style("border-radius", "0")
            .Style("background", locked ? "rgba(30,24,3,.35)" : "rgba(0,20,30,.3)")
            .Style("color", White).Style("font-family", "inherit")
            .Style("cursor", "pointer").Style("box-sizing", "border-box")
            .AriaLabel(locked ? "Unmapped structure" : $"Approach {room.Name}")
            .OnClick(() => approachRoom(room.Id))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("left", "50%").Style("top", "10%")
                .Style("transform", "translateX(-50%)")
                .Style("padding", ".2rem .4rem")
                .Style("background", "rgba(1,4,10,.85)")
                .Style("border", $"1px solid {accent}55")
                .Style("font-size", room.Id == "engineering" ? ".75rem" : ".55rem")
                .Style("letter-spacing", ".1em").Style("white-space", "nowrap")
                .Style("pointer-events", "none").Text(room.Name))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("inset", "8%")
                .Style("border", $"1px solid {accent}55")
                .Style("pointer-events", "none"));
    }

    private static ElementBuilder Yup(object receiver, SpatialRoom room, bool selected, Action<string> approachRoom)
    {
        var accent = selected ? Magenta : Cyan;
        return WorkshopGui.Button(receiver)
            .PositionAt(room.X, room.Y)
            .Style("z-index", "5").Style("width", "350px").Style("height", "200px")
            .Style("padding", "0").Style("border", $"6px solid {accent}")
            .Style("border-radius", "0").Style("background", "transparent")
            .Style("box-shadow", $"0 0 28px {accent}22")
            .Style("cursor", "pointer").Style("box-sizing", "border-box")
            .AriaLabel("Approach Yup conference room")
            .OnClick(() => approachRoom(room.Id))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("left", "50%").Style("top", "12px")
                .Style("transform", "translateX(-50%)")
                .Style("font-size", ".75rem").Style("letter-spacing", ".2em")
                .Style("color", White).Style("white-space", "nowrap")
                .Style("pointer-events", "none").Text("YUP // CONFERENCE ROOM"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
                .Style("width", "92px").Style("height", "54px")
                .Style("transform", "translate(-50%,-50%)")
                .Style("border", $"2px solid {accent}88")
                .Style("border-radius", "50%").Style("pointer-events", "none"))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("left", "50%").Style("bottom", "10px")
                .Style("transform", "translateX(-50%)")
                .Style("font-size", ".4rem").Style("letter-spacing", ".14em")
                .Style("color", accent).Style("white-space", "nowrap")
                .Style("pointer-events", "none").Text("DOOR / ENTER"));
    }

    private static ElementBuilder Avatar(object receiver, double x, double y)
        => WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 40 40").Attribute("aria-label", "Your position")
            .Style("position", "absolute").Style("left", $"{x}%").Style("top", $"{y}%")
            .Style("width", "34px").Style("height", "34px")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "7")
            .Style("pointer-events", "none").Style("overflow", "visible")
            .Style("transition", "left .28s linear, top .28s linear")
            .Content(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "8")
                .Attribute("fill", "none").Attribute("stroke", White).Attribute("stroke-opacity", ".4")
                .Child(WorkshopGui.Element(receiver, "animate")
                    .Attribute("attributeName", "r").Attribute("values", "6;13;6")
                    .Attribute("dur", "1.8s").Attribute("repeatCount", "indefinite")))
            .Content(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "4").Attribute("fill", White));

    private static ElementBuilder Controls(object receiver, Action<double, double> moveAvatar)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", ".75rem").Style("bottom", ".75rem")
            .Style("z-index", "8").Style("padding", ".4rem")
            .Style("border", "1px solid rgba(0,234,255,.18)").Style("background", "rgba(1,5,11,.9)")
            .Style("align-items", "center").Style("gap", ".15rem");
        panel.Content(Step(receiver, "▲", () => moveAvatar(0, -3)));
        var row = WorkshopGui.MultiPanel(receiver).Style("display", "flex").Style("gap", ".15rem");
        row.Content(Step(receiver, "◀", () => moveAvatar(-3, 0)));
        row.Content(Step(receiver, "▼", () => moveAvatar(0, 3)));
        row.Content(Step(receiver, "▶", () => moveAvatar(3, 0)));
        panel.Content(row);
        return panel;
    }

    private static ElementBuilder Step(object receiver, string label, Action action)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("width", "28px").Style("height", "26px").Style("padding", "0")
            .Style("border", "1px solid rgba(0,234,255,.28)").Style("background", "rgba(0,234,255,.04)")
            .Style("color", Cyan).Style("cursor", "pointer").OnClick(action);

    private static ElementBuilder Readout(object receiver, double x, double y, HubRuntime runtime)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", ".75rem").Style("bottom", ".75rem")
            .Style("z-index", "8").Style("padding", ".35rem .5rem")
            .Style("border", "1px solid rgba(0,234,255,.12)").Style("background", "rgba(1,5,11,.78)")
            .Content(WorkshopGui.Element(receiver, "strong")
                .Style("font-size", ".7rem").Style("color", Cyan)
                .Text($"{x:0} : {y:0}"))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("font-size", ".38rem").Style("color", "#58747e")
                .Style("letter-spacing", ".1em").Text($" // {runtime.Hub.LoadedBundles.Count} BUNDLES"));

    private static ElementBuilder Inspector(object receiver, IReadOnlyList<SpatialRoom> rooms, string? selectedRoomId,
        bool unknownUnlocked, Action enterSelectedRoom, Action unlockUnknown)
    {
        var selected = rooms.FirstOrDefault(x => x.Id == selectedRoomId);
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", ".75rem").Style("top", ".75rem")
            .Style("z-index", "8").Style("width", "280px").Style("padding", ".7rem")
            .Style("border", "1px solid rgba(0,234,255,.15)").Style("background", "rgba(1,5,11,.88)");

        if (selected is null)
            return panel.Content(WorkshopGui.Element(receiver, "span")
                .Style("font-size", ".42rem").Style("color", Cyan).Style("letter-spacing", ".14em")
                .Text("WORKSHOP // WALK THE WORLD"));

        panel.Content(WorkshopGui.Element(receiver, "span")
            .Style("font-size", ".42rem").Style("color", selected.Id == "unknown" ? Yellow : Cyan)
            .Style("letter-spacing", ".14em").Text(selected.Architecture));
        panel.Content(WorkshopGui.Element(receiver, "h2")
            .Style("margin", ".35rem 0 .15rem").Style("font-size", "1.25rem").Text(selected.Name));
        panel.Content(WorkshopGui.Element(receiver, "p")
            .Style("margin", ".45rem 0 .65rem").Style("font-size", ".66rem")
            .Style("line-height", "1.45").Style("color", "#91aab2").Text(selected.Description));
        panel.Content(WorkshopGui.Button(receiver)
            .Label(selected.Id == "unknown" && !unknownUnlocked ? "UNLOCK" : "ENTER ROOM →")
            .Style("width", "100%").Style("padding", ".5rem")
            .Style("border", $"1px solid {(selected.Id == "unknown" ? Yellow : Cyan)}66")
            .Style("background", "transparent").Style("color", selected.Id == "unknown" ? Yellow : Cyan)
            .Style("font-family", "inherit").Style("cursor", "pointer")
            .OnClick(selected.Id == "unknown" && !unknownUnlocked ? unlockUnknown : enterSelectedRoom));
        return panel;
    }
}
