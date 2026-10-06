using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Experiences;
using TheSingularityWorkshop.Workshop.MicroBundles;
using TheSingularityWorkshop.Workshop.Configuration;

namespace TheSingularityWorkshop.Services;

public sealed class WorkshopExperienceService : IDisposable
{
    public event Action? StateChanged;
    private readonly FirstContactFsm _firstContact = new();
    private readonly IFsmCos _compositionSystem = new FsmCos(new WorkshopCompositionCatalog());
    private readonly WorkshopExperienceCatalog _experienceCatalog;
    private readonly WorkshopManifestStore _manifestStore;

    public WorkshopExperienceService(WorkshopExperienceCatalog? experienceCatalog = null, WorkshopManifestStore? manifestStore = null)
    {
        _experienceCatalog = experienceCatalog ?? new WorkshopExperienceCatalog();
        _manifestStore = manifestStore ?? new WorkshopManifestStore();
    }
    private bool _disposed;
    private bool _entryRequested;
    private bool _experienceComposed;

    public string CurrentState { get; private set; } = "Intro";
    public bool IsFirstVisit { get; private set; }
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public IExperience? SelectedExperience { get; private set; }
    public IReadOnlyList<IExperience> LoadedExperiences { get; private set; } = Array.Empty<IExperience>();
    public WorkshopManifestDocument Manifest => _manifestStore.Manifest;
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
        SelectedExperience = ResolveRunningExperience();
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

        var experienceIds = Manifest.Experiences.Startup
            .Concat(Manifest.Experiences.Transitioning)
            .Concat(Manifest.Experiences.Running)
            .Distinct()
            .ToArray();

        var experiences = experienceIds
            .Select(ResolveExperience)
            .ToArray();

        LoadedExperiences = experiences;

        var bundleIds = experiences
            .SelectMany(experience => experience.MicroBundleIds)
            .Distinct()
            .Select(BundleRequest.Unconfigured)
            .ToArray();

        RuntimeAssembly = _compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: Manifest.RuntimeId,
            Bundles: bundleIds));

        _experienceComposed = experiences.All(experience =>
            experience.MicroBundleIds.All(bundleId => RuntimeAssembly.Bundles.Any(bundle => bundle.Id == bundleId)));

        if (!_experienceComposed || !Manifest.Experiences.RequiredCapabilities.All(capability =>
                experiences.Any(experience => experience.Capabilities.Contains(capability))))
        {
            SetState("FirstContact");
            throw new InvalidOperationException(
                "The configured Workshop Experiences could not be composed with the required capabilities.");
        }

        SetState("LivingGui");
    }

    private IExperience ResolveRunningExperience()
    {
        var experienceId = Manifest.Experiences.Running.FirstOrDefault();
        if (experienceId == 0)
            throw new InvalidOperationException("The Workshop manifest does not declare a running Experience.");

        return ResolveExperience(experienceId);
    }

    private IExperience ResolveExperience(ulong experienceId)
    {
        if (_experienceCatalog.TryResolve(experienceId, out var experience) && experience is not null)
            return experience;

        throw new InvalidOperationException(
            $"The Workshop manifest references unknown Experience {experienceId}.");
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
