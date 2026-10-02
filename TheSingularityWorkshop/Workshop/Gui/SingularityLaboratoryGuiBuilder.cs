using TheSingularityWorkshop.Workshop.Laboratory;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Presentation builder for the Singularity Laboratory. It consumes only the
/// laboratory's renderer-neutral room/instrument model.
/// </summary>
public static class SingularityLaboratoryGuiBuilder
{
    public static GuiNode Build()
    {
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

        return root.Build();
    }
}
