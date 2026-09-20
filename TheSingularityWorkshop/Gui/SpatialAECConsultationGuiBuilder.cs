namespace TheSingularityWorkshop.Gui;

 /// <summary>Conference-room interrogation that captures intent before any drafting begins.</summary>
public static class SpatialAECConsultationGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string White = "#ffffff";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver,
        SpatialAECIntentModel intent,
        string avatarName,
        Action<string> answer,
        Action generate,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Room(receiver));
        root.Content(Team(receiver, intent, avatarName, answer, generate));
        root.Content(Exit(receiver, exit));
        return root;
    }

    private static ElementBuilder Room(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 100")
            .Attribute("preserveAspectRatio", "none")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh")
            .Style("pointer-events", "none");

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "8").Attribute("y", "10").Attribute("width", "84").Attribute("height", "78")
            .Attribute("fill", "rgba(0,234,255,.015)").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".35").Attribute("stroke-width", ".3"));
        svg.Child(WorkshopGui.Element(receiver, "ellipse")
            .Attribute("cx", "50").Attribute("cy", "53").Attribute("rx", "24").Attribute("ry", "13")
            .Attribute("fill", "rgba(0,234,255,.025)").Attribute("stroke", Cyan).Attribute("stroke-width", "1"));
        svg.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", "50").Attribute("y", "53.7").Attribute("fill", "#7895a1").Attribute("font-size", "1.3").Attribute("text-anchor", "middle")
            .Text("INTENT TABLE"));
        return svg;
    }

    private static ElementBuilder Team(object receiver, SpatialAECIntentModel intent, string avatarName, Action<string> answer, Action generate)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "4%").Style("top", "7%")
            .Style("width", "min(38rem,92vw)")
            .Style("padding", "1.35rem 1rem 1rem")
            .Style("box-sizing", "border-box")
            .Style("border", $"1px solid {Cyan}66")
            .Style("background", "rgba(1,6,12,.92)");

        // Keep the title inside the frame's safe area. The consultation panel sits
        // over the room's upper cyan geometry, so its own top inset must be explicit.
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("color", Cyan)
            .Style("font-size", ".48rem")
            .Style("line-height", "1.4")
            .Style("letter-spacing", ".18em")
            .Text("AEC CONSULTATION // TEAM LEADS"));

        // Treat the intent statement and layer tracker as separate semantic bands;
        // do not let the tracker visually collapse into the description.
        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".7rem")
            .Style("padding-bottom", ".65rem")
            .Style("border-bottom", "1px solid rgba(0,234,255,.14)")
            .Style("color", White)
            .Style("font-family", "system-ui,sans-serif")
            .Style("font-size", ".72rem")
            .Style("line-height", "1.45")
            .Text("You are not here to operate a CAD tool. You are here to describe the structure you mean."));

        panel.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".7rem")
            .Style("color", Yellow)
            .Style("font-size", ".42rem")
            .Style("line-height", "1.35")
            .Style("letter-spacing", ".15em")
            .Text($"CURRENT LAYER {Math.Min(intent.CurrentLayer + 1, 9)} / 9 // {SpatialAECIntentModel.LayerNames[Math.Min(intent.CurrentLayer, 8)].ToUpperInvariant()}"));

        var leads = new[] { "RECEPTION / MAYA", "ARCHITECTURE / ARI", "STRUCTURAL / SAM", "SYSTEMS / NOOR", "PROGRAM / KAI" };
        var leadRow = WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("gap", ".3rem").Style("flex-wrap", "wrap").Style("margin-top", ".55rem");
        foreach (var lead in leads)
            leadRow.Content(WorkshopGui.Element(receiver, "span").Style("padding", ".25rem .35rem").Style("border", "1px solid rgba(0,234,255,.22)").Style("color", "#9ab0b8").Style("font-size", ".33rem").Text(lead));
        panel.Content(leadRow);
        panel.Content(TableModel(receiver, intent));

        if (!intent.IsComplete)
        {
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", "1rem").Style("font-family", "system-ui,sans-serif").Style("font-size", "1.15rem").Style("color", White)
                .Text(Question(intent)));

            var choices = WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("flex-wrap", "wrap").Style("gap", ".4rem").Style("margin-top", ".7rem");
            foreach (var option in intent.OptionsForCurrentLayer)
                choices.Content(WorkshopGui.Button(receiver).Label(option)
                    .Style("padding", ".5rem .65rem").Style("border", $"1px solid {Yellow}66")
                    .Style("background", "rgba(255,211,77,.04)").Style("color", Yellow)
                    .Style("font-family", "inherit").Style("font-size", ".38rem").Style("cursor", "pointer")
                    .OnClick(() => answer(option)));
            panel.Content(choices);
        }
        else
        {
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".8rem").Style("color", Yellow).Style("font-size", ".5rem").Style("letter-spacing", ".14em").Text("INTENT RESOLVED"));
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("font-family", "system-ui,sans-serif").Style("font-size", "1.25rem").Style("color", White).Text(intent.BuildingType));
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("color", "#7895a1").Style("font-size", ".36rem").Text(intent.Summary));
            panel.Content(WorkshopGui.Button(receiver).Label("GENERATE SPATIAL STRUCTURE →")
                .Style("margin-top", ".8rem").Style("padding", ".7rem .85rem")
                .Style("border", $"1px solid {Cyan}88").Style("background", "rgba(0,234,255,.06)")
                .Style("color", Cyan).Style("font-family", "inherit").Style("cursor", "pointer").OnClick(generate));
        }

        return panel;
    }

    private static ElementBuilder TableModel(object receiver, SpatialAECIntentModel intent)
    {
        var table = WorkshopGui.Panel(receiver)
            .Style("margin-top", ".7rem").Style("padding", ".6rem")
            .Style("border", "1px solid rgba(255,211,77,.28)")
            .Style("background", "linear-gradient(135deg,rgba(255,211,77,.045),rgba(0,234,255,.025))");

        table.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".35rem").Style("letter-spacing", ".14em")
            .Style("color", Yellow)
            .Text("CONFERENCE TABLE // DEFAULT ONTOLOGY MODEL"));

        table.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".4rem").Style("font-family", "system-ui,sans-serif")
            .Style("font-size", ".85rem").Style("color", White)
            .Text(intent.CurrentDefaultArtifact));

        table.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".25rem").Style("font-size", ".33rem")
            .Style("color", "#9ab0b8")
            .Text($"DEFAULT FOR {SpatialAECIntentModel.LayerNames[Math.Min(intent.CurrentLayer, 8)].ToUpperInvariant()} // {intent.CurrentDefault}"));

        var motion = intent.IsComplete ? "MODEL READY // TEAM IS STANDING BY" : "TEAM LEADS // RETRIEVING VIABLE MATCHING MODEL";
        table.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".35rem").Style("font-size", ".3rem")
            .Style("color", Cyan)
            .Text(motion));

        return table;
    }

    private static string Question(SpatialAECIntentModel intent)
        => intent.CurrentLayer switch
        {
            0 => "Is this for reality or fiction?",
            1 => "What world/domain does the structure belong to?",
            2 => "What kind of thing are we making?",
            3 => "What is the functional building family?",
            4 => "What is the concrete facility expression?",
            5 => "How is the structure organized in space?",
            6 => "What discipline or knowledge family defines it?",
            7 => "What is the intended operational purpose?",
            8 => "What is the ultimate building type?",
            _ => "Intent captured."
        };

    private static ElementBuilder Exit(object receiver, Action exit)
        => WorkshopGui.Button(receiver).Label("← RETURN TO LOBBY")
            .Style("position", "absolute").Style("right", "2%").Style("bottom", "2%")
            .Style("padding", ".5rem .7rem").Style("border", $"1px solid {Magenta}66")
            .Style("background", "rgba(1,4,10,.8)").Style("color", Magenta)
            .Style("font-family", "inherit").Style("cursor", "pointer").OnClick(exit);
}
