using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Chemistry;

/// <summary>Research-laboratory capability categories inspired by modern academic chemistry facilities.</summary>
public enum ChemistryFacilityKind
{
    Synthesis,
    SamplePreparation,
    NuclearMagneticResonance,
    MassSpectrometry,
    InfraredSpectroscopy,
    Chromatography,
    ElectronParamagneticResonance,
    XRayDiffraction,
    Glovebox,
    ThermalAnalysis,
    ComputationalChemistry,
    InstrumentationService
}

/// <summary>Domain description of a research-lab facility, independent of its visual representation.</summary>
public sealed record ChemistryFacility(
    string Id,
    string Name,
    ChemistryFacilityKind Kind,
    string Description,
    bool RequiresTraining = true,
    bool SupportsAutomation = false);

/// <summary>
/// The first Chemistry Lab facility inventory. These are capability abstractions,
/// not claims that the Workshop owns or operates the named commercial instruments.
/// </summary>
public static class ChemistryFacilityCatalog
{
    public static IReadOnlyList<ChemistryFacility> ResearchFacilities { get; } =
    [
        new("synthesis-bench", "Synthesis Bay", ChemistryFacilityKind.Synthesis,
            "Reaction setup, controlled addition, purification, and isolation work.", true, true),
        new("sample-prep", "Sample Preparation", ChemistryFacilityKind.SamplePreparation,
            "Prepare, dilute, filter, aliquot, and label samples before analysis.", true, true),
        new("nmr-suite", "NMR Suite", ChemistryFacilityKind.NuclearMagneticResonance,
            "Non-destructive magnetic-resonance analysis for molecular structure and dynamics.", true, true),
        new("mass-spec", "Mass Spectrometry", ChemistryFacilityKind.MassSpectrometry,
            "Mass-to-charge analysis for molecular characterization and elemental composition workflows.", true, true),
        new("ftir-station", "FTIR / IR Station", ChemistryFacilityKind.InfraredSpectroscopy,
            "Vibrational spectroscopy for functional-group and material characterization.", true, true),
        new("chromatography", "Chromatography Bay", ChemistryFacilityKind.Chromatography,
            "GC, LC, HPLC, and related separation workflows for complex mixtures.", true, true),
        new("epr-suite", "EPR Suite", ChemistryFacilityKind.ElectronParamagneticResonance,
            "Electron-paramagnetic-resonance analysis of species with unpaired electrons.", true, false),
        new("xray-diffraction", "X-Ray Diffraction", ChemistryFacilityKind.XRayDiffraction,
            "Crystal and structural analysis, including variable-temperature diffraction workflows.", true, false),
        new("glovebox", "Inert Atmosphere Glovebox", ChemistryFacilityKind.Glovebox,
            "Controlled-atmosphere manipulation of air- or moisture-sensitive chemistry.", true, false),
        new("thermal-analysis", "Thermal Analysis", ChemistryFacilityKind.ThermalAnalysis,
            "Controlled heating, cooling, and thermal-property characterization.", true, true),
        new("computational", "Computational Chemistry", ChemistryFacilityKind.ComputationalChemistry,
            "Molecular modeling, theoretical chemistry, simulation, and data interpretation.", false, true),
        new("instrument-service", "Instrumentation Workshop", ChemistryFacilityKind.InstrumentationService,
            "Calibration, diagnosis, repair, modification, and custom instrumentation support.", true, false)
    ];
}
