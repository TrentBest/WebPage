using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Infrastructure.Hub;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Viewport-first Workshop world builder.
/// The visitor is the camera anchor: the avatar remains centered while the
/// world moves beneath it. Spatial objects expose interaction points rather
/// than asking the visitor to navigate through a secondary GUI.
/// </summary>
public static class SpatialWorldGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder Build(
        object receiver,
        IReadOnlyList<SpatialRoom> rooms,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Action<double, double> moveAvatar,
        Action<double, double> moveAvatarTo,
        Func<string, Task> interactRoom)
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

        root.Content(World(
            receiver,
            rooms,
            unknownUnlocked,
            avatarX,
            avatarY,
            onKeyDown,
            moveAvatar,
            moveAvatarTo,
            interactRoom));

        return root;
    }

    private static ElementBuilder World(
        object receiver,
        IReadOnlyList<SpatialRoom> rooms,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Action<double, double> moveAvatar,
        Action<double, double> moveAvatarTo,
        Func<string, Task> interactRoom)
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
            .AriaLabel("Workshop world. Click a place to walk. Use WASD or arrow keys.")
            .OnKeyDown(onKeyDown)
            .PreventDefault("onkeydown");

        var camera = WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100vw")
            .Style("height", "100vh")
            .Style("overflow", "visible")
            .Style("transform", CameraTransform(avatarX, avatarY))
            .Style("transition", "transform .28s linear")
            .Style("will-change", "transform")
            .Style("pointer-events", "none");

        camera.Content(Grid(receiver));

        foreach (var room in rooms)
        {
            var locked = room.Id == "unknown" && !unknownUnlocked;
            camera.Content(Building(receiver, room, locked, interactRoom));
        }

        world.Content(camera);
        world.Content(WalkSurface(receiver, avatarX, avatarY, moveAvatarTo));
        world.Content(Avatar(receiver));
        return world;
    }

    private static string CameraTransform(double avatarX, double avatarY)
        => $"translate(calc(50vw - {avatarX:0.##}vw), calc(50vh - {avatarY:0.##}vh))";

    private static ElementBuilder WalkSurface(
        object receiver,
        double avatarX,
        double avatarY,
        Action<double, double> moveAvatarTo)
    {
        const int cells = 24;
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("z-index", "2")
            .Style("display", "grid")
            .Style("grid-template-columns", $"repeat({cells}, 1fr)")
            .Style("grid-template-rows", $"repeat({cells}, 1fr)")
            .Style("pointer-events", "none");

        for (var row = 0; row < cells; row++)
        for (var column = 0; column < cells; column++)
        {
            var screenX = (column + .5) / cells * 100;
            var screenY = (row + .5) / cells * 100;
            var worldX = avatarX + screenX - 50;
            var worldY = avatarY + screenY - 50;

            surface.Content(WorkshopGui.Button(receiver)
                .Style("display", "block")
                .Style("width", "100%")
                .Style("height", "100%")
                .Style("min-width", "0")
                .Style("min-height", "0")
                .Style("margin", "0")
                .Style("padding", "0")
                .Style("border", "0")
                .Style("outline", "none")
                .Style("background", "transparent")
                .Style("pointer-events", "auto")
                .Style("cursor", "crosshair")
                .AriaLabel($"Walk to {worldX:0.#}, {worldY:0.#}")
                .OnClick(() => moveAvatarTo(worldX, worldY)));
        }

        return surface;
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
            .Style("opacity", ".42");

        for (var i = 0; i <= 20; i++)
        {
            var p = i * 5;
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", p).Attribute("y1", "0")
                .Attribute("x2", p).Attribute("y2", "100")
                .Attribute("stroke", Cyan)
                .Attribute("stroke-opacity", ".12")
                .Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "0").Attribute("y1", p)
                .Attribute("x2", "100").Attribute("y2", p)
                .Attribute("stroke", Cyan)
                .Attribute("stroke-opacity", ".12")
                .Attribute("stroke-width", ".12"));
        }

        return svg;
    }

    private static ElementBuilder Building(
        object receiver,
        SpatialRoom room,
        bool locked,
        Func<string, Task> interactRoom)
    {
        var accent = locked
            ? Yellow
            : room.Id == "engineering" ? Cyan : Green;

        var size = room.Id switch
        {
            "engineering" => (30d, 25d),
            "experience" => (27d, 22d),
            "creation" => (31d, 23d),
            _ => (24d, 20d)
        };

        return WorkshopGui.Button(receiver)
            .PositionAt(room.X, room.Y)
            .Style("z-index", "5")
            .Style("width", $"{size.Item1}%")
            .Style("height", $"{size.Item2}%")
            .Style("padding", "0")
            .Style("border", $"2px solid {accent}99")
            .Style("border-radius", "0")
            .Style("background", locked ? "rgba(30,24,3,.35)" : "rgba(0,20,30,.3)")
            .Style("color", White)
            .Style("font-family", "inherit")
            .Style("cursor", locked ? "default" : "pointer")
            .Style("box-sizing", "border-box")
            .Style("pointer-events", locked ? "none" : "auto")
            .AriaLabel(locked ? "Unmapped structure" : $"Interact with {room.Name}")
            .OnClick(() => interactRoom(room.Id))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("position", "absolute")
                .Style("left", "50%")
                .Style("top", "10%")
                .Style("transform", "translateX(-50%)")
                .Style("padding", ".2rem .4rem")
                .Style("background", "rgba(1,4,10,.85)")
                .Style("border", $"1px solid {accent}55")
                .Style("font-size", room.Id == "engineering" ? ".75rem" : ".55rem")
                .Style("letter-spacing", ".1em")
                .Style("white-space", "nowrap")
                .Style("pointer-events", "none")
                .Text(room.Name))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute")
                .Style("inset", "8%")
                .Style("border", $"1px solid {accent}55")
                .Style("pointer-events", "none"));
    }

    private static ElementBuilder Avatar(object receiver)
        => WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 40 40")
            .Attribute("aria-label", "Your position")
            .Style("position", "absolute")
            .Style("left", "50%")
            .Style("top", "50%")
            .Style("width", "34px")
            .Style("height", "34px")
            .Style("transform", "translate(-50%,-50%)")
            .Style("z-index", "7")
            .Style("pointer-events", "none")
            .Style("overflow", "visible")
            .Content(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "20")
                .Attribute("cy", "20")
                .Attribute("r", "8")
                .Attribute("fill", "none")
                .Attribute("stroke", White)
                .Attribute("stroke-opacity", ".4")
                .Child(WorkshopGui.Element(receiver, "animate")
                    .Attribute("attributeName", "r")
                    .Attribute("values", "6;13;6")
                    .Attribute("dur", "1.8s")
                    .Attribute("repeatCount", "indefinite")))
            .Content(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", "20")
                .Attribute("cy", "20")
                .Attribute("r", "4")
                .Attribute("fill", White));
}
