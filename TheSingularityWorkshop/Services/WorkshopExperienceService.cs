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
    private bool _startupPrepared;
    private bool _preloadPrepared;
    private Stopwatch? _monikerClock;

    public event Action? StateChanged;

    public string CurrentState { get; private set; } = "ManifestLoading";
    public bool IsFirstVisit { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsDeepDive { get; private set; }
    public WebPageHostManifest? Manifest { get; private set; }
    public WebPageManifestExperience? CurrentManifestExperience { get; private set; }
    public WebPageManifestExperience? PreloadedManifestExperience { get; private set; }
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

        CurrentManifestExperience = Manifest.Startup[0];
        ComposeCurrentStartup();
        PrepareNextStartup();

        _monikerClock = Stopwatch.StartNew();
        SetState(CurrentManifestExperience.Kind.Equals("Moniker", StringComparison.OrdinalIgnoreCase)
            ? "Moniker"
            : CurrentManifestExperience.Kind);

        _manifestLoaded = true;
    }

    private void ComposeCurrentStartup()
    {
        if (CurrentManifestExperience is null || CurrentManifestExperience.MicroBundleIds.Count == 0)
            return;

        RuntimeAssembly = _compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 1,
            Bundles: CurrentManifestExperience.MicroBundleIds
                .Select(BundleRequest.Unconfigured)
                .ToArray()));

        _startupPrepared = RuntimeAssembly.TryGetBundle<MonikerMicroBundle>(
            (ulong)MonikerMicroBundle.BundleId,
            out var moniker);
        if (_startupPrepared && moniker is TheSingularityWorkshop.SingularityHub.IMicroBundle hubBundle)
            _hubRuntime.Hub.LoadBundle(hubBundle);
        if (!_startupPrepared)
            throw new InvalidOperationException("The manifest-selected Moniker could not be composed by FSM_COS.");
    }

    private void PrepareNextStartup()
    {
        if (Manifest is null || Manifest.Startup.Count < 2)
            return;

        PreloadedManifestExperience = Manifest.Startup[1];

        // The Hub is the WebPage's Experience, so an empty MicroBundle list means
        // the host itself is already the next executable surface. For a future
        // Experience, its MicroBundles are composed here while the Moniker presents.
        if (PreloadedManifestExperience.MicroBundleIds.Count == 0)
        {
            _preloadPrepared = true;
            return;
        }

        var preloadAssembly = _compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 2,
            Bundles: PreloadedManifestExperience.MicroBundleIds
                .Select(BundleRequest.Unconfigured)
                .ToArray()));

        _preloadPrepared = preloadAssembly.Bundles.Count > 0;
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
            SetState("Hub");
        }
    }

    public void RequestEntry()
    {
        if (CurrentState.Equals("Moniker", StringComparison.OrdinalIgnoreCase))
        {
            _monikerClock?.Stop();
            SetState("Hub");
        }
    }

    public async Task RestartFromManifestAsync(bool deepDive)
    {
        if (Manifest is null)
            await InitializeAsync();

        IsDeepDive = deepDive;
        _monikerClock?.Restart();
        CurrentManifestExperience = Manifest!.Startup[0];
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
