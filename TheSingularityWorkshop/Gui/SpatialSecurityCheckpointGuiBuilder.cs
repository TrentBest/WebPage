namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>
/// Spatial presentation of a reusable airport-style security checkpoint.
/// Interaction targets sit on the represented floor plan: the visitor clicks
/// the physical queue space, tray rollers, X-ray tray, scanner, and collection
/// point rather than operating a detached sequence of menu commands.
/// </summary>
public static class SpatialSecurityCheckpointGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Yellow = "#ffd34d";
    private const string Magenta = "#ff38d1";
    private const string Green = "#52e05a";
    private const string Red = "#ff5a70";
    private const string White = "#ffffff";
    private const string Muted = "#8ea7b2";
    private const string Ink = "#02080f";

    public static ElementBuilder Build(
        object receiver, SpatialSecurityCheckpointModel checkpoint, SpatialLaboratoryTrafficModel traffic,
        string userName, Action enterQueue, Action<int> moveQueue, Action placeTray, Action loadBelongings,
        Action scanner, Action collectTray, Action returnToCampus, Action enterFacility)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(Environment(receiver, checkpoint, userName));
        root.Content(InteractionLayer(receiver, checkpoint, enterQueue, moveQueue, placeTray, loadBelongings, scanner, collectTray, enterFacility));
        root.Content(Frame(receiver, checkpoint, traffic));
        root.Content(WorkshopGui.Button(receiver).Label("RETURN")
            .Style("position", "fixed").Style("right", "2rem").Style("bottom", "2rem")
            .Style("z-index", "90").Style("padding", ".85rem 1.25rem")
            .Style("background", "rgba(2,8,15,.9)").Style("border", $"1px solid {Muted}88")
            .Style("color", White).Style("font-family", "inherit").Style("font-size", "1rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(returnToCampus));
        return root;
    }

    private static ElementBuilder Environment(object receiver, SpatialSecurityCheckpointModel checkpoint, string userName)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 120 72").Style("position", "absolute")
            .Style("inset", "0").Style("width", "100vw").Style("height", "100vh");

        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("width", "120").Attribute("height", "72").Attribute("fill", Ink));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "3").Attribute("y", "4").Attribute("width", "114").Attribute("height", "64")
            .Attribute("fill", "#06131b").Attribute("stroke", "#23404b").Attribute("stroke-width", ".35"));

        // The floor is the interface. No queue cards, tables, or status lists.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 5 25 H 115 V 65 H 5 Z")
            .Attribute("fill", "#071820").Attribute("stroke", "#31515f").Attribute("stroke-width", ".25"));

        for (var x = 7; x <= 113; x += 7)
            svg.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", $"M {x} 25 L {x + 5} 65")
                .Attribute("stroke", "#17313b").Attribute("stroke-width", ".16"));

        for (var y = 31; y <= 61; y += 6)
            svg.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", $"M 5 {y} H 115")
                .Attribute("stroke", "#17313b").Attribute("stroke-width", ".16"));

        // Heavy security threshold.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 6 25 V 12 H 43 V 25")
            .Attribute("fill", "#0b2029").Attribute("stroke", Cyan).Attribute("stroke-width", ".45"));
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", "9").Attribute("y", "15").Attribute("width", "31").Attribute("height", "7")
            .Attribute("fill", Ink).Attribute("stroke", "#31515f").Attribute("stroke-width", ".3"));
        svg.Child(Label(receiver, 11, 19.8, "SECURITY THRESHOLD", Yellow, 1.25));
        svg.Child(Label(receiver, 11, 23.5, "CONTROLLED ACCESS", Muted, .7));

        // Physical approach lane and people.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 15 27 H 44").Attribute("stroke", Cyan).Attribute("stroke-width", "1")
            .Attribute("stroke-dasharray", "3 2").Attribute("opacity", ".65"));
        svg.Child(Label(receiver, 17, 34, "WALK THE PLAN", Cyan, .95));
        DrawPerson(receiver, svg, 20, 42, Yellow, false);
        DrawPerson(receiver, svg, 27, 49, Yellow, false);
        DrawPerson(receiver, svg, 34, 56, Muted, false);
        DrawPerson(receiver, svg, 42, 39, Magenta, true);
        svg.Child(Label(receiver, 38, 46, userName.ToUpperInvariant(), Magenta, .75));

        // Physical conveyor and tray.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 45 39 H 78 L 82 57 H 49 Z")
            .Attribute("fill", "#0d222c").Attribute("stroke", Yellow).Attribute("stroke-width", ".45"));
        for (var x = 51; x <= 76; x += 5)
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x.ToString()).Attribute("y1", "40").Attribute("x2", (x + 3).ToString()).Attribute("y2", "55")
                .Attribute("stroke", "#45616b").Attribute("stroke-width", ".65"));

        var trayX = checkpoint.TrayOnRollers ? "54" : "48";
        var trayY = checkpoint.TrayOnRollers ? "43" : "45";
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", trayX).Attribute("y", trayY).Attribute("width", "12").Attribute("height", "5").Attribute("rx", "1")
            .Attribute("fill", checkpoint.TrayOnRollers ? $"{Yellow}32" : "#10232c")
            .Attribute("stroke", Yellow).Attribute("stroke-width", ".5"));
        if (checkpoint.BelongingsOnTray)
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", "57").Attribute("y", "44").Attribute("width", "6").Attribute("height", "3")
                .Attribute("fill", $"{Cyan}55").Attribute("stroke", Cyan).Attribute("stroke-width", ".3"));
        svg.Child(Label(receiver, 48, 38, "DIVEST", Yellow, 1.05));

        // X-ray machine: a physical volume, not a flowchart box.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 65 34 H 80 V 55 H 65 Z")
            .Attribute("fill", "#091a23").Attribute("stroke", Cyan).Attribute("stroke-width", ".7"));
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 68 37 H 77 V 52 H 68 Z")
            .Attribute("fill", Ink).Attribute("stroke", "#39717e").Attribute("stroke-width", ".35"));
        svg.Child(Label(receiver, 67, 32, "SCREENING CORE", Cyan, .85));

        // Body scanner portal.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 84 57 V 38 Q 84 34 88 34 H 96 Q 100 34 100 38 V 57")
            .Attribute("fill", "none").Attribute("stroke", Magenta).Attribute("stroke-width", "1.2"));
        svg.Child(Label(receiver, 84, 31.8, "WALK THROUGH", Magenta, .9));

        // Collection zone.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 101 40 H 112 V 54 H 101 Z")
            .Attribute("fill", "#0a1d25").Attribute("stroke", Green).Attribute("stroke-width", ".55"));
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 103 43 H 110 M 103 47 H 110 M 103 51 H 110")
            .Attribute("stroke", "#4c7760").Attribute("stroke-width", ".5"));
        svg.Child(Label(receiver, 101, 37.5, "COLLECTION", Green, .9));

        if (checkpoint.HasClearedCheckpoint)
        {
            svg.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", "M 106 25 V 17 H 115 V 25")
                .Attribute("fill", $"{Green}18").Attribute("stroke", Green).Attribute("stroke-width", ".65"));
            svg.Child(Label(receiver, 106.5, 15, "CLEARED", Green, .9));
            svg.Child(Label(receiver, 105, 28.5, "FACILITY ACCESS", Green, .65));
        }

        var instruction = checkpoint.Stage switch
        {
            SpatialSecurityCheckpointStage.Approach => "ENTER THE THRESHOLD",
            SpatialSecurityCheckpointStage.Queue => "MOVE WITH THE LINE",
            SpatialSecurityCheckpointStage.TrayReady => "TAKE THE TRAY",
            SpatialSecurityCheckpointStage.TrayLoaded => "LOAD YOUR BELONGINGS",
            SpatialSecurityCheckpointStage.Scanner => "STEP THROUGH",
            SpatialSecurityCheckpointStage.Collection => "COLLECT YOUR BELONGINGS",
            SpatialSecurityCheckpointStage.Cleared => "THE THRESHOLD IS OPEN",
            _ => "WALK THE PLAN"
        };
        svg.Child(Label(receiver, 47, 63, instruction, White, 1.05));
        return svg;
    }

    private static ElementBuilder InteractionLayer(
        object receiver, SpatialSecurityCheckpointModel checkpoint, Action enterQueue, Action<int> moveQueue,
        Action placeTray, Action loadBelongings, Action scanner, Action collectTray, Action enterFacility)
    {
        var layer = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("pointer-events", "none").Style("z-index", "60");

        switch (checkpoint.Stage)
        {
            case SpatialSecurityCheckpointStage.Approach:
                layer.Content(Hotspot(receiver, "ENTER", "5vw", "16vh", "32vw", "22vh", Cyan, enterQueue));
                break;

            case SpatialSecurityCheckpointStage.Queue:
                // Vacant floor positions remain spatial interactions, never a list.
                foreach (var slot in checkpoint.VacantQueueSlots)
                {
                    var top = 34 + slot * 8;
                    layer.Content(Hotspot(receiver, "MOVE", "13vw", $"{top}vh", "27vw", "7vh", Green, () => moveQueue(slot)));
                }
                break;

            case SpatialSecurityCheckpointStage.TrayReady:
                layer.Content(Hotspot(receiver, "TAKE TRAY", "39vw", "50vh", "18vw", "16vh", Yellow, placeTray));
                break;

            case SpatialSecurityCheckpointStage.TrayLoaded:
                layer.Content(Hotspot(receiver, "LOAD", "45vw", "50vh", "24vw", "16vh", Cyan, loadBelongings));
                break;

            case SpatialSecurityCheckpointStage.Scanner:
                layer.Content(Hotspot(receiver, "STEP THROUGH", "68vw", "43vh", "17vw", "27vh", Magenta, scanner));
                break;

            case SpatialSecurityCheckpointStage.Collection:
                layer.Content(Hotspot(receiver, "COLLECT", "83vw", "47vh", "14vw", "20vh", Green, collectTray));
                break;
        }

        if (checkpoint.HasClearedCheckpoint)
            layer.Content(Hotspot(receiver, "ENTER FACILITY", "86vw", "19vh", "12vw", "15vh", Green, enterFacility));

        return layer;
    }

    private static ElementBuilder Frame(object receiver, SpatialSecurityCheckpointModel checkpoint, SpatialLaboratoryTrafficModel traffic)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var accent = checkpoint.Agitated ? Red : checkpoint.HasClearedCheckpoint ? Green : Cyan;

        // Inset frame + inset title eliminates the old header clipping.
        var frame = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "1.1rem")
            .Style("pointer-events", "none").Style("border", $"1px solid {Cyan}66")
            .Style("box-sizing", "border-box").Style("z-index", "80");

        frame.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "1.4rem").Style("top", "1.25rem")
            .Style("padding", ".35rem .75rem").Style("background", Ink)
            .Style("font-size", "clamp(1rem,1.35vw,1.35rem)").Style("font-weight", "700")
            .Style("letter-spacing", ".13em").Style("color", Yellow)
            .Text("SINGULARITY LABORATORY // SECURITY"));

        frame.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("right", "1.4rem").Style("top", "1.3rem")
            .Style("padding", ".35rem .65rem").Style("background", Ink)
            .Style("font-size", "clamp(.8rem,1vw,1rem)").Style("letter-spacing", ".1em")
            .Style("color", accent).Text(checkpoint.HasClearedCheckpoint ? "THRESHOLD OPEN" : "ACTIVE"));

        frame.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "1.4rem").Style("bottom", "1.25rem")
            .Style("padding", ".3rem .55rem").Style("background", Ink)
            .Style("font-size", "clamp(.72rem,.9vw,.9rem)").Style("letter-spacing", ".08em")
            .Style("color", Muted)
            .Text(checkpoint.HasClearedCheckpoint ? "THE MAP IS THE SPATIAL TRUTH" : $"GUARDS // {traffic.OnDutyGuards(now).Count} ON DUTY"));

        if (!checkpoint.HasClearedCheckpoint)
            frame.Content(WorkshopGui.Element(receiver, "div")
                .Style("position", "absolute").Style("left", "50%").Style("transform", "translateX(-50%)")
                .Style("bottom", "1.25rem").Style("max-width", "45vw")
                .Style("padding", ".3rem .65rem").Style("background", Ink)
                .Style("font-family", "system-ui,sans-serif")
                .Style("font-size", "clamp(.8rem,1vw,1rem)").Style("text-align", "center")
                .Style("color", White).Text(checkpoint.LastMessage));

        return frame;
    }

    private static ElementBuilder Hotspot(object receiver, string label, string left, string top, string width, string height, string accent, Action action)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("pointer-events", "auto").Style("position", "absolute")
            .Style("left", left).Style("top", top).Style("width", width).Style("height", height)
            .Style("box-sizing", "border-box").Style("background", $"{accent}08")
            .Style("border", $"1px solid {accent}35").Style("color", $"{accent}aa")
            .Style("font-family", "inherit").Style("font-size", "clamp(.85rem,1vw,1rem)")
            .Style("font-weight", "700").Style("letter-spacing", ".1em").Style("cursor", "pointer")
            .OnClick(action);

    private static void DrawPerson(object receiver, ElementBuilder svg, double x, double y, string color, bool highlighted)
    {
        var scale = highlighted ? 1.35 : 1;
        svg.Child(WorkshopGui.Element(receiver, "circle")
            .Attribute("cx", x.ToString("0.##")).Attribute("cy", (y - 4 * scale).ToString("0.##"))
            .Attribute("r", (1.2 * scale).ToString("0.##")).Attribute("fill", color));
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d",
                $"M {x - 1.6 * scale} {y + 1.5 * scale} " +
                $"Q {x} {y - 1.5 * scale} {x + 1.6 * scale} {y + 1.5 * scale} " +
                $"L {x + 2 * scale} {y + 5 * scale} H {x - 2 * scale} Z")
            .Attribute("fill", highlighted ? $"{color}55" : $"{color}22")
            .Attribute("stroke", color).Attribute("stroke-width", highlighted ? ".45" : ".3"));
    }

    private static ElementBuilder Label(object receiver, double x, double y, string text, string color, double size)
        => WorkshopGui.Element(receiver, "text")
            .Attribute("x", x.ToString("0.##")).Attribute("y", y.ToString("0.##"))
            .Attribute("fill", color).Attribute("font-size", size.ToString("0.##"))
            .Attribute("font-family", "Consolas,'Courier New',monospace")
            .Attribute("font-weight", size >= 1 ? "700" : "400")
            .Attribute("letter-spacing", size >= 1 ? ".06em" : ".03em").Text(text);
}
