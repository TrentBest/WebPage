using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Minimal runtime data carried by the page-level FSM.
    /// </summary>
    public sealed class PageStateContext : IStateContext
    {
        public string Name { get; set; } = "PageFSMContext";
        public bool IsValid { get; set; } = true;
        public object? SingularityHub { get; }

        public bool EnterRequested { get; set; }
        public bool LivingGuiPopulated { get; set; }
        public bool MonikerReady { get; set; }
        public bool GravityReleased { get; set; }
        public bool LivingGuiFallen { get; set; }
        public bool NavigationReady { get; set; }

        public long StateTicks { get; set; }
        public long TotalTicks { get; set; }

        public PageStateContext(object? singularityHub = null)
        {
            SingularityHub = singularityHub;
        }

        public void ResetStateClock() => StateTicks = 0;
    }
}