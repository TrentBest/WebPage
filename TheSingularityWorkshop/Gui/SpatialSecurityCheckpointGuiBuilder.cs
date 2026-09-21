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
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        SpatialLaboratoryTrafficModel traffic,
        string userName,
        Action enterQueue,
        Action<int> moveQueue,
        Action placeTray,
        Action loadBelongings,
        Action scanner,
        Action collectTray,
        Action returnToCampus,
        Action enterFacility)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(FloorPlan(receiver, checkpoint, userName));
        root.Content(InteractionLayer(receiver, checkpoint, enterQueue, moveQueue, placeTray, loadBelongings, scanner, collectTray, enterFacility));
        root.Content(Header(receiver, checkpoint, traffic));
        root.Content(Status(receiver, checkpoint));
        root.Content(WorkshopGui.Button(receiver).Label("← RETURN TO CAMPUS")
            .Style("position", "fixed").Style("right", "1rem").Style("bottom", "1rem")
            .Style("z-index", "75").Style("padding", ".75rem 1rem")
            .Style("background", "rgba(1,6,12,.94)").Style("border", $"1px solid {Muted}66")
            .Style("color", Muted).Style("font-family", "inherit").Style("cursor", "pointer")
            .OnClick(returnToCampus));

        return root;
    }

    private static ElementBuilder FloorPlan(object receiver, SpatialSecurityCheckpointModel checkpoint, string userName)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 120 72")
            .Style("position", "absolute").Style("inset", "0")
            .Style("width", "100vw").Style("height", "100vh");

        svg.Child(WorkshopGui.Element(receiver, "rect").Attribute("width", "120").Attribute("height", "72").Attribute("fill", "#031019"));
        svg.Child(WorkshopGui.Element(receiver, "path").Attribute("d", "M 4 6 H 116 V 66 H 4 Z")
            .Attribute("fill", "#061923").Attribute("stroke", Cyan).Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "path").Attribute("d", "M 7 17 H 46 V 60 H 7 Z")
            .Attribute("fill", "#081b24").Attribute("stroke", "#31515f").Attribute("stroke-width", ".3"));

        svg.Child(Label(receiver, 8, 10, "SINGULARITY LABORATORY", Yellow, 2.1));
        svg.Child(Label(receiver, 8, 14, "BUILDING ENTRY → SECURITY → CLEARED FACILITY", Muted, .85));
        svg.Child(Label(receiver, 10, 21, "QUEUE", Cyan, 1.1));

        for (var slot = 0; slot < SpatialSecurityCheckpointModel.QueueCapacity; slot++)
        {
            var y = 26 + slot * 6;
            var agent = checkpoint.Agents.FirstOrDefault(item => item.QueueSlot == slot);
            var player = checkpoint.PlayerQueueSlot == slot;
            var occupied = agent != default;
            var accent = player ? Magenta : occupied ? Yellow : Green;

            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", "12").Attribute("y", y.ToString("0.##"))
                .Attribute("width", "28").Attribute("height", "4").Attribute("rx", "1")
                .Attribute("fill", $"{accent}12").Attribute("stroke", accent).Attribute("stroke-width", ".35"));

            var name = player ? "YOU" : occupied ? agent.Name : "VACATED";
            svg.Child(Label(receiver, 26, y + 2.6, name, accent, .75));
        }

        Station(receiver, svg, 49, 22, 13, 15, "TRAY", Yellow,
            checkpoint.Stage == SpatialSecurityCheckpointStage.TrayReady, checkpoint.TrayOnRollers);
        Station(receiver, svg, 64, 22, 18, 15, "X-RAY", Cyan,
            checkpoint.TrayOnRollers, checkpoint.BelongingsOnTray);
        Station(receiver, svg, 84, 22, 13, 15, "SCANNER", Magenta,
            checkpoint.Stage == SpatialSecurityCheckpointStage.Scanner, checkpoint.ScannerCleared);
        Station(receiver, svg, 99, 22, 12, 15, "COLLECT", Green,
            checkpoint.Stage == SpatialSecurityCheckpointStage.Collection, checkpoint.HasClearedCheckpoint);

        svg.Child(WorkshopGui.Element(receiver, "path").Attribute("d", "M 46 29 H 113")
            .Attribute("stroke", "#6d8b95").Attribute("stroke-width", ".7").Attribute("stroke-dasharray", "1.4 1.4"));

        svg.Child(Label(receiver, 50, 42, "← CLICK THE PHYSICAL STATION TO PROGRESS →", Muted, .8));
        svg.Child(Label(receiver, 49, 51, checkpoint.TrayOnRollers ? "TRAY: ON ROLLERS" : "TRAY: WAITING", Yellow, .7));
        svg.Child(Label(receiver, 64, 53, checkpoint.BelongingsOnTray ? "BELONGINGS: LOADED" : "BELONGINGS: WITH YOU", Cyan, .7));
        svg.Child(Label(receiver, 84, 55, checkpoint.ScannerCleared ? "AGENT: CLEARED" : "AGENT: WATCHING", Magenta, .7));
        svg.Child(Label(receiver, 99, 57, checkpoint.HasClearedCheckpoint ? "BADGE BOUNDARY ACTIVE" : "WAITING", Green, .7));
        svg.Child(Label(receiver, 9, 64, $"VISITOR // {userName.ToUpperInvariant()} // BADGE {checkpoint.BadgeId} // {checkpoint.BadgeClearance.ToString().ToUpperInvariant()}", White, .7));

        return svg;
    }

    private static ElementBuilder InteractionLayer(
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        Action enterQueue,
        Action<int> moveQueue,
        Action placeTray,
        Action loadBelongings,
        Action scanner,
        Action collectTray,
        Action enterFacility)
    {
        var layer = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0")
            .Style("pointer-events", "none").Style("z-index", "60");

        if (checkpoint.Stage == SpatialSecurityCheckpointStage.Approach)
            layer.Content(Hotspot(receiver, "ENTER SECURITY", "8vw", "28vh", "27vw", "7vh", Cyan, enterQueue));

        if (checkpoint.Stage == SpatialSecurityCheckpointStage.Queue)
        {
            for (var slot = 0; slot < SpatialSecurityCheckpointModel.QueueCapacity; slot++)
            {
                var y = 26 + slot * 6;
                var isVacant = checkpoint.VacantQueueSlots.Contains(slot);
                layer.Content(Hotspot(receiver,
                    isVacant ? $"MOVE HERE // SPACE {slot + 1}" : "OCCUPIED",
                    "10vw", $"{((y - 1) / 72d) * 100:0.##}vh",
                    "32vw", "6vh", isVacant ? Green : Muted,
                    () => moveQueue(slot), isVacant));
            }
        }

        if (checkpoint.Stage == SpatialSecurityCheckpointStage.TrayReady)
            layer.Content(Hotspot(receiver, "CLICK TRAY // PUT ON ROLLERS", "40vw", "31vh", "16vw", "15vh", Yellow, placeTray));

        if (checkpoint.Stage == SpatialSecurityCheckpointStage.TrayLoaded)
            layer.Content(Hotspot(receiver, "CLICK TRAY // LOAD YOUR STUFF", "54vw", "31vh", "22vw", "15vh", Cyan, loadBelongings));

        if (checkpoint.Stage == SpatialSecurityCheckpointStage.Scanner)
            layer.Content(Hotspot(receiver, "WALK THROUGH // WAIT FOR AGENT", "69vw", "31vh", "16vw", "15vh", Magenta, scanner));

        if (checkpoint.Stage == SpatialSecurityCheckpointStage.Collection)
            layer.Content(Hotspot(receiver, "CLICK TRAY // COLLECT YOUR STUFF", "82vw", "31vh", "16vw", "15vh", Green, collectTray));

        if (checkpoint.HasClearedCheckpoint)
            layer.Content(Hotspot(receiver, "FREE ROAM // ENTER FACILITY", "77vw", "66vh", "19vw", "8vh", Green, enterFacility));

        return layer;
    }

    private static ElementBuilder Header(object receiver, SpatialSecurityCheckpointModel checkpoint, SpatialLaboratoryTrafficModel traffic)
    {
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var header = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("left", "0").Style("right", "0").Style("top", "0")
            .Style("width", "100vw").Style("box-sizing", "border-box")
            .Style("padding", ".42rem .8rem")
            .Style("background", "rgba(1,6,12,.96)").Style("border-bottom", $"1px solid {Cyan}55")
            .Style("z-index", "100").Style("display", "flex").Style("align-items", "center").Style("gap", "1rem");

        header.Content(WorkshopGui.Element(receiver, "div").Style("flex", "1 1 auto").Style("min-width", "0")
            .Style("font-size", "1rem").Style("letter-spacing", ".13em").Style("color", Yellow)
            .Text("SINGULARITY LABORATORY // SECURITY CHECKPOINT"));
        header.Content(WorkshopGui.Element(receiver, "div").Style("flex", "0 0 auto").Style("font-size", ".8rem").Style("color", Muted)
            .Text($"BADGE {checkpoint.BadgeClearance.ToString().ToUpperInvariant()} // QUEUE {checkpoint.PlayerQueueSlot + 1}/{SpatialSecurityCheckpointModel.QueueCapacity}"));
        header.Content(WorkshopGui.Element(receiver, "div").Style("flex", "0 0 auto").Style("font-size", ".8rem").Style("color", Cyan)
            .Text($"GUARDS {traffic.OnDutyGuards(now).Count} // {now:HH:mm}"));
        return header;
    }

    private static ElementBuilder Status(object receiver, SpatialSecurityCheckpointModel checkpoint)
    {
        var accent = checkpoint.Agitated ? Red : checkpoint.HasClearedCheckpoint ? Green : Yellow;
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("left", "1rem").Style("bottom", "1rem")
            .Style("max-width", "50rem").Style("padding", ".55rem .75rem")
            .Style("background", "rgba(1,6,12,.95)").Style("border", $"1px solid {accent}66").Style("z-index", "90");

        panel.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".8rem").Style("letter-spacing", ".12em").Style("color", accent)
            .Text(checkpoint.Stage.ToString().ToUpperInvariant()));
        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".2rem").Style("font-family", "system-ui,sans-serif")
            .Style("font-size", "1rem").Style("color", White).Text(checkpoint.LastMessage));

        if (checkpoint.CutAttempts > 0)
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".18rem").Style("font-size", ".8rem").Style("color", Red)
                .Text($"DIGITEN AGITATION // {checkpoint.CutAttempts} CUT ATTEMPT(S) // LINE STILL ALLOWS PROGRESS"));

        return panel;
    }

    private static ElementBuilder Hotspot(
        object receiver, string label, string left, string top, string width, string height,
        string accent, Action action, bool enabled = true)
        => WorkshopGui.Button(receiver).Label(label)
            .Style("pointer-events", enabled ? "auto" : "none")
            .Style("position", "absolute").Style("left", left).Style("top", top)
            .Style("width", width).Style("height", height).Style("box-sizing", "border-box")
            .Style("background", enabled ? $"{accent}14" : "rgba(1,6,12,.2)")
            .Style("border", enabled ? $"1px solid {accent}99" : $"1px solid {Muted}22")
            .Style("color", enabled ? accent : $"{Muted}55")
            .Style("font-family", "inherit").Style("font-size", ".8rem")
            .Style("letter-spacing", ".06em").Style("cursor", enabled ? "pointer" : "default")
            .OnClick(action);

    private static ElementBuilder Label(object receiver, double x, double y, string text, string color, double size)
        => WorkshopGui.Element(receiver, "text")
            .Attribute("x", x.ToString("0.##")).Attribute("y", y.ToString("0.##"))
            .Attribute("fill", color).Attribute("font-size", size.ToString("0.##")).Text(text);

    private static void Station(object receiver, ElementBuilder svg, double x, double y, double width, double height,
        string label, string color, bool active, bool occupied)
    {
        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", x.ToString("0.##")).Attribute("y", y.ToString("0.##"))
            .Attribute("width", width.ToString("0.##")).Attribute("height", height.ToString("0.##"))
            .Attribute("fill", active || occupied ? $"{color}20" : "#07131b")
            .Attribute("stroke", color).Attribute("stroke-width", ".4"));
        svg.Child(Label(receiver, x + width / 2, y + height / 2, label, color, 1));
    }
}
