using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.Workshop.Laboratory;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Presentation builder for the Singularity Laboratory. It consumes only the
/// laboratory's renderer-neutral room/instrument model.
/// </summary>
public static class SingularityLaboratoryGuiBuilder
{
    public static GuiNode Build() => Build(Array.Empty<ConfigurableLaboratoryRoom>());

    public static GuiNode Build(IEnumerable<ConfigurableLaboratoryRoom> configurableRooms, IReadOnlyList<LaboratoryResult>? results = null)
    {
        ArgumentNullException.ThrowIfNull(configurableRooms);

        var configurableRoomList = configurableRooms.ToList();
        var resultList = results ?? Array.Empty<LaboratoryResult>();
        var root = GuiBuilder.Create("Panel", "singularity-laboratory")
            .Property("style", "display:grid;gap:1rem;padding:1rem;border:1px solid rgba(0,234,255,.35);background:rgba(3,12,24,.8);color:#eafcff;font-family:Consolas,'Courier New',monospace;")
            .Child("Text", "laboratory-title", b => b
                .Text("SINGULARITY LABORATORY // SCIENTIFIC TEST FACILITY")
                .Property("style", "color:#00eaff;font-weight:800;letter-spacing:.14em;"))
            .Child("Text", "laboratory-subtitle", b => b
                .Text("THE LAB BENCH IS A UNIT TEST YOU CAN WALK INTO.")
                .Property("style", "color:#ffd34d;font-weight:700;letter-spacing:.08em;"))
            .Child("Text", "laboratory-notice", b => b
                .Text("Rooms are scientific capabilities. Instruments expose executable experiments. Presentation does not own the physics.")
                .Property("style", "color:#b7d9e0;font-size:.75rem;"));

        root.Child("Panel", "laboratory-rooms", panel => panel
            .Property("style", "display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:.6rem;")
            .Child("Text", "rooms-title", title => title
                .Text("RESEARCH FLOOR // ROOMS")
                .Property("style", "grid-column:1/-1;color:#00eaff;font-weight:800;letter-spacing:.1em;")));

        foreach (var room in LaboratoryRoomCatalog.Rooms)
        {
            root.Child("Panel", $"room-{room.Id}", card => card
                .Property("style", "display:grid;gap:.4rem;padding:.75rem;border:1px solid rgba(0,234,255,.16);background:rgba(0,234,255,.025);")
                .Child("Text", $"room-name-{room.Id}", name => name
                    .Text(room.Name)
                    .Property("style", "font-weight:800;color:#00eaff;"))
                .Child("Text", $"room-discipline-{room.Id}", discipline => discipline
                    .Text(room.Discipline.ToString().ToUpperInvariant())
                    .Property("style", "color:#ff2cff;font-size:.62rem;letter-spacing:.08em;"))
                .Child("Text", $"room-description-{room.Id}", description => description
                    .Text(room.Description)
                    .Property("style", "color:#b7d9e0;font-size:.68rem;"))
                .Child("Text", $"room-instruments-{room.Id}", instruments => instruments
                    .Text($"INSTRUMENTS: {string.Join(" // ", room.Instruments.Select(x => x.Name))}")
                    .Property("style", "color:#7896a0;font-size:.6rem;"))
                .Child("Text", $"room-experiments-{room.Id}", experiments => experiments
                    .Text($"EXPERIMENTS: {string.Join(" // ", room.ExperimentIds)}")
                    .Property("style", "color:#7896a0;font-size:.6rem;")));
        }

        root.Child("Panel", "configurable-laboratory-rooms", panel => panel
            .Property("style", "display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:.6rem;margin-top:.6rem;")
            .Child("Text", "configurable-rooms-title", title => title
                .Text("USER-CONFIGURABLE FLOOR // EMPTY ROOMS")
                .Property("style", "grid-column:1/-1;color:#ffd34d;font-weight:800;letter-spacing:.1em;")));

        foreach (var room in configurableRoomList)
        {
            root.Child("Panel", $"configurable-room-{room.Id}", card => card
                .Property("style", "display:grid;gap:.4rem;padding:.75rem;border:1px dashed rgba(255,211,77,.35);background:rgba(255,211,77,.025);")
                .Child("Text", $"configurable-room-name-{room.Id}", name => name
                    .Text(room.Name)
                    .Property("style", "font-weight:800;color:#ffd34d;"))
                .Child("Text", $"configurable-room-state-{room.Id}", state => state
                    .Text(room.Discipline is null ? "EMPTY // READY FOR USER CONFIGURATION" : $"CONFIGURED // {room.Discipline.Value.ToString().ToUpperInvariant()}")
                    .Property("style", "color:#ff2cff;font-size:.62rem;letter-spacing:.08em;"))
                .Child("Text", $"configurable-room-description-{room.Id}", description => description
                    .Text(room.Description)
                    .Property("style", "color:#b7d9e0;font-size:.68rem;"))
                .Child("Text", $"configurable-room-instruments-{room.Id}", instruments => instruments
                    .Text($"INSTRUMENTS: {(room.Instruments.Count == 0 ? "NONE // USER ADDS" : string.Join(" // ", room.Instruments.Select(x => x.Name)))}")
                    .Property("style", "color:#7896a0;font-size:.6rem;"))
                .Child("Text", $"configurable-room-experiments-{room.Id}", experiments => experiments
                    .Text($"EXPERIMENTS: {(room.Experiments.Count == 0 ? "NONE // USER ADDS" : string.Join(" // ", room.Experiments.Select(x => x.Id)))}")
                    .Property("style", "color:#7896a0;font-size:.6rem;"))
                .Child("Panel", $"configurable-room-experiment-actions-{room.Id}", actions =>
                {
                    actions.Property("style", "display:flex;flex-wrap:wrap;gap:.25rem;");
                    foreach (var experiment in room.Experiments)
                    {
                        actions.Child("Button", $"run-{room.Id}-{experiment.Id}", button => button
                            .Text($"RUN // {experiment.Name.ToUpperInvariant()}")
                            .Property("command", $"run:{room.Id}:{experiment.Id}")
                            .Property("style", "cursor:pointer;color:#9cff57;font:inherit;font-size:.55rem;"));
                    }
                })
                .Child("Panel", $"configurable-room-actions-{room.Id}", actions => actions
                    .Property("style", "display:flex;flex-wrap:wrap;gap:.35rem;margin-top:.35rem;")
                    .Child("Button", $"configure-mechanics-{room.Id}", button => button
                        .Text("MECHANICS").Property("command", $"configure:{room.Id}:Mechanics").Property("style", "cursor:pointer;color:#00eaff;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-electromagnetism-{room.Id}", button => button
                        .Text("ELECTROMAGNETISM").Property("command", $"configure:{room.Id}:Electromagnetism").Property("style", "cursor:pointer;color:#00eaff;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-thermodynamics-{room.Id}", button => button
                        .Text("THERMODYNAMICS").Property("command", $"configure:{room.Id}:Thermodynamics").Property("style", "cursor:pointer;color:#00eaff;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-fluid-dynamics-{room.Id}", button => button
                        .Text("FLUID DYNAMICS").Property("command", $"configure:{room.Id}:FluidDynamics").Property("style", "cursor:pointer;color:#00eaff;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-materials-{room.Id}", button => button
                        .Text("MATERIALS").Property("command", $"configure:{room.Id}:Materials").Property("style", "cursor:pointer;color:#ffd34d;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-fracture-{room.Id}", button => button
                        .Text("FRACTURE").Property("command", $"configure:{room.Id}:FractureMechanics").Property("style", "cursor:pointer;color:#ffd34d;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-nuclear-{room.Id}", button => button
                        .Text("NUCLEAR").Property("command", $"configure:{room.Id}:NuclearPhysics").Property("style", "cursor:pointer;color:#ff2cff;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-chemistry-{room.Id}", button => button
                        .Text("CHEMISTRY").Property("command", $"configure:{room.Id}:Chemistry").Property("style", "cursor:pointer;color:#ff2cff;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"configure-research-{room.Id}", button => button
                        .Text("RESEARCH").Property("command", $"configure:{room.Id}:Research").Property("style", "cursor:pointer;color:#ff2cff;font:inherit;font-size:.6rem;"))
                    .Child("Button", $"clear-room-{room.Id}", button => button
                        .Text("CLEAR")
                        .Property("command", $"clear:{room.Id}")
                        .Property("style", "cursor:pointer;padding:.35rem .5rem;border:1px solid rgba(255,44,255,.35);background:rgba(255,44,255,.04);color:#ff2cff;font:inherit;font-size:.6rem;"))));
        }

        foreach (var result in resultList)
            root.Child("Text", $"result-{result.ExperimentId}", node => node
                .Text($"RESULT // {result.Name.ToUpperInvariant()} // {(result.Passed ? "PASS" : "FAIL")} // {result.Evidence}")
                .Property("style", result.Passed ? "color:#9cff57;font-size:.62rem;" : "color:#ff5577;font-size:.62rem;"));

        return root.Build();
    }
}
