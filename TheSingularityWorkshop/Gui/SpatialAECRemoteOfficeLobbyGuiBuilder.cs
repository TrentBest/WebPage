namespace TheSingularityWorkshop.Gui;

/// <summary>
/// The staged arrival experience for the AEC Remote Office.
/// The visitor enters a real floor-plan presentation, walks to reception,
/// is acknowledged by the receptionist, and is then led into the conference room.
/// </summary>
public static class SpatialAECRemoteOfficeLobbyGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";
    private const string Muted = "#7895a1";
    private const string Ink = "#02080f";

    /// <summary>Defines the current physical/presentation stage of the office arrival.</summary>
    public enum Stage
    {
        Arrival,
        Reception,
        Conference
    }

    public static ElementBuilder Build(
        object receiver,
        string avatarName,
        Stage stage,
        SpatialWorkshopScene scene,
        SpatialWorkshopArrangement arrangement,
        string? selectedBuildingId,
        Action speakToReceptionist,
        Action beginConsultation,
        Action<string, int, int> moveBuilding,
        Action<string> editBuilding,
        Action exitLobby)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Styles(receiver));
        root.Content(FloorPlan(receiver, stage));
        root.Content(Avatar(receiver, avatarName, stage));
        root.Content(StageOverlay(receiver, avatarName, stage, speakToReceptionist, beginConsultation));
        root.Content(WorkshopConfiguration(receiver, scene, arrangement, selectedBuildingId, moveBuilding, editBuilding));
        root.Content(Exit(receiver, exitLobby));
        return root;
    }


    private static ElementBuilder WorkshopConfiguration(
        object receiver,
        SpatialWorkshopScene scene,
        SpatialWorkshopArrangement arrangement,
        string? selectedBuildingId,
        Action<string, int, int> moveBuilding,
        Action<string> editBuilding)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", "2%").Style("top", "17%").Style("z-index", "35")
            .Style("width", "min(30rem,31vw)").Style("max-height", "68vh").Style("overflow", "auto")
            .Style("padding", ".75rem")
            .Style("border", $"1px solid {Yellow}55")
            .Style("background", "rgba(1,6,12,.94)");

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Yellow).Style("font-size", ".43rem").Style("letter-spacing", ".15em")
            .Text("AEC REMOTE OFFICE // AUTHORIZED WORKSHOP CONFIGURATION"));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".35rem").Style("color", Muted).Style("font-size", ".33rem").Style("line-height", "1.45")
            .Text("This is the only place in the visitor experience where Workshop campus geometry may be proposed. The master map is read-only because a campus change affects everyone."));

        foreach (var item in scene.Interactables)
        {
            var selected = string.Equals(item.Id, selectedBuildingId, StringComparison.OrdinalIgnoreCase);
            var bounds = arrangement.GetBounds(item);
            var row = WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".3rem").Style("padding", ".35rem")
                .Style("border", $"1px solid {(selected ? Cyan : "#ffffff")}33")
                .Style("background", selected ? $"{Cyan}0a" : "rgba(255,255,255,.018)");

            row.Content(WorkshopGui.Element(receiver, "div")
                .Style("color", selected ? Cyan : White).Style("font-size", ".37rem")
                .Text($"{item.Name.ToUpperInvariant()} // X {bounds.X:0.#} Y {bounds.Y:0.#}"));

            var controls = WorkshopGui.Element(receiver, "div")
                .Style("display", "flex").Style("gap", ".2rem").Style("margin-top", ".25rem").Style("flex-wrap", "wrap");

            foreach (var (label, dx, dy) in new[] { ("←", -1, 0), ("↑", 0, -1), ("↓", 0, 1), ("→", 1, 0) })
                controls.Content(WorkshopGui.Button(receiver).Label(label)
                    .Style("padding", ".22rem .34rem").Style("font-family", "inherit").Style("font-size", ".34rem")
                    .Style("cursor", "pointer").OnClick(() => moveBuilding(item.Id, dx, dy)));

            controls.Content(WorkshopGui.Button(receiver).Label("EDIT BUILDING")
                .Style("padding", ".22rem .42rem").Style("border", $"1px solid {Magenta}66")
                .Style("background", $"{Magenta}0a").Style("color", Magenta)
                .Style("font-family", "inherit").Style("font-size", ".34rem").Style("cursor", "pointer")
                .OnClick(() => editBuilding(item.Id)));

            row.Content(controls);
            panel.Content(row);
        }

        return panel;
    }

    private static ElementBuilder FloorPlan(object receiver, Stage stage)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("background", "radial-gradient(circle at 50% 42%, #102633 0, #02080f 58%)");

        // Architectural field.
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "8").Attribute("y", "15").Attribute("width", "84").Attribute("height", "70")
            .Attribute("fill", "rgba(0,234,255,.018)")
            .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".55").Attribute("stroke-width", ".45"));

        // Major rooms.
        Room(svg, receiver, 10, 18, 23, 24, "ENTRY / SECURITY", Cyan);
        Room(svg, receiver, 35, 18, 22, 24, "RECEPTION", Yellow);
        Room(svg, receiver, 59, 18, 31, 24, "CONFERENCE", Magenta);
        Room(svg, receiver, 10, 45, 35, 36, "ARCHITECTURE + PROGRAM", Cyan);
        Room(svg, receiver, 47, 45, 20, 36, "STRUCTURAL", Yellow);
        Room(svg, receiver, 69, 45, 21, 36, "SYSTEMS / MEP", Cyan);

        // Circulation spine.
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "31").Attribute("y", "42").Attribute("width", "38").Attribute("height", "6")
            .Attribute("fill", "rgba(255,211,77,.035)")
            .Attribute("stroke", Yellow).Attribute("stroke-opacity", ".22").Attribute("stroke-width", ".2"));

        // Reception desk and conference table.
        Furniture(svg, receiver, 39, 31, 14, 3, Yellow);
        Furniture(svg, receiver, 65, 29, 20, 7, Magenta);
        Furniture(svg, receiver, 69, 25, 12, 2, White);

        // Doors.
        Door(svg, receiver, 34, 30, "entry");
        Door(svg, receiver, 57, 30, "conference");

        // Route the visitor through the architecture.
        var route = stage switch
        {
            Stage.Arrival => "18,76 18,55 27,55 27,42 40,42 40,34",
            Stage.Reception => "18,76 18,55 27,55 27,42 40,42 40,34",
            _ => "18,76 18,55 27,55 40,55 52,55 68,55 68,42 74,34"
        };

        svg.Child(WorkshopGui.Element(receiver, "polyline")
            .Attribute("points", route)
            .Attribute("fill", "none")
            .Attribute("stroke", stage == Stage.Conference ? Magenta : Yellow)
            .Attribute("stroke-opacity", ".35")
            .Attribute("stroke-width", ".65")
            .Attribute("stroke-dasharray", "1.4 1.1"));

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "50").Attribute("y", "9")
            .Attribute("fill", Yellow).Attribute("font-size", "2.2")
            .Attribute("text-anchor", "middle")
            .Text("AEC REMOTE OFFICE"));

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "50").Attribute("y", "12.5")
            .Attribute("fill", Muted).Attribute("font-size", ".95")
            .Attribute("text-anchor", "middle")
            .Text("LIVE FLOOR PLAN // ARRIVAL → RECEPTION → DESIGN AUTHORITY"));

        return svg;
    }

    private static void Room(ElementBuilder svg, object receiver, double x, double y, double width, double height, string label, string accent)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("width", width).Attribute("height", height)
            .Attribute("fill", $"{accent}05")
            .Attribute("stroke", accent).Attribute("stroke-opacity", ".32").Attribute("stroke-width", ".28"));

        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", x + width / 2).Attribute("y", y + height / 2)
            .Attribute("fill", accent).Attribute("fill-opacity", ".72")
            .Attribute("font-size", ".78").Attribute("text-anchor", "middle")
            .Text(label));
    }

    private static void Furniture(ElementBuilder svg, object receiver, double x, double y, double width, double height, string accent)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y)
            .Attribute("width", width).Attribute("height", height)
            .Attribute("rx", "1")
            .Attribute("fill", $"{accent}0a")
            .Attribute("stroke", accent).Attribute("stroke-opacity", ".45").Attribute("stroke-width", ".22"));
    }

    private static void Door(ElementBuilder svg, object receiver, double x, double y, string id)
    {
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x).Attribute("cy", y).Attribute("r", "1")
            .Attribute("fill", id == "conference" ? Magenta : Yellow)
            .Attribute("fill-opacity", ".28")
            .Attribute("stroke", id == "conference" ? Magenta : Yellow)
            .Attribute("stroke-width", ".3"));
    }

    private static ElementBuilder Avatar(object receiver, string name, Stage stage)
    {
        var (x, y) = stage switch
        {
            Stage.Arrival => (18d, 76d),
            Stage.Reception => (40d, 34d),
            _ => (68d, 34d)
        };

        return WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", $"{x}%").Style("top", $"{y}%")
            .Style("z-index", "20")
            .Style("transform", "translate(-50%,-50%)")
            .Style("transition", "left 1.15s cubic-bezier(.22,.75,.25,1),top 1.15s cubic-bezier(.22,.75,.25,1)")
            .Style("pointer-events", "none")
            .Style("text-align", "center")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", "1.4rem")
                .Style("color", Cyan)
                .Style("text-shadow", $"0 0 16px {Cyan}")
                .Text("◉"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".15rem")
                .Style("padding", ".15rem .28rem")
                .Style("border", $"1px solid {Cyan}55")
                .Style("background", "rgba(1,4,10,.82)")
                .Style("color", White)
                .Style("font-size", ".34rem")
                .Text($"YOU // {name}"));
    }

    private static ElementBuilder StageOverlay(
        object receiver,
        string avatarName,
        Stage stage,
        Action speakToReceptionist,
        Action beginConsultation)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "2%").Style("top", "17%")
            .Style("z-index", "30")
            .Style("width", "min(28rem,30vw)")
            .Style("padding", ".85rem")
            .Style("border", $"1px solid {(stage == Stage.Conference ? Magenta : Cyan)}66")
            .Style("background", "rgba(1,6,12,.92)")
            .Style("box-shadow", $"0 0 34px {(stage == Stage.Conference ? Magenta : Cyan)}12");

        var title = stage switch
        {
            Stage.Arrival => "ARRIVAL // FLOOR PLAN ACTIVE",
            Stage.Reception => "RECEPTION // YOU HAVE ARRIVED",
            _ => "CONFERENCE ROOM // PRESENTATION READY"
        };

        var body = stage switch
        {
            Stage.Arrival => "The office is not a menu. It is a place. Your avatar is walking through the actual schematic toward the receptionist.",
            Stage.Reception => "Welcome. I am the receptionist for the AEC Remote Office. The architecture, structural and systems authorities are standing by.",
            _ => "The design authorities have assembled. We will establish the building intent first, then expose the structure and program so you can change the building deliberately."
        };

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", stage == Stage.Conference ? Magenta : Yellow)
            .Style("font-size", ".43rem").Style("letter-spacing", ".16em")
            .Text(title));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".55rem")
            .Style("font-family", "system-ui,sans-serif")
            .Style("font-size", ".78rem")
            .Style("line-height", "1.45")
            .Style("color", White)
            .Text(body));

        if (stage == Stage.Reception)
        {
            panel.Content(WorkshopGui.Button(receiver)
                .Label("SPEAK WITH RECEPTIONIST →")
                .Style("margin-top", ".7rem").Style("width", "100%")
                .Style("padding", ".55rem")
                .Style("border", $"1px solid {Yellow}88")
                .Style("background", $"{Yellow}0a")
                .Style("color", Yellow)
                .Style("font-family", "inherit")
                .Style("cursor", "pointer")
                .OnClick(speakToReceptionist));
        }
        else if (stage == Stage.Conference)
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".65rem")
                .Style("padding", ".55rem")
                .Style("border-left", $"2px solid {Magenta}99")
                .Style("color", "#c8d7dc")
                .Style("font-family", "system-ui,sans-serif")
                .Style("font-size", ".68rem")
                .Text("PRESENTATION: intent → ontology → default structural basis → plan → authority-driven changes."));

            panel.Content(WorkshopGui.Button(receiver)
                .Label("BEGIN AEC DESIGN CONSULTATION →")
                .Style("margin-top", ".7rem").Style("width", "100%")
                .Style("padding", ".58rem")
                .Style("border", $"1px solid {Magenta}99")
                .Style("background", $"{Magenta}0a")
                .Style("color", Magenta)
                .Style("font-family", "inherit")
                .Style("cursor", "pointer")
                .OnClick(beginConsultation));
        }

        return panel;
    }

    private static ElementBuilder Exit(object receiver, Action exitLobby)
        => WorkshopGui.Button(receiver).Label("← RETURN TO CITY")
            .Style("position", "absolute").Style("right", "2%").Style("bottom", "2%")
            .Style("z-index", "40")
            .Style("padding", ".5rem .7rem")
            .Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.82)")
            .Style("color", Magenta)
            .Style("font-family", "inherit")
            .Style("cursor", "pointer")
            .OnClick(exitLobby);

    private static ElementBuilder Styles(object receiver)
        => WorkshopGui.Element(receiver, "style").Text(
            "@keyframes aec-breathe{0%,100%{opacity:.58;filter:brightness(1)}50%{opacity:1;filter:brightness(1.65)}}" +
            ".aec-plan-marker{animation:aec-breathe 1.8s ease-in-out infinite}" +
            "@media(prefers-reduced-motion:reduce){.aec-plan-marker{animation:none;opacity:.9}}");
}
