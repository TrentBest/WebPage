using System;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Owns the first-contact presentation as an explicit FSM.
/// Razor renders the state and the values produced by the FSM; it does not schedule presentation phases.
/// </summary>
public sealed class FirstContactFsm : IDisposable
{
    private const double PhaseDurationTicks = 30d;

    private readonly string _group = $"FirstContact.Sequence:{Guid.NewGuid():N}";
    private readonly string _definition = $"FirstContact.SequenceFSM:{Guid.NewGuid():N}";
    private readonly FirstContactContext _context = new();
    private readonly FSMHandle _fsm;
    private bool _disposed;

    public FirstContactFsm()
    {
        FSM_API.FSM_API.Create.CreateProcessingGroup(_group);
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_definition, -1, _group)
            .State("FirstOnly", EnterFirstOnly, UpdateFirstOnly, null)
            .State("FirstFadingSecondComingIn", EnterCrossfade, UpdateCrossfade, null)
            .State("SecondOnly", EnterSecondOnly, UpdateSecondOnly, null)
            .State("Gateway", EnterGateway, null, null)
            .State("Moniker", EnterState, UpdateClock, null)
            .State("HubGrowth", EnterState, UpdateClock, null)
            .State("Landing", null, null, null)
            .Transition("FirstOnly", "FirstFadingSecondComingIn", c => ((FirstContactContext)c).StatementOpacity >= 1d)
            .Transition("FirstFadingSecondComingIn", "SecondOnly", c => ((FirstContactContext)c).QuestionOpacity >= 1d)
            .Transition("SecondOnly", "Gateway", c => ((FirstContactContext)c).QuestionOpacity <= 0d)
            .Transition("Gateway", "Moniker", c => ((FirstContactContext)c).EntryRequested)
            .Transition("Moniker", "HubGrowth", c => ((FirstContactContext)c).Ticks >= 60)
            .Transition("HubGrowth", "Landing", c => ((FirstContactContext)c).HubReady)
            .WithInitialState("FirstOnly")
            .BuildDefinition();

        _fsm = FSM_API.FSM_API.Create.CreateInstance(_definition, _context, _group);
    }

    public string CurrentState => _fsm.CurrentState;
    public long StateTicks => _context.Ticks;
    public bool IsLanding => CurrentState == "Landing";
    public bool EntryRequested => _context.EntryRequested;
    public double PresentationProgress => Math.Clamp(_context.Ticks / PhaseDurationTicks, 0d, 1d);
    public double StatementOpacity => _context.StatementOpacity;
    public double QuestionOpacity => _context.QuestionOpacity;

    public FirstContactSoundCue LastSoundCue { get; private set; }
    public event Action<FirstContactSoundCue>? SoundCueRequested;

    public void Start() => _context.Started = true;
    public void RequestEntry() => _context.EntryRequested = true;
    public void SetHubReady() => _context.HubReady = true;

    public void Update()
    {
        if (_disposed || !_context.Started || IsLanding)
            return;

        var before = _fsm.CurrentState;
        FSM_API.FSM_API.Interaction.Update(_group);

        if (before == _fsm.CurrentState)
            return;

        LastSoundCue = _fsm.CurrentState switch
        {
            "FirstFadingSecondComingIn" => FirstContactSoundCue.DigitalResonance,
            "Gateway" => FirstContactSoundCue.Silence,
            "Moniker" => FirstContactSoundCue.Water,
            "HubGrowth" => FirstContactSoundCue.HubArrival,
            "Landing" => FirstContactSoundCue.Silence,
            _ => FirstContactSoundCue.None
        };

        SoundCueRequested?.Invoke(LastSoundCue);
    }

    private static void EnterFirstOnly(IStateContext context)
    {
        var state = (FirstContactContext)context;
        state.Ticks = 0;
        state.StatementOpacity = 0d;
        state.QuestionOpacity = 0d;
    }

    private static void UpdateFirstOnly(IStateContext context)
    {
        var state = (FirstContactContext)context;
        state.Ticks++;
        state.StatementOpacity = FadeWave(state.Ticks / PhaseDurationTicks);
        state.QuestionOpacity = 0d;
    }

    private static void EnterCrossfade(IStateContext context)
    {
        var state = (FirstContactContext)context;
        state.Ticks = 0;
        state.StatementOpacity = 1d;
        state.QuestionOpacity = 0d;
    }

    private static void UpdateCrossfade(IStateContext context)
    {
        var state = (FirstContactContext)context;
        state.Ticks++;
        var incoming = FadeWave(state.Ticks / PhaseDurationTicks);
        state.StatementOpacity = 1d - incoming;
        state.QuestionOpacity = incoming;
    }

    private static void EnterSecondOnly(IStateContext context)
    {
        var state = (FirstContactContext)context;
        state.Ticks = 0;
        state.StatementOpacity = 0d;
        state.QuestionOpacity = 1d;
    }

    private static void UpdateSecondOnly(IStateContext context)
    {
        var state = (FirstContactContext)context;
        state.Ticks++;
        state.StatementOpacity = 0d;
        state.QuestionOpacity = 1d - FadeWave(state.Ticks / PhaseDurationTicks);
    }

    private static void EnterGateway(IStateContext context)
    {
        var state = (FirstContactContext)context;
        state.Ticks = 0;
        state.StatementOpacity = 0d;
        state.QuestionOpacity = 0d;
    }

    private static void EnterState(IStateContext context)
        => ((FirstContactContext)context).Ticks = 0;

    private static void UpdateClock(IStateContext context)
        => ((FirstContactContext)context).Ticks++;

    private static double FadeWave(double progress)
    {
        var p = Math.Clamp(progress, 0d, 1d);
        return 0.5d + (0.5d * Math.Sin((p - 0.5d) * Math.PI));
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_definition, _group);
        _disposed = true;
    }

    public enum FirstContactSoundCue
    {
        None,
        DigitalResonance,
        Water,
        HubArrival,
        Silence
    }

    private sealed class FirstContactContext : IStateContext
    {
        public string Name { get; set; } = "FirstContact.SequenceFSM";
        public bool IsValid { get; set; } = true;
        public bool Started { get; set; }
        public long Ticks { get; set; }
        public bool EntryRequested { get; set; }
        public bool HubReady { get; set; }
        public double StatementOpacity { get; set; }
        public double QuestionOpacity { get; set; }
    }
}
