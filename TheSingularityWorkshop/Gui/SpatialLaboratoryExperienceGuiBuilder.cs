namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>
/// Diegetic presentation of the Singularity Laboratory.
/// The visitor interacts with security, administration, physical inventory, gravity specimens,
/// experiment recipes, and the third-dimensional rendering instrument as parts of the place itself.
/// </summary>
public static class SpatialLaboratoryExperienceGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpatialLaboratoryManifest laboratory,
        string userName,
        string? selectedFloorId,
        Action<string> selectFloor,
        Action exit,
        Action? scanBadge = null,
        Action<string>? prepareExperiment = null,
        string? preparedExperimentId = null,
        string? gravityBodyAId = null,
        string? gravityBodyBId = null,
        Action<string, string>? selectGravityBodies = null,
        bool threeDimensionalActive = false,
        Action? enterThreeDimensionalLab = null)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "radial-gradient(circle at 50% 35%, #102c3c 0, #03070c 45%, #010204 100%)")
            .Style("color", "#fff").Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(Backdrop(receiver));
        root.Content(Header(receiver, laboratory, userName));

        // The entrance is part of the building, not a modal. The guards and scanner are
        // visible before the visitor is allowed into the research tower.
        root.Content(SecurityGate(receiver, laboratory.Security, scanBadge));

        var tower = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4vw").Style("right", "4vw")
            .Style("top", "22vh").Style("bottom", "8vh")
            .Style("display", "grid")
            .Style("grid-template-columns", "minmax(210px,.72fr) minmax(0,2fr)")
            .Style("gap", "1rem").Style("overflow", "hidden");

        var floors = WorkshopGui.Panel(receiver)
            .Style("overflow-y", "auto").Style("padding", ".65rem")
            .Style("border", "1px solid rgba(0,234,255,.22)")
            .Style("background", "rgba(1,8,14,.76)");

        foreach (var floor in laboratory.Floors)
        {
            var selected = floor.Id == selectedFloorId;
            floors.Content(WorkshopGui.Button(receiver)
                .Label($"{(floor.Level + 1):00} // {floor.Name}")
                .Style("display", "block").Style("width", "100%").Style("margin-bottom", ".45rem")
                .Style("padding", ".55rem").Style("text-align", "left")
                .Style("border", $"1px solid {(selected ? "#ffd34d" : "#00eaff")}{(selected ? "aa" : "35")}")
                .Style("background", selected ? "rgba(255,211,77,.08)" : "rgba(0,234,255,.025)")
                .Style("color", selected ? "#ffd34d" : "#d9eef4")
                .Style("font-family", "inherit").Style("font-size", ".43rem")
                .Style("letter-spacing", ".08em").Style("cursor", "pointer")
                .OnClick(() => selectFloor(floor.Id)));
        }

        var selectedFloor = laboratory.Floors.FirstOrDefault(x => x.Id == selectedFloorId);
        if (selectedFloor == default && laboratory.Floors.Count > 0)
            selectedFloor = laboratory.Floors[0];

        var content = WorkshopGui.Panel(receiver)
            .Style("overflow-y", "auto").Style("padding", "1rem")
            .Style("border", "1px solid rgba(255,211,77,.18)")
            .Style("background", "rgba(5,8,14,.68)");

        if (laboratory.Administration.Services.Count > 0)
            content.Content(Administration(receiver, laboratory.Administration));

        if (laboratory.Floors.Count > 0)
        {
            content.Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".48rem").Style("letter-spacing", ".18em")
                .Style("color", "#ff38d1")
                .Text($"FLOOR {selectedFloor.Level + 1:00} // {selectedFloor.Name}"));

            var simulators = laboratory.Simulators
                .Where(x => x.DomainIds.Contains(selectedFloor.DomainId, StringComparer.OrdinalIgnoreCase))
                .ToList();

            content.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".6rem").Style("font-size", ".5rem").Style("color", "#8ea7b2")
                .Text($"{simulators.Count} COMPOSABLE RESEARCH CAPABILITIES // DOMAIN {selectedFloor.DomainId.ToUpperInvariant()}"));

            if (selectedFloor.DomainId.Equals("gravity", StringComparison.OrdinalIgnoreCase))
                content.Content(GravityLab(receiver, laboratory.GravityBodies, gravityBodyAId, gravityBodyBId, selectGravityBodies));

            if (selectedFloor.DomainId.Equals("simulation", StringComparison.OrdinalIgnoreCase))
                content.Content(SimulationLab(receiver, laboratory, threeDimensionalActive, enterThreeDimensionalLab));

            if (selectedFloor.DomainId.Equals("security", StringComparison.OrdinalIgnoreCase))
                content.Content(SecurityFloor(receiver, laboratory.Security));

            if (selectedFloor.DomainId.Equals("administration", StringComparison.OrdinalIgnoreCase))
                content.Content(AdministrationFloor(receiver, laboratory.Administration));

            content.Content(ExperimentPreparation(
                receiver,
                laboratory.Inventory,
                SpatialLaboratoryExperimentCatalog.CreateDefault().Templates,
                prepareExperiment,
                preparedExperimentId));

            foreach (var simulator in simulators)
            {
                content.Content(WorkshopGui.Panel(receiver)
                    .Style("margin-top", ".65rem").Style("padding", ".75rem")
                    .Style("border", "1px solid rgba(0,234,255,.18)")
                    .Style("background", "rgba(0,234,255,.025)")
                    .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".55rem").Style("color", "#fff").Text(simulator.Name))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("font-size", ".4rem").Style("letter-spacing", ".12em").Style("color", "#ffd34d").Text($"SCALE // {simulator.Scale}"))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".4rem").Style("font-size", ".48rem").Style("line-height", "1.45").Style("color", "#aebfc8").Text(simulator.Purpose)));
            }
        }

        tower.Content(floors).Content(content);
        root.Content(tower);

        root.Content(WorkshopGui.Button(receiver).Label("← RETURN TO CAMPUS")
            .Style("position", "fixed").Style("left", "4vw").Style("bottom", "2vh").Style("z-index", "20")
            .Style("padding", ".5rem .7rem").Style("border", "1px solid #ff38d166")
            .Style("background", "rgba(20,4,18,.9)").Style("color", "#ff38d1")
            .Style("font-family", "inherit").Style("font-size", ".42rem")
            .Style("letter-spacing", ".1em").Style("cursor", "pointer").OnClick(exit));

        return root;
    }

    private static ElementBuilder Backdrop(object receiver)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("background", "linear-gradient(90deg, transparent 0 49.8%, rgba(0,234,255,.06) 50%, transparent 50.2%), repeating-linear-gradient(0deg, transparent 0 42px, rgba(0,234,255,.025) 43px, transparent 44px)")
            .Style("pointer-events", "none");

    private static ElementBuilder Header(object receiver, SpatialLaboratoryManifest lab, string userName)
        => WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4vw").Style("top", "3vh").Style("z-index", "5")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", "clamp(1.1rem,3.8vw,3.2rem)").Style("letter-spacing", ".2em").Style("color", "#00eaff").Text(lab.Name))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".4rem").Style("font-size", ".48rem").Style("letter-spacing", ".18em").Style("color", "#ffd34d")
                .Text($"DIEGETIC RESEARCH FACILITY // VISITOR {userName.ToUpperInvariant()}"))
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".5rem").Style("max-width", "860px").Style("font-size", ".48rem").Style("line-height", "1.5").Style("color", "#aebfc8")
                .Text("The lab is an instrument. Prepare experiments by assembling physical apparatus, not by filling out a worksheet. The resulting composition becomes the experiment definition."));

    private static ElementBuilder SecurityGate(object receiver, SpatialLaboratorySecurity security, Action? scanBadge)
    {
        var status = security.BadgeAccepted ? "BADGE ACCEPTED // RESEARCH ACCESS GRANTED" : "BADGE REQUIRED // SCAN TO ENTER";
        var color = security.BadgeAccepted ? "#52e05a" : "#ffd34d";

        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", "4vw").Style("top", "3vh").Style("z-index", "10")
            .Style("width", "min(390px,34vw)").Style("padding", ".75rem")
            .Style("border", $"1px solid {color}66").Style("background", "rgba(1,4,10,.9)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".43rem").Style("letter-spacing", ".14em").Style("color", color).Text("SECURITY GATE"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".38rem").Style("color", "#d9eef4").Text(status));

        foreach (var guard in security.Guards)
        {
            panel.Content(WorkshopGui.Element(receiver, "div")
                .Style("margin-top", ".4rem").Style("font-size", ".36rem").Style("color", "#8ea7b2")
                .Text($"{guard.Name} // {guard.Greeting}"));
        }

        if (!security.BadgeAccepted && scanBadge is not null)
            panel.Content(WorkshopGui.Button(receiver).Label("[ SCAN BADGE ]")
                .Style("margin-top", ".55rem").Style("padding", ".45rem .7rem")
                .Style("border", "1px solid #ffd34d88").Style("background", "rgba(255,211,77,.08)")
                .Style("color", "#ffd34d").Style("font-family", "inherit").Style("font-size", ".4rem").OnClick(scanBadge));

        return panel;
    }

    private static ElementBuilder SecurityFloor(object receiver, SpatialLaboratorySecurity security)
        => WorkshopGui.Panel(receiver).Style("margin-top", "1rem").Style("padding", "1rem")
            .Style("border", "1px solid rgba(255,211,77,.35)").Style("background", "rgba(255,211,77,.035)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("color", "#ffd34d").Text("SECURITY DESK"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".5rem").Style("font-size", ".48rem").Style("line-height", "1.5").Style("color", "#c9d8de")
                .Text(security.BadgeAccepted ? "Visitor identity is accepted. The guards can direct the visitor toward the administrative office or research floors." : "The visitor has not yet scanned a badge. Research-floor access remains locked."));

    private static ElementBuilder Administration(object receiver, SpatialLaboratoryAdministration administration)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("margin-bottom", ".8rem").Style("padding", ".65rem")
            .Style("border", "1px solid rgba(0,234,255,.2)").Style("background", "rgba(0,234,255,.025)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".42rem").Style("letter-spacing", ".12em").Style("color", "#00eaff")
                .Text($"{administration.Name} // {administration.DigitenId.ToUpperInvariant()}"));

        foreach (var service in administration.Services)
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".34rem").Style("color", "#aebfc8").Text($"• {service.Name} — {service.Description}"));

        return panel;
    }

    private static ElementBuilder AdministrationFloor(object receiver, SpatialLaboratoryAdministration administration)
        => WorkshopGui.Panel(receiver).Style("margin-top", "1rem").Style("padding", "1rem")
            .Style("border", "1px solid rgba(0,234,255,.2)").Style("background", "rgba(0,234,255,.025)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("color", "#00eaff").Text(administration.Name))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".5rem").Style("font-size", ".48rem").Style("line-height", "1.5").Style("color", "#c9d8de")
                .Text("The administrator is a Digiten in the building. The visitor can ask for a guided tour, request a visitor pass, or apply for laboratory space."));

    private static ElementBuilder GravityLab(
        object receiver,
        SpatialGravityBodyCatalog catalog,
        string? bodyAId,
        string? bodyBId,
        Action<string, string>? selectBodies)
    {
        var a = catalog.Find(bodyAId ?? "earth") ?? catalog.Bodies.First();
        var b = catalog.Find(bodyBId ?? "moon") ?? catalog.Bodies.Skip(1).First();
        var heatmap = catalog.Heatmap(a.Id, b.Id, 32);
        var interactionIndex = heatmap.Count(x => x.GravityInteractionActive);

        var panel = WorkshopGui.Panel(receiver)
            .Style("margin-top", "1rem").Style("padding", "1rem")
            .Style("border", "1px solid rgba(77,166,255,.35)").Style("background", "rgba(2,12,24,.72)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("color", "#4da6ff").Text("GRAVITY BODY TRAY"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".4rem").Style("color", "#aebfc8")
                .Text("Drop specimens into the primary tray. Each body keeps its own coloration; the orbit and field overlay are derived from the selected pair."));

        panel.Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("gap", ".35rem").Style("flex-wrap", "wrap").Style("margin-top", ".65rem")
            .Content(WorkshopGui.Element(receiver, "span").Style("padding", ".35rem .5rem").Style("border", $"1px solid {a.Color}99").Style("color", a.Color).Text(a.Name))
            .Content(WorkshopGui.Element(receiver, "span").Style("padding", ".35rem .5rem").Style("border", $"1px solid {b.Color}99").Style("color", b.Color).Text(b.Name)));

        foreach (var body in catalog.Bodies)
        {
            panel.Content(WorkshopGui.Button(receiver)
                .Label($"{body.Name} // {body.Kind.ToUpperInvariant()} // {body.MassKg:E2} KG")
                .Style("display", "inline-block").Style("margin", ".18rem").Style("padding", ".35rem")
                .Style("border", $"1px solid {body.Color}66").Style("background", "rgba(0,0,0,.22)")
                .Style("color", body.Color).Style("font-family", "inherit").Style("font-size", ".33rem")
                .OnClick(() => selectBodies?.Invoke(a.Id, body.Id)));
        }

        panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".7rem").Style("font-size", ".38rem").Style("letter-spacing", ".08em").Style("color", "#ffd34d")
            .Text($"ORBITAL PATH // {a.Name} ↔ {b.Name} // FIELD SAMPLES {heatmap.Count} // ACTIVE INTERACTION CELLS {interactionIndex}"));

        panel.Content(GravityOrbitSvg(receiver, a, b, heatmap));
        return panel;
    }

    private static ElementBuilder GravityOrbitSvg(object receiver, SpatialGravityBody a, SpatialGravityBody b, System.Collections.Generic.IReadOnlyList<SpatialGravityFieldSample> heatmap)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 34").Attribute("preserveAspectRatio", "none")
            .Style("width", "100%").Style("height", "150px").Style("margin-top", ".5rem")
            .Style("background", "rgba(0,0,0,.22)").Style("border", "1px solid rgba(0,234,255,.12)");

        svg.Child(WorkshopGui.Element(receiver, "ellipse").Attribute("cx", "50").Attribute("cy", "17").Attribute("rx", "43").Attribute("ry", "11").Attribute("fill", "none").Attribute("stroke", a.Color).Attribute("stroke-opacity", ".35").Attribute("stroke-width", ".35"));
        svg.Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", "50").Attribute("cy", "17").Attribute("r", "4").Attribute("fill", a.Color).Attribute("fill-opacity", ".22").Attribute("stroke", a.Color).Attribute("stroke-width", ".4"));
        svg.Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", "78").Attribute("cy", "17").Attribute("r", "2.5").Attribute("fill", b.Color).Attribute("fill-opacity", ".35").Attribute("stroke", b.Color).Attribute("stroke-width", ".4"));

        for (var i = 0; i < heatmap.Count; i += 2)
        {
            var sample = heatmap[i];
            var x = 8 + sample.PathPosition * 84;
            var opacity = Math.Clamp(.08 + sample.Intensity * .65, .08, .75);
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x.ToString("0.##")).Attribute("y1", "4")
                .Attribute("x2", x.ToString("0.##")).Attribute("y2", "30")
                .Attribute("stroke", "#ff38d1").Attribute("stroke-opacity", opacity.ToString("0.##"))
                .Attribute("stroke-width", ".7"));
        }

        return svg;
    }

    private static ElementBuilder ExperimentPreparation(
        object receiver,
        System.Collections.Generic.IReadOnlyList<SpatialLaboratoryInventoryItem> inventory,
        System.Collections.Generic.IReadOnlyList<SpatialLaboratoryExperimentTemplate> templates,
        Action<string>? prepareExperiment,
        string? preparedExperimentId)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("margin-top", "1rem").Style("padding", "1rem")
            .Style("border", "1px solid rgba(255,56,209,.24)").Style("background", "rgba(255,56,209,.025)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("color", "#ff38d1").Text("LAB PREPARATION BENCH"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".35rem").Style("font-size", ".42rem").Style("line-height", "1.5").Style("color", "#aebfc8")
                .Text("Choose a physical recipe. The apparatus becomes the lab definition; the written worksheet is no longer the primary interface."));

        foreach (var template in templates)
        {
            var prepared = template.Id == preparedExperimentId;
            panel.Content(WorkshopGui.Button(receiver)
                .Label($"{(prepared ? "✓ " : "")}{template.Name} // {template.Domain}")
                .Style("display", "block").Style("width", "100%").Style("margin-top", ".35rem")
                .Style("padding", ".5rem").Style("text-align", "left")
                .Style("border", $"1px solid {(prepared ? "#52e05a" : "#ff38d1")}66")
                .Style("background", prepared ? "rgba(82,224,90,.07)" : "rgba(255,56,209,.025)")
                .Style("color", prepared ? "#52e05a" : "#ffd7f4").Style("font-family", "inherit").Style("font-size", ".38rem")
                .OnClick(() => prepareExperiment?.Invoke(template.Id))
                .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".25rem").Style("font-size", ".32rem").Style("color", "#8ea7b2")
                    .Text($"APPARATUS // {string.Join(" + ", template.RequiredInventoryIds.Select(id => inventory.FirstOrDefault(x => x.Id == id).Name))}")));
        }

        return panel;
    }

    private static ElementBuilder SimulationLab(object receiver, SpatialLaboratoryManifest lab, bool active, Action? enter)
    {
        var experiment = SpatialLineRenderingExperiment.CreateDefault();
        var panel = WorkshopGui.Panel(receiver)
            .Style("margin-top", "1rem").Style("padding", "1rem")
            .Style("border", "1px solid rgba(255,56,209,.32)").Style("background", "rgba(255,56,209,.035)")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("color", "#ff38d1").Text(lab.ThreeDimensionalLab.Name))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("font-size", ".45rem").Style("line-height", "1.5").Style("color", "#c9d8de").Text(lab.ThreeDimensionalLab.Description))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".55rem").Style("font-size", ".36rem").Style("color", "#ffd34d")
                .Text($"3D SOURCE LINES // {experiment.Stats.Source3DLines:N0}    PROJECTED // {experiment.Stats.ProjectedLines:N0}    RENDERED // {experiment.Stats.RenderedLines:N0}"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".2rem").Style("font-size", ".36rem").Style("color", "#8ea7b2")
                .Text($"TEXTURE // {experiment.Stats.TextureWidth:N0} TEXELS    GPU-SHAPED PAYLOAD // {experiment.Stats.GpuPayloadBytes:N0} BYTES"));

        if (enter is not null)
            panel.Content(WorkshopGui.Button(receiver).Label(active ? "[ EXIT 3D VIEW ]" : "[ ENTER THIRD DIMENSION ]")
                .Style("margin-top", ".7rem").Style("padding", ".5rem .7rem")
                .Style("border", "1px solid #ff38d188").Style("background", "rgba(255,56,209,.08)")
                .Style("color", "#ff38d1").Style("font-family", "inherit").Style("font-size", ".4rem")
                .OnClick(enter));

        if (active)
        {
            panel.Content(ThreeDimensionalViewport(receiver));
            panel.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".55rem").Style("font-size", ".36rem").Style("letter-spacing", ".08em").Style("color", "#00eaff")
                .Text("3D GEOMETRY → PROJECTION → LINEWORK → SPATIAL LINE RENDERER → GPU-SHAPED PAYLOAD"));
        }

        return panel;
    }

    private static ElementBuilder ThreeDimensionalViewport(object receiver)
    {
        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", "0 0 100 54").Attribute("preserveAspectRatio", "none")
            .Style("width", "100%").Style("height", "300px").Style("margin-top", ".7rem")
            .Style("background", "radial-gradient(circle at 50% 50%, rgba(0,234,255,.08), rgba(0,0,0,.4))")
            .Style("border", "1px solid rgba(0,234,255,.25)");

        // A deliberately cheap wireframe projection. This is the instrument we can push
        // until the browser renderer becomes the bottleneck, then replace only this
        // manifestation with a GPU/WebGL/WebGPU path.
        for (var z = 0; z <= 4; z++)
        {
            var y = 44 - z * 8;
            svg.Child(WorkshopGui.Element(receiver, "polygon")
                .Attribute("points", $"{20 + z * 2},{y} {50},{y - 8} {80 - z * 2},{y} {50},{y + 8}")
                .Attribute("fill", "none").Attribute("stroke", "#00eaff").Attribute("stroke-opacity", ".35").Attribute("stroke-width", ".35"));
        }

        for (var i = 0; i < 5; i++)
        {
            var x = 20 + i * 15;
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", "44")
                .Attribute("x2", 50 + (x - 50) * .35).Attribute("y2", "12")
                .Attribute("stroke", "#ff38d1").Attribute("stroke-opacity", ".5").Attribute("stroke-width", ".4"));
        }

        svg.Child(WorkshopGui.Element(receiver, "circle").Attribute("cx", "50").Attribute("cy", "12").Attribute("r", "3")
            .Attribute("fill", "#ffd34d").Attribute("fill-opacity", ".35").Attribute("stroke", "#ffd34d").Attribute("stroke-width", ".5"));

        return svg;
    }
}
