using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;
using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Captures the user's intended AEC structure before conventional drafting begins.
/// The interrogation walks the nine ontology layers from broad reality/fiction context
/// down to the concrete building type, then assembles a navigable spatial concept.
/// FSM_API owns the interrogation lifecycle; this class owns only the semantic data
/// carried by the FSM context.
/// </summary>
public sealed class SpatialAECIntentModel : IStateContext, IDisposable
{
    public static readonly IReadOnlyList<string> LayerNames =
    [
        "Paradigm", "Domain", "Kingdom", "Phylum", "Class",
        "Order", "Family", "Genus", "Species"
    ];

    private readonly List<string?> _answers = new(new string?[LayerNames.Count]);
    private readonly FSMHandle _handle;
    private readonly string _processingGroup;
    private bool _advanceRequested;
    private bool _disposed;

    public SpatialAECIntentModel()
    {
        _processingGroup = $"SpatialAECIntent:{Guid.NewGuid():N}";
        fsm_API.Create.CreateProcessingGroup(_processingGroup);

        var builder = fsm_API.Create.CreateFiniteStateMachine(
            "SpatialAECIntent",
            processRate: -1,
            processingGroup: _processingGroup);

        for (var i = 0; i < LayerNames.Count; i++)
        {
            var layer = i;
            builder.State(
                LayerNames[layer],
                onEnter: _ => EnterLayer(layer),
                onUpdate: _ => { },
                onExit: _ => { });
        }

        builder
            .State(
                "Complete",
                onEnter: _ => CompleteInterrogation(),
                onUpdate: _ => { },
                onExit: _ => { })
            .WithInitialState(LayerNames[0]);

        for (var i = 0; i < LayerNames.Count - 1; i++)
        {
            var from = LayerNames[i];
            var to = LayerNames[i + 1];
            builder.Transition(from, to, _ => _advanceRequested);
        }

        builder.Transition(
            LayerNames[^1],
            "Complete",
            _ => _advanceRequested);

        builder.BuildDefinition();

        _handle = fsm_API.Create.CreateInstance(
            "SpatialAECIntent",
            this,
            _processingGroup);

        _handle.Update();
    }

    public string Name { get; set; } = "AEC Intent Interrogation";

    public bool IsValid { get; set; } = true;

    /// <summary>The ontology layer currently being interrogated. Nine means complete.</summary>
    public int CurrentLayer { get; private set; }

    public IReadOnlyList<string?> Answers => _answers;

    public string? CurrentAnswer
        => IsComplete ? null : _answers[CurrentLayer];

    /// <summary>Default spatial model the team places on the conference table for the active ontology layer.</summary>
    public string CurrentDefault
        => IsComplete
            ? BuildingType
            : EstablishedDefaultForCurrentLayer();

    private string EstablishedDefaultForCurrentLayer()
    {
        var options = OptionsForCurrentLayer;

        var preferred = CurrentLayer switch
        {
            0 => "REALITY",
            1 => "BUILT ENVIRONMENT",
            2 => "FACILITY",
            3 => "OFFICE",
            4 => "OFFICE BUILDING",
            5 => "SINGLE BUILDING",
            6 => "ENGINEERING",
            7 => "DEVELOPMENT",
            8 => "CUSTOM FACILITY",
            _ => "NO DEFAULT"
        };

        return options.Contains(preferred, StringComparer.OrdinalIgnoreCase)
            ? preferred
            : options.FirstOrDefault() ?? "NO DEFAULT";
    }

    /// <summary>Diegetic artifact name used while the team retrieves a matching model.</summary>
    public string CurrentDefaultArtifact
        => IsComplete
            ? $"BUILDING MODEL // {BuildingType}"
            : CurrentLayer switch
            {
                0 => $"ONTOLOGY TOKEN // {CurrentDefault}",
                1 => $"CITY MASSING // {CurrentDefault}",
                2 => $"FACILITY MASSING // {CurrentDefault}",
                3 => $"FLOOR PLAN // {CurrentDefault}",
                4 => $"BUILDING KIT // {CurrentDefault}",
                5 => $"STACKING MODEL // {CurrentDefault}",
                6 => $"DISCIPLINE KIT // {CurrentDefault}",
                7 => $"PROGRAM MODEL // {CurrentDefault}",
                8 => $"BUILDING MODEL // {CurrentDefault}",
                _ => "MODEL"
            };

    public bool IsComplete => CurrentLayer >= LayerNames.Count;

    public string BuildingType { get; private set; } = "UNRESOLVED STRUCTURE";

    public string Summary
        => string.Join(" / ", _answers.Where(x => !string.IsNullOrWhiteSpace(x)));

    /// <summary>
    /// Shops the concrete AEC ontology catalog using the answers already selected.
    /// Every option shown here exists on at least one compatible catalog record.
    /// </summary>
    public IReadOnlyList<string> OptionsForCurrentLayer
        => SpatialAECOntologyCatalog.OptionsForLayer(CurrentLayer, _answers);

    /// <summary>
    /// Returns the concrete catalog item selected by the completed interrogation.
    /// Null means the interrogation has not resolved to a building yet.
    /// </summary>
    public SpatialAECBuildingType? SelectedBuildingType
        => IsComplete ? SpatialAECOntologyCatalog.Resolve(_answers) : null;

    /// <summary>
    /// Returns the pre-established code basis for the selected building.
    /// Fictional selections receive an explicitly fictional derivative profile.
    /// </summary>
    public SpatialAECCodeProfile? SelectedCodeProfile
        => SelectedBuildingType is null
            ? null
            : SpatialAECPlanExperienceFactory.Create(SelectedBuildingType).CodeProfile;

    /// <summary>
    /// Creates the default plan-view experience for the selected building.
    /// The renderer can walk this data without inventing geometry or capabilities.
    /// </summary>
    public SpatialAECPlanExperience CreatePlanExperience()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var buildingType = SelectedBuildingType
            ?? throw new InvalidOperationException(
                "The AEC intent must resolve to a concrete building before a plan experience can be created.");

        return SpatialAECPlanExperienceFactory.Create(buildingType);
    }

    public void Answer(string value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (IsComplete)
            return;

        if (!OptionsForCurrentLayer.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException(
                $"'{value}' is not a valid answer for {LayerNames[CurrentLayer]}.",
                nameof(value));

        var layerBeingAnswered = CurrentLayer;
        var expectedNextState = layerBeingAnswered == LayerNames.Count - 1
            ? "Complete"
            : LayerNames[layerBeingAnswered + 1];

        _answers[layerBeingAnswered] = value.ToUpperInvariant();
        _advanceRequested = true;
        _handle.Update();

        var expectedNextLayer = layerBeingAnswered + 1;
        if (CurrentLayer != expectedNextLayer)
            _handle.TransitionTo(expectedNextState);

        _advanceRequested = false;
    }

    public void Reset()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        for (var i = 0; i < _answers.Count; i++)
            _answers[i] = null;

        BuildingType = "UNRESOLVED STRUCTURE";
        _advanceRequested = false;

        if (!string.Equals(_handle.CurrentState, LayerNames[0], StringComparison.Ordinal))
            _handle.TransitionTo(LayerNames[0]);
        else
            EnterLayer(0);
    }

    public SpatialAECGeneratedStructure Generate(double squareUnits = 2400)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!IsComplete)
            throw new InvalidOperationException(
                "The AEC intent must reach the concrete building type before generation.");

        var normalized = Math.Max(400, squareUnits);
        var footprint = Math.Sqrt(normalized);
        var floorCount = _answers[5] switch
        {
            "VERTICAL FACILITY" => 6,
            "CAMPUS FACILITY" => 3,
            "MULTI-BUILDING" => 2,
            _ => 1
        };

        var rooms = new List<SpatialAECGeneratedRoom>
        {
            new("lobby", "Arrival / Reception", footprint * .16, footprint * .16),
            new("conference", "Conference / Briefing", footprint * .14, footprint * .12),
            new("research", "Research Core", footprint * .28, footprint * .24),
            new("fabrication", "Fabrication / Prototyping", footprint * .22, footprint * .18),
            new("support", "Building Support", footprint * .12, footprint * .16),
            new("systems", "MEP / Systems", footprint * .10, footprint * .24)
        };

        return new SpatialAECGeneratedStructure(
            BuildingType,
            normalized,
            floorCount,
            Math.Round(footprint, 1),
            rooms,
            Summary);
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        fsm_API.Interaction.DestroyFiniteStateMachine(
            "SpatialAECIntent",
            _processingGroup);

        _disposed = true;
        IsValid = false;
    }

    private void EnterLayer(int layer)
    {
        CurrentLayer = layer;
        _advanceRequested = false;
    }

    private void CompleteInterrogation()
    {
        CurrentLayer = LayerNames.Count;
        BuildingType = ResolveBuildingType();
        _advanceRequested = false;
    }

    private string ResolveBuildingType()
    {
        var catalogMatch = SpatialAECOntologyCatalog.Resolve(_answers);
        if (catalogMatch is not null)
            return catalogMatch.Name == "High-Tech Research Laboratory"
                ? "SCI-FI HIGH-TECH RESEARCH LABORATORY"
                : catalogMatch.Name;

        return _answers[8] ?? "CUSTOM FACILITY";
    }
}

/// <summary>Generated spatial concept produced from an AEC intent interrogation.</summary>
public sealed record SpatialAECGeneratedStructure(
    string BuildingType,
    double SquareUnits,
    int Floors,
    double ApproximateSideLength,
    IReadOnlyList<SpatialAECGeneratedRoom> Rooms,
    string OntologySummary);

/// <summary>A room-sized semantic region in a generated AEC concept.</summary>
public sealed record SpatialAECGeneratedRoom(
    string Id,
    string Name,
    double Width,
    double Depth);
