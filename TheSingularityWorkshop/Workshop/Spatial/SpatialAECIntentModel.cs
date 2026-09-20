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

        // FSM_API enters the initial state on the first explicit step.
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
            : OptionsForCurrentLayer.FirstOrDefault() ?? "NO DEFAULT";

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

    public IReadOnlyList<string> OptionsForCurrentLayer
        => CurrentLayer switch
        {
            0 => ["REALITY", "FICTION"],
            1 => ["BUILT ENVIRONMENT", "INDUSTRIAL", "CIVIC", "RESIDENTIAL", "RESEARCH"],
            2 => ["FACILITY", "CAMPUS", "INFRASTRUCTURE", "WORKPLACE"],
            3 => ["LABORATORY", "OFFICE", "FACTORY", "HOSPITAL", "WAREHOUSE"],
            4 => ["RESEARCH FACILITY", "HIGH-TECH LABORATORY", "PRODUCTION LABORATORY", "FIELD LAB"],
            5 => ["SINGLE BUILDING", "MULTI-BUILDING", "VERTICAL FACILITY", "CAMPUS FACILITY"],
            6 => ["SCIENCE", "ENGINEERING", "MEDICAL", "ENERGY", "AEROSPACE"],
            7 => ["ADVANCED RESEARCH", "APPLIED RESEARCH", "DEVELOPMENT", "TESTING"],
            8 => ["SCI-FI HIGH-TECH LABORATORY", "RESEARCH LABORATORY", "ENGINEERING LABORATORY", "CUSTOM FACILITY"],
            _ => []
        };

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

        // This is an event-driven FSM. The semantic operation advances exactly one
        // ontology layer; it does not tick a process group shared by unrelated actors.
        _handle.Update();

        // FSM_API 1.0.x can evaluate the transition condition without synchronizing
        // the handle's CurrentState in some package builds. The transition itself is
        // still owned by FSM_API; this fallback asks the handle to perform the same
        // declared transition explicitly when the normal step did not advance it.
        // FSM_API advances the handle's state during Step, while the target state's
        // Enter action is performed on the following update cycle. This model exposes
        // the semantic layer immediately, so explicitly enter the declared target when
        // the state name has advanced but the semantic context has not.
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
        if (_answers[0] == "FICTION" &&
            _answers[4] == "HIGH-TECH LABORATORY" &&
            _answers[6] == "SCIENCE" &&
            _answers[7] == "ADVANCED RESEARCH")
        {
            return "SCI-FI HIGH-TECH RESEARCH LABORATORY";
        }

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
