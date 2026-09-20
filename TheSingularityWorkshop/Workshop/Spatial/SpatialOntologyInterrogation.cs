namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.SingularityHub;

/// <summary>
/// The diegetic ontology interrogation performed around a physical conference table.
/// The ontology is presented as data on the table; the interrogation can refine it
/// without turning the presentation into a conventional form.
/// </summary>
public sealed class SpatialOntologyInterrogation
{
    public SpatialOntologyInterrogation(OntologySignature defaultOntology, IReadOnlyList<string>? layerNames = null)
    {
        DefaultOntology = defaultOntology;
        LayerNames = layerNames is { Count: OntologySignature.LayerCount }
            ? layerNames
            : Enumerable.Range(0, OntologySignature.LayerCount)
                .Select(index => $"LAYER-{index}")
                .ToArray();
    }

    public OntologySignature DefaultOntology { get; }
    public IReadOnlyList<string> LayerNames { get; }

    public IReadOnlyList<SpatialOntologyLayer> TablePresentation
        => Enumerable.Range(0, OntologySignature.LayerCount)
            .Select(index => new SpatialOntologyLayer(index, LayerNames[index], DefaultOntology[index]))
            .ToArray();

    public SpatialOntologyLayer ResolveLayer(int index)
        => TablePresentation.First(x => x.Index == index);
}

public readonly record struct SpatialOntologyLayer(int Index, string Name, int Value);

/// <summary>
/// A participant in the model-sourcing crew. The crew is a world actor concern;
/// the GUI only manifests its progress.
/// </summary>
public readonly record struct SpatialModelScout(
    string Id,
    string Name,
    string Specialty);

public readonly record struct SpatialModelCandidate(
    string Id,
    string Name,
    string Category,
    string Source,
    bool Viable,
    string Reason);

/// <summary>
/// Physical handoff state for proposed building components.
/// </summary>
public sealed record SpatialModelSourcingManifest(
    IReadOnlyList<SpatialModelScout> Scouts,
    IReadOnlyList<SpatialModelCandidate> Candidates)
{
    public static SpatialModelSourcingManifest CreateDefault()
        => new(
            [
                new("scout-architecture", "ARCHITECTURAL SCOUT", "building envelope and rooms"),
                new("scout-interior", "INTERIOR SCOUT", "furniture and interior fixtures"),
                new("scout-equipment", "EQUIPMENT SCOUT", "AEC equipment and specialty objects")
            ],
            [
                new("reception-desk", "Reception Desk", "furniture", "model-library", true, "Matches the reception interaction point."),
                new("conference-table", "Conference Table", "furniture", "model-library", true, "Anchors ontology interrogation."),
                new("chair", "Conference Chair", "furniture", "model-library", true, "Provides a seat-based elevation viewpoint."),
                new("trophy-case", "Trophy Case", "wall-detail", "model-library", true, "Wall-mounted hallway interaction target.")
            ]);
}
