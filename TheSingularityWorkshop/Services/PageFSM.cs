using System;
using TheSingularityWorkshop.FSM_API;

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
        public const string Shutdown = "SHUTDOWN";

        private readonly FSMHandle _handle;
        private bool _disposed;

        public PageStateContext Context { get; }
        public string CurrentState => _handle.CurrentState;
        public event Action<string>? StateChanged;

        /// <summary>Behavior hooks supplied by the host instead of being hardwired into the FSM.</summary>
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
            FSM_API.Create.CreateProcessingGroup(ProcessingGroup);

            FSM_API.Create.CreateFiniteStateMachine("PageFSM", -1, ProcessingGroup)
                .State(Initializing, c => behavior?.OnInitialization?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(Gateway, c => behavior?.OnGateway?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(GatewayExit, c => behavior?.OnGatewayExit?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(LivingGuiIgnition, c => behavior?.OnLivingGuiIgnition?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(LivingGuiPopulating, c => behavior?.OnLivingGuiPopulating?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(MonikerReveal, c => behavior?.OnMonikerReveal?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(Gravity, c => behavior?.OnGravity?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(LivingGuiDissipating, c => behavior?.OnLivingGuiDissipating?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(NavigationArrival, c => behavior?.OnNavigationArrival?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(Running, c => behavior?.OnRunning?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .State(Shutdown, c => behavior?.OnShutdown?.Invoke((PageStateContext)c), c => Tick((PageStateContext)c), null)
                .WithInitialState(Initializing)
                .Transition(Initializing, Gateway, c => true)
                .Transition(Gateway, GatewayExit, c => ((PageStateContext)c).EnterRequested)
                .Transition(GatewayExit, LivingGuiIgnition, c => ((PageStateContext)c).StateTicks >= 1)
                .Transition(LivingGuiIgnition, LivingGuiPopulating, c => ((PageStateContext)c).StateTicks >= 1)
                .Transition(LivingGuiPopulating, MonikerReveal, c => ((PageStateContext)c).LivingGuiPopulated)
                .Transition(MonikerReveal, Gravity, c => ((PageStateContext)c).MonikerReady)
                .Transition(Gravity, LivingGuiDissipating, c => ((PageStateContext)c).GravityReleased)
                .Transition(LivingGuiDissipating, NavigationArrival, c => ((PageStateContext)c).LivingGuiFallen)
                .Transition(NavigationArrival, Running, c => ((PageStateContext)c).StateTicks >= 1)
                .BuildDefinition();

            _handle = FSM_API.Create.CreateInstance("PageFSM", Context, ProcessingGroup);
        }

        private void Tick(PageStateContext context)
        {
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

        public void Shutdown()
        {
            if (!_disposed) _handle.TransitionTo(Shutdown);
        }

        public void Update()
        {
            if (_disposed) return;
            FSM_API.Interaction.Update(ProcessingGroup);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}