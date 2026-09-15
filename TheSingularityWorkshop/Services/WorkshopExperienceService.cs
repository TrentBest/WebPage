namespace TheSingularityWorkshop.Services;

/// <summary>Owns host lifecycle state without stepping an Experience.</summary>
public sealed class WorkshopExperienceService
{
    public event Action? StateChanged;

    public string CurrentState { get; private set; } = "Intro";
    public bool ShowUnity => false;
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }

    public void Initialize(bool returningVisitor = false)
    {
        if (IsInitialized) return;
        IsInitialized = true;
    }

    public void MarkVisited() { }

    public void RequestEntry()
    {
        SelectedFlexExperience = FlexExperienceCatalog.SelectDefault();
        SetState("FlexHello");
    }

    public void Tick() { }

    public void MarkUnityStarted() { }

    public void SetCriticalMassReached() => SetState("CriticalMass");

    public void SetState(string state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        if (CurrentState == state) return;
        CurrentState = state;
        StateChanged?.Invoke();
    }
}
