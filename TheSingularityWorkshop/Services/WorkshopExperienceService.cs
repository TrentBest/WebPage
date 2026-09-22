using TheSingularityWorkshop.Workshop.Plant;

namespace TheSingularityWorkshop.Services;

/// <summary>Owns the page lifecycle and the first-contact presentation FSM.</summary>
public sealed class WorkshopExperienceService : IDisposable
{
    public event Action? StateChanged;

    private readonly FirstContactFsm _firstContact = new();
    private PlantGrowthMicroBundle? _plant;
    private bool _disposed;

    public string CurrentState { get; private set; } = "Intro";
    public bool IsFirstVisit { get; private set; }
    public bool ShowUnity => false;
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public FirstContactFsm FirstContact => _firstContact;
    public PlantGrowthMicroBundle? Plant => _plant;

    public void Initialize(bool returningVisitor = false)
    {
        if (IsInitialized) return;

        IsInitialized = true;
        IsFirstVisit = !returningVisitor;

        if (IsFirstVisit)
        {
            CurrentState = "FirstContact";
            _firstContact.Start();
            StateChanged?.Invoke();
        }
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

        if (_firstContact.CurrentState == "HubGrowth" && _plant is null)
        {
            _plant = new PlantGrowthMicroBundle();
            _plant.Start();
        }

        _plant?.Update();

        if (_plant?.IsSettled == true)
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
        _plant?.Dispose();
        _firstContact.Dispose();
        _disposed = true;
    }
}