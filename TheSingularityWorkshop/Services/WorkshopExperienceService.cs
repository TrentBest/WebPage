using TheSingularityWorkshop.Workshop.Plant;

namespace TheSingularityWorkshop.Services;

/// <summary>Owns the page lifecycle and the first-contact presentation FSM.</summary>
public sealed class WorkshopExperienceService : IDisposable
{
    public event Action? StateChanged;

    private readonly FirstContactFsm _firstContact = new();
    private bool _disposed;

    public string CurrentState { get; private set; } = "Intro";
    public bool IsFirstVisit { get; private set; }
    public bool ShowUnity => false;
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public FirstContactFsm FirstContact => _firstContact;

    public void Initialize(bool returningVisitor = false)
    {
        if (IsInitialized) return;

        IsInitialized = true;
        IsFirstVisit = !returningVisitor;

        // Every browser launch begins at the authored first-contact boundary.
        // Returning-visitor persistence must not bypass the perception sequence.
        CurrentState = "FirstContact";
        _firstContact.Start();
        StateChanged?.Invoke();
    }

    public void MarkVisited() { }

    public void RequestEntry()
    {
        if (CurrentState != "FirstContact" || _firstContact.CurrentState != "Gateway")
            return;

        SelectedFlexExperience = FlexExperienceCatalog.SelectDefault();
        _firstContact.RequestEntry();
        StateChanged?.Invoke();
    }

    public void Tick()
    {
        if (_disposed || !IsInitialized || CurrentState != "FirstContact") return;

        _firstContact.Update();

        // The Hub is now manifested as a presentation-layer warp rather than
        // the older vine-growth demo. Keep the plant micro-bundle independent so
        // it remains available for future experiences without owning first contact.
        if (_firstContact.CurrentState == "HubGrowth" && _firstContact.StateTicks >= 24)
            _firstContact.SetHubReady();

        if (_firstContact.IsLanding)
            SetState("Intro");
    }

    public void MarkUnityStarted() { }

    public void SetCriticalMassReached() => SetState("CriticalMass");

    public void SetState(string state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        if (CurrentState == state) return;
        CurrentState = state;
        StateChanged?.Invoke();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _firstContact.Dispose();
        _disposed = true;
    }
}