using System;
using TheSingularityWorkshop.FSM_API;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// The outermost FSM for the Workshop web experience.
    /// Blazor is presentation only. FSM_API owns progression and the Hub owns the
    /// application heartbeat boundary, including the nested Living GUI/physics groups.
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

        public static readonly TimeSpan DefaultMonikerPresentationDuration = TimeSpan.FromSeconds(3);
        private const long GravityReleaseTicks = 1;
        private long MonikerPresentationTicks => Math.Max(1, (long)Math.Round(MonikerPresentationDuration.TotalMilliseconds / 33.0));

        private readonly FSMHandle _handle;
        private readonly LivingGuiFsm _livingGuiRuntime;
        private readonly HubKernel _hub;
        private readonly string _processingGroup;
        private readonly string _gravityProcessingGroup;
        private bool _disposed;

        public PageStateContext Context { get; }
        public HubKernel Hub => _hub;
        public string CurrentState => _handle.CurrentState;
        public bool IsValid => _handle.IsValid;
        public bool LivingGuiIsValid => _livingGuiRuntime.IsValid;
        public string LivingGuiState => _livingGuiRuntime.CurrentState;
        public string InstanceProcessingGroup => _processingGroup;
        public string LivingGuiProcessingGroup => _livingGuiRuntime.ProcessingGroup;
        public string LivingGuiRootGrowthProcessingGroup => _livingGuiRuntime.RootGrowthProcessingGroup;
        public string LivingGuiSeedFlightProcessingGroup => _livingGuiRuntime.SeedFlightProcessingGroup;
        public string LivingGuiSeedScalingProcessingGroup => _livingGuiRuntime.SeedScalingProcessingGroup;
        public string LivingGuiMatureGrowthProcessingGroup => _livingGuiRuntime.MatureGrowthProcessingGroup;
        public string LivingGuiReproductionProcessingGroup => _livingGuiRuntime.ReproductionProcessingGroup;
        public string LivingGuiParentRecoveryProcessingGroup => _livingGuiRuntime.ParentRecoveryProcessingGroup;
        public string LivingGuiActivePhase => _livingGuiRuntime.ActivePhase;
        public int SeedlingCount => _livingGuiRuntime.SeedlingCount;
        public IReadOnlyList<LivingGuiOrganismFsm> Organisms => _livingGuiRuntime.Organisms;
        public string GravityProcessingGroup => _gravityProcessingGroup;
        public TimeSpan MonikerPresentationDuration { get; }
        public event Action<string>? StateChanged;
        public event Action? LivingGuiChanged;

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

        public PageFSM(object? singularityHub = null, Behavior? behavior = null, TimeSpan? monikerPresentationDuration = null)
        {
            Context = new PageStateContext(singularityHub);
            _hub = singularityHub as HubKernel ?? new HubKernel();
            MonikerPresentationDuration = monikerPresentationDuration ?? TimeSpan.FromSeconds(WorkshopPresentationProfile.Current.MonikerDurationSeconds);
            if (MonikerPresentationDuration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(monikerPresentationDuration));

            _processingGroup = $"{ProcessingGroup}:{Guid.NewGuid():N}";
            _gravityProcessingGroup = $"Gravity:{Guid.NewGuid():N}";

            fsm_API.Create.CreateProcessingGroup(_processingGroup);
            fsm_API.Create.CreateProcessingGroup(_gravityProcessingGroup);

            _hub
                .RegisterProcessGroup(_processingGroup)
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
                .State(MonikerReveal, Enter(c =>
                {
                    // This is the handoff beat: the population threshold has been
                    // observed, but Gravity has not yet released the organisms.
                    c.MonikerPresentationStartTick = -1;
                    behavior?.OnMonikerReveal?.Invoke(c);
                }), Tick, null)
                .State(Gravity, Enter(c =>
                {
                    c.MonikerPresentationStartTick = c.TotalTicks;
                    c.MonikerReady = true;
                    c.GravityReleased = false;
                    c.LivingGuiFallen = false;
                    c.FreezeLivingGui();
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
                           context.LivingNodes.Count >= PageStateContext.PopulationObservationThreshold &&
                           context.PopulationCompletedTick >= 0 &&
                           context.TotalTicks > context.PopulationCompletedTick;
                })
                .Transition(MonikerReveal, Gravity, c => ((PageStateContext)c).StateTicks >= GravityReleaseTicks)
                .Transition(Gravity, LivingGuiDissipating, c => ((PageStateContext)c).LivingGuiFallen)
                .Transition(LivingGuiDissipating, NavigationArrival, c =>
                {
                    var context = (PageStateContext)c;
                    return context.NavigationReady ||
                           (context.LivingGuiFallen &&
                            context.MonikerPresentationStartTick >= 0 &&
                            context.TotalTicks - context.MonikerPresentationStartTick >= MonikerPresentationTicks);
                })
                .Transition(NavigationArrival, Running, c => ((PageStateContext)c).StateTicks >= 1)
                .BuildDefinition();

            _handle = fsm_API.Create.CreateInstance("PageFSM", Context, _processingGroup);
            _livingGuiRuntime = new LivingGuiFsm(_hub, Context, _processingGroup);
            _livingGuiRuntime.PopulationChanged += HandleLivingGuiChanged;

            fsm_API.Create.CreateFiniteStateMachine("GravityFSM", -1, _gravityProcessingGroup)
                .State("Falling", onEnter: null, onUpdate: _ => Context.AdvanceGravity(), onExit: null)
                .WithInitialState("Falling")
                .BuildDefinition();

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
            var stateBeforeTick = _handle.CurrentState;

            context.TotalTicks++;
            context.StateTicks++;

            if (stateBeforeTick == LivingGuiPopulating &&
                !context.LivingGuiFrozen)
            {
                _livingGuiRuntime.Update();
            }
            else if (stateBeforeTick == Gravity &&
                     !context.LivingGuiFallen)
            {
                _hub.UpdateProcessGroup(_gravityProcessingGroup);
            }

            StateChanged?.Invoke(stateBeforeTick);
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

        private void HandleLivingGuiChanged() => LivingGuiChanged?.Invoke();
        public void SignalNavigationReady() => Context.NavigationReady = true;

        public void Shutdown()
        {
            if (!_disposed) _handle.TransitionTo(ShutdownState);
        }

        public void Update()
        {
            if (_disposed)
                return;

            _hub.Update();
            StateChanged?.Invoke(CurrentState);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _livingGuiRuntime.PopulationChanged -= HandleLivingGuiChanged;
            _livingGuiRuntime.Dispose();
            fsm_API.Interaction.DestroyFiniteStateMachine("PageFSM", _processingGroup);
            fsm_API.Interaction.DestroyFiniteStateMachine("GravityFSM", _gravityProcessingGroup);
            _disposed = true;
        }
    }
}
