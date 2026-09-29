using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Plant;

namespace TheSingularityWorkshop.Services;

public sealed class WorkshopExperienceService : IDisposable
{
    public event Action? StateChanged;
    private readonly FirstContactFsm _firstContact = new();
    private readonly IFsmCos _compositionSystem = new FsmCos(new WorkshopCompositionCatalog());
    private bool _disposed;

    public string CurrentState { get; private set; } = "Intro";
    public bool IsFirstVisit { get; private set; }
    public bool ShowUnity => false;
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public WorkshopIntegrationMode? IntegrationMode { get; private set; }
    public FirstContactFsm FirstContact => _firstContact;
    public RuntimeAssembly? RuntimeAssembly { get; private set; }

    public GuiNode? ShellComposition =>
        RuntimeAssembly?.TryGetBundle<WorkshopShellCompositionBundle>(
            WorkshopShellCompositionBundle.BundleId,
            out var shell) == true
            ? shell!.Composition
            : null;

    public GuiNode? MonikerComposition =>
        RuntimeAssembly?.TryGetBundle<MonikerCompositionBundle>(
            MonikerCompositionBundle.BundleId,
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
        if (IsInitialized)
            return;

        IsInitialized = true;
        IsFirstVisit = !returningVisitor;
        CurrentState = "FirstContact";
        _firstContact.Start();
        StateChanged?.Invoke();
    }

    public void MarkVisited()
    {
    }

    public void RequestEntry()
    {
        if (CurrentState != "FirstContact" || _firstContact.CurrentState != "Gateway")
            return;

        _firstContact.RequestEntry();
        StateChanged?.Invoke();
    }

    public void SelectIntegration(WorkshopIntegrationMode mode)
    {
        if (CurrentState != "FirstContact" || !_firstContact.IsIntegrationChoice)
            return;

        IntegrationMode = mode;

        if (mode == WorkshopIntegrationMode.FsmCos)
        {
            RuntimeAssembly = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: 1UL,
                Bundles: new[]
                {
                    BundleRequest.Unconfigured(WorkshopShellCompositionBundle.BundleId),
                    BundleRequest.Unconfigured(MonikerCompositionBundle.BundleId),
                    BundleRequest.Unconfigured(AiExchangeCompositionBundle.BundleId)
                }));
        }
        else
        {
            RuntimeAssembly = null;
        }

        SelectedFlexExperience = FlexExperienceCatalog.SelectDefault();
        _firstContact.SelectIntegration();
        StateChanged?.Invoke();
    }

    public void Tick()
    {
        if (_disposed || !IsInitialized || CurrentState != "FirstContact")
            return;

        _firstContact.Update();

        if (_firstContact.CurrentState == "HubGrowth" && _firstContact.StateTicks >= 30)
            _firstContact.SetHubReady();

        if (_firstContact.IsLanding)
            SetState("Intro");
    }

    public void MarkUnityStarted()
    {
    }

    public void SetCriticalMassReached() => SetState("CriticalMass");

    public void SetState(string state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);

        if (CurrentState == state)
            return;

        CurrentState = state;
        StateChanged?.Invoke();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _firstContact.Dispose();
        _disposed = true;
    }
}
