using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Infrastructure.Hub;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Builds the spatial Explore Experience from Workshop GUI primitives.
/// The page owns state and behavior; this builder owns presentation composition.
/// The map is intentionally expressed as a blueprint: structures are places,
/// not cards, and the visitor can move through the environment.
/// </summary>
public static class SpatialExploreGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff2cff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Ink = "#01040a";

    public static RenderFragment Build(
        object receiver,
        HubRuntime runtime,
        IReadOnlyList<SpatialRoom> rooms,
        string? selectedRoomId,
        string? insideRoomId,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Action<double, double> moveAvatar,
        Action<double, double> moveAvatarTo,
        Action<string> approachRoom,
        Action enterSelectedRoom,
        Action unlockUnknown,
        Action exitRoom)
        => builder =>
        {
            var root = WorkshopGui.Panel(receiver)
                .Style("position", "relative")
                .Style("min-height", "100vh")
                .Style("overflow", "hidden")
                .Style("background", Ink)
                .Style("color", "#eafcff")
                .Style("font-family", "Consolas, 'Courier New', monospace")
                .Style("display", "flex")
                .Style("flex-direction", "column");

            root.Content(Warp(receiver));
            root.Content(Header(receiver, runtime));
            root.Content(MapAndInspector(receiver, runtime, rooms, selectedRoomId, insideRoomId,
                unknownUnlocked, avatarX, avatarY, onKeyDown, moveAvatar, moveAvatarTo, approachRoom,
                enterSelectedRoom, unlockUnknown, exitRoom));
            root.Content(Footer(receiver));

            builder.AddContent(0, root.Build());
        };

    private static ElementBuilder Warp(object receiver)
    {
        var layer = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("z-index", "20")
            .Style("pointer-events", "none");

        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute")
            .Style("inset", "0")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("background", "#000");

        var endpoints = new[]
        {
            (95d,50d),(93.5,61.6),(89,72.5),(81.8,81.8),(72.5,89),(61.6,93.5),
            (50,95),(38.4,93.5),(27.5,89),(18.2,81.8),(11,72.5),(6.5,61.6),
            (5,50),(6.5,38.4),(11,27.5),(18.2,18.2),(27.5,11),(38.4,6.5),
            (50,5),(61.6,6.5),(72.5,11),(81.8,18.2),(89,27.5),(93.5,38.4)
        };

        for (var i = 0; i < endpoints.Length; i++)
        {
            var (x, y) = endpoints[i];
            var line = WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "50").Attribute("y1", "50")
                .Attribute("x2", x).Attribute("y2", y)
                .Attribute("stroke", White).Attribute("stroke-width", "0.12")
                .Attribute("opacity", "0");
            line.Child(Animate(receiver, "x2", $"{x};{50 + (x - 50) * 3.2}", "2.4s", $"{i * 0.035:0.###}s"));
            line.Child(Animate(receiver, "y2", $"{y};{50 + (y - 50) * 3.2}", "2.4s", $"{i * 0.035:0.###}s"));
            line.Child(Animate(receiver, "opacity", "0;.9;0", "2.4s", $"{i * 0.035:0.###}s"));
            svg.Child(line);
        }

        var core = WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "50").Attribute("cy", "50").Attribute("r", "2").Attribute("fill", White);
        core.Child(Animate(receiver, "r", "2;46", "2.6s", "0s"));
        core.Child(Animate(receiver, "opacity", "1;0", "2.6s", "0s"));
        svg.Child(core);
        layer.Child(svg);

        var caption = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
            .Style("transform", "translate(-50%,-50%)").Style("align-items", "center")
            .Style("gap", ".4rem").Style("text-align", "center")
            .Style("background", "transparent").Style("border", "0").Attribute("aria-hidden", "true");
        caption.Content(WorkshopGui.Element(receiver, "span")
            .Style("font-size", ".55rem").Style("letter-spacing", ".4em").Style("color", Cyan).Text("ENTERING"));
        caption.Content(WorkshopGui.Element(receiver, "strong")
            .Style("font-size", "clamp(1.8rem, 6vw, 5rem)").Style("letter-spacing", ".08em")
            .Style("color", White).Style("text-shadow", "0 0 22px rgba(0,234,255,.8)").Text("THE WORKSHOP"));
        layer.Child(caption);
        return layer;
    }

    private static ElementBuilder Header(object receiver, HubRuntime runtime)
    {
        var header = WorkshopGui.Panel(receiver)
            .Style("margin", ".75rem").Style("padding", ".8rem 1rem")
            .Style("border", "1px solid rgba(0,234,255,.18)")
            .Style("background", "rgba(1,7,15,.82)").Style("display", "flex")
            .Style("justify-content", "space-between").Style("align-items", "flex-end")
            .Style("gap", "2rem").Style("z-index", "2");

        var copy = WorkshopGui.Panel(receiver).Style("border", "0").Style("background", "transparent");
        copy.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan).Style("font-size", ".56rem").Style("letter-spacing", ".2em")
            .Text("EXPERIENCE // EXPLORATION"));
        copy.Heading("The Workshop is a place.", 1)
            .Style("margin", ".35rem 0 .2rem").Style("font-size", "clamp(1.6rem,3vw,3.1rem)").Style("line-height", "1");
        copy.Paragraph("Walk it. Find its boundaries. Discover what is alive inside.")
            .Style("margin", "0").Style("color", "#718e99")
            .Style("font-family", "system-ui, sans-serif").Style("font-size", ".8rem");
        header.Content(copy);

        var readout = WorkshopGui.MultiPanel(receiver)
            .Style("display", "flex").Style("gap", "1.2rem").Style("align-items", "center")
            .Style("white-space", "nowrap").Style("color", "#58747e")
            .Style("font-size", ".48rem").Style("letter-spacing", ".08em");
        readout.Content(Metric(receiver, runtime.Hub.LoadedBundles.Count, "BUNDLES"));
        readout.Content(Metric(receiver, runtime.ArbitrationMutations, "MUTATIONS"));
        readout.Content(Metric(receiver, runtime.Hub.Audit.Events.Count, "EVENTS"));
        header.Content(readout);
        return header;
    }

    private static ElementBuilder Metric(object receiver, int value, string label)
        => WorkshopGui.Element(receiver, "span").Text($"{value} {label}")
            .Style("color", "#58747e").Style("font-size", ".48rem").Style("letter-spacing", ".08em");

    private static ElementBuilder MapAndInspector(
        object receiver,
        HubRuntime runtime,
        IReadOnlyList<SpatialRoom> rooms,
        string? selectedRoomId,
        string? insideRoomId,
        bool unknownUnlocked,
        double avatarX,
        double avatarY,
        Action<KeyboardEventArgs> onKeyDown,
        Action<double, double> moveAvatar,
        Action<double, double> moveAvatarTo,
        Action<string> approachRoom,
        Action enterSelectedRoom,
        Action unlockUnknown,
        Action exitRoom)
    {
        var body = WorkshopGui.MultiPanel(receiver)
            .Style("display", "grid").Style("grid-template-columns", "minmax(0, 1fr) 300px")
            .Style("gap", ".75rem").Style("flex", "1").Style("min-height", "0")
            .Style("margin", "0 .75rem").Style("z-index", "1");

        body.Content(Map(receiver, rooms, selectedRoomId, unknownUnlocked, avatarX, avatarY,
            onKeyDown, moveAvatar, moveAvatarTo, approachRoom));
        body.Content(Inspector(receiver, runtime, rooms, selectedRoomId, insideRoomId,
            unknownUnlocked, enterSelectedRoom, unlockUnknown, exitRoom));
        return body;
    }

    private static ElementBuilder Map(
        object receiver,
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
        var map = WorkshopGui.Panel(receiver)
            .Style("position", "relative").Style("min-height", "620px").Style("overflow", "hidden")
            .Style("border", "1px solid rgba(0,234,255,.2)").Style("outline", "none")
            .Style("background", "#020a12").Attribute("tabindex", "0")
            .Attribute("id", "workshop-spatial-map")
            .AriaLabel("Interactive Workshop blueprint. Click a location to walk there. Use WASD or arrow keys to move.")
            .OnKeyDown(onKeyDown).PreventDefault("onkeydown");

        map.Content(MapGrid(receiver));
        map.Content(MapClickGrid(receiver, moveAvatarTo));

        foreach (var room in rooms)
        {
            var locked = room.Id == "unknown" && !unknownUnlocked;
            var focused = selectedRoomId == room.Id;
            map.Content(BlueprintBuilding(receiver, room, focused, locked, approachRoom));
        }

        map.Content(Avatar(receiver, avatarX, avatarY));
        map.Content(MovementControls(receiver, moveAvatar));
        map.Content(PositionReadout(receiver, avatarX, avatarY));

        if (unknownUnlocked)
            map.Content(DiscoveryNotice(receiver));

        return map;
    }

    private static ElementBuilder MapGrid(object receiver)
    {
        var grid = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100").Attribute("preserveAspectRatio", "none")
            .Attribute("aria-hidden", "true")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100%").Style("height", "100%").Style("opacity", ".4")
            .Style("pointer-events", "none");

        for (var i = 0; i <= 20; i++)
        {
            var p = i * 5;
            grid.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", p).Attribute("y1", "0").Attribute("x2", p).Attribute("y2", "100")
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".12").Attribute("stroke-width", ".12"));
            grid.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", "0").Attribute("y1", p).Attribute("x2", "100").Attribute("y2", p)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".12").Attribute("stroke-width", ".12"));
        }

        grid.Child(WorkshopGui.Element(receiver, "line")
            .Attribute("x1", "5").Attribute("y1", "49").Attribute("x2", "95").Attribute("y2", "49")
            .Attribute("stroke", Magenta).Attribute("stroke-opacity", ".4").Attribute("stroke-width", ".12"));
        return grid;
    }

    /// <summary>
    /// A deliberately invisible interaction lattice. It gives the prototype a
    /// real walk surface now while keeping the coordinate system independent of
    /// viewport dimensions. The lattice can later become terrain/navigation.
    /// </summary>
    private static ElementBuilder MapClickGrid(object receiver, Action<double, double> moveAvatarTo)
    {
        var grid = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0").Style("z-index", "2")
            .Style("display", "grid").Style("grid-template-columns", "repeat(20, 1fr)")
            .Style("grid-template-rows", "repeat(20, 1fr)").Style("pointer-events", "none");

        for (var row = 0; row < 20; row++)
        {
            for (var column = 0; column < 20; column++)
            {
                var x = column * 5 + 2.5;
                var y = row * 5 + 2.5;
                grid.Content(WorkshopGui.Button(receiver)
                    .Style("width", "100%").Style("height", "100%").Style("padding", "0")
                    .Style("margin", "0").Style("border", "0").Style("background", "transparent")
                    .Style("outline", "none").Style("cursor", "crosshair")
                    .Style("pointer-events", "auto")
                    .AriaLabel($"Walk to {x:0.#}, {y:0.#}")
                    .OnClick(() => moveAvatarTo(x, y)));
            }
        }

        return grid;
    }

    private static ElementBuilder BlueprintBuilding(
        object receiver,
        SpatialRoom room,
        bool focused,
        bool locked,
        Action<string> approachRoom)
    {
        var accent = locked ? Yellow : focused ? Magenta : room.Id == "engineering" ? Cyan : Green;
        var (width, height) = room.Id switch
        {
            "engineering" => (31d, 27d),
            "experience" => (29d, 23d),
            "creation" => (34d, 25d),
            _ => (25d, 21d)
        };

        var building = WorkshopGui.Button(receiver)
            .PositionAt(room.X, room.Y)
            .Style("z-index", "5").Style("width", $"{width}%").Style("height", $"{height}%")
            .Style("padding", "0").Style("border", $"1px solid {accent}99")
            .Style("background", locked ? "rgba(30,24,3,.35)" : "rgba(0,20,30,.34)")
            .Style("color", White).Style("cursor", "pointer").Style("font-family", "inherit")
            .Style("box-sizing", "border-box")
            .AriaLabel(locked ? "Unmapped structure" : $"Approach {room.Name}")
            .Title(locked ? "Unmapped structure" : $"Approach {room.Name}")
            .OnClick(() => approachRoom(room.Id));

        var floor = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "7%")
            .Style("border", $"1px solid {accent}55")
            .Style("background", "repeating-linear-gradient(90deg, transparent 0 19px, rgba(0,234,255,.05) 20px 21px)")
            .Style("box-shadow", focused ? $"0 0 35px {accent}33, inset 0 0 28px {accent}12" : "inset 0 0 18px rgba(0,234,255,.04)")
            .Style("pointer-events", "none");

        var topWall = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "10%").Style("right", "10%").Style("top", "24%")
            .Style("height", "1px").Style("background", $"{accent}88");
        var midWall = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "46%").Style("top", "24%").Style("bottom", "10%")
            .Style("width", "1px").Style("background", $"{accent}66");
        floor.Child(topWall).Child(midWall);

        var core = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "50%").Style("top", "61%")
            .Style("width", room.Id == "engineering" ? "30px" : "20px")
            .Style("height", room.Id == "engineering" ? "30px" : "20px")
            .Style("transform", "translate(-50%,-50%) rotate(45deg)")
            .Style("border", $"1px solid {accent}").Style("box-shadow", $"0 0 14px {accent}55")
            .Style("background", $"{accent}0d");
        floor.Child(core);

        var door = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "50%").Style("bottom", "-1px")
            .Style("width", "20%").Style("height", "4px").Style("transform", "translateX(-50%)")
            .Style("background", Ink).Style("border-left", $"1px solid {accent}")
            .Style("border-right", $"1px solid {accent}");
        floor.Child(door);
        building.Child(floor);

        var title = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("top", "8%")
            .Style("transform", "translateX(-50%)").Style("align-items", "center")
            .Style("gap", ".15rem").Style("white-space", "nowrap")
            .Style("background", "rgba(1,4,10,.78)").Style("border", $"1px solid {accent}55")
            .Style("padding", ".22rem .45rem").Style("pointer-events", "none");
        title.Content(WorkshopGui.Element(receiver, "b")
            .Style("font-size", room.Id == "engineering" ? ".7rem" : ".58rem")
            .Style("letter-spacing", ".12em").Style("color", White).Text(room.Name));
        title.Content(WorkshopGui.Element(receiver, "small")
            .Style("font-size", ".4rem").Style("letter-spacing", ".1em")
            .Style("color", accent).Text(room.Kind));
        building.Child(title);

        var scale = WorkshopGui.Element(receiver, "span")
            .Style("position", "absolute").Style("right", ".45rem").Style("bottom", ".35rem")
            .Style("font-size", ".36rem").Style("letter-spacing", ".08em").Style("color", $"{accent}99")
            .Style("pointer-events", "none").Text(room.Id == "engineering" ? "PLAN // A-001" : "PLAN // SPACE");
        building.Child(scale);
        return building;
    }

    private static ElementBuilder Avatar(object receiver, double x, double y)
    {
        var avatar = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 40 40").Attribute("aria-label", "Your position")
            .Style("position", "absolute").Style("left", $"{x}%").Style("top", $"{y}%")
            .Style("width", "30px").Style("height", "30px")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "7").Style("overflow", "visible")
            .Style("pointer-events", "none");
        avatar.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "8")
            .Attribute("fill", "none").Attribute("stroke", White).Attribute("stroke-opacity", ".35")
            .Child(Animate(receiver, "r", "6;13;6", "1.8s", "0s"))
            .Child(Animate(receiver, "opacity", ".7;.1;.7", "1.8s", "0s")));
        avatar.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", "20").Attribute("cy", "20").Attribute("r", "4").Attribute("fill", White));
        return avatar;
    }

    private static ElementBuilder MovementControls(object receiver, Action<double, double> moveAvatar)
    {
        var controls = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", ".75rem").Style("bottom", ".75rem")
            .Style("z-index", "8").Style("align-items", "center").Style("gap", ".2rem")
            .Style("padding", ".45rem").Style("border", "1px solid rgba(0,234,255,.16)")
            .Style("background", "rgba(1,5,11,.86)");
        controls.Content(MoveButton(receiver, "▲", () => moveAvatar(0, -5)));
        var row = WorkshopGui.MultiPanel(receiver).Style("display", "flex").Style("gap", ".2rem");
        row.Content(MoveButton(receiver, "◀", () => moveAvatar(-5, 0)));
        row.Content(MoveButton(receiver, "▼", () => moveAvatar(0, 5)));
        row.Content(MoveButton(receiver, "▶", () => moveAvatar(5, 0)));
        controls.Content(row);
        controls.Content(WorkshopGui.Element(receiver, "small")
            .Style("color", "#4f6c76").Style("font-size", ".4rem").Style("letter-spacing", ".08em")
            .Text("CLICK / WASD / ARROWS"));
        return controls;
    }

    private static ElementBuilder MoveButton(object receiver, string label, Action action)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("width", "27px").Style("height", "25px").Style("padding", "0")
            .Style("border", "1px solid rgba(0,234,255,.28)").Style("background", "rgba(0,234,255,.035)")
            .Style("color", Cyan).Style("cursor", "pointer").OnClick(action);

    private static ElementBuilder PositionReadout(object receiver, double x, double y)
    {
        var readout = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", ".75rem").Style("bottom", ".75rem")
            .Style("z-index", "8").Style("align-items", "flex-end").Style("gap", ".2rem")
            .Style("color", "#506d77").Style("font-size", ".45rem").Style("letter-spacing", ".1em")
            .Style("background", "transparent").Style("border", "0");
        readout.Content(WorkshopGui.Element(receiver, "span").Text("POSITION"));
        readout.Content(WorkshopGui.Element(receiver, "strong").Style("color", Cyan).Style("font-size", ".75rem").Text($"{x:0} : {y:0}"));
        readout.Content(WorkshopGui.Element(receiver, "small").Style("font-size", ".4rem").Text("CLICK A PLACE TO WALK"));
        return readout;
    }

    private static ElementBuilder DiscoveryNotice(object receiver)
    {
        var notice = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("top", "50%")
            .Style("transform", "translate(-50%,-50%)").Style("z-index", "9")
            .Style("align-items", "center").Style("gap", ".35rem")
            .Style("padding", "1rem 1.5rem").Style("border", $"1px solid {Yellow}")
            .Style("background", "rgba(20,16,2,.9)").Style("box-shadow", "0 0 45px rgba(255,211,77,.18)");
        notice.Content(WorkshopGui.Element(receiver, "span")
            .Style("color", Yellow).Style("font-size", ".5rem").Style("letter-spacing", ".2em").Text("BOUNDARY UNLOCKED"));
        notice.Content(WorkshopGui.Element(receiver, "strong")
            .Style("color", White).Style("font-size", ".75rem").Style("letter-spacing", ".1em")
            .Text("NEW FUNCTIONAL SPACES ONLINE"));
        return notice;
    }

    private static ElementBuilder Inspector(
        object receiver,
        HubRuntime runtime,
        IReadOnlyList<SpatialRoom> rooms,
        string? selectedRoomId,
        string? insideRoomId,
        bool unknownUnlocked,
        Action enterSelectedRoom,
        Action unlockUnknown,
        Action exitRoom)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("min-width", "0").Style("padding", "1rem")
            .Style("border", "1px solid rgba(255,44,255,.2)").Style("background", "rgba(2,8,17,.9)")
            .Style("display", "flex").Style("flex-direction", "column");
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan).Style("font-size", ".5rem").Style("letter-spacing", ".16em")
            .Text("LOCAL SPACE // INSPECTOR"));

        var inside = rooms.FirstOrDefault(x => x.Id == insideRoomId);
        var selected = rooms.FirstOrDefault(x => x.Id == selectedRoomId);

        if (inside is not null)
        {
            panel.Content(WorkshopGui.Element(receiver, "h2")
                .Style("margin", ".8rem 0 .3rem").Style("font-size", "1.2rem").Text(inside.Name));
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Magenta).Style("font-size", ".48rem").Style("letter-spacing", ".12em")
                .Text("INSIDE EXPERIENCE"));
            panel.Content(WorkshopGui.Element(receiver, "p")
                .Style("margin", ".8rem 0").Style("color", "#91aab2")
                .Style("font-family", "system-ui, sans-serif").Style("font-size", ".8rem")
                .Text(inside.Description));
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", "auto").Style("padding", ".7rem")
                .Style("border", "1px solid rgba(0,234,255,.12)").Style("color", "#58747e")
                .Style("font-size", ".55rem").Text($"ARCHITECTURE // {inside.Architecture}"));
            panel.Content(ActionButton(receiver, "← EXIT TO MAP", exitRoom, Cyan));
            return panel;
        }

        if (selected is null)
        {
            panel.Content(WorkshopGui.Element(receiver, "h2")
                .Style("margin", ".8rem 0 .3rem").Style("font-size", "1.2rem").Text("Explore the Workshop"));
            panel.Content(WorkshopGui.Element(receiver, "p")
                .Style("color", "#718e99").Style("font-family", "system-ui, sans-serif").Style("font-size", ".8rem")
                .Text("This is a place, not a menu. Click anywhere to walk. Buildings are landmarks; rooms reveal their deeper tools."));
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", "auto").Style("color", "#46636e").Style("font-size", ".48rem")
                .Style("letter-spacing", ".12em").Text("WORKSHOP // SPATIAL MODEL // TOUR WITHOUT RAILS"));
            return panel;
        }

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".8rem").Style("color", selected.Architecture == "UNKNOWN" ? Yellow : Cyan)
            .Style("font-size", ".5rem").Style("letter-spacing", ".18em").Text(selected.Architecture));
        panel.Content(WorkshopGui.Element(receiver, "h2")
            .Style("margin", ".5rem 0 .25rem").Style("font-size", "1.5rem").Text(selected.Name));
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", "#58747e").Style("font-size", ".5rem").Style("letter-spacing", ".1em").Text(selected.Kind));
        panel.Content(WorkshopGui.Element(receiver, "p")
            .Style("margin", ".9rem 0").Style("color", "#91aab2")
            .Style("font-family", "system-ui, sans-serif").Style("font-size", ".8rem")
            .Text(selected.Description));

        if (selected.Id == "unknown" && !unknownUnlocked)
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", "auto").Style("padding", ".75rem")
                .Style("border", $"1px solid {Yellow}55").Style("background", "rgba(255,211,77,.04)")
                .Style("color", Yellow).Style("font-size", ".52rem").Style("letter-spacing", ".08em")
                .Text("WARNING // UNMAPPED STRUCTURE"));
            panel.Content(ActionButton(receiver, "WARNING...", unlockUnknown, Yellow));
        }
        else
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", "auto").Style("padding", ".7rem")
                .Style("border", "1px solid rgba(0,234,255,.12)").Style("color", "#58747e")
                .Style("font-size", ".55rem").Text($"BOUNDARY // {selected.Architecture}"));
            panel.Content(ActionButton(receiver, "ENTER ROOM →", enterSelectedRoom, Magenta));
        }

        return panel;
    }

    private static ElementBuilder ActionButton(object receiver, string label, Action action, string accent)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("margin-top", ".7rem").Style("padding", ".7rem .8rem")
            .Style("border", $"1px solid {accent}77").Style("background", $"{accent}12")
            .Style("color", accent).Style("font-family", "inherit")
            .Style("font-size", ".55rem").Style("letter-spacing", ".12em")
            .Style("cursor", "pointer").OnClick(action);

    private static ElementBuilder Footer(object receiver)
        => WorkshopGui.Panel(receiver)
            .Style("margin", ".75rem").Style("padding", ".65rem 1rem")
            .Style("border", "1px solid rgba(0,234,255,.1)").Style("background", "rgba(1,5,10,.7)")
            .Style("align-items", "center").Style("text-align", "center").Style("z-index", "2")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("color", "#5a7781").Style("font-size", ".48rem").Style("letter-spacing", ".16em")
                .Text("THE BUILDER IS THE DOCUMENTATION."))
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("color", "#334b54").Style("font-size", ".43rem").Style("letter-spacing", ".1em")
                .Text("EXPERIENCE // NOT A CARD CATALOG"));

    private static ElementBuilder Animate(object receiver, string attributeName, string values, string duration, string begin)
        => WorkshopGui.Element(receiver, "animate")
            .Attribute("attributeName", attributeName).Attribute("values", values)
            .Attribute("dur", duration).Attribute("begin", begin).Attribute("repeatCount", "indefinite");
}

public sealed record SpatialRoom(
    string Id,
    string Name,
    string Kind,
    double X,
    double Y,
    string Description,
    string Architecture,
    string? Capability);
