using System.Net.Http.Json;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
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
    private readonly IFsmCos _compositionSystem = new FsmCos(new WorkshopCompositionCatalog());
    private readonly HubRuntime _hubRuntime;
    private bool _disposed;

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
    public RuntimeAssembly? RuntimeAssembly { get; private set; }

    private readonly Dictionary<string, RuntimeAssembly> _hubRuntimeAssemblies =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeAssembly> _capabilityRuntimeAssemblies =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>FSM_COS assemblies for the manifest-declared hub destinations.</summary>
    public IReadOnlyDictionary<string, RuntimeAssembly> HubRuntimeAssemblies => _hubRuntimeAssemblies;

    /// <summary>FSM_COS assemblies for supporting capabilities that are not top-level hub destinations.</summary>
    public IReadOnlyDictionary<string, RuntimeAssembly> CapabilityRuntimeAssemblies => _capabilityRuntimeAssemblies;

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

        var manifestJson = await _httpClient.GetStringAsync(ManifestPath);
        Manifest = WebPageHostManifestParser.Parse(manifestJson);

        if (Manifest.Startup.Count == 0)
            throw new InvalidOperationException("The WebPage host manifest contains no startup Experience.");

        if (Manifest.Running.Count == 0)
            throw new InvalidOperationException("The WebPage host manifest contains no primary running Experience.");

        CurrentManifestExperience = Manifest.Startup[0];
        PrimaryManifestExperience = Manifest.Running[0];

        ComposePrimaryExperience();
        ComposeManifestHubBundles();
        ComposeManifestCapabilities();

        // This is composition metadata, not a presentation clock. PageFSM owns
        // gateway, population, reveal, gravity, and navigation timing.
        SetState("Moniker");
    }

    private void ComposePrimaryExperience()
    {
        var primaryExperience = PrimaryManifestExperience;
        if (primaryExperience?.MicroBundles is not { Count: > 0 } primaryBundles)
            throw new InvalidOperationException("The manifest-selected primary Experience has no MicroBundles.");

        // The manifest supplies only the primary Experience root. Its MicroBundle
        // declares the canonical Moniker dependency, so FSM_COS closes the graph.
        RuntimeAssembly = _compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 1,
            Bundles: primaryBundles
                .Select(root => root.ToRuntimeManifestEntry())
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

    private void ComposeManifestHubBundles()
    {
        _hubRuntimeAssemblies.Clear();

        var hubItems = Manifest?.Hub ?? Array.Empty<WebPageManifestHubItem>();
        for (var index = 0; index < hubItems.Count; index++)
        {
            var item = hubItems[index];
            if (item.MicroBundles is null || item.MicroBundles.Count == 0)
                continue;

            var assembly = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: 100UL + (ulong)index,
                Bundles: item.MicroBundles
                    .Select(root => root.ToRuntimeManifestEntry())
                    .ToArray()));

            _hubRuntimeAssemblies[item.Name] = assembly;
        }
    }

    private void ComposeManifestCapabilities()
    {
        _capabilityRuntimeAssemblies.Clear();

        var capabilities = Manifest?.Capabilities ?? Array.Empty<WebPageManifestCapability>();
        foreach (var capability in capabilities)
        {
            if (capability.MicroBundles is null || capability.MicroBundles.Count == 0)
                continue;

            var assembly = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: (ulong)(200UL + (ulong)_capabilityRuntimeAssemblies.Count),
                Bundles: capability.MicroBundles
                    .Select(root => root.ToRuntimeManifestEntry())
                    .ToArray()));

            _capabilityRuntimeAssemblies[capability.Name] = assembly;
        }
    }

    /// <summary>
    /// Compatibility entry point. The actual landing transition is owned by
    /// FSMManagerService/PageFSM; this service only composes manifest capabilities.
    /// </summary>
    public void RequestEntry()
    {
        if (Manifest is null)
            throw new InvalidOperationException("The Workshop manifest must be composed before entry.");
    }

    public Task RestartFromManifestAsync(bool deepDive)
    {
        if (Manifest is null)
            throw new InvalidOperationException("The Workshop manifest must be composed before restarting.");

        IsDeepDive = deepDive;
        CurrentManifestExperience = Manifest.Startup[0];
        PrimaryManifestExperience = Manifest.Running[0];
        SetState("Moniker");
        return Task.CompletedTask;
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

        _disposed = true;
    }
}