using System.Linq;
using TheSingularityWorkshop.Workshop.Chemistry;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Domain-expert presentation builder for the Chemistry MicroBundle.
/// It consumes chemistry domain data but owns no chemistry data itself.
/// </summary>
public static class ChemistryLabGuiBuilder
{
    public static GuiNode Build()
    {
        var root = GuiBuilder.Create("Panel", "chemistry-lab")
            .Property("style", "display:grid;gap:1rem;padding:1rem;border:1px solid rgba(0,234,255,.35);background:rgba(3,12,24,.8);color:#eafcff;font-family:Consolas,'Courier New',monospace;")
            .Child("Text", "chemistry-title", b => b
                .Text("CHEMISTRY LAB // RESEARCH FACILITY")
                .Property("style", "color:#00eaff;font-weight:800;letter-spacing:.14em;"))
            .Child("Text", "chemistry-subtitle", b => b
                .Text("SYNTHESIS // CHARACTERIZATION // MATERIALS // COMPUTATION // INSTRUMENTATION")
                .Property("style", "color:#7896a0;font-size:.7rem;"))
            .Child("Text", "chemistry-notice", b => b
                .Text("A research environment: instruments are capabilities, samples are state, and only runnable interactions are surfaced at the current scope.")
                .Property("style", "color:#b7d9e0;font-size:.75rem;"));

        root.Child("Panel", "facility-inventory", panel => panel
            .Property("style", "display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:.6rem;")
            .Child("Text", "facility-title", title => title
                .Text("RESEARCH FACILITIES")
                .Property("style", "grid-column:1/-1;color:#ffd34d;font-weight:800;letter-spacing:.1em;")));

        foreach (var facility in ChemistryFacilityCatalog.ResearchFacilities)
        {
            root.Child("Panel", $"facility-{facility.Id}", card => card
                .Property("style", "display:grid;gap:.35rem;padding:.75rem;border:1px solid rgba(0,234,255,.16);background:rgba(0,234,255,.025);")
                .Child("Text", $"facility-name-{facility.Id}", name => name
                    .Text(facility.Name)
                    .Property("style", "font-weight:800;color:#00eaff;"))
                .Child("Text", $"facility-kind-{facility.Id}", kind => kind
                    .Text(facility.Kind.ToString().ToUpperInvariant())
                    .Property("style", "color:#ff2cff;font-size:.62rem;letter-spacing:.08em;"))
                .Child("Text", $"facility-description-{facility.Id}", description => description
                    .Text(facility.Description)
                    .Property("style", "color:#b7d9e0;font-size:.68rem;"))
                .Child("Text", $"facility-mode-{facility.Id}", mode => mode
                    .Text($"TRAINING: {(facility.RequiresTraining ? "REQUIRED" : "NOT REQUIRED")} // AUTOMATION: {(facility.SupportsAutomation ? "AVAILABLE" : "MANUAL")}")
                    .Property("style", "color:#7896a0;font-size:.58rem;")));
        }

        root.Child("Panel", "periodic-data", panel => panel
            .Property("style", "display:grid;gap:.6rem;padding:.8rem;border:1px solid rgba(0,234,255,.16);")
            .Child("Text", "periodic-title", title => title
                .Text("ELEMENTAL DATA")
                .Property("style", "color:#00eaff;font-weight:800;letter-spacing:.1em;")));

        foreach (var atom in ElementalCatalog.Core.Values.OrderBy(x => x.AtomicNumber))
        {
            root.Child("Panel", $"element-{atom.Symbol}", cell => cell
                .Property("style", "display:grid;grid-template-columns:3rem 1fr;gap:.6rem;padding:.65rem;border:1px solid rgba(0,234,255,.16);background:rgba(0,234,255,.025);")
                .Child("Text", $"symbol-{atom.Symbol}", symbol => symbol
                    .Text(atom.Symbol)
                    .Property("style", "font-size:1.5rem;font-weight:800;color:#ff2cff;"))
                .Child("Panel", $"data-{atom.Symbol}", data => data
                    .Child("Text", $"name-{atom.Symbol}", name => name.Text(atom.Name))
                    .Child("Text", $"atomic-number-{atom.Symbol}", number => number.Text($"Z = {atom.AtomicNumber} // Ar = {atom.AtomicWeight:0.###}"))
                    .Child("Text", $"category-{atom.Symbol}", category => category.Text(atom.Category).Property("style", "color:#7896a0;font-size:.65rem;"))));
        }

        root.Child("Panel", "fictional-elements", panel => panel
            .Property("style", "padding:.8rem;border:1px dashed rgba(255,211,77,.5);background:rgba(255,211,77,.03);")
            .Child("Text", "fictional-title", title => title.Text("FICTIONAL ELEMENTS // PLAYGROUND").Property("style", "color:#ffd34d;font-weight:800;"))
            .Child("Text", "unobtanium", text => text.Text("UNOBTANIUM — deliberately fictional; demonstrates that the same Atom/Builder/GUI pipeline can host invented ontology.").Property("style", "color:#a89562;font-size:.7rem;")));

        return root.Build();
    }
}
