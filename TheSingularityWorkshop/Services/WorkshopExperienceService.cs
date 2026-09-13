namespace TheSingularityWorkshop.Services;

/// <summary>Owns host lifecycle state without stepping an Experience.</summary>
public sealed class WorkshopExperienceService
{
    public event Action? StateChanged;

    public string CurrentState { get; private set; } = "Intro";
    public bool ShowUnity => CurrentState == "Unity";
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }

    public void Initialize(bool returningVisitor = false)
    {
        if (IsInitialized) return;
        IsInitialized = true;
        if (returningVisitor) MarkUnityStarted();
    }

    public void MarkVisited() { }

    public void RequestEntry()
    {
        SelectedFlexExperience = FlexExperienceCatalog.Select(Random.Shared);
        SetState("FlexHello");
    }

    public void Tick() { }

    public void MarkUnityStarted() => SetState("Unity");

    public void SetCriticalMassReached() => SetState("CriticalMass");

    public void SetState(string state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        if (CurrentState == state) return;
        CurrentState = state;
        StateChanged?.Invoke();
    }
}
