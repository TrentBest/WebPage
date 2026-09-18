namespace TheSingularityWorkshop.Gui;

/// <summary>Builds the first-visit garden from the same recursive GUI primitives used by Explore.</summary>
public static class SpatialGardenGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Green = "#52e05a";
    private const string Yellow = "#ffd34d";
    private const string White = "#ffffff";
    private const string Magenta = "#ff38d1";

    /// <summary>Creates the garden threshold and exposes its three deliberate exits.</summary>
    public static ElementBuilder Build(
        object receiver,
        SpatialGardenManifest garden,
        string visitorName,
        Action<string> enterDestination,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", "radial-gradient(circle at 50% 42%, #10261f 0%, #06120f 48%, #02070a 100%)")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace")
            .Attribute("id", "spatial-welcome-garden");

        root.Content(Styles(receiver));
        root.Content(Ground(receiver));
        root.Content(Title(receiver, garden, visitorName));
        root.Content(Gazebo(receiver, garden.Gazebo, () => enterDestination("hangout")));
        root.Content(SettingsShed(receiver, garden.SettingsShed, () => enterDestination("settings")));
        root.Content(Gate(receiver, GetGate(garden, "hangout"), "left", () => enterDestination("hangout")));
        root.Content(Gate(receiver, GetGate(garden, "exit"), "center", exit));
        root.Content(Gate(receiver, GetGate(garden, "singularity-station"), "right", () => enterDestination("singularity-station")));
        root.Content(LearningMarkers(receiver));

        return root;
    }

    private static SpatialGardenGate GetGate(SpatialGardenManifest garden, string id)
        => garden.FindGate(id) ?? throw new InvalidOperationException($"Garden gate '{id}' is required by the default presentation.");

    private static ElementBuilder Ground(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("background-image", "radial-gradient(circle, rgba(82,224,90,.16) 1px, transparent 1px)")
            .Style("background-size", "28px 28px")
            .Style("opacity", ".7")
            .Style("pointer-events", "none");

    private static ElementBuilder Title(object receiver, SpatialGardenManifest garden, string visitorName)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "50%").Style("top", "7%")
            .Style("transform", "translateX(-50%)")
            .Style("width", "min(760px,84vw)")
            .Style("text-align", "center").Style("z-index", "20")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Green).Style("font-size", ".55rem").Style("letter-spacing", ".35em")
                .Text("FIRST VISIT // ORIENTATION"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".65rem").Style("color", Yellow)
                .Style("font-size", "clamp(1.5rem,4vw,3rem)").Style("letter-spacing", ".16em")
                .Style("text-shadow", "0 0 18px rgba(255,211,77,.45)")
                .Text(garden.Name))
            .Content(WorkshopGui.Element(receiver, "div")
                 .Style("margin", "1rem auto 0").Style("max-width", "620px")
                .Style("color", "#d7e8dd").Style("font-family", "system-ui,sans-serif")
                .Style("font-size", ".9rem").Style("line-height", "1.6")
                .Text($"Welcome, {visitorName}. {garden.Purpose}"));

    private static ElementBuilder Gazebo(object receiver, SpatialGardenLandmark landmark, Action enter)
        => Landmark(receiver, landmark, "left:8%;top:37%;width:18%;height:18%;", Magenta, "gazebo-roof",
            "A social shelter with a view of the garden. Future shared Experiences can gather here.", enter);

    private static ElementBuilder SettingsShed(object receiver, SpatialGardenLandmark landmark, Action enter)
        => Landmark(receiver, landmark, "right:8%;top:37%;width:18%;height:18%;", Cyan, "settings-shed",
            "A small practical building. Personal settings belong in a place, not in the middle of the world.", enter);

    private static ElementBuilder Landmark(
        object receiver,
        SpatialGardenLandmark landmark,
        string position,
        string accent,
        string cssId,
        string description,
        Action enter)
    {
        var shell = WorkshopGui.Element(receiver, "div")
             .Style("position", "absolute").Style("inset", position)
            .Style("z-index", "12").Style("cursor", "pointer")
            .Style("display", "flex").Style("flex-direction", "column")
            .Style("align-items", "center").Style("justify-content", "center")
            .Style("border", $"1px solid {accent}77")
            .Style("background", "rgba(1,8,8,.76)")
            .Style("box-shadow", $"0 0 32px {accent}18,inset 0 0 24px {accent}08")
            .Attribute("id", cssId)
            .AriaLabel(landmark.Name)
            .Title($"{landmark.Name} // {description}")
            .OnClick(enter);

        shell.Content(WorkshopGui.Element(receiver, "div")
            .Style("width", "70%").Style("height", "34%")
            .Style("border", $"1px solid {accent}99")
            .Style("border-radius", "50% 50% 10% 10%")
            .Style("background", $"{accent}12")
            .Style("box-shadow", $"0 0 22px {accent}22")
            .Text(""));
        shell.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".7rem").Style("color", accent)
            .Style("font-size", ".58rem").Style("letter-spacing", ".14em")
            .Text(landmark.Name));
        shell.Content(WorkshopGui.Element(receiver, "div")
            .Style("margin-top", ".35rem").Style("max-width", "90%")
            .Style("color", "#b9cbc5").Style("font-family", "system-ui,sans-serif")
            .Style("font-size", ".55rem").Style("line-height", "1.35")
            .Style("text-align", "center").Text(landmark.Purpose));

        return shell;
    }

    private static ElementBuilder Gate(object receiver, SpatialGardenGate gate, string side, Action enter)
    {
        var horizontal = side == "left" ? "9%" : side == "right" ? "79%" : "44%";
        var gateColor = gate.Id == "exit" ? Yellow : Cyan;

        var shell = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", horizontal).Style("bottom", "10%")
            .Style("width", "18%").Style("height", "25%")
            .Style("z-index", "15").Style("cursor", "pointer")
            .Style("display", "flex").Style("flex-direction", "column")
            .Style("align-items", "center").Style("justify-content", "flex-end")
            .OnClick(enter)
            .AriaLabel(gate.Name)
            .Title(gate.Purpose);

        shell.Content(WorkshopGui.Element(receiver, "div")
            .Style("width", "70%").Style("height", "80%")
            .Style("box-sizing", "border-box")
            .Style("border", $"2px solid {gateColor}aa")
            .Style("border-bottom", "0")
            .Style("background", "linear-gradient(180deg, transparent, rgba(0,234,255,.04))")
            .Style("box-shadow", $"0 0 28px {gateColor}18")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("bottom", "7%")
                .Style("left", "50%").Style("transform", "translateX(-50%)")
                .Style("padding", ".32rem .55rem")
                .Style("border", $"1px solid {gateColor}88")
                .Style("background", "rgba(1,5,8,.92)")
                .Style("color", gateColor).Style("font-size", ".52rem")
                .Style("letter-spacing", ".1em").Style("white-space", "nowrap")
                .Text(gate.Name)));
        return shell;
    }

    private static ElementBuilder LearningMarkers(object receiver)
    {
        var marker = (string title, string text, string left, string top) =>
            WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", left).Style("top", top)
                .Style("z-index", "10").Style("max-width", "190px")
                .Style("padding", ".5rem .65rem")
                .Style("border-left", $"2px solid {Green}")
                .Style("background", "rgba(1,6,6,.72)")
                .Style("color", "#c9ddd1")
                .Style("font-family", "system-ui,sans-serif")
                .Style("font-size", ".57rem").Style("line-height", "1.4")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", Green).Style("font-family", "inherit").Style("font-size", ".5rem").Style("letter-spacing", ".12em").Text(title))
                .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Text(text));

        return WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("pointer-events", "none")
            .Content(marker("MOVE", "Use the same movement controls you will use throughout the spatial world.", "5%", "68%"))
            .Content(marker("INTERACT", "Approach a place and choose it. The world contains Experiences, not just buttons.", "38%", "60%"))
            .Content(marker("EXPLORE", "Nothing here requires a perfect path. You are allowed to wander.", "66%", "68%"));
    }

    private static ElementBuilder Styles(object receiver)
        => WorkshopGui.Element(receiver, "style").Text(
            "@keyframes garden-breathe{0%,100%{filter:brightness(1);transform:scale(1)}50%{filter:brightness(1.16);transform:scale(1.018)}}#spatial-welcome-garden [aria-label]:hover{animation:garden-breathe 1.8s ease-in-out infinite}");
}
