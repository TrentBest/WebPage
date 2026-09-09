using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Owns the state machine for the Workshop's first-contact experience.
/// The UI renders the current state; it does not decide the lifecycle itself.
/// </summary>
public sealed class WorkshopExperienceService
{
    private const string ProcessingGroup = "WorkshopExperience";
    private const string FsmName = "WorkshopExperience";

    private WorkshopExperienceContext? _context;
    private FSMHandle? _handle;
    private bool _initialized;

    public event Action? StateChanged;

    public string CurrentState => _handle?.CurrentState ?? "Uninitialized";
    public bool IsInitialized => _initialized;
    public bool IsFirstVisit => _context?.IsFirstVisit ?? true;
    public bool ShowNavigation => CurrentState == "Navigation";
    public bool ShowUnity => CurrentState is "Unity" or "Navigation";

    public void Initialize(bool returningVisitor)
    {
        if (_initialized)
        {
            return;
        }

        _context = new WorkshopExperienceContext
        {
            IsFirstVisit = !returningVisitor,
            IsValid = true
        };

        FSM_API.FSM_API.Create.CreateFiniteStateMachine(FsmName, -1, ProcessingGroup)
            .State("Intro", OnIntroEnter, OnStateUpdate, OnIntroExit)
            .State("Living", OnLivingEnter, OnStateUpdate, OnLivingExit)
            .State("Unity", OnUnityEnter, OnStateUpdate, OnUnityExit)
            .State("Navigation", OnNavigationEnter, OnStateUpdate, OnNavigationExit)
            .WithInitialState(returningVisitor ? "Navigation" : "Intro")
            .Transition("Intro", "Living", c => ((WorkshopExperienceContext)c).EnterRequested)
            .Transition("Living", "Unity", c => ((WorkshopExperienceContext)c).CriticalMassReached)
            .Transition("Unity", "Navigation", c => ((WorkshopExperienceContext)c).UnityStarted && ((WorkshopExperienceContext)c).ElapsedMilliseconds >= 3000)
            .BuildDefinition();

        _handle = FSM_API.FSM_API.Create.CreateInstance(FsmName, _context, ProcessingGroup);
        _initialized = true;
        StateChanged?.Invoke();
    }

    public void RequestEntry()
    {
        if (_context is null) return;
        _context.EnterRequested = true;
    }

    public void SetCriticalMassReached()
    {
        if (_context is null) return;
        _context.CriticalMassReached = true;
    }

    public void MarkUnityStarted()
    {
        if (_context is null) return;
        _context.UnityStarted = true;
    }

    public void Tick()
    {
        if (!_initialized || _context is null) return;

        _context.ElapsedMilliseconds += 50;
        FSM_API.FSM_API.Interaction.Update(ProcessingGroup);
        StateChanged?.Invoke();
    }

    private static void OnIntroEnter(IStateContext context) { }
    private static void OnIntroExit(IStateContext context) { }
    private static void OnLivingEnter(IStateContext context) => ((WorkshopExperienceContext)context).ElapsedMilliseconds = 0;
    private static void OnLivingExit(IStateContext context) { }
    private static void OnUnityEnter(IStateContext context) => ((WorkshopExperienceContext)context).ElapsedMilliseconds = 0;
    private static void OnUnityExit(IStateContext context) { }
    private static void OnNavigationEnter(IStateContext context) { }
    private static void OnNavigationExit(IStateContext context) { }
    private static void OnStateUpdate(IStateContext context) { }

    private sealed class WorkshopExperienceContext : IStateContext
    {
        public string Name { get; set; } = FsmName;
        public bool IsValid { get; set; }
        public bool IsFirstVisit { get; set; }
        public bool EnterRequested { get; set; }
        public bool CriticalMassReached { get; set; }
        public bool UnityStarted { get; set; }
        public int ElapsedMilliseconds { get; set; }
    }
}
