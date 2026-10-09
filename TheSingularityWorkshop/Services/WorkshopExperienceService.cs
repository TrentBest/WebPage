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
    private CancellationTokenSource? _presentationCancellation;

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

        Manifest = await _httpClient.GetFromJsonAsync<WebPageHostManifest>(ManifestPath)
            ?? throw new InvalidOperationException("The WebPage host manifest could not be loaded.");

        if (Manifest.Startup.Count == 0)
            throw new InvalidOperationException("The WebPage host manifest contains no startup Experience.");

        if (Manifest.Running.Count == 0)
            throw new InvalidOperationException("The WebPage host manifest contains no primary running Experience.");

        CurrentManifestExperience = Manifest.Startup[0];
        PrimaryManifestExperience = Manifest.Running[0];

        ComposePrimaryExperience();
        ComposeManifestHubBundles();
        ComposeManifestCapabilities();

        SetState("Moniker");
        _ = PresentMonikerAsync(_presentationCancellation = new CancellationTokenSource());
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
                .Select(MicroBundleDependencyRequest.Unconfigured)
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
            if (item.MicroBundleIds is null || item.MicroBundleIds.Count == 0)
                continue;

            var assembly = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: 100UL + (ulong)index,
                Bundles: item.MicroBundleIds
                    .Select(MicroBundleDependencyRequest.Unconfigured)
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
            if (capability.MicroBundleIds is null || capability.MicroBundleIds.Count == 0)
                continue;

            var assembly = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: (ulong)(200UL + (ulong)_capabilityRuntimeAssemblies.Count),
                Bundles: capability.MicroBundleIds
                    .Select(MicroBundleDependencyRequest.Unconfigured)
                    .ToArray()));

            _capabilityRuntimeAssemblies[capability.Name] = assembly;
        }
    }

    private async Task PresentMonikerAsync(CancellationTokenSource cancellation)
    {
        try
        {
            var seconds = Math.Max(0, CurrentManifestExperience?.PresentationSeconds ?? 0);
            await Task.Delay(TimeSpan.FromSeconds(seconds), cancellation.Token);
            if (!_disposed && CurrentState.Equals("Moniker", StringComparison.OrdinalIgnoreCase))
            {
                ActivatePrimaryExperience();
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            // Presentation was interrupted by explicit entry or restart.
        }
    }

    public void RequestEntry()
    {
        if (CurrentState.Equals("Moniker", StringComparison.OrdinalIgnoreCase))
        {
            _presentationCancellation?.Cancel();
            ActivatePrimaryExperience();
        }
    }

    private void ActivatePrimaryExperience()
    {
        if (PrimaryManifestExperience is null)
            throw new InvalidOperationException("No primary Experience is available in the WebPage manifest.");

        SetState("Hub");
    }

    public async Task RestartFromManifestAsync(bool deepDive)
    {
        if (Manifest is null)
            await InitializeAsync();

        IsDeepDive = deepDive;
        _presentationCancellation?.Cancel();
        _presentationCancellation = new CancellationTokenSource();
        CurrentManifestExperience = Manifest!.Startup[0];
        PrimaryManifestExperience = Manifest.Running[0];
        SetState("Moniker");
        _ = PresentMonikerAsync(_presentationCancellation);
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
        _presentationCancellation?.Cancel();
        _presentationCancellation?.Dispose();
    }
}