using System;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>Owns the first-contact sequence. Razor renders state; it does not schedule it.</summary>
public sealed class FirstContactFsm : IDisposable
{
    private readonly string _group = $"FirstContact.Sequence:{Guid.NewGuid():N}";
    private readonly string _definition = $"FirstContact.SequenceFSM:{Guid.NewGuid():N}";
    private readonly FirstContactContext _context = new();
    private readonly FSMHandle _fsm;
    private bool _disposed;

    public FirstContactFsm()
    {
        FSM_API.FSM_API.Create.CreateProcessingGroup(_group);
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_definition, -1, _group)
            .State("Statement", EnterState, UpdateClock, null)
            .State("Question", EnterState, UpdateClock, null)
            .State("Gateway", EnterState, UpdateClock, null)
            .State("Moniker", EnterState, UpdateClock, null)
            .State("HubGrowth", EnterState, UpdateClock, null)
            .State("Landing", null, null, null)
            .Transition("Statement", "Question", c => ((FirstContactContext)c).Ticks >= 20)
            .Transition("Question", "Gateway", c => ((FirstContactContext)c).Ticks >= 24)
            .Transition("Gateway", "Moniker", c => ((FirstContactContext)c).EntryRequested)
            .Transition("Moniker", "HubGrowth", c => ((FirstContactContext)c).Ticks >= 60)
            .Transition("HubGrowth", "Landing", c => ((FirstContactContext)c).HubReady)
            .WithInitialState("Statement")
            .BuildDefinition();

        _fsm = FSM_API.FSM_API.Create.CreateInstance(_definition, _context, _group);
    }

    public string CurrentState => _fsm.CurrentState;
    public long StateTicks => _context.Ticks;
    public bool IsLanding => CurrentState == "Landing";
    public bool EntryRequested => _context.EntryRequested;
    public FirstContactSoundCue LastSoundCue { get; private set; }
    public event Action<FirstContactSoundCue>? SoundCueRequested;

    public void Start() => _context.Started = true;
    public void RequestEntry() => _context.EntryRequested = true;
    public void SetHubReady() => _context.HubReady = true;

    public void Update()
    {
        if (_disposed || !_context.Started || IsLanding) return;

        var before = _fsm.CurrentState;
        FSM_API.FSM_API.Interaction.Update(_group);

        if (before != _fsm.CurrentState)
        {
            LastSoundCue = _fsm.CurrentState switch
            {
                "Question" => FirstContactSoundCue.DigitalResonance,
                "Gateway" => FirstContactSoundCue.Silence,
                "Moniker" => FirstContactSoundCue.Water,
                "HubGrowth" => FirstContactSoundCue.HubArrival,
                "Landing" => FirstContactSoundCue.Silence,
                _ => FirstContactSoundCue.None
            };
            SoundCueRequested?.Invoke(LastSoundCue);
        }
    }

    private static void EnterState(IStateContext context)
        => ((FirstContactContext)context).Ticks = 0;

    private static void UpdateClock(IStateContext context)
        => ((FirstContactContext)context).Ticks++;

    public void Dispose()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_definition, _group);
        _disposed = true;
    }

    public enum FirstContactSoundCue { None, DigitalResonance, Water, HubArrival, Silence }

    private sealed class FirstContactContext : IStateContext
    {
        public string Name { get; set; } = "FirstContact.SequenceFSM";
        public bool IsValid { get; set; } = true;
        public bool Started { get; set; }
        public long Ticks { get; set; }
        public bool EntryRequested { get; set; }
        public bool HubReady { get; set; }
    }
}