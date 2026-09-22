using System;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>Owns the first-contact sequence. Razor renders state; it does not schedule it.</summary>
public sealed class FirstContactFsm : IDisposable
{
    public const string Group = "FirstContact.Sequence";
    public const string Definition = "FirstContact.SequenceFSM";

    private readonly FirstContactContext _context = new();
    private readonly FSMHandle _fsm;
    private bool _disposed;

    public FirstContactFsm()
    {
        FSM_API.FSM_API.Create.CreateProcessingGroup(Group);
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(Definition, -1, Group)
            .State("Statement", EnterState, UpdateClock, null)
            .State("Question", EnterState, UpdateClock, null)
            .State("Moniker", EnterState, UpdateClock, null)
            .State("HubGrowth", EnterState, UpdateClock, null)
            .State("Landing", null, null, null)
            .Transition("Statement", "Question", c => ((FirstContactContext)c).Ticks >= 70)
            .Transition("Question", "Moniker", c => ((FirstContactContext)c).Ticks >= 70)
            .Transition("Moniker", "HubGrowth", c => ((FirstContactContext)c).Ticks >= 60)
            .Transition("HubGrowth", "Landing", c => ((FirstContactContext)c).HubReady)
            .WithInitialState("Statement")
            .BuildDefinition();

        _fsm = FSM_API.FSM_API.Create.CreateInstance(Definition, _context, Group);
    }

    public string CurrentState => _fsm.CurrentState;
    public long StateTicks => _context.Ticks;
    public bool IsLanding => CurrentState == "Landing";
    public FirstContactSoundCue LastSoundCue { get; private set; }
    public event Action<FirstContactSoundCue>? SoundCueRequested;

    public void Start() => _context.Started = true;
    public void SetHubReady() => _context.HubReady = true;

    public void Update()
    {
        if (_disposed || !_context.Started || IsLanding) return;

        var before = _fsm.CurrentState;
        FSM_API.FSM_API.Interaction.Update(Group);

        if (before != _fsm.CurrentState)
        {
            LastSoundCue = _fsm.CurrentState switch
            {
                "Question" => FirstContactSoundCue.DigitalResonance,
                "Moniker" => FirstContactSoundCue.Water,
                "HubGrowth" => FirstContactSoundCue.HubArrival,
                "Landing" => FirstContactSoundCue.Silence,
                _ => FirstContactSoundCue.None
            };
            SoundCueRequested?.Invoke(LastSoundCue);
        }
    }

    private static void EnterState(IStateContext context) => ((FirstContactContext)context).Ticks = 0;
    private static void UpdateClock(IStateContext context) => ((FirstContactContext)context).Ticks++;

    public void Dispose()
    {
        if (_disposed) return;
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(Definition, Group);
        _disposed = true;
    }

    public enum FirstContactSoundCue { None, DigitalResonance, Water, HubArrival, Silence }

    private sealed class FirstContactContext : IStateContext
    {
        public string Name { get; set; } = Definition;
        public bool IsValid { get; set; } = true;
        public bool Started { get; set; }
        public long Ticks { get; set; }
        public bool HubReady { get; set; }
    }
}