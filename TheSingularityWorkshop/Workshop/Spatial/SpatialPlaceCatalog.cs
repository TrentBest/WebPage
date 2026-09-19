namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines the authoritative boundary between the Workshop tool domain and the
/// places presented in Singularity City. A place may expose a Workshop tool
/// without becoming part of the Workshop itself.
/// </summary>
public enum SpatialDomain
{
    Workshop,
    SingularityCity
}

/// <summary>
/// Authoritative identity for a spatial destination.
/// SceneId is the runtime scene key; ToolId identifies the Workshop capability
/// that owns the editable definition, when one exists.
/// </summary>
public sealed record SpatialPlace(
    string Id,
    string Name,
    SpatialDomain Domain,
    string SceneId,
    string? ToolId,
    string Description);

/// <summary>
/// Single source of truth for spatial destinations exposed by Explore.
/// Interactables point at these records rather than inventing destination names
/// in presentation code.
/// </summary>
public static class SpatialPlaceCatalog
{
    private static readonly IReadOnlyList<SpatialPlace> Places =
    [
        // Workshop-owned authoring tools.
        new("workshop.forge", "The Forge", SpatialDomain.Workshop, "forge", "fsm-forge", "FSM state, lifecycle, composition, and executable machinery."),
        new("workshop.image", "Image Workshop", SpatialDomain.Workshop, "image-workshop", "image-workshop", "Image creation and visual asset tooling."),
        new("workshop.blueprints", "Blueprint Library", SpatialDomain.Workshop, "library", "blueprint-library", "Architectural and spatial blueprint authoring."),
        new("workshop.fsm-bench", "FSM Workbench", SpatialDomain.Workshop, "fsm-workbench", "fsm-workbench", "Focused FSM construction and inspection."),
        new("workshop.npc-studio", "NPC Studio", SpatialDomain.Workshop, "npc-studio", "npc-studio", "NPC authoring and behavioral composition."),
        new("workshop.storage", "Storage Bins", SpatialDomain.Workshop, "storage", "data-storage", "Persistent Workshop assets and generated artifacts."),

        // City places. These may expose Workshop capabilities, but the place itself
        // belongs to the world the visitor is exploring.
        new("city.mansion", "Singularity Mansion", SpatialDomain.SingularityCity, "mansion", "building-authoring", "A full architectural residence and its nested spaces."),
        new("city.transit", "Singularity Transit", SpatialDomain.SingularityCity, "singularity-transit", "transit-authoring", "The city's transportation interchange."),
        new("city.ontology-mall", "Singularity Ontology Mall", SpatialDomain.SingularityCity, "singularity-ontology-mall", "ontology-authoring", "A navigable physical manifestation of the ontology."),
        new("city.laboratory", "Singularity Laboratory", SpatialDomain.SingularityCity, "singularity-lab", "laboratory-authoring", "Research facilities and their departments."),
        new("city.station", "Singularity Station", SpatialDomain.SingularityCity, "singularity-station", "station-authoring", "A major civic and transportation station."),
        new("city.shipyard", "Singularity Shipyard", SpatialDomain.SingularityCity, "singularity-shipyard", "vehicle-builder", "A working dockyard where vessel definitions become visible things."),
        new("city.capitol", "Singularity Capitol", SpatialDomain.SingularityCity, "singularity-capitol", "civic-authoring", "Civic and administrative facilities."),
        new("city.ocean-shipyard", "Ocean Shipyard", SpatialDomain.SingularityCity, "ocean-shipyard", "vehicle-builder", "A waterfront yard for boats and marine construction."),
        new("city.maze", "Workshop Maze", SpatialDomain.SingularityCity, "maze", "maze-authoring", "A playable spatial environment."),
        new("city.aec-office", "AEC Remote Office", SpatialDomain.SingularityCity, "aec-remote-office", "aec-authoring", "A remote architectural, engineering, and construction office."),
        new("city.taxi", "Singularity Taxi", SpatialDomain.SingularityCity, "singularity-taxi", "taxi-authoring", "A local transport service and vehicle experience.")
    ];

    public static IReadOnlyList<SpatialPlace> All => Places;

    public static IReadOnlyList<SpatialPlace> WorkshopPlaces
        => Places.Where(x => x.Domain == SpatialDomain.Workshop).ToArray();

    public static IReadOnlyList<SpatialPlace> CityPlaces
        => Places.Where(x => x.Domain == SpatialDomain.SingularityCity).ToArray();

    public static SpatialPlace Resolve(string sceneId)
        => Places.FirstOrDefault(x => string.Equals(x.SceneId, sceneId, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No spatial place is registered for scene '{sceneId}'.");

    public static bool TryResolve(string sceneId, out SpatialPlace place)
    {
        place = Places.FirstOrDefault(x => string.Equals(x.SceneId, sceneId, StringComparison.OrdinalIgnoreCase))!;
        return place is not null;
    }

    /// <summary>
    /// Validates that every interactable has an explicit place and that its
    /// runtime destination is exactly the registered scene.
    /// </summary>
    public static void Validate(IEnumerable<SpatialInteractable> interactables)
    {
        ArgumentNullException.ThrowIfNull(interactables);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var interactable in interactables)
        {
            if (!seen.Add(interactable.Id))
                throw new InvalidOperationException($"Duplicate spatial interactable id '{interactable.Id}'.");

            var place = Resolve(interactable.ExperienceId);
            if (!string.Equals(place.Name, interactable.Name, StringComparison.Ordinal))
                throw new InvalidOperationException(
                    $"Spatial interactable '{interactable.Id}' is named '{interactable.Name}' but its place is '{place.Name}'.");

            if (!string.Equals(place.SceneId, interactable.ExperienceId, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Spatial interactable '{interactable.Id}' routes to '{interactable.ExperienceId}' instead of registered scene '{place.SceneId}'.");
        }
    }
}
