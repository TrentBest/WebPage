namespace TheSingularityWorkshop.Services;

/// <summary>
/// Owns the host-level lifecycle state used by the Workshop landing presentation.
/// The service deliberately does not render or step an Experience; it only exposes
/// the current host phase and the selected Flex experience.
/// </summary>
public sealed class WorkshopExperienceService
{
    /// <summary>Raised when the host lifecycle state changes.</summary>
    public event Action? StateChanged;

    /// <summary>Gets the current host lifecycle state.</summary>
    public string CurrentState { get; private set; } = "Intro";

    /// <summary>Gets whether the Unity layer should currently be visible.</summary>
    public bool ShowUnity => CurrentState == "Unity";

    /// <summary>Gets the currently selected Flex experience.</summary>
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }

    /// <summary>Marks the Workshop as visited without changing presentation state.</summary>
    public void MarkVisited()
    {
    }

    /// <summary>
    /// Initializes the host lifecycle. Initialization is intentionally idempotent so
    /// Blazor rendering can safely call it more than once.
    /// </summary>
    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Enters the current Flex experience. The first Flex is the intentionally simple
    /// moniker Hello World experience; richer experiences can be added to the catalog.
    /// </summary>
    public void RequestEntry()
    {
        SelectedFlexExperience = FlexExperienceCatalog.Select(new Random());
        SetState("Flex");
    }

    /// <summary>Transitions to a named host lifecycle state.</summary>
    public void SetState(string state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);

        if (CurrentState == state)
        {
            return;
        }

        CurrentState = state;
        StateChanged?.Invoke();
    }
}
