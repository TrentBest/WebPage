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
        // The moniker is the startup presentation of the actual Living GUI Flex.
        // Keep the presentation phase explicit; MainLayout releases it after the
        // configured window and advances the host into the selected experience.
        SelectedFlexExperience = FlexExperienceCatalog.Available.Single(x => x.Id == "living-gui");
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
