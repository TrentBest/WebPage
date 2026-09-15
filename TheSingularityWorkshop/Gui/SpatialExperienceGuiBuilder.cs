using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Infrastructure.Hub;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the Workshop's navigable world from recursive GUI primitives.
/// This builder deliberately treats the viewport as a world camera: the map is
/// fixed to the viewport, the visitor is a world entity, and buildings are
/// spatial landmarks rather than application panels.
/// </summary>
public static class SpatialExperienceGuiBuilder
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
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("min-height", "0")
            .Style("max-height", "100vh")
            .Style("overflow", "hidden")
            .Style("position", "relative")
            .Style("background", Ink)
            .Style("color", "#eafcff")
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
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", "#020a12")
            .Style("border", "0")
            .Style("box-sizing", "border-box")
            .Attribute("id", "workshop-spatial-map")
            .Attribute("tabindex", "0")
            .AriaLabel("Workshop world. Click to walk. Use WASD or arrow keys to move.")
            .OnKeyDown(onKeyDown)
            .PreventDefault("onkeydown")
            .OnClick(args => moveAvatarTo(
                Math.Clamp(args.OffsetX / Math.Max(1, args.Target?.ClientWidth ?? 1) * 100, 4, 96),
                Math.Clamp(args.OffsetY / Math.Max(1, args.Target?.ClientHeight ?? 1) * 100, 8, 92)));

        world.Content(Grid(receiver));

        foreach (var room in rooms)
        {
            var locked = room.Id == "unknown" && !unknownUnlocked;
            world.Content(Building(receiver, room, selectedRoomId == room.Id, locked, approachRoom));
        }

        world.Content(Avatar(receiver, avatarX, avatarY));
        world.Content(Compass(receiver, moveAvatar));
        world.Content(Readout(receiver, avatarX, avatarY, runtime));
        return world;
    }

    private static ElementBuilder Grid(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("pointer-events", "none")
            .Style("opacity", ".45");

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

        svg.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "5").Attribute("y1", "49").Attribute("x2", "95").Attribute("y2", "49")
            .Attribute("stroke", Magenta).Attribute("stroke-opacity", ".3").Attribute("stroke-width", ".12"));
        return svg;
    }

    private static ElementBuilder Building(
        object receiver,
        SpatialRoom room,
        bool selected,
        bool locked,
        Action<string> approachRoom)
    {
        var accent = locked ? Yellow : selected ? Magenta : room.Id == "engineering" ? Cyan : Green;
        var (width, height) = room.Id switch
        {
            "engineering" => (30d, 25d),
            "experience" => (27d, 22d),
            "creation" => (31d, 23d),
            "yup" => (350d, 200d),
            _ => (24d, 20d)
        };

        if (room.Id == "yup")
            return YupBlueprint(receiver, room, selected, approachRoom);

        return WorkshopGui.Button(receiver)
            .PositionAt(room.X, room.Y)
            .Style("z-index", "5")
            .Style("width", $"{width}%")
            .Style("height", $"{height}%")
            .Style("padding", "0")
            .Style("border", $"2px solid {accent}99")
            .Style("border-radius", "0")
            .Style("background", locked ? "rgba(30,24,3,.35)" : "rgba(0,20,30,.3)")
            .Style("color", White)
            .Style("cursor", "pointer")
            .Style("font-family", "inherit")
            .Style("box-sizing", "border-box")
            .AriaLabel(locked ? "Unmapped structure" : $"Approach {room.Name}")
            .OnClick(() => approachRoom(room.Id))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("inset", "7%")
                .Style("border", $"1px solid {accent}66")
                .Style("pointer-events", "none")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("position", "absolute").Style("left", "8%").Style("right", "8%")
                    .Style("top", "24%").Style("height", "1px").Style("background", $"{accent}88"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("position", "absolute").Style("left", "46%").Style("top", "24%")
                    .Style("bottom", "8%").Style("width", "1px").Style("background", $"{accent}66")))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("left", "50%").Style("top", "9%")
                .Style("transform", "translateX(-50%)")
                .Style("padding", ".2rem .4rem")
                .Style("background", "rgba(1,4,10,.82)")
                .Style("border", $"1px solid {accent}55")
                .Style("font-size", room.Id == "engineering" ? ".72rem" : ".55rem")
                .Style("letter-spacing", ".1em")
                .Style("white-space", "nowrap")
                .Style("pointer-events", "none")
                .Text(room.Name))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute").Style("right", ".4rem").Style("bottom", ".3rem")
                .Style("font-size", ".36rem").Style("color", $"{accent}aa")
                .Style("pointer-events", "none")
                .Text(room.Architecture));
    }

    private static ElementBuilder YupBlueprint(object receiver, SpatialRoom room, bool selected, Action<string> approachRoom)
    {
        var border = selected ? Magenta : Cyan;
        return WorkshopGui.Button(receiver)
            .PositionAt(room.X, room.Y)
            .Style("z-index", "5")
            .Style("width", "350px")
            .Style("height", "200px")
            .Style("padding", "0")
            .Style("border", $"6px solid {border}")
            .Style("border-radius", "0")
            .Style("background", "transparent")
            .Style("box-shadow", $"0 0 28px {border}22")
            .Style("cursor", "pointer")
            .Style("box-sizing", "border-box")
            .AriaLabel("Approach Yup conference room")
            .OnClick(() => approachRoom(room.Id))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("inset", "10px")
                .Style("border", $"2px solid {border}66")
                .Style("background", "transparent")
                .Style("pointer-events", "none")
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("position", "absolute").Style("left", "50%").Style("top", "12px")
                    .Style("transform", "translateX(-50%)")
                    .Style("font-size", ".75rem").Style("letter-spacing", ".2em")
                    .Style("color", White).Style("white-space", "nowrap")
                    .Text("YUP // CONFERENCE ROOM"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
                    .Style("width", "92px").Style("height", "54px")
                    .Style("transform", "translate(-50%,-50%)")
                    .Style("border", $"2px solid {border}88")
                    .Style("border-radius", "50%")
                    .Style("box-shadow", $"0 0 18px {border}22"))
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("position", "absolute").Style("left", "50%").Style("bottom", "10px")
                    .Style("transform", "translateX(-50%)")
                    .Style("font-size", ".4rem").Style("letter-spacing", ".14em")
                    .Style("color", border).Style("white-space", "nowrap")
                    .Text("DOOR / ENTER")));
    }

    private static ElementBuilder Avatar(object receiver, double x, double y)
        => WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 40 40")
            .Attribute("aria-label", "Your position")
            .Style("position", "absolute")
            .Style("left", $"{x}%")
            .Style("top", $"{y}%")
            .Style("width", "34px")
            .Style("height", "34px")
            .Style("transform", "translate(-50%,-50%)")
            .Style("z-index", "7")
            .Style("overflow", "visible")
            .Style("pointer-events", "none")
            .Style("transition", "left .28s linear, top .28s linear")
            .Content(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "8")
                .Attribute("fill", "none").Attribute("stroke", White).Attribute("stroke-opacity", ".4")
                .Child(WorkshopGui.Element(receiver, "animate")
                    .Attribute("attributeName", "r").Attribute("values", "6;13;6")
                    .Attribute("dur", "1.8s").Attribute("repeatCount", "indefinite")))
            .Content(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "4")
                .Attribute("fill", White));

    private static ElementBuilder Compass(object receiver, Action<double, double> moveAvatar)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", ".75rem").Style("bottom", ".75rem")
            .Style("z-index", "8").Style("padding", ".4rem")
            .Style("background", "rgba(1,5,11,.9)").Style("border", "1px solid rgba(0,234,255,.18)")
            .Style("align-items", "center").Style("gap", ".15rem");
        panel.Content(StepButton(receiver, "▲", () => moveAvatar(0, -3)));
        var row = WorkshopGui.MultiPanel(receiver).Style("display", "flex").Style("gap", ".15rem");
        row.Content(StepButton(receiver, "◀", () => moveAvatar(-3, 0)));
        row.Content(StepButton(receiver, "▼", () => moveAvatar(0, 3)));
        row.Content(StepButton(receiver, "▶", () => moveAvatar(3, 0)));
        panel.Content(row);
        panel.Content(WorkshopGui.Element(receiver, "small")
            .Style("font-size", ".38rem").Style("color", "#58747e")
            .Style("letter-spacing", ".08em").Text("CLICK / WASD / ARROWS"));
        return panel;
    }

    private static ElementBuilder StepButton(object receiver, string label, Action action)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("width", "28px").Style("height", "26px").Style("padding", "0")
            .Style("border", "1px solid rgba(0,234,255,.28)").Style("background", "rgba(0,234,255,.04)")
            .Style("color", Cyan).Style("cursor", "pointer").OnClick(action);

    private static ElementBuilder Readout(object receiver, double x, double y, HubRuntime runtime)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", ".75rem").Style("bottom", ".75rem")
            .Style("z-index", "8").Style("align-items", "flex-end").Style("gap", ".15rem")
            .Style("padding", ".35rem .5rem").Style("background", "rgba(1,5,11,.78)")
            .Style("border", "1px solid rgba(0,234,255,.12)")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("font-size", ".4rem").Style("color", "#58747e").Style("letter-spacing", ".1em")
                .Text("POSITION // BUNDLES"))
            .Content(WorkshopGui.Element(receiver, "strong")
                .Style("font-size", ".72rem").Style("color", Cyan)
                .Text($"{x:0} : {y:0} // {runtime.Hub.LoadedBundles.Count}"));

    private static ElementBuilder Inspector(
        object receiver,
        IReadOnlyList<SpatialRoom> rooms,
        string? selectedRoomId,
        bool unknownUnlocked,
        Action enterSelectedRoom,
        Action unlockUnknown)
    {
        var selected = rooms.FirstOrDefault(x => x.Id == selectedRoomId);
        if (selected is null)
            return WorkshopGui.Panel(receiver)
                .Style("position", "absolute").Style("right", ".75rem").Style("top", ".75rem")
                .Style("z-index", "8").Style("width", "280px").Style("padding", ".65rem")
                .Style("border", "1px solid rgba(0,234,255,.15)")
                .Style("background", "rgba(1,5,11,.84)")
                .Content(WorkshopGui.Element(receiver, "span")
                    .Style("font-size", ".42rem").Style("color", Cyan).Style("letter-spacing", ".14em")
                    .Text("WORKSHOP // WALK THE WORLD"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".35rem").Style("font-size", ".58rem").Style("color", "#718e99")
                    .Text("Buildings are landmarks. Enter one to reveal its capability."));

        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", ".75rem").Style("top", ".75rem")
            .Style("z-index", "8").Style("width", "280px")
            .Style("padding", ".75rem")
            .Style("border", $"1px solid {(selected.Id == "unknown" ? Yellow : Cyan)}55")
            .Style("background", "rgba(1,5,11,.9)");
        panel.Content(WorkshopGui.Element(receiver, "span")
            .Style("font-size", ".42rem").Style("letter-spacing", ".14em")
            .Style("color", selected.Id == "unknown" ? Yellow : Cyan).Text(selected.Architecture));
        panel.Content(WorkshopGui.Element(receiver, "h2")
            .Style("margin", ".35rem 0 .15rem").Style("font-size", "1.25rem")
            .Text(selected.Name));
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".45rem").Style("letter-spacing", ".1em")
            .Style("color", "#58747e").Text(selected.Kind));
        panel.Content(WorkshopGui.Element(receiver, "p")
            .Style("margin", ".55rem 0").Style("font-size", ".68rem")
            .Style("line-height", "1.45").Style("color", "#91aab2")
            .Text(selected.Description));

        if (selected.Id == "unknown" && !unknownUnlocked)
            panel.Content(SmallAction(receiver, "UNLOCK", unlockUnknown, Yellow));
        else
            panel.Content(SmallAction(receiver, "ENTER ROOM →", enterSelectedRoom, Cyan));
        return panel;
    }

    private static ElementBuilder SmallAction(object receiver, string label, Action action, string accent)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("width", "100%").Style("padding", ".55rem")
            .Style("border", $"1px solid {accent}66").Style("background", $"{accent}0d")
            .Style("color", accent).Style("font-family", "inherit")
            .Style("font-size", ".5rem").Style("letter-spacing", ".12em")
            .Style("cursor", "pointer").OnClick(action);
}
