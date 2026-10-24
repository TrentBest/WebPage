namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Presents the authored Castle Siege destination while the Physics-owned projectile
/// experiment is pending. This builder contains no trajectory, collision, or damage logic.
/// </summary>
public static class SpatialCastleSiegeGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Gold = "#ffd34d";
    private const string White = "#f1f7fb";
    private const string Muted = "#93a9b4";
    private const string Stone = "#263744";

    private static readonly (string Id, string Label)[] TargetSections =
    [
        ("left", "LEFT ZONE"),
        ("center", "CENTER ZONE"),
        ("right", "RIGHT ZONE")
    ];

    /// <summary>Builds the commander-view scene and its presentation-only controls.</summary>
    public static ElementBuilder Build(
        object receiver,
        string commanderName,
        string selectedTarget,
        bool labelsVisible,
        Action<string> selectTarget,
        Action toggleLabels,
        Action exitScene)
    {
        ArgumentNullException.ThrowIfNull(receiver);
        ArgumentNullException.ThrowIfNull(selectTarget);
        ArgumentNullException.ThrowIfNull(toggleLabels);
        ArgumentNullException.ThrowIfNull(exitScene);

        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed")
            .Style("inset", "0")
            .Style("display", "grid")
            .Style("grid-template-rows", "auto minmax(0, 1fr) auto auto")
            .Style("gap", ".65rem")
            .Style("padding", "clamp(.6rem, 1.5vw, 1.2rem)")
            .Style("overflow", "hidden")
            .Style("box-sizing", "border-box")
            .Style("background", "radial-gradient(ellipse at 52% 28%, #14303d 0%, #06111d 58%, #02060c 100%)")
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "style").Text("""
            .castle-siege-button:focus-visible{outline:2px solid #f1f7fb;outline-offset:3px}
            .castle-siege-button:hover{filter:brightness(1.2)}
            @media(max-width:680px){
              .castle-siege-header{align-items:flex-start;flex-direction:column}
              .castle-siege-stage{min-height:220px}
              .castle-siege-controls{justify-content:flex-start}
              .castle-siege-status{font-size:.68rem}
            }
            """));

        var header = WorkshopGui.Panel(receiver)
            .Style("display", "flex")
            .Style("align-items", "center")
            .Style("justify-content", "space-between")
            .Style("gap", ".75rem")
            .Style("flex-wrap", "wrap")
            .Style("padding", ".3rem .2rem");

        header.Content(WorkshopGui.Panel(receiver)
            .Style("display", "grid")
            .Style("gap", ".25rem")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Cyan)
                .Style("font-size", ".65rem")
                .Style("font-weight", "800")
                .Style("letter-spacing", ".18em")
                .Text("SINGULARITY CITY // CASTLE SIEGE"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", White)
                .Style("font-size", "clamp(1.1rem, 2.2vw, 1.65rem)")
                .Style("font-weight", "800")
                .Text("COMMANDER'S SHOULDER VIEW"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", Muted)
                .Style("font-family", "system-ui, sans-serif")
                .Style("font-size", ".78rem")
                .Text($"Visitor: {commanderName} · authored scene scaffold")));

        header.Content(WorkshopGui.Button(receiver)
            .Label("← RETURN TO CITY")
            .Attribute("id", "castle-siege-exit")
            .Style("padding", ".65rem .85rem")
            .Style("border", $"1px solid {Cyan}77")
            .Style("background", "rgba(0,234,255,.06)")
            .Style("color", Cyan)
            .Style("font-family", "inherit")
            .Style("font-size", ".68rem")
            .Style("font-weight", "800")
            .Style("letter-spacing", ".08em")
            .Style("cursor", "pointer")
            .OnClick(exitScene));

        root.Content(header);
        root.Content(Scene(receiver, selectedTarget, labelsVisible));

        var controls = WorkshopGui.Panel(receiver)
            .Attribute("aria-label", "Castle Siege presentation controls")
            .Style("display", "flex")
            .Style("align-items", "center")
            .Style("justify-content", "center")
            .Style("flex-wrap", "wrap")
            .Style("gap", ".5rem")
            .Style("padding", ".35rem .2rem");

        controls.Content(WorkshopGui.Element(receiver, "span")
            .Style("color", Muted)
            .Style("font-size", ".62rem")
            .Style("letter-spacing", ".12em")
            .Style("margin-right", ".25rem")
            .Text("HIGHLIGHT ILLUSTRATIVE ZONE"));

        foreach (var target in TargetSections)
        {
            var selected = string.Equals(selectedTarget, target.Id, StringComparison.OrdinalIgnoreCase);
            controls.Content(WorkshopGui.Button(receiver)
                .Label(target.Label)
                .Attribute("aria-pressed", selected ? "true" : "false")
                .AriaLabel($"Select {target.Label.ToLowerInvariant()} as the authored target region")
                .Style("padding", ".55rem .7rem")
                .Style("border", $"1px solid {(selected ? Gold : Cyan)}88")
                .Style("background", selected ? "rgba(255,211,77,.13)" : "rgba(0,234,255,.04)")
                .Style("color", selected ? Gold : White)
                .Style("font-family", "inherit")
                .Style("font-size", ".62rem")
                .Style("font-weight", "800")
                .Style("letter-spacing", ".06em")
                .Style("cursor", "pointer")
                .Style("touch-action", "manipulation")
                .Attribute("class", "castle-siege-button")
                .OnClick(() => selectTarget(target.Id)));
        }

        controls.Content(WorkshopGui.Button(receiver)
            .Label(labelsVisible ? "HIDE LABELS" : "SHOW LABELS")
            .Attribute("aria-pressed", labelsVisible ? "true" : "false")
            .Style("padding", ".55rem .7rem")
            .Style("border", "1px solid #93a9b466")
            .Style("background", "rgba(147,169,180,.06)")
            .Style("color", Muted)
            .Style("font-family", "inherit")
            .Style("font-size", ".62rem")
            .Style("font-weight", "800")
            .Style("letter-spacing", ".06em")
            .Style("cursor", "pointer")
            .Style("touch-action", "manipulation")
            .Attribute("class", "castle-siege-button")
            .OnClick(toggleLabels));

        root.Content(controls);

        root.Content(WorkshopGui.Panel(receiver)
            .Attribute("role", "status")
            .Attribute("aria-live", "polite")
            .Style("display", "grid")
            .Style("grid-template-columns", "auto minmax(0, 1fr)")
            .Style("align-items", "start")
            .Style("gap", ".6rem")
            .Style("padding", ".7rem .8rem")
            .Style("border", "1px solid rgba(255,211,77,.35)")
            .Style("background", "rgba(12,14,18,.88)")
            .Style("font-family", "system-ui, sans-serif")
            .Content(WorkshopGui.Element(receiver, "strong")
                .Style("color", Gold)
                .Style("font-family", "Consolas, 'Courier New', monospace")
                .Style("font-size", ".7rem")
                .Text("PHYSICS NOT CONNECTED"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("color", "#c5d1d7")
                .Style("font-size", ".76rem")
                .Style("line-height", "1.45")
                .Text("Authored scene only. No trajectory, hit/miss, impact, or damage result is computed. The highlighted zone is presentation-only, not Experience-owned target geometry. Physics-backed trajectory, hit/miss, impact, and damage remain gated on the validated contract tracked by issue #23.")));

        return root;
    }

    private static ElementBuilder Scene(object receiver, string selectedTarget, bool labelsVisible)
    {
        var scene = WorkshopGui.Panel(receiver)
            .Style("position", "relative")
            .Style("min-height", "0")
            .Style("overflow", "hidden")
            .Style("display", "grid")
            .Style("place-items", "center")
            .Style("border", "1px solid rgba(0,234,255,.28)")
            .Style("background", "linear-gradient(180deg,rgba(5,15,25,.45),rgba(1,5,10,.94))")
            .Style("box-shadow", "inset 0 0 48px rgba(0,234,255,.05)");

        scene.Content(SceneSvg(receiver, selectedTarget, labelsVisible));
        scene.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute")
            .Style("left", ".65rem")
            .Style("bottom", ".55rem")
            .Style("padding", ".25rem .4rem")
            .Style("border", "1px solid rgba(255,211,77,.3)")
            .Style("background", "rgba(1,4,10,.82)")
            .Style("color", Gold)
            .Style("font-size", ".56rem")
            .Style("letter-spacing", ".12em")
            .Style("pointer-events", "none")
            .Text("ILLUSTRATIVE FIXTURE — NOT PHYSICS-BACKED"));

        return scene;
    }

    private static ElementBuilder SceneSvg(object receiver, string selectedTarget, bool labelsVisible)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 1200 600")
            .Attribute("role", "img")
            .Attribute("aria-label", "Illustrative commander-view fixture with a siege engine and castle silhouette. The highlighted zone is not canonical Experience geometry; no projectile trajectory or impact is simulated.")
            .Style("display", "block")
            .Style("width", "100%")
            .Style("height", "100%")
            .Style("max-height", "58vh")
            .Style("min-height", "220px")
            .Style("pointer-events", "none");

        svg.Child(SvgRect(receiver, "0", "0", "1200", "600", "#081725"));
        svg.Child(SvgRect(receiver, "0", "265", "1200", "115", "#102b37"));
        svg.Child(SvgPath(receiver, "M0 325 L110 250 L215 310 L340 205 L470 318 L600 230 L720 315 L865 215 L1005 315 L1110 245 L1200 285 L1200 450 L0 450 Z", "#0a222d", "none", "0"));
        svg.Child(SvgPath(receiver, "M0 380 Q250 345 460 375 T850 370 T1200 355 L1200 600 L0 600 Z", "#152a2d", "none", "0"));

        // Castle massing is authored presentation geometry, not collision geometry.
        svg.Child(SvgRect(receiver, "555", "190", "70", "190", "#263744", "#8296a1", "3"));
        svg.Child(SvgRect(receiver, "925", "190", "70", "190", "#263744", "#8296a1", "3"));
        svg.Child(SvgRect(receiver, "590", "235", "370", "145", "#263744", "#8296a1", "3"));

        var sections = new[]
        {
            (Id: "left", X: 590, Width: 120),
            (Id: "center", X: 710, Width: 130),
            (Id: "right", X: 840, Width: 120)
        };

        foreach (var section in sections)
        {
            var selected = string.Equals(selectedTarget, section.Id, StringComparison.OrdinalIgnoreCase);
            svg.Child(SvgRect(
                receiver,
                section.X.ToString(System.Globalization.CultureInfo.InvariantCulture),
                "240",
                section.Width.ToString(System.Globalization.CultureInfo.InvariantCulture),
                "135",
                selected ? "#51452e" : "#263744",
                selected ? Gold : "#8296a1",
                selected ? "5" : "2"));
        }

        // Crenellations and tower caps are authored silhouette details.
        for (var x = 595; x < 950; x += 28)
            svg.Child(SvgRect(receiver, x.ToString(System.Globalization.CultureInfo.InvariantCulture), "218", "15", "20", "#344956", "#8296a1", "1.5"));

        svg.Child(SvgRect(receiver, "550", "178", "80", "24", "#344956", "#8296a1", "2"));
        svg.Child(SvgRect(receiver, "920", "178", "80", "24", "#344956", "#8296a1", "2"));
        for (var x = 555; x < 625; x += 20)
            svg.Child(SvgRect(receiver, x.ToString(System.Globalization.CultureInfo.InvariantCulture), "170", "11", "12", "#344956", "#8296a1", "1"));
        for (var x = 925; x < 995; x += 20)
            svg.Child(SvgRect(receiver, x.ToString(System.Globalization.CultureInfo.InvariantCulture), "170", "11", "12", "#344956", "#8296a1", "1"));

        svg.Child(SvgPath(receiver, "M745 375 L745 300 Q775 268 805 300 L805 375 Z", "#08131d", "#a2b1b8", "2"));
        svg.Child(SvgLine(receiver, "775", "240", "775", "375", "#a2b1b8", "2", ".5"));

        var targetX = selectedTarget switch { "left" => 650, "right" => 900, _ => 775 };
        svg.Child(SvgCircle(receiver, targetX.ToString(System.Globalization.CultureInfo.InvariantCulture), "305", "15", "none", Gold, "3"));
        svg.Child(SvgLine(receiver, (targetX - 24).ToString(System.Globalization.CultureInfo.InvariantCulture), "305", (targetX + 24).ToString(System.Globalization.CultureInfo.InvariantCulture), "305", Gold, "2", ".9"));
        svg.Child(SvgLine(receiver, targetX.ToString(System.Globalization.CultureInfo.InvariantCulture), "281", targetX.ToString(System.Globalization.CultureInfo.InvariantCulture), "329", Gold, "2", ".9"));

        // Siege engine silhouette: a prop at rest, with no inferred arm or projectile dynamics.
        svg.Child(SvgLine(receiver, "130", "480", "395", "480", "#8c9ba4", "13"));
        svg.Child(SvgCircle(receiver, "205", "500", "24", "#111c25", "#9baab2", "5"));
        svg.Child(SvgCircle(receiver, "335", "500", "24", "#111c25", "#9baab2", "5"));
        svg.Child(SvgLine(receiver, "230", "470", "275", "390", "#9baab2", "12"));
        svg.Child(SvgLine(receiver, "345", "470", "275", "390", "#9baab2", "12"));
        svg.Child(SvgLine(receiver, "275", "390", "425", "315", "#b6a07a", "15"));
        svg.Child(SvgRect(receiver, "251", "376", "45", "26", "#6c5840", "#b6a07a", "2"));
        svg.Child(SvgCircle(receiver, "275", "390", "14", "#0a1219", "#b6a07a", "5"));
        svg.Child(SvgLine(receiver, "425", "315", "444", "337", "#b6a07a", "3"));
        svg.Child(SvgCircle(receiver, "449", "344", "8", "#d7c49c", "#f0e2c4", "2"));

        // Shoulder silhouette establishes the intended camera composition without implying a live avatar rig.
        svg.Child(SvgCircle(receiver, "1070", "463", "28", "#071018", "#5c707a", "2"));
        svg.Child(SvgPath(receiver, "M1008 600 Q1010 510 1040 500 L1100 500 Q1130 510 1132 600 Z", "#071018", "#5c707a", "2"));

        if (labelsVisible)
        {
            svg.Child(SvgText(receiver, "35", "50", "SINGULARITY CITY // CASTLE SIEGE", Cyan, "18", "800", ".14em"));
            svg.Child(SvgText(receiver, "35", "78", $"ILLUSTRATIVE ZONE: {selectedTarget.ToUpperInvariant()}", Gold, "17", "800", ".1em"));
            svg.Child(SvgText(receiver, "640", "205", "CASTLE WALL / AUTHORED TARGET", White, "15", "700", ".08em"));
            svg.Child(SvgText(receiver, "105", "430", "SIEGE ENGINE PROP", White, "15", "700", ".08em"));
            svg.Child(SvgText(receiver, "1010", "420", "COMMANDER", Muted, "13", "700", ".1em"));
        }

        return svg;
    }

    private static ElementBuilder SvgRect(object receiver, string x, string y, string width, string height, string fill, string stroke = "none", string strokeWidth = "0")
        => WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x).Attribute("y", y).Attribute("width", width).Attribute("height", height)
            .Attribute("fill", fill).Attribute("stroke", stroke).Attribute("stroke-width", strokeWidth);

    private static ElementBuilder SvgPath(object receiver, string data, string fill, string stroke, string strokeWidth)
        => WorkshopGui.Element(receiver, "path")
            .Attribute("d", data).Attribute("fill", fill).Attribute("stroke", stroke).Attribute("stroke-width", strokeWidth);

    private static ElementBuilder SvgLine(object receiver, string x1, string y1, string x2, string y2, string stroke, string strokeWidth, string opacity = "1")
        => WorkshopGui.Element(receiver, "line")
            .Attribute("x1", x1).Attribute("y1", y1).Attribute("x2", x2).Attribute("y2", y2)
            .Attribute("stroke", stroke).Attribute("stroke-width", strokeWidth).Attribute("stroke-opacity", opacity)
            .Attribute("stroke-linecap", "round");

    private static ElementBuilder SvgCircle(object receiver, string cx, string cy, string radius, string fill, string stroke, string strokeWidth)
        => WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", cx).Attribute("cy", cy).Attribute("r", radius)
            .Attribute("fill", fill).Attribute("stroke", stroke).Attribute("stroke-width", strokeWidth);

    private static ElementBuilder SvgText(object receiver, string x, string y, string text, string fill, string fontSize, string fontWeight, string letterSpacing)
        => WorkshopGui.Element(receiver, "text")
            .Attribute("x", x).Attribute("y", y).Attribute("fill", fill)
            .Attribute("font-family", "Consolas, 'Courier New', monospace")
            .Attribute("font-size", fontSize).Attribute("font-weight", fontWeight)
            .Attribute("letter-spacing", letterSpacing).Text(text);
}
