using System;
using System.Threading;
using System.Threading.Tasks;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Compatibility facade over the real page-level FSM.
    /// FSM_API owns progression; the application heartbeat advances the page FSM,
    /// while the session-scoped SingularityHub instance owns the process-group scheduler.
    /// </summary>
    public sealed class FSMManagerService : IDisposable
    {
        private const int HeartbeatMilliseconds = 33;

        private readonly CancellationTokenSource _shutdown = new();
        private readonly PeriodicTimer _heartbeat = new(TimeSpan.FromMilliseconds(HeartbeatMilliseconds));
        private readonly Task _heartbeatTask;
        private bool _disposed;
        private IExperienceExecutionRuntime _executionRuntime = default!;

        public PageFSM Page { get; }
        public IExperienceExecutionRuntime ExecutionRuntime => Volatile.Read(ref _executionRuntime);
        public PageStateContext Context => Page.Context;
        public string CurrentState => Page.CurrentState;
        public string LivingGuiState => Page.LivingGuiState;
        public string LivingGuiActivePhase => Page.LivingGuiActivePhase;
        public bool LivingGuiIsValid => Page.LivingGuiIsValid;
        public TimeSpan MonikerPresentationDuration => Page.MonikerPresentationDuration;
        /// <summary>Point-in-time observation of the canonical page/Living GUI runtime.</summary>
        public PageFSM.RuntimeObservation Observation => ExecutionRuntime.Observe();

        public event Action<string>? StateChanged
        {
            add => Page.StateChanged += value;
            remove => Page.StateChanged -= value;
        }

        public event Action? LivingGuiChanged
        {
            add => Page.LivingGuiChanged += value;
            remove => Page.LivingGuiChanged -= value;
        }

        /// <summary>
        /// Creates the page FSM against the application's registered Hub.
        /// This is the critical composition boundary: PageFSM and LivingGuiFsm must
        /// schedule through the same Hub instance that the host registered.
        /// </summary>
        public FSMManagerService(HubKernel hub)
        {
            Page = new PageFSM(hub);
            _executionRuntime = new PageFsmExecutionRuntime(Page);
            _heartbeatTask = RunHeartbeatAsync(_shutdown.Token);
        }

        /// <summary>Prepares a manifest-bound adapter without changing the active runtime.</summary>
        public IExperienceExecutionRuntime PrepareManifestExecution(IExperience experience, RuntimeAssembly assembly)
        {
            ArgumentNullException.ThrowIfNull(experience);
            ArgumentNullException.ThrowIfNull(assembly);
            return new PageFsmExecutionRuntime(Page, experience, assembly);
        }

        /// <summary>Routes future ticks and observations through a validated Experience adapter.</summary>
        public void ActivateExecutionRuntime(IExperienceExecutionRuntime runtime)
        {
            ArgumentNullException.ThrowIfNull(runtime);
            if (!ReferenceEquals(runtime.Page, Page))
                throw new InvalidOperationException("The execution adapter must preserve the active PageFSM instance.");
            if (!runtime.IsManifestBound)
                throw new InvalidOperationException("Only a validated manifest-bound Experience runtime can be activated.");

            Volatile.Write(ref _executionRuntime, runtime);
        }

        public void RequestEnter() => Page.RequestEnter();
        public void SignalLivingGuiPopulated() => Page.SignalLivingGuiPopulated();
        public void SignalMonikerReady() => Page.SignalMonikerReady();
        public void SignalGravityReleased() => Page.SignalGravityReleased();
        public void SignalLivingGuiFallen() => Page.SignalLivingGuiFallen();
        public void SignalNavigationReady() => Page.SignalNavigationReady();
        public void Step() => ExecutionRuntime.Tick();
        public void Shutdown() => Page.Shutdown();

        private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (await _heartbeat.WaitForNextTickAsync(cancellationToken))
                    ExecutionRuntime.Tick();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Normal service shutdown.
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _shutdown.Cancel();
            _heartbeat.Dispose();
            Page.Dispose();
            _shutdown.Dispose();
        }
    }
}
