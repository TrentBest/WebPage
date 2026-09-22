namespace TheSingularityWorkshop.Gui;

/// <summary>
/// The first intentionally explorable research level.
/// The room is authored as a believable environment rather than a button collection;
/// its hidden capability switch is one object among many scientific artifacts.
/// </summary>
public sealed class SpatialLaboratoryDiscoveryLevel
{
    private SpatialLaboratoryDiscoveryLevel(
        SpatialBounds floor,
        IReadOnlyList<SpatialLaboratoryArtifact> artifacts,
        SpatialLaboratorySwitchManifest hiddenSwitch)
    {
        Floor = floor;
        Artifacts = artifacts;
        HiddenSwitch = hiddenSwitch;
    }

    public SpatialBounds Floor { get; }
    public IReadOnlyList<SpatialLaboratoryArtifact> Artifacts { get; }
    public SpatialLaboratorySwitchManifest HiddenSwitch { get; }

    public static SpatialLaboratoryDiscoveryLevel CreatePhysicsResearchLevel()
    {
        var artifacts = new List<SpatialLaboratoryArtifact>
        {
            Artifact("whiteboard-relativity", "Relativity Whiteboard", "WHITEBOARD", 7, 7, 8, 4),
            Artifact("whiteboard-field-equations", "Field Equations", "WHITEBOARD", 18, 7, 8, 4),
            Artifact("whiteboard-unification", "Unification Notes", "WHITEBOARD", 29, 7, 8, 4),

            Artifact("experiment-pendulum", "Precision Pendulum", "EXPERIMENT", 7, 15, 5, 5),
            Artifact("experiment-orbit", "Orbital Dynamics Table", "EXPERIMENT", 15, 15, 7, 5),
            Artifact("experiment-vacuum", "Vacuum Chamber", "EXPERIMENT", 25, 15, 6, 5),
            Artifact("experiment-plasma", "Plasma Containment Rig", "EXPERIMENT", 34, 15, 6, 5),

            Artifact("instrument-spectrometer", "Spectrometer", "INSTRUMENT", 7, 25, 5, 4),
            Artifact("instrument-interferometer", "Interferometer", "INSTRUMENT", 14, 25, 6, 4),
            Artifact("instrument-microscope", "Quantum Microscope", "INSTRUMENT", 23, 25, 5, 4),
            Artifact("instrument-sensor-array", "Sensor Array", "INSTRUMENT", 30, 25, 8, 4),

            Artifact("console-simulation", "Simulation Console", "CONSOLE", 7, 33, 7, 4),
            Artifact("console-data", "Research Data Terminal", "CONSOLE", 17, 33, 7, 4),
            Artifact("console-observation", "Observation Console", "CONSOLE", 27, 33, 8, 4),

            Artifact("bench-materials", "Materials Bench", "WORKBENCH", 7, 42, 9, 4),
            Artifact("bench-chemistry", "Chemistry Bench", "WORKBENCH", 19, 42, 8, 4),
            Artifact("bench-electronics", "Field Electronics Bench", "WORKBENCH", 30, 42, 8, 4),

            Artifact("cabinet-archives", "Experiment Archive", "ARCHIVE", 7, 51, 5, 7),
            Artifact("cabinet-samples", "Sample Archive", "ARCHIVE", 14, 51, 5, 7),
            Artifact("cabinet-equipment", "Equipment Storage", "STORAGE", 21, 51, 6, 7),
            Artifact("cabinet-prototypes", "Prototype Storage", "STORAGE", 29, 51, 9, 7),

            Artifact("display-event-horizon", "Event Horizon Display", "DISPLAY", 7, 62, 8, 5),
            Artifact("display-tectonic", "Tectonic Simulation", "DISPLAY", 18, 62, 8, 5),
            Artifact("display-cosmic", "Cosmic Scale Projection", "DISPLAY", 29, 62, 9, 5),

            Artifact("workstation-researcher-13", "Researcher Workstation #13", "WORKSTATION", 7, 73, 7, 5),
            Artifact("workstation-director", "Director Research Station", "WORKSTATION", 17, 73, 8, 5),
            Artifact("workstation-analysis", "Analysis Workstation", "WORKSTATION", 28, 73, 9, 5)
        };

        var hiddenSwitch = new SpatialLaboratorySwitchManifest(
            "switch.research-laboratory.3d",
            "RESEARCH SYSTEM // 07",
            new SpatialPoint(34.5, 84),
            "wall-service-panel",
            "A recessed physical switch hidden among ordinary laboratory infrastructure.",
            TheSingularityWorkshop.World.HiddenWorldSwitch.ResearchLaboratoryThreeDimensionality);

        return new SpatialLaboratoryDiscoveryLevel(
            new SpatialBounds(2, 60, 34, 34),
            artifacts,
            hiddenSwitch);
    }

    private static SpatialLaboratoryArtifact Artifact(
        string id,
        string name,
        string category,
        double x,
        double y,
        double width,
        double height)
        => new(id, name, category, new SpatialBounds(x, y, width, height));
}

public readonly record struct SpatialLaboratoryArtifact(
    string Id,
    string Name,
    string Category,
    SpatialBounds Bounds);

public sealed record SpatialLaboratorySwitchManifest(
    string Id,
    string Label,
    SpatialPoint Position,
    string PhysicalForm,
    string Description,
    TheSingularityWorkshop.World.HiddenWorldSwitch Capability);
