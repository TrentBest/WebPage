using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Workshop.Agents;

/// <summary>
/// A persistent digital inhabitant of the Singularity World.
///
/// A Digiten is not an FSM. It is an actor whose behavior is orchestrated by
/// one or more FSMs. The first behavioral slice uses one life/intent FSM;
/// additional independent concerns such as locomotion, social behavior and
/// domain roles may later acquire their own concurrent FSMs.
/// </summary>
public sealed class DigitenMicroBundle : IDisposable
{
    private readonly FSMHandle _behavior;
    private readonly DigitenContext _context;
    private readonly string _processingGroup;
    private readonly string _fsmName;
    private bool _disposed;

    public DigitenMicroBundle(int id, string name, byte initialBehaviorIndex = 0)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A Digiten name is required.", nameof(name));

        Id = id;
        Name = name;
        _context = new DigitenContext(id, name)
        {
            IsValid = true,
            BehaviorIndex = initialBehaviorIndex
        };

        _processingGroup = $"Digiten_{id}";
        _fsmName = $"DigitenBehavior_{id}";

        FSM_API.FSM_API.Create.CreateProcessingGroup(_processingGroup);
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_fsmName, -1, _processingGroup)
            .State("Existing", onEnter: EnterExisting, onUpdate: _ => { }, onExit: _ => { })
            .State("PursuingGoal", onEnter: EnterPursuingGoal, onUpdate: _ => { }, onExit: _ => { })
            .State("Navigating", onEnter: EnterNavigating, onUpdate: _ => { }, onExit: _ => { })
            .State("Interacting", onEnter: EnterInteracting, onUpdate: _ => { }, onExit: _ => { })
            .State("Resting", onEnter: EnterResting, onUpdate: _ => { }, onExit: _ => { })
            .Transition("Existing", "PursuingGoal", c => ((DigitenContext)c).HasGoal)
            .Transition("PursuingGoal", "Navigating", c => ((DigitenContext)c).ShouldNavigate)
            .Transition("Navigating", "Interacting", c => ((DigitenContext)c).HasReachedInteraction)
            .Transition("Interacting", "Resting", c => ((DigitenContext)c).InteractionComplete)
            .Transition("Resting", "PursuingGoal", c => ((DigitenContext)c).HasGoal)
            .WithInitialState("Existing")
            .BuildDefinition();

        _behavior = FSM_API.FSM_API.Create.CreateInstance(_fsmName, _context, _processingGroup);
    }

    /// <summary>Stable persistent actor identity.</summary>
    public int Id { get; }

    /// <summary>Human-readable identity; not the actor's behavioral state.</summary>
    public string Name { get; }

    /// <summary>Current FSM_API state name for diagnostics and persistence.</summary>
    public string CurrentBehavior => _behavior.CurrentState;

    /// <summary>
    /// Compact interpreted state index. A renderer or texture-backed persistence
    /// layer can store this as one byte and resolve its meaning through a catalog.
    /// The byte is an index into behavior interpretation, not a second FSM.
    /// </summary>
    public byte BehaviorIndex => _context.BehaviorIndex;

    /// <summary>Current high-level goal, if one has been selected.</summary>
    public DigitenGoal? Goal => _context.Goal;

    /// <summary>FSM context retained as the actor's durable behavioral state bag.</summary>
    public IStateContext Context => _context;

    public void SetGoal(DigitenGoal goal)
    {
        ArgumentNullException.ThrowIfNull(goal);
        _context.Goal = goal;
        _context.HasGoal = true;
        _context.ShouldNavigate = true;
        _context.InteractionComplete = false;
        _context.BehaviorIndex = goal.BehaviorIndex;
    }

    public void SetNavigationRequired(bool required) => _context.ShouldNavigate = required;

    public void ArriveAtInteraction() => _context.HasReachedInteraction = true;

    public void CompleteInteraction()
    {
        _context.InteractionComplete = true;
        _context.HasReachedInteraction = false;
    }

    /// <summary>Advance this Digiten through its FSM_API behavioral lifecycle.</summary>
    public void Update()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.Update(_processingGroup);

        if (!_behavior.HasEnteredCurrentState)
            _behavior.TransitionTo(_behavior.CurrentState);
    }

    public void Dispose()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_fsmName, _processingGroup);
        _disposed = true;
    }

    private static void EnterExisting(IStateContext context) => ((DigitenContext)context).BehaviorLabel = "Existing";
    private static void EnterPursuingGoal(IStateContext context) => ((DigitenContext)context).BehaviorLabel = "PursuingGoal";
    private static void EnterNavigating(IStateContext context) => ((DigitenContext)context).BehaviorLabel = "Navigating";
    private static void EnterInteracting(IStateContext context) => ((DigitenContext)context).BehaviorLabel = "Interacting";
    private static void EnterResting(IStateContext context) => ((DigitenContext)context).BehaviorLabel = "Resting";
}

/// <summary>
/// Goal expressed as intent rather than machine-specific behavior.
/// A Digiten may want to visit, acquire, investigate, work, socialize or rest;
/// the world determines which capabilities can satisfy that intent.
/// </summary>
public sealed record DigitenGoal(string Id, string Description, byte BehaviorIndex = 0);

/// <summary>
/// Compact state bag for a Digiten behavioral FSM.
/// </summary>
public sealed class DigitenContext : IStateContext
{
    public DigitenContext(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; set; }
    public bool IsValid { get; set; }
    public string BehaviorLabel { get; set; } = "Created";
    public byte BehaviorIndex { get; set; }
    public DigitenGoal? Goal { get; set; }
    public bool HasGoal { get; set; }
    public bool ShouldNavigate { get; set; }
    public bool HasReachedInteraction { get; set; }
    public bool InteractionComplete { get; set; }
}

/// <summary>
/// Optional semantic catalog for interpreting compact state indexes.
/// The same byte can mean different things under different behavior catalogs;
/// persistence stores the index while the active FSM/domain supplies meaning.
/// </summary>
public interface IDigitenBehaviorCatalog
{
    string? Describe(byte behaviorIndex);
    IReadOnlyList<byte> KnownIndexes { get; }
}
