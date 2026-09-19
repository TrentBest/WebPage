namespace TheSingularityWorkshop.Gui;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Presentation catalog for the Ontology Mall. It organizes Experiences and MicroBundles
/// by semantic family before the mall grows a full nine-layer ontology renderer.
/// </summary>
public sealed record SpatialOntologyMallCatalog(IReadOnlyList<SpatialOntologyMallEntry> Entries)
{
    public static SpatialOntologyMallCatalog CreateDefault()
        => new(
            [
                new("experience.explore", "Explore", SpatialOntologyMallKind.Experience, "Experience", "Navigate the Workshop, city, world, and future spatial maps."),
                new("experience.pong", "Pong", SpatialOntologyMallKind.Experience, "Experience", "Idle playable Experience."),
                new("experience.living-gui", "Living GUI", SpatialOntologyMallKind.Experience, "Experience", "Living interface population and recursive GUI composition."),

                new("microbundle.sailing", "Sailing", SpatialOntologyMallKind.MicroBundle, "Mobility / Marine", "Personal sailing, crews, navigation, age-of-sail, and maritime gameplay."),
                new("microbundle.solar-system", "Solar System Activity", SpatialOntologyMallKind.MicroBundle, "Mobility / Space", "Orbital facilities, freighters, passenger ships, yachts, shuttles, and survey craft."),
                new("microbundle.fsm", "FSM", SpatialOntologyMallKind.MicroBundle, "Computation", "State, lifecycle, transition, and executable behavior primitives."),
                new("microbundle.ontology", "Ontology", SpatialOntologyMallKind.MicroBundle, "Knowledge", "Semantic coordinates used to organize the Workshop's things."),
                new("microbundle.gui-builders", "GUI Builders", SpatialOntologyMallKind.MicroBundle, "Construction", "Recursive fluent builders for expressive interfaces."),

                new("domain.air", "Aviation", SpatialOntologyMallKind.Domain, "Mobility / Air", "Enter an aircraft and elect to fly as a future spatial vehicle Experience."),
                new("domain.maritime", "Maritime", SpatialOntologyMallKind.Domain, "Mobility / Marine", "Watercraft, ports, sailing, naval, and ocean-going Experiences."),
                new("domain.space", "Space", SpatialOntologyMallKind.Domain, "Mobility / Space", "Orbital and interplanetary Experiences.")
            ]);

    public IReadOnlyList<SpatialOntologyMallEntry> OfKind(SpatialOntologyMallKind kind)
        => Entries.Where(x => x.Kind == kind).ToArray();
}

public readonly record struct SpatialOntologyMallEntry(
    string Id,
    string Name,
    SpatialOntologyMallKind Kind,
    string Family,
    string Description);

public enum SpatialOntologyMallKind
{
    Experience,
    MicroBundle,
    Domain
}
