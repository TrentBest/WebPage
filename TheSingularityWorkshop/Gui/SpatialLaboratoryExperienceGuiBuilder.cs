namespace TheSingularityWorkshop.Gui;

using System;
using System.Linq;

/// <summary>Renders the Singularity Laboratory as a scalable multi-floor research Experience.</summary>
/// <remarks>
/// The builder consumes only the laboratory manifest. Floors and experiments are composition data,
/// so a future editor can add research domains without creating another renderer.
/// </remarks>
public static class SpatialLaboratoryExperienceGuiBuilder
{
    public static ElementBuilder Build(
        object receiver,
        SpatialLaboratoryManifest laboratory,
        string userName,
        string? selectedFloorId,
        Action<string> selectFloor,
        Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "hidden")
            .Style("background", "radial-gradient(circle at 50% 35%, #102c3c 0, #03070c 45%, #010204 100%)")
            .Style("color", "#fff").Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("inset", "0")
            .Style("background", "linear-gradient(90deg, transparent 0 49.8%, rgba(0,234,255,.06) 50%, transparent 50.2%), repeating-linear-gradient(0deg, transparent 0 42px, rgba(0,234,255,.025) 43px, transparent 44px)")
            .Style("pointer-events", "none"));

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4vw").Style("top", "3vh").Style("z-index", "5")
            .Content(WorkshopGui.Element(receiver, "div").Style("font-size", "clamp(1.1rem,3.8vw,3.2rem)").Style("letter-spacing", ".2em").Style("color", "#00eaff").Text(laboratory.Name))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".4rem").Style("font-size", ".48rem").Style("letter-spacing", ".18em").Style("color", "#ffd34d").Text($"RESEARCH TOWER // {laboratory.Floors.Count} FLOORS // VISITOR {userName.ToUpperInvariant()}"))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".65rem").Style("max-width", "760px").Style("font-size", ".55rem").Style("line-height", "1.5").Style("color", "#aebfc8").Text("Each floor is a common field of research. Mathematical capabilities can be recomposed into new experiments, simulations, and Experiences.")));

        var tower = WorkshopGui.Element(receiver, "div")
            .Style("position", "absolute").Style("left", "4vw").Style("right", "4vw").Style("top", "25vh").Style("bottom", "8vh")
            .Style("display", "grid").Style("grid-template-columns", "minmax(230px,.8fr) minmax(0,2fr)").Style("gap", "1rem").Style("overflow", "hidden");

        var floors = WorkshopGui.Panel(receiver)
            .Style("overflow-y", "auto").Style("padding", ".65rem")
            .Style("border", "1px solid rgba(0,234,255,.22)").Style("background", "rgba(1,8,14,.76)");

        foreach (var floor in laboratory.Floors)
        {
            var selected = floor.Id == selectedFloorId;
            floors.Content(WorkshopGui.Button(receiver)
                .Label($"{floor.Level + 1:00} // {floor.Name}")
                .Style("display", "block").Style("width", "100%").Style("margin-bottom", ".45rem").Style("padding", ".55rem").Style("text-align", "left")
                .Style("border", $"1px solid {(selected ? "#ffd34d" : "#00eaff")}{(selected ? "aa" : "35")}")
                .Style("background", selected ? "rgba(255,211,77,.08)" : "rgba(0,234,255,.025)")
                .Style("color", selected ? "#ffd34d" : "#d9eef4").Style("font-family", "inherit").Style("font-size", ".43rem").Style("letter-spacing", ".08em").Style("cursor", "pointer")
                .OnClick(() => selectFloor(floor.Id)));
        }

        var selectedFloor = laboratory.Floors.FirstOrDefault(x => x.Id == selectedFloorId);
        if (selectedFloor == default && laboratory.Floors.Count > 0)
            selectedFloor = laboratory.Floors[0];

        var content = WorkshopGui.Panel(receiver)
            .Style("overflow-y", "auto").Style("padding", "1rem")
            .Style("border", "1px solid rgba(255,211,77,.18)").Style("background", "rgba(5,8,14,.68)");

        if (laboratory.Floors.Count > 0)
        {
            content.Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".48rem").Style("letter-spacing", ".18em").Style("color", "#ff38d1").Text($"FLOOR {selectedFloor.Level + 1:00} // {selectedFloor.Name}"));

            var simulators = laboratory.Simulators.Where(x => x.DomainIds.Contains(selectedFloor.DomainId, StringComparer.OrdinalIgnoreCase)).ToList();
            content.Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".6rem").Style("font-size", ".5rem").Style("color", "#8ea7b2").Text($"{simulators.Count} COMPOSABLE RESEARCH CAPABILITIES // DOMAIN {selectedFloor.DomainId.ToUpperInvariant()}"));

            if (selectedFloor.DomainId.Equals("security", StringComparison.OrdinalIgnoreCase))
            {
                content.Content(WorkshopGui.Panel(receiver)
                    .Style("margin-top", "1rem").Style("padding", "1rem")
                    .Style("border", "1px solid rgba(255,211,77,.35)").Style("background", "rgba(255,211,77,.035)")
                    .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("color", "#ffd34d").Text("ACCESS CONTROL // SIMULATED SECURITY"))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".5rem").Style("font-size", ".48rem").Style("line-height", "1.5").Style("color", "#c9d8de").Text("Badge-in is required before descending into the research floors. Safety, data handling, and experiment permissions are explicit capabilities—not hidden assumptions."))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".6rem").Style("font-size", ".4rem").Style("letter-spacing", ".1em").Style("color", "#ff38d1").Text("FUTURE: IDENTITY // ACCESS POLICY // SAFETY PROFILE // DATA CONSENT")));
            }
            else if (selectedFloor.DomainId.Equals("administration", StringComparison.OrdinalIgnoreCase))
            {
                content.Content(WorkshopGui.Panel(receiver)
                    .Style("margin-top", "1rem").Style("padding", "1rem")
                    .Style("border", "1px solid rgba(0,234,255,.2)").Style("background", "rgba(0,234,255,.025)")
                    .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".62rem").Style("color", "#00eaff").Text("DIRECTOR'S OFFICE"))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".5rem").Style("font-size", ".48rem").Style("line-height", "1.5").Style("color", "#c9d8de").Text("Operations, project listings, laboratory occupancy, active researchers, requisitions, and facility status belong here."))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".6rem").Style("font-size", ".4rem").Style("letter-spacing", ".1em").Style("color", "#ffd34d").Text("FUTURE: PROJECT REGISTRY // OCCUPANCY // REQUISITIONS // OPERATIONS")));
            }

            foreach (var simulator in simulators)
            {
                content.Content(WorkshopGui.Panel(receiver)
                    .Style("margin-top", ".65rem").Style("padding", ".75rem").Style("border", "1px solid rgba(0,234,255,.18)").Style("background", "rgba(0,234,255,.025)")
                    .Content(WorkshopGui.Element(receiver, "div").Style("font-size", ".55rem").Style("color", "#fff").Text(simulator.Name))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".3rem").Style("font-size", ".4rem").Style("letter-spacing", ".12em").Style("color", "#ffd34d").Text($"SCALE // {simulator.Scale}"))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".4rem").Style("font-size", ".48rem").Style("line-height", "1.45").Style("color", "#aebfc8").Text(simulator.Purpose))
                    .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", ".45rem").Style("font-size", ".36rem").Style("letter-spacing", ".08em").Style("color", "#ff38d1").Text($"DOMAINS // {string.Join(" + ", simulator.DomainIds.Select(x => x.ToUpperInvariant()))}")));
            }
        }

        // The rendering laboratory is itself an instrument: the 3D model is projected
        // into the same FSM-backed linework/render/texture lifecycle used by the Mansion.
        if (selectedFloor.DomainId.Equals("simulation", StringComparison.OrdinalIgnoreCase))
        {
            var experiment = SpatialLineRenderingExperiment.CreateDefault();
            content.Content(WorkshopGui.Panel(receiver)
                .Style("margin-top", "1rem").Style("padding", "1rem")
                .Style("border", "1px solid rgba(255,56,209,.32)")
                .Style("background", "rgba(255,56,209,.035)")
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("font-size", ".62rem").Style("color", "#ff38d1")
                    .Text(experiment.Name))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".5rem").Style("font-size", ".48rem")
                    .Style("line-height", "1.5").Style("color", "#c9d8de")
                    .Text(experiment.Description))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".7rem").Style("font-size", ".42rem")
                    .Style("line-height", "1.7").Style("color", "#ffd34d")
                    .Text($"3D SOURCE LINES // {experiment.Stats.Source3DLines:N0}    PROJECTED // {experiment.Stats.ProjectedLines:N0}    RENDERED // {experiment.Stats.RenderedLines:N0}"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".2rem").Style("font-size", ".42rem")
                    .Style("line-height", "1.7").Style("color", "#8ea7b2")
                    .Text($"TEXTURE // {experiment.Stats.TextureWidth:N0} TEXELS    GPU-SHAPED PAYLOAD // {experiment.Stats.GpuPayloadBytes:N0} BYTES"))
                .Content(WorkshopGui.Element(receiver, "div")
                    .Style("margin-top", ".55rem").Style("font-size", ".38rem")
                    .Style("letter-spacing", ".08em").Style("color", "#00eaff")
                    .Text("3D GEOMETRY → PROJECTION → FSM LINEWORK → SPATIAL LINE RENDERER → RGBA FLOAT TEXTURE")));
        }

        tower.Content(floors).Content(content);
        root.Content(tower);

        root.Content(WorkshopGui.Element(receiver, "div")
            .Style("position", "fixed").Style("right", "4vw").Style("bottom", "2vh").Style("z-index", "10")
            .Style("font-size", ".38rem").Style("letter-spacing", ".12em").Style("color", "#738892")
            .Text("THE LAB GROWS BY COMPOSITION // ADD A FLOOR, DOMAIN, OR EXPERIMENT"));

        root.Content(WorkshopGui.Button(receiver).Label("← RETURN TO CAMPUS")
            .Style("position", "fixed").Style("left", "4vw").Style("bottom", "2vh").Style("z-index", "10")
            .Style("padding", ".5rem .7rem").Style("border", "1px solid #ff38d166").Style("background", "rgba(20,4,18,.9)").Style("color", "#ff38d1")
            .Style("font-family", "inherit").Style("font-size", ".42rem").Style("letter-spacing", ".1em").Style("cursor", "pointer").OnClick(exit));

        return root;
    }
}
