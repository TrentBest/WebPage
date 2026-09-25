using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Plant;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Owns the page lifecycle and the first-contact presentation FSM.
/// FSM_COS assembles the semantic composition; the Experience remains responsible
/// for executing the presentation of that composition.
/// </summary>
public sealed class WorkshopExperienceService : IDisposable
{
    public event Action? StateChanged;

    private readonly FirstContactFsm _firstContact = new();
    private readonly IFsmCos _compositionSystem =
        new FsmCos(new WorkshopCompositionCatalog());
    private bool _disposed;

    public string CurrentState { get; private set; } = "Intro";
    public bool IsFirstVisit { get; private set; }
    public bool ShowUnity => false;
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public FirstContactFsm FirstContact => _firstContact;

    /// <summary>Semantic runtime assembly produced for the current WebForge host.</summary>
    public RuntimeAssembly? RuntimeAssembly { get; private set; }

    /// <summary>Semantic Moniker composition assembled by FSM_COS, before presentation executes.</summary>
    public GuiNode? MonikerComposition =>
        RuntimeAssembly?.LoadedBundles
            .OfType<MonikerCompositionBundle>()
            .Select(bundle => bundle.Composition)
            .FirstOrDefault(composition => composition is not null);

    public void Initialize(bool returningVisitor = false)
    {
        if (IsInitialized) return;

        IsInitialized = true;
        IsFirstVisit = !returningVisitor;

        // The first manifest is intentionally tiny: the Moniker is the zeroth
        // composition slot. FSM_COS loads/arbitrates it without executing its
        // presentation Experience.
        RuntimeAssembly = _compositionSystem.Execute(
            new RuntimeManifest(
                RuntimeId: 1UL,
                Bundles: new[]
                {
                    BundleRequest.Unconfigured(MonikerCompositionBundle.BundleId)
                }));

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
        if (_firstContact.CurrentState == "HubGrowth" && _firstContact.StateTicks >= 30)
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