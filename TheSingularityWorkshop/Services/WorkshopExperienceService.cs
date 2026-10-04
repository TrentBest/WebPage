using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Experiences;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Services;

public sealed class WorkshopExperienceService : IDisposable
{
    public event Action? StateChanged;
    private readonly FirstContactFsm _firstContact = new();
    private readonly IFsmCos _compositionSystem = new FsmCos(new WorkshopCompositionCatalog());
    private bool _disposed;
    private bool _entryRequested;
    private bool _experienceComposed;

    public string CurrentState { get; private set; } = "Intro";
    public bool IsFirstVisit { get; private set; }
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public IExperience? SelectedExperience { get; private set; }
    public FirstContactFsm FirstContact => _firstContact;
    public RuntimeAssembly? RuntimeAssembly { get; private set; }

    public GuiNode? MonikerComposition =>
        RuntimeAssembly?.TryGetBundle<MonikerMicroBundle>(
            (ulong)MonikerMicroBundle.BundleId,
            out var moniker) == true
            ? moniker!.Composition
            : null;

    public AiExchangeCompositionBundle? AiExchangeComposition =>
        RuntimeAssembly?.TryGetBundle<AiExchangeCompositionBundle>(
            AiExchangeCompositionBundle.BundleId,
            out var ai) == true
            ? ai
            : null;

    public void Initialize(bool returningVisitor = false)
    {
        if (IsInitialized) return;

        IsInitialized = true;
        IsFirstVisit = !returningVisitor;
        CurrentState = "FirstContact";
        _firstContact.Start();
        StateChanged?.Invoke();
    }

    public void MarkVisited() { }

    /// <summary>
    /// Records the visitor's explicit permission to leave first contact.
    /// The actual Experience is not composed until the first-contact FSM reaches Landing.
    /// </summary>
    public void RequestEntry()
    {
        if (CurrentState != "FirstContact" || _firstContact.CurrentState != "Gateway") return;

        SelectedFlexExperience = FlexExperienceCatalog.SelectDefault();
        SelectedExperience = new LivingGuiExperience();
        _entryRequested = true;
        _firstContact.RequestEntry();
        StateChanged?.Invoke();
    }

    public void Tick()
    {
        if (_disposed || !IsInitialized || CurrentState != "FirstContact") return;

        _firstContact.Update();

        if (_firstContact.IsLanding)
            ActivateSelectedExperience();
    }

    private void ActivateSelectedExperience()
    {
        if (!_entryRequested || _experienceComposed || SelectedExperience is null)
            return;

        SetState("ExperienceLoading");

        // The Experience asks for its Living GUI MicroBundle. FSM_COS resolves
        // that bundle's Moniker dependency before loading the Experience capability.
        RuntimeAssembly = _compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: SelectedExperience.Id,
            Bundles: SelectedExperience.MicroBundleIds
                .Select(BundleRequest.Unconfigured)
                .ToArray()));

        _experienceComposed = RuntimeAssembly.TryGetBundle<LivingGuiExperienceMicroBundle>(
            LivingGuiExperienceMicroBundle.BundleId,
            out _);

        if (!_experienceComposed)
        {
            SetState("FirstContact");
            throw new InvalidOperationException(
                "The Living GUI Experience could not be composed by FSM_COS.");
        }

        SetState("LivingGui");
    }

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
