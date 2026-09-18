namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// Declarative interior of a construction-site Experience.
/// Planned structures remain data until their capability is explicitly activated.
/// </summary>
public sealed class SpatialConstructionExperience
{
    public SpatialConstructionExperience(string id, string title, string foremanIntroduction, IEnumerable<SpatialPlannedFeature> features)
    {
        Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Construction Experience id is required.", nameof(id)) : id;
        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("Construction Experience title is required.", nameof(title)) : title;
        ForemanIntroduction = string.IsNullOrWhiteSpace(foremanIntroduction) ? throw new ArgumentException("Foreman introduction is required.", nameof(foremanIntroduction)) : foremanIntroduction;
        Features = new ReadOnlyCollection<SpatialPlannedFeature>((features ?? throw new ArgumentNullException(nameof(features))).ToList());
    }

    public string Id { get; }
    public string Title { get; }
    public string ForemanIntroduction { get; }
    public IReadOnlyList<SpatialPlannedFeature> Features { get; }
}

/// <summary>
/// A future facility or subsystem represented as pure declarative data until activated.
/// The renderer may show it as a blueprint, rough massing, or holographic ghost.
/// </summary>
public sealed record SpatialPlannedFeature(
    string Id,
    string Name,
    string Purpose,
    SpatialBounds Bounds,
    bool Active = false,
    double Completion = 0,
    SpatialPlannedFeatureKind Kind = SpatialPlannedFeatureKind.Building,
    string? Detail = null);

public enum SpatialPlannedFeatureKind
{
    Building,
    Studio,
    Laboratory,
    Theater,
    Shop,
    Service,
    Courtyard
}

public static class SpatialConstructionExperienceCatalog
{
    public static SpatialConstructionExperience Expansion
        => new(
            "construction-site",
            "WORKSHOP EXPANSION // GUIDED TOUR",
            "You're looking at the Workshop before it exists. I can show you what we're building, but until a capability is activated, it remains a definition.",
            [
                new("artist-studio", "ARTIST STUDIO", "Visual creation, illustration, sculpture, materials, and image-making.", new(8, 14, 18, 11), Detail: "A flexible studio with work surfaces, display walls, and material storage."),
                new("sound-studio", "SOUND / RECORDING", "Recording, composition, orchestration, Foley, mixing, and mastering.", new(30, 14, 18, 11), Detail: "The Performing Arts Center's sound-production wing."),
                new("movie-lot", "MOVIE LOT", "Actors, puppets, sets, cameras, lighting, effects, and virtual production.", new(52, 14, 22, 11), Kind: SpatialPlannedFeatureKind.Theater, Detail: "A production floor where scenes can be staged and revised."),
                new("gui-lab", "GUI LAB", "Recursive GUI construction independent of the eventual manifestation platform.", new(8, 32, 18, 11), Kind: SpatialPlannedFeatureKind.Laboratory),
                new("fsm-forge", "FSM FORGE", "Deterministic state, lifecycle, transitions, and process-group construction.", new(30, 32, 18, 11), Kind: SpatialPlannedFeatureKind.Laboratory),
                new("code-lab", "CODE LAB", "Executable artifact construction, inspection, compilation, and packaging.", new(52, 32, 18, 11), Kind: SpatialPlannedFeatureKind.Laboratory),
                new("writing-studio", "WRITING / BOOK STUDIO", "Compose stories, timelines, scenes, and interactive narrative artifacts.", new(8, 50, 18, 11), Kind: SpatialPlannedFeatureKind.Studio),
                new("visualization", "VISUALIZATION / STORYBOARD", "Turn ideas into spatial, temporal, cinematic, and interactive visualizations.", new(30, 50, 18, 11), Kind: SpatialPlannedFeatureKind.Studio),
                new("creation-bay", "CREATION BAY", "Assemble resources, GUIs, FSMs, code, and other focused capabilities into MicroBundles.", new(52, 50, 18, 11), Kind: SpatialPlannedFeatureKind.Laboratory),
                new("creator-market", "CREATOR MARKET", "A future place where creators can present and sell their finished digital creations.", new(8, 68, 24, 10), Kind: SpatialPlannedFeatureKind.Shop),
                new("services", "PRODUCTION SERVICES", "Storage, materials, infrastructure, and shared systems supporting the studios.", new(36, 68, 18, 10), Kind: SpatialPlannedFeatureKind.Service),
                new("performance-center", "SINGULARITY PERFORMING ARTS CENTER", "A larger future venue for rehearsal, performance, recording, and audio experimentation.", new(58, 66, 24, 12), Kind: SpatialPlannedFeatureKind.Theater, Detail: "Music, theater, sound design, and performance all meet here.")
            ]);
}
