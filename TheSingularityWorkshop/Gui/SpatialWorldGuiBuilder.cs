using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.Interaction;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Viewport-first Workshop world builder.
/// The visitor is the camera anchor: the avatar remains centered while the
/// world moves beneath it. Spatial suites expose interaction points rather
/// than asking the visitor to navigate through a secondary GUI.
/// </summary>
public static class SpatialWorldGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static ElementBuilder Build(
        object receiver,
        IReadOnlyList<ExperienceSpace> rooms,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        string tourKicker,
        string tourMessage,
        string? hoveredInteractableId,
        WorkshopSign? openSign,
        Func<KeyboardEventArgs, Task> onKeyDown,
        Func<double, double, Task> moveAvatarTo,
        Func<string, Task> interactRoom,
        Action<string?> setHoveredInteractable,
        Action<WorkshopSign> showSign,
        Action closeSign)
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

        root.Content(World(receiver, rooms, unknownUnlocked, avatarX, avatarY, tourKicker, tourMessage,
            hoveredInteractableId, openSign, onKeyDown, moveAvatarTo, interactRoom,
            setHoveredInteractable, showSign, closeSign));
        return root;
    }

    private static ElementBuilder World(
        object receiver,
        IReadOnlyList<ExperienceSpace> rooms,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        string tourKicker,
        string tourMessage,
        string? hoveredInteractableId,
        WorkshopSign? openSign,
        Func<KeyboardEventArgs, Task> onKeyDown,
        Func<double, double, Task> moveAvatarTo,
        Func<string, Task> interactRoom,
        Action<string?> setHoveredInteractable,
        Action<WorkshopSign> showSign,
        Action closeSign)
    {
        var world = WorkshopGui.Panel(receiver)
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", "#020a12")
            .Style("border", "0")
            .Style("box-sizing", "border-box")
            .Style("touch-action", "manipulation")
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
            camera.Content(Building(receiver, room, locked,
                string.Equals(room.Id, hoveredInteractableId, StringComparison.Ordinal),
                interactRoom, setHoveredInteractable, showSign));
        }

        world.Content(camera);
        world.Content(WalkSurface(receiver, avatarX, avatarY, moveAvatarTo));
        world.Content(Avatar(receiver));
        world.Content(TourCard(receiver, tourKicker, tourMessage));
        world.Content(WorkshopInteractableGuiBuilder.SignFace(receiver, openSign, closeSign));
        return world;
    }

    private static string CameraTransform(double avatarX, double avatarY)
        => $"translate(calc(50vw - {avatarX:0.##}vw), calc(50vh - {avatarY:0.##}vh))";

    private static ElementBuilder TourCard(object receiver, string kicker, string message)
        => WorkshopGui.Element(receiver, "section")
            .Style("position", "fixed").Style("left", "50%").Style("top", ".7rem")
            .Style("transform", "translateX(-50%)").Style("z-index", "30")
            .Style("width", "min(680px, calc(100vw - 1rem))").Style("padding", ".65rem .8rem")
            .Style("box-sizing", "border-box").Style("border", "1px solid rgba(0,234,255,.34)")
            .Style("border-left", "3px solid rgba(0,234,255,.78)").Style("background", "rgba(1,4,10,.84)")
            .Style("backdrop-filter", "blur(8px)").Style("box-shadow", "0 0 32px rgba(0,234,255,.08)")
            .Style("pointer-events", "none")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".48rem").Style("letter-spacing", ".22em").Style("color", Cyan).Style("margin-bottom", ".35rem").Text(kicker))
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", "clamp(.56rem, 1.15vw, .78rem)").Style("line-height", "1.45").Style("letter-spacing", ".04em").Style("color", White).Style("max-width", "100%").Text(message));

    private static ElementBuilder WalkSurface(object receiver, double avatarX, double avatarY, Func<double, double, Task> moveAvatarTo)
    {
        const int cells = 24;
        var surface = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "2")
            .Style("display", "grid").Style("grid-template-columns", $"repeat({cells}, 1fr)")
            .Style("grid-template-rows", $"repeat({cells}, 1fr)").Style("pointer-events", "none");

        for (var row = 0; row < cells; row++)
        for (var column = 0; column < cells; column++)
        {
            var screenX = (column + .5) / cells * 100;
            var screenY = (row + .5) / cells * 100;
            var worldX = avatarX + screenX - 50;
            var worldY = avatarY + screenY - 50;
            surface.Content(WorkshopGui.Button(receiver)
                .Style("display", "block").Style("width", "100%").Style("height", "100%")
                .Style("min-width", "0").Style("min-height", "0").Style("margin", "0")
                .Style("padding", "0").Style("border", "0").Style("outline", "none")
                .Style("background", "transparent").Style("pointer-events", "auto")
                .Style("cursor", "crosshair").AriaLabel($"Walk to {worldX:0.#}, {worldY:0.#}")
                .OnClick(() => moveAvatarTo(worldX, worldY)));
        }
        return surface;
    }

    private static ElementBuilder Grid(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true").Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%").Style("pointer-events", "none").Style("opacity", ".42");

        for (var i = 0; i <= 20; i++)
        {
            var p = i * 5;
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", p).Attribute("y1", "0").Attribute("x2", p).Attribute("y2", "100").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".12").Attribute("stroke-width", ".12"));
            svg.Child(WorkshopGui.Element(receiver, "line").Attribute("x1", "0").Attribute("y1", p).Attribute("x2", "100").Attribute("y2", p).Attribute("stroke", Cyan).Attribute("stroke-opacity", ".12").Attribute("stroke-width", ".12"));
        }
        return svg;
    }

    private static ElementBuilder Building(object receiver, ExperienceSpace room, bool locked, bool hovered, Func<string, Task> interactRoom, Action<string?> setHoveredInteractable, Action<WorkshopSign> showSign)
    {
        var interactable = WorkshopInteractableCatalog.Create(room, onInteract: null, onHover: id => setHoveredInteractable(id));
        var accent = locked ? "#ffd34d" : interactable.Presentation.MapIntent.Accent;

        var building = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", $"{room.X:0.##}%").Style("top", $"{room.Y:0.##}%")
            .Style("width", $"{room.Width}%").Style("height", $"{room.Height}%").Style("box-sizing", "border-box")
            .Style("z-index", "5").Style("pointer-events", locked ? "none" : "auto").Style("cursor", locked ? "default" : "pointer")
            .Style("border", $"2px solid {accent}{(hovered ? "ee" : "99")}").Style("background", locked ? "rgba(30,24,3,.35)" : "rgba(0,20,30,.3)")
            .Style("color", White).Style("font-family", "inherit")
            .Style("transition", "transform .18s ease, border-color .18s ease, box-shadow .18s ease")
            .Style("transform", hovered ? "scale(1.025)" : "scale(1)")
            .Style("box-shadow", hovered ? $"0 0 36px {accent}66" : $"0 0 12px {accent}16")
            .Attribute("role", "button").Attribute("tabindex", locked ? "-1" : "0")
            .AriaLabel(locked ? "Unmapped structure" : $"Interact with {room.Name}")
            .OnMouseEnter(() => HoverInteractable(interactable))
            .OnMouseLeave(() => setHoveredInteractable(null))
            .OnClick(() => interactRoom(room.Id));

        building.Content(InteractableSchematicGuiBuilder.Build(receiver, interactable, hovered));
        building.Content(WorkshopGui.Element(receiver, "span")
            .Style("position", "absolute").Style("left", "50%").Style("top", "8%").Style("transform", "translateX(-50%)")
            .Style("z-index", "2").Style("padding", ".2rem .4rem").Style("background", "rgba(1,4,10,.9)")
            .Style("border", $"1px solid {accent}{(hovered ? "cc" : "55")}").Style("font-size", "clamp(.42rem, 1vw, .75rem)")
            .Style("letter-spacing", ".06em").Style("max-width", "90%").Style("white-space", "normal")
            .Style("text-align", "center").Style("pointer-events", "none").Text(room.Name));

        var frame = WorkshopGui.Element(receiver, "div").Style("position", "absolute").Style("inset", "0").Style("pointer-events", "none");
        frame.Content(building);
        if (interactable is WorkshopInteractable workshop)
            frame.Content(WorkshopInteractableGuiBuilder.SignButton(receiver, workshop, showSign).Style("pointer-events", locked ? "none" : "auto"));
        return frame;
    }

    private static void HoverInteractable(IInteractable interactable)
    {
        var context = new InteractionContext("workshop-floor", InteractionScope.World, new HashSet<string>(StringComparer.Ordinal));
        new InteractableExecutor().TryExecute(interactable, InteractionTrigger.Hover, context);
    }

    private static ElementBuilder Avatar(object receiver)
        => WorkshopGui.Element(receiver, "div").Attribute("aria-label", "Your position")
            .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
            .Style("width", "42px").Style("height", "42px").Style("transform", "translate(-50%,-50%)")
            .Style("z-index", "7").Style("pointer-events", "none").Style("border", $"1px solid {White}")
            .Style("border-radius", "50%").Style("box-sizing", "border-box").Style("overflow", "hidden")
            .Content(WorkshopGui.Image(receiver)
                .Attribute("src", "https://avatars.githubusercontent.com/u/16405167?v=4")
                .Attribute("alt", "Workshop visitor").Style("width", "100%").Style("height", "100%")
                .Style("object-fit", "cover").Style("border-radius", "50%"));
}
