using System;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Compatibility facade over the real page-level FSM.
    /// The FSM_API owns progression; this service only exposes the live PageFSM to UI consumers.
    /// </summary>
    public sealed class FSMManagerService : IDisposable
    {
        public PageFSM Page { get; }

        public PageStateContext Context => Page.Context;

        public string CurrentState => Page.CurrentState;

        public event Action<string>? StateChanged
        {
            add => Page.StateChanged += value;
            remove => Page.StateChanged -= value;
        }

        public FSMManagerService()
        {
            Page = new PageFSM();
        }

        public void RequestEnter() => Page.RequestEnter();

        public void SignalLivingGuiPopulated() => Page.SignalLivingGuiPopulated();

        public void SignalMonikerReady() => Page.SignalMonikerReady();

        public void SignalGravityReleased() => Page.SignalGravityReleased();

        public void SignalLivingGuiFallen() => Page.SignalLivingGuiFallen();

        public void Step() => Page.Update();

        public void Shutdown() => Page.Shutdown();

        public void Dispose() => Page.Dispose();
    }
}