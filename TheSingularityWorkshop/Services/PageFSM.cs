using System;
using TheSingularityWorkshop.FSM_API;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// The outermost FSM for the Workshop web experience.
    /// Blazor is the presentation surface; FSM_API owns progression.
    /// </summary>
    public sealed class PageFSM : IDisposable
    {
        public const string ProcessingGroup = "PageFSM";
        public const string Initializing = "PAGE_INITIALIZING";
        public const string Gateway = "GATEWAY";
        public const string GatewayExit = "GATEWAY_EXIT";
        public const string LivingGuiIgnition = "LIVING_GUI_IGNITION";
        public const string LivingGuiPopulating = "LIVING_GUI_POPULATING";
        public const string MonikerReveal = "MONIKER_REVEAL";
        public const string Gravity = "GRAVITY";
        public const string LivingGuiDissipating = "LIVING_GUI_DISSIPATING";
        public const string NavigationArrival = "NAVIGATION_ARRIVAL";
        public const string Running = "RUNNING";
        public const string ShutdownState = "SHUTDOWN";

        private readonly FSMHandle _handle;
        private bool _disposed;

        public PageStateContext Context { get; }
        public string CurrentState => _handle.CurrentState;
        public event Action<string>? StateChanged;

        public sealed class Behavior
        {
            public Action<PageStateContext>? OnInitialization { get; init; }
            public Action<PageStateContext>? OnGateway { get; init; }
            public Action<PageStateContext>? OnGatewayExit { get; init; }
            public Action<PageStateContext>? OnLivingGuiIgnition { get; init; }
            public Action<PageStateContext>? OnLivingGuiPopulating { get; init; }
            public Action<PageStateContext>? OnMonikerReveal { get; init; }
            public Action<PageStateContext>? OnGravity { get; init; }
            public Action<PageStateContext>? OnLivingGuiDissipating { get; init; }
            public Action<PageStateContext>? OnNavigationArrival { get; init; }
            public Action<PageStateContext>? OnRunning { get; init; }
            public Action<PageStateContext>? OnShutdown { get; init; }
        }

        public PageFSM(object? singularityHub = null, Behavior? behavior = null)
        {
            Context = new PageStateContext(singularityHub);
            fsm_API.Create.CreateProcessingGroup(ProcessingGroup);

            fsm_API.Create.CreateFiniteStateMachine("PageFSM", -1, ProcessingGroup)
                .State(Initializing, Enter(behavior?.OnInitialization), Tick, null)
                .State(Gateway, Enter(behavior?.OnGateway), Tick, null)
                .State(GatewayExit, Enter(behavior?.OnGatewayExit), Tick, null)
                .State(LivingGuiIgnition, Enter(c =>
                {
                    c.BeginLivingGui();
                    behavior?.OnLivingGuiIgnition?.Invoke(c);
                }), Tick, null)
                .State(LivingGuiPopulating, Enter(behavior?.OnLivingGuiPopulating), Tick, null)
                .State(MonikerReveal, Enter(behavior?.OnMonikerReveal), Tick, null)
                .State(Gravity, Enter(behavior?.OnGravity), Tick, null)
                .State(LivingGuiDissipating, Enter(behavior?.OnLivingGuiDissipating), Tick, null)
                .State(NavigationArrival, Enter(behavior?.OnNavigationArrival), Tick, null)
                .State(Running, Enter(behavior?.OnRunning), Tick, null)
                .State(ShutdownState, Enter(behavior?.OnShutdown), Tick, null)
                .WithInitialState(Initializing)
                .Transition(Initializing, Gateway, c => true)
                .Transition(Gateway, GatewayExit, c => ((PageStateContext)c).EnterRequested)
                .Transition(GatewayExit, LivingGuiIgnition, c => ((PageStateContext)c).StateTicks >= 1)
                .Transition(LivingGuiIgnition, LivingGuiPopulating, c => ((PageStateContext)c).StateTicks >= 1)
                .Transition(LivingGuiPopulating, MonikerReveal, c => ((PageStateContext)c).LivingGuiPopulated)
                // MonikerReveal is intentionally terminal for this landing sequence.
                // The moniker must remain visible until navigation leaves the page.
                .BuildDefinition();

            _handle = fsm_API.Create.CreateInstance("PageFSM", Context, ProcessingGroup);
        }

        private Action<IStateContext> Enter(Action<PageStateContext>? behavior)
        {
            return c =>
            {
                var context = (PageStateContext)c;
                context.ResetStateClock();
                behavior?.Invoke(context);
            };
        }

        private void Tick(IStateContext stateContext)
        {
            var context = (PageStateContext)stateContext;
            context.TotalTicks++;
            context.StateTicks++;

            if (CurrentState == LivingGuiPopulating)
                context.AdvanceLivingGui();

            StateChanged?.Invoke(CurrentState);
        }

        public void RequestEnter()
        {
            Context.EnterRequested = true;
            Update();
        }

        public void SignalLivingGuiPopulated() => Context.LivingGuiPopulated = true;
        public void SignalMonikerReady() => Context.MonikerReady = true;
        public void SignalGravityReleased() => Context.GravityReleased = true;
        public void SignalLivingGuiFallen() => Context.LivingGuiFallen = true;
        public void SignalNavigationReady() => Context.NavigationReady = true;

        public void Shutdown()
        {
            if (!_disposed) _handle.TransitionTo(ShutdownState);
        }

        public void Update()
        {
            if (!_disposed) fsm_API.Interaction.Update(ProcessingGroup);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}