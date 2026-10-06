using System.Diagnostics;
using System.Net.Http.Json;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Infrastructure.Hub;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Experiences;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Services;

public sealed class WorkshopExperienceService : IDisposable
{
    private const string ManifestPath = "Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json";

    private readonly HttpClient _httpClient;
    private readonly FirstContactFsm _firstContact = new();
    private readonly IFsmCos _compositionSystem = new FsmCos(new WorkshopCompositionCatalog());
    private readonly HubRuntime _hubRuntime;
    private bool _disposed;
    private bool _manifestLoaded;
    private Stopwatch? _monikerClock;

    public event Action? StateChanged;

    public string CurrentState { get; private set; } = "ManifestLoading";
    public bool IsFirstVisit { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsDeepDive { get; private set; }
    public WebPageHostManifest? Manifest { get; private set; }
    public WebPageManifestExperience? CurrentManifestExperience { get; private set; }
    public WebPageManifestExperience? PrimaryManifestExperience { get; private set; }
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

    public LivingGuiExperienceMicroBundle? PrimaryExperienceComposition =>
        RuntimeAssembly?.TryGetBundle<LivingGuiExperienceMicroBundle>(
            (ulong)LivingGuiExperienceMicroBundle.BundleId,
            out var primary) == true
            ? primary
            : null;

    public AiExchangeCompositionBundle? AiExchangeComposition =>
        RuntimeAssembly?.TryGetBundle<AiExchangeCompositionBundle>(
            AiExchangeCompositionBundle.BundleId,
            out var ai) == true
            ? ai
            : null;

    public WorkshopExperienceService(HttpClient httpClient, HubRuntime hubRuntime)
    {
        _httpClient = httpClient;
        _hubRuntime = hubRuntime;
    }

    public async Task InitializeAsync(bool returningVisitor = false)
    {
        if (IsInitialized)
            return;

        IsInitialized = true;
        IsFirstVisit = !returningVisitor;
        SetState("ManifestLoading");

        Manifest = await _httpClient.GetFromJsonAsync<WebPageHostManifest>(ManifestPath)
            ?? throw new InvalidOperationException("The WebPage host manifest could not be loaded.");

        if (Manifest.Startup.Count == 0)
            throw new InvalidOperationException("The WebPage host manifest contains no startup Experience.");

        if (Manifest.Running.Count == 0)
            throw new InvalidOperationException("The WebPage host manifest contains no primary running Experience.");

        CurrentManifestExperience = Manifest.Startup[0];
        PrimaryManifestExperience = Manifest.Running[0];

        ComposePrimaryExperience();

        _monikerClock = Stopwatch.StartNew();
        SetState("Moniker");
        _manifestLoaded = true;
    }

    private void ComposePrimaryExperience()
    {
        if (PrimaryManifestExperience is null || PrimaryManifestExperience.MicroBundleIds.Count == 0)
            throw new InvalidOperationException("The manifest-selected primary Experience has no MicroBundles.");

        // The manifest supplies only the primary Experience root. Its MicroBundle
        // declares the canonical Moniker dependency, so FSM_COS closes the graph.
        RuntimeAssembly = _compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 1,
            Bundles: PrimaryManifestExperience.MicroBundleIds
                .Select(BundleRequest.Unconfigured)
                .ToArray()));

        var hasMoniker = RuntimeAssembly.TryGetBundle<MonikerMicroBundle>(
            (ulong)MonikerMicroBundle.BundleId,
            out _);
        var hasPrimary = RuntimeAssembly.TryGetBundle<LivingGuiExperienceMicroBundle>(
            LivingGuiExperienceMicroBundle.BundleId,
            out _);

        if (!hasMoniker || !hasPrimary)
            throw new InvalidOperationException(
                "FSM_COS did not compose the manifest-selected primary Experience and its canonical Moniker dependency.");

        if (RuntimeAssembly.TryGetBundle<MonikerMicroBundle>(
                (ulong)MonikerMicroBundle.BundleId,
                out var moniker) &&
            moniker is TheSingularityWorkshop.SingularityHub.IMicroBundle hubBundle)
        {
            _hubRuntime.Hub.LoadBundle(hubBundle);
        }

        _hubRuntime.ArbitrateManifest();
    }

    public void Tick()
    {
        if (_disposed || !IsInitialized || !_manifestLoaded)
            return;

        if (CurrentState.Equals("Moniker", StringComparison.OrdinalIgnoreCase) &&
            _monikerClock is not null &&
            CurrentManifestExperience is not null &&
            _monikerClock.Elapsed >= TimeSpan.FromSeconds(CurrentManifestExperience.PresentationSeconds))
        {
            _monikerClock.Stop();
            ActivatePrimaryExperience();
        }
    }

    public void RequestEntry()
    {
        if (CurrentState.Equals("Moniker", StringComparison.OrdinalIgnoreCase))
        {
            _monikerClock?.Stop();
            ActivatePrimaryExperience();
        }
    }

    private void ActivatePrimaryExperience()
    {
        if (PrimaryManifestExperience is null)
            throw new InvalidOperationException("No primary Experience is available in the WebPage manifest.");

        SetState("LivingGui");
    }

    public async Task RestartFromManifestAsync(bool deepDive)
    {
        if (Manifest is null)
            await InitializeAsync();

        IsDeepDive = deepDive;
        _monikerClock?.Restart();
        CurrentManifestExperience = Manifest!.Startup[0];
        PrimaryManifestExperience = Manifest.Running[0];
        SetState("Moniker");
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