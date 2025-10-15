using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services
{
    /// <summary>
    /// A concrete implementation of IStateContext to hold webpage-specific data.
    /// This is the 'data bag' for your FSM in the UI.
    /// </summary>
    public class PageStateContext : IStateContext
    {
        public string Name { get; set; } = "WebpageFSMContext";
        public bool IsValid { get; set; } = true;

        // Example data point to track state in the UI
        public int ClickCount { get; set; } = 0;
        public string Message { get; set; } = "System Idle";
        public bool NavigationVisible { get; set; } = false;
        public bool CoreTabsStaged { get; set; } = false;
        public bool AllTabsComplete { get; set; } = false;
    }
}