using System;
using TheSingularityWorkshop.FSM_API;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// The outermost FSM for the Workshop web experience.
    /// Blazor is the presentation surface; FSM_API owns progression while the
    /// Hub owns process-group scheduling and this FSM selects exactly one nested
    /// runtime pipeline after each page heartbeat.
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

        private const long GravityReleaseTicks = 1;
        private const long NavigationDelayTicks = 91;

        private readonly FSMHandle _handle;
        private readonly FSMHandle _livingGuiHandle;
        private readonly HubKernel _hub;
        private readonly string _processingGroup;
        private readonly string _livingGuiProcessingGroup;
        private readonly string _gravityProcessingGroup;
        private bool _disposed;

        public PageStateContext Context { get; }
        public HubKernel Hub => _hub;
        public string CurrentState => _handle.CurrentState;
        public string LivingGuiState => _livingGuiHandle.CurrentState;
        public string InstanceProcessingGroup => _processingGroup;
        public string LivingGuiProcessingGroup => _livingGuiProcessingGroup;
        public string GravityProcessingGroup => _gravityProcessingGroup;
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
            _hub = singularityHub as HubKernel ?? new HubKernel();
            _processingGroup = $"{ProcessingGroup}:{Guid.NewGuid():N}";
            _livingGuiProcessingGroup = $"LivingGui:{Guid.NewGuid():N}";
            _gravityProcessingGroup = $"Gravity:{Guid.NewGuid():N}";

            fsm_API.Create.CreateProcessingGroup(_processingGroup);
            fsm_API.Create.CreateProcessingGroup(_livingGuiProcessingGroup);
            fsm_API.Create.CreateProcessingGroup(_gravityProcessingGroup);

            _hub
                .RegisterProcessGroup(_processingGroup)
                .RegisterProcessGroup(_livingGuiProcessingGroup, _processingGroup)
                .RegisterProcessGroup(_gravityProcessingGroup, _processingGroup);

            fsm_API.Create.CreateFiniteStateMachine("PageFSM", -1, _processingGroup)
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
                .State(Gravity, Enter(c =>
                {
                    c.GravityReleased = false;
                    c.LivingGuiFallen = false;
                    behavior?.OnGravity?.Invoke(c);
                }), Tick, null)
                .State(LivingGuiDissipating, Enter(behavior?.OnLivingGuiDissipating), Tick, null)
                .State(NavigationArrival, Enter(behavior?.OnNavigationArrival), Tick, null)
                .State(Running, Enter(behavior?.OnRunning), Tick, null)
                .State(ShutdownState, Enter(behavior?.OnShutdown), Tick, null)
                .WithInitialState(Initializing)
                .Transition(Initializing, Gateway, c => true)
                .Transition(Gateway, GatewayExit, c => ((PageStateContext)c).EnterRequested)
                .Transition(GatewayExit, LivingGuiIgnition, c => ((PageStateContext)c).StateTicks >= 1)
                .Transition(LivingGuiIgnition, LivingGuiPopulating, c => ((PageStateContext)c).StateTicks >= 1)
                .Transition(LivingGuiPopulating, MonikerReveal, c =>
                {
                    var context = (PageStateContext)c;
                    return context.LivingGuiPopulated &&
                           context.PopulationCompletedTick >= 0 &&
                           context.TotalTicks > context.PopulationCompletedTick;
                })
                .Transition(MonikerReveal, Gravity, c => ((PageStateContext)c).StateTicks >= GravityReleaseTicks)
                .Transition(Gravity, LivingGuiDissipating, c => ((PageStateContext)c).LivingGuiFallen)
                .Transition(LivingGuiDissipating, NavigationArrival, c => ((PageStateContext)c).StateTicks >= NavigationDelayTicks)
                .Transition(NavigationArrival, Running, c => ((PageStateContext)c).StateTicks >= 1)
                .BuildDefinition();

            fsm_API.Create.CreateFiniteStateMachine("LivingGuiFSM", -1, _livingGuiProcessingGroup)
                .State("Populating", onEnter: null, onUpdate: _ => Context.AdvanceLivingGui(), onExit: null)
                .WithInitialState("Populating")
                .BuildDefinition();

            fsm_API.Create.CreateFiniteStateMachine("GravityFSM", -1, _gravityProcessingGroup)
                .State("Falling", onEnter: null, onUpdate: _ => Context.AdvanceGravity(), onExit: null)
                .WithInitialState("Falling")
                .BuildDefinition();

            _handle = fsm_API.Create.CreateInstance("PageFSM", Context, _processingGroup);
            _livingGuiHandle = fsm_API.Create.CreateInstance("LivingGuiFSM", Context, _livingGuiProcessingGroup);
            fsm_API.Create.CreateInstance("GravityFSM", Context, _gravityProcessingGroup);

            Update();
        }

        private static Action<IStateContext> Enter(Action<PageStateContext>? behavior)
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

        /// <summary>
        /// Advances exactly one page heartbeat, then selects the one nested
        /// process group owned by the state that was already active at the start
        /// of that heartbeat. A newly-entered state gets its first nested tick on
        /// the following heartbeat.
        /// </summary>
        public void Update()
        {
            if (_disposed)
                return;

            var stateBeforeHeartbeat = CurrentState;
            _hub.Update();

            if (stateBeforeHeartbeat == LivingGuiPopulating &&
                CurrentState == LivingGuiPopulating &&
                !Context.LivingGuiFrozen &&
                !Context.LivingGuiPopulated)
            {
                _hub.UpdateProcessGroup(_livingGuiProcessingGroup);
            }
            else if (stateBeforeHeartbeat == Gravity &&
                     CurrentState == Gravity &&
                     !Context.LivingGuiFallen)
            {
                _hub.UpdateProcessGroup(_gravityProcessingGroup);
            }

            StateChanged?.Invoke(CurrentState);
        }

        public void Dispose()
        {
            if (_disposed) return;
            fsm_API.Interaction.DestroyFiniteStateMachine("PageFSM", _processingGroup);
            fsm_API.Interaction.DestroyFiniteStateMachine("LivingGuiFSM", _livingGuiProcessingGroup);
            fsm_API.Interaction.DestroyFiniteStateMachine("GravityFSM", _gravityProcessingGroup);
            _disposed = true;
        }
    }
}