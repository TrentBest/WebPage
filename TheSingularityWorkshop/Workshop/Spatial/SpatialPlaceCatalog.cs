namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

public enum SpatialDomain
{
    Workshop,
    SingularityCity
}

public sealed record SpatialPlace(
    string Id,
    string Name,
    SpatialDomain Domain,
    string SceneId,
    string? ToolId,
    SpatialPoint EntryPoint,
    string Description);

/// <summary>
/// Authoritative destination catalog for Explore. A place belongs either to the
/// Workshop tool domain or to Singularity City; a city place may expose a
/// Workshop tool without becoming part of the Workshop.
/// </summary>
public static class SpatialPlaceCatalog
{
    private static readonly IReadOnlyList<SpatialPlace> Places =
    [
        new("workshop.forge", "The Forge", SpatialDomain.Workshop, "forge", "fsm-forge", new SpatialPoint(50, 78), "FSM state, lifecycle, composition, and executable machinery."),
        new("workshop.image", "Image Workshop", SpatialDomain.Workshop, "image-workshop", "image-workshop", new SpatialPoint(50, 78), "Image creation and visual asset tooling."),
        new("workshop.blueprints", "Blueprint Library", SpatialDomain.Workshop, "library", "blueprint-library", new SpatialPoint(50, 78), "Architectural and spatial blueprint authoring."),
        new("workshop.fsm-bench", "FSM Workbench", SpatialDomain.Workshop, "fsm-workbench", "fsm-workbench", new SpatialPoint(50, 78), "Focused FSM construction and inspection."),
        new("workshop.npc-studio", "NPC Studio", SpatialDomain.Workshop, "npc-studio", "npc-studio", new SpatialPoint(50, 78), "NPC authoring and behavioral composition."),
        new("workshop.storage", "Storage Bins", SpatialDomain.Workshop, "storage", "data-storage", new SpatialPoint(50, 78), "Persistent Workshop assets and generated artifacts."),

        new("city.mansion", "Singularity Mansion", SpatialDomain.SingularityCity, "mansion", "building-authoring", new SpatialPoint(50, 78), "A full architectural residence and its nested spaces."),
        new("city.transit", "Singularity Transit", SpatialDomain.SingularityCity, "singularity-transit", "transit-authoring", new SpatialPoint(50, 78), "The city's transportation interchange."),
        new("city.ontology-mall", "Singularity Ontology Mall", SpatialDomain.SingularityCity, "singularity-ontology-mall", "ontology-authoring", new SpatialPoint(50, 78), "A navigable physical manifestation of the ontology."),
        new("workshop.laboratory", "Singularity Laboratory", SpatialDomain.Workshop, "singularity-lab", "laboratory-authoring", new SpatialPoint(50, 78), "Research facilities and their departments."),
        new("city.station", "Singularity Station", SpatialDomain.SingularityCity, "singularity-station", "station-authoring", new SpatialPoint(50, 78), "A major civic and transportation station."),
        new("city.shipyard", "Singularity Shipyard", SpatialDomain.SingularityCity, "singularity-shipyard", "vehicle-builder", new SpatialPoint(50, 72), "A working dockyard where vessel definitions become visible things."),
        new("city.capitol", "Singularity Capitol", SpatialDomain.SingularityCity, "singularity-capitol", "civic-authoring", new SpatialPoint(50, 78), "Civic and administrative facilities."),
        new("city.ocean-shipyard", "Ocean Shipyard", SpatialDomain.SingularityCity, "ocean-shipyard", "vehicle-builder", new SpatialPoint(50, 72), "A waterfront yard for boats and marine construction."),
        new("city.maze", "Workshop Maze", SpatialDomain.SingularityCity, "maze", "maze-authoring", new SpatialPoint(50, 78), "A playable spatial environment."),
        new("workshop.aec-office", "AEC Remote Office", SpatialDomain.Workshop, "aec-remote-office", "aec-authoring", new SpatialPoint(50, 78), "A remote architectural, engineering, and construction office."),
        new("city.taxi", "Singularity Taxi", SpatialDomain.SingularityCity, "singularity-taxi", "taxi-authoring", new SpatialPoint(50, 78), "A local transport service and vehicle experience.")
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
