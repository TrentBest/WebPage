using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Compatibility facade over the real page-level FSM.
    /// FSM_API owns progression; this service supplies the application heartbeat
    /// so the page presentation can continue without Razor owning transitions.
    /// </summary>
    public sealed class FSMManagerService : IDisposable
    {
        private const int HeartbeatMilliseconds = 33;

        private readonly CancellationTokenSource _shutdown = new();
        private readonly PeriodicTimer _heartbeat = new(TimeSpan.FromMilliseconds(HeartbeatMilliseconds));
        private readonly Task _heartbeatTask;
        private bool _disposed;

        public PageFSM Page { get; }
        public PageStateContext Context => Page.Context;
        public string CurrentState => Page.CurrentState;
        public string LivingGuiState => Page.LivingGuiState;
        public string LivingGuiActivePhase => Page.LivingGuiActivePhase;

        public event Action<string>? StateChanged
        {
            add => Page.StateChanged += value;
            remove => Page.StateChanged -= value;
        }

        public FSMManagerService()
        {
            Page = new PageFSM();
            _heartbeatTask = RunHeartbeatAsync(_shutdown.Token);
        }

        public void RequestEnter() => Page.RequestEnter();
        public void SignalLivingGuiPopulated() => Page.SignalLivingGuiPopulated();
        public void SignalMonikerReady() => Page.SignalMonikerReady();
        public void SignalGravityReleased() => Page.SignalGravityReleased();
        public void SignalLivingGuiFallen() => Page.SignalLivingGuiFallen();
        public void SignalNavigationReady() => Page.SignalNavigationReady();
        public void Step() => Page.Update();
        public void Shutdown() => Page.Shutdown();

        private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (await _heartbeat.WaitForNextTickAsync(cancellationToken))
                    Page.Update();
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
