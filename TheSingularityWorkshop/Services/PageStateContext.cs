using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// Runtime data carried by the page-level FSM.
    /// The context is intentionally small: the FSM owns progression; this object only carries state data and signals.
    /// </summary>
    public sealed class PageStateContext : IStateContext
    {
        public string Name { get; set; } = "PageFSMContext";
        public bool IsValid { get; set; } = true;

        /// <summary>Reference to the lightweight Singularity Hub owned by the host application.</summary>
        public object? SingularityHub { get; }

        /// <summary>Set when the visitor activates the gateway.</summary>
        public bool EnterRequested { get; set; }

        /// <summary>Set when the Living GUI has finished materializing its architecture.</summary>
        public bool LivingGuiPopulated { get; set; }

        /// <summary>Set when the moniker reveal transition is visually ready.</summary>
        public bool MonikerReady { get; set; }

        /// <summary>Set when gravity is allowed to release the Living GUI.</summary>
        public bool GravityReleased { get; set; }

        /// <summary>Set after the final Living GUI element has fallen away.</summary>
        public bool LivingGuiFallen { get; set; }

        /// <summary>Number of FSM update ticks spent in the current state.</summary>
        public long StateTicks { get; set; }

        /// <summary>Number of FSM update ticks since page initialization.</summary>
        public long TotalTicks { get; set; }

        public PageStateContext(object? singularityHub = null)
        {
            SingularityHub = singularityHub;
        }

        public void ResetStateClock() => StateTicks = 0;
    }
}