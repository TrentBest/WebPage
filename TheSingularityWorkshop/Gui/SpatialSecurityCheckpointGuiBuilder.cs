namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;

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
        string userName, Action<MouseEventArgs> worldClick, Action<int> moveQueue, Action placeTray, Action loadBelongings,
        Action scanner, Action collectTray, Action returnToCampus, Action enterFacility)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", Ink).Style("color", White)
            .Style("font-family", "Consolas,'Courier New',monospace");

        root.Content(Environment(receiver, checkpoint, traffic, userName, worldClick, moveQueue, placeTray, loadBelongings, scanner, collectTray, enterFacility));
        root.Content(Frame(receiver, checkpoint, traffic));
        root.Content(WorkshopGui.Button(receiver).Label("RETURN")
            .Style("position", "fixed").Style("right", "2rem").Style("bottom", "2rem")
            .Style("z-index", "90").Style("padding", ".85rem 1.25rem")
            .Style("background", "rgba(2,8,15,.9)").Style("border", $"1px solid {Muted}88")
            .Style("color", White).Style("font-family", "inherit").Style("font-size", "1rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer").OnClick(returnToCampus));
        return root;
    }

    private static ElementBuilder Environment(
        object receiver,
        SpatialSecurityCheckpointModel checkpoint,
        SpatialLaboratoryTrafficModel traffic,
        string userName,
        Action<MouseEventArgs> worldClick,
        Action<int> moveQueue,
        Action placeTray,
        Action loadBelongings,
        Action scanner,
        Action collectTray,
        Action enterFacility)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 120 72").Style("position", "absolute")
            .Style("inset", "0").Style("width", "100vw").Style("height", "100vh")
            .Style("cursor", "pointer")
            .OnClick(worldClick);

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
        // The Digitens define the queue. Empty space is intentionally empty.
        const queueX = 42d;
        const queueStep = 4.6d;
        foreach (var agent in checkpoint.Agents.OrderBy(agent => agent.QueueSlot))
        {
            var y = 29 + agent.QueueSlot * queueStep;
            var highlighted = agent.QueueSlot == checkpoint.PlayerQueueSlot;
            var person = WorkshopGui.Element(receiver, "g")
                .OnClick(_ => moveQueue(agent.QueueSlot))
                .StopPropagation("onclick")
                .Style("cursor", "pointer");

            person.Child(WorkshopGui.Element(receiver, "circle")
                .Attribute("cx", queueX.ToString("0.##"))
                .Attribute("cy", (y - 2).ToString("0.##"))
                .Attribute("r", highlighted ? "1.7" : "1.35")
                .Attribute("fill", highlighted ? Magenta : Cyan));
            person.Child(WorkshopGui.Element(receiver, "path")
                .Attribute("d", $"M {queueX - 2} {y + 2} Q {queueX} {y - 1} {queueX + 2} {y + 2} L {queueX + 2.3} {y + 5} H {queueX - 2.3} Z")
                .Attribute("fill", highlighted ? $"{Magenta}44" : $"{Cyan}1c")
                .Attribute("stroke", highlighted ? Magenta : Cyan)
                .Attribute("stroke-width", highlighted ? ".5" : ".3"));

            var speech = checkpoint.GetAgentSpeech(agent.Id);
            if (!string.IsNullOrWhiteSpace(speech))
                person.Child(Speech(receiver, queueX + 4.5, y - 1, speech, agent.QueueSlot < checkpoint.PlayerQueueSlot ? Yellow : Red));

            svg.Child(person);

            if (agent.QueueSlot == 0)
                svg.Child(Label(receiver, 48, y - 1, ServiceText(checkpoint.GetAgentStage(agent.Id)), Yellow, .72));
        }

        var userY = 29 + checkpoint.PlayerQueueSlot * queueStep;
        DrawPerson(receiver, svg, queueX, userY, Magenta, true);
        svg.Child(Label(receiver, 47, userY + 1, userName.ToUpperInvariant(), Magenta, .72));

        // Empty floor positions are spatial interaction targets, not buttons.
        for (var slot = 0; slot < SpatialSecurityCheckpointModel.QueueCapacity; slot++)
        {
            if (slot == checkpoint.PlayerQueueSlot || checkpoint.Agents.Any(agent => agent.QueueSlot == slot))
                continue;

            var y = 29 + slot * queueStep;
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", (queueX - 3.2).ToString("0.##"))
                .Attribute("y", (y - 2.4).ToString("0.##"))
                .Attribute("width", "6.4").Attribute("height", "5")
                .Attribute("fill", "transparent").Attribute("stroke", "none")
                .OnClick(_ => moveQueue(slot))
                .StopPropagation("onclick"));
        }

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
            .Attribute("stroke", Yellow).Attribute("stroke-width", ".5")
            .OnClick(_ => checkpoint.Stage == SpatialSecurityCheckpointStage.TrayReady ? placeTray() : loadBelongings())
            .StopPropagation("onclick"));
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
            .Attribute("fill", Ink).Attribute("stroke", "#39717e").Attribute("stroke-width", ".35")
            .OnClick(_ => loadBelongings()).StopPropagation("onclick"));
        svg.Child(Label(receiver, 67, 32, "SCREENING CORE", Cyan, .85));

        // Body scanner portal.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 84 57 V 38 Q 84 34 88 34 H 96 Q 100 34 100 38 V 57")
            .Attribute("fill", "none").Attribute("stroke", Magenta).Attribute("stroke-width", "1.2")
            .OnClick(_ => scanner()).StopPropagation("onclick"));
        svg.Child(Label(receiver, 84, 31.8, "WALK THROUGH", Magenta, .9));

        // Collection zone.
        svg.Child(WorkshopGui.Element(receiver, "path")
            .Attribute("d", "M 101 40 H 112 V 54 H 101 Z")
            .Attribute("fill", "#0a1d25").Attribute("stroke", Green).Attribute("stroke-width", ".55")
            .OnClick(_ => collectTray()).StopPropagation("onclick"));
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
        if (checkpoint.HasClearedCheckpoint)
            svg.Child(WorkshopGui.Element(receiver, "rect")
                .Attribute("x", "101").Attribute("y", "16").Attribute("width", "13").Attribute("height", "42")
                .Attribute("fill", "transparent").Attribute("stroke", "none")
                .OnClick(_ => enterFacility())
                .StopPropagation("onclick"));

        return svg;
    }

    private static ElementBuilder Frame(object receiver, SpatialSecurityCheckpointModel checkpoint, SpatialLaboratoryTrafficModel traffic)
    {