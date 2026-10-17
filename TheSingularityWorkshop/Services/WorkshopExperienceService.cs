using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Infrastructure.Hub;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Experiences;
using TheSingularityWorkshop.Workshop.MicroBundles;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;

namespace TheSingularityWorkshop.Services;

public sealed class WorkshopExperienceService : IDisposable
{
    private const string DefaultManifestPath = "Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json";

    private readonly HttpClient _httpClient;
    private readonly IFsmCos _compositionSystem = new FsmCos(new WorkshopCompositionCatalog());
    private readonly HubRuntime _hubRuntime;
    private ulong _runtimeGeneration;
    private bool _disposed;

    public event Action? StateChanged;

    public string CurrentState { get; private set; } = "ManifestLoading";
    public bool IsFirstVisit { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsDeepDive { get; private set; }
    public string ActiveManifestPath { get; private set; } = DefaultManifestPath;
    public string? LastManifestError { get; private set; }
    public string? LastRuntimeCleanupError { get; private set; }
    public WebPageHostManifest? Manifest { get; private set; }
    public DemonstrationRegistry? Demonstrations { get; private set; }
    public WebPageManifestExperience? CurrentManifestExperience { get; private set; }
    public WebPageManifestExperience? PrimaryManifestExperience { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public IExperience? SelectedExperience { get; private set; }
    public RuntimeAssembly? RuntimeAssembly { get; private set; }

    private readonly Dictionary<string, RuntimeAssembly> _hubRuntimeAssemblies =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, RuntimeAssembly> _capabilityRuntimeAssemblies =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, RuntimeAssembly> HubRuntimeAssemblies => _hubRuntimeAssemblies;
    public IReadOnlyDictionary<string, RuntimeAssembly> CapabilityRuntimeAssemblies => _capabilityRuntimeAssemblies;

    public GuiNode? MonikerComposition =>
        RuntimeAssembly?.TryGetBundle<MonikerMicroBundle>(
            (ulong)MonikerMicroBundle.BundleId, out var moniker) == true
            ? moniker!.Composition
            : null;

    public string? MonikerBehaviorClass =>
        RuntimeAssembly?.TryGetBundle<MonikerSwayBehaviorMicroBundle>(
            (ulong)MonikerSwayBehaviorMicroBundle.BundleId, out _) == true
            ? MonikerSwayBehaviorMicroBundle.PresentationClass
            : null;

    public LivingGuiExperienceMicroBundle? PrimaryExperienceComposition =>
        RuntimeAssembly?.TryGetBundle<LivingGuiExperienceMicroBundle>(
            LivingGuiExperienceMicroBundle.BundleId, out var primary) == true
            ? primary
            : null;

    public AiExchangeCompositionBundle? AiExchangeComposition =>
        _capabilityRuntimeAssemblies.Values
            .Select(assembly => assembly.TryGetBundle<AiExchangeCompositionBundle>(
                AiExchangeCompositionBundle.BundleId, out var ai) ? ai : null)
            .FirstOrDefault(ai => ai is not null);

    public WorkshopExperienceService(HttpClient httpClient, HubRuntime hubRuntime)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _hubRuntime = hubRuntime ?? throw new ArgumentNullException(nameof(hubRuntime));
    }

    public async Task InitializeAsync(bool returningVisitor = false)
    {
        ThrowIfDisposed();
        if (IsInitialized)
            return;

        IsFirstVisit = !returningVisitor;
        SetState("ManifestLoading");
        await SwitchManifestAsync(DefaultManifestPath);
    }

    /// <summary>
    /// Loads and composes a candidate host manifest, then atomically activates it.
    /// The source must be a same-origin relative asset path. Executable MicroBundle
    /// artifacts must be resolved by the trusted repository/provider boundary, not URLs.
    /// </summary>
    public async Task SwitchManifestAsync(string manifestPath)
    {
        ThrowIfDisposed();
        ValidateManifestPath(manifestPath);

        RuntimeAssembly? candidatePrimary = null;
        var candidateHubAssemblies = new Dictionary<string, RuntimeAssembly>(StringComparer.OrdinalIgnoreCase);
        var candidateCapabilities = new Dictionary<string, RuntimeAssembly>(StringComparer.OrdinalIgnoreCase);
        HubKernel? candidateHub = null;
        var activated = false;

        try
        {
            var manifestJson = await _httpClient.GetStringAsync(manifestPath);
            var candidateManifest = WebPageHostManifestParser.Parse(manifestJson);

            if (candidateManifest.Startup is not { Count: > 0 })
                throw new InvalidOperationException("The selected host manifest contains no startup Experience.");
            if (candidateManifest.Running is not { Count: > 0 })
                throw new InvalidOperationException("The selected host manifest contains no primary running Experience.");

            var startupExperience = candidateManifest.Startup[0];
            var primaryExperience = candidateManifest.Running[0];
            if (primaryExperience.MicroBundles is not { Count: > 0 } primaryBundles)
                throw new InvalidOperationException("The manifest-selected primary Experience has no MicroBundles.");

            var generation = checked(++_runtimeGeneration);
            var runtimeBase = checked(generation * 1000UL);
            var roots = primaryBundles.Select(root => root.ToRuntimeManifestEntry()).ToList();
            if (startupExperience.MicroBundles is { Count: > 0 } startupBundles)
                roots.InsertRange(0, startupBundles.Select(root => root.ToRuntimeManifestEntry()));

            candidatePrimary = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: runtimeBase + 1,
                Bundles: roots.ToArray()));

            var hasMoniker = candidatePrimary.TryGetBundle<MonikerMicroBundle>(
                (ulong)MonikerMicroBundle.BundleId, out _);
            var hasPrimary = candidatePrimary.TryGetBundle<LivingGuiExperienceMicroBundle>(
                LivingGuiExperienceMicroBundle.BundleId, out _);
            var startupSelected = startupExperience.MicroBundles is { Count: > 0 };
            var hasMonikerExperience = !startupSelected ||
                candidatePrimary.TryGetBundle<MonikerExperienceMicroBundle>(
                    (ulong)MonikerExperienceMicroBundle.BundleId, out _);
            var hasSwayBehavior = !startupSelected ||
                candidatePrimary.TryGetBundle<MonikerSwayBehaviorMicroBundle>(
                    (ulong)MonikerSwayBehaviorMicroBundle.BundleId, out _);

            if (!hasMonikerExperience || !hasSwayBehavior || !hasMoniker || !hasPrimary)
                throw new InvalidOperationException(
                    "FSM_COS did not compose the selected Moniker Experience, stationary Moniker, sway behavior, and primary Experience.");

            ComposeManifestHubBundles(candidateManifest, runtimeBase, candidateHubAssemblies);
            ComposeManifestCapabilities(candidateManifest, runtimeBase, candidateCapabilities);

            candidateHub = new HubKernel();
            if (candidatePrimary.TryGetBundle<MonikerMicroBundle>(
                    (ulong)MonikerMicroBundle.BundleId, out var moniker) &&
                moniker is TheSingularityWorkshop.SingularityHub.IMicroBundle hubBundle)
            {
                candidateHub.LoadBundle(hubBundle);
            }
            candidateHub.ExecuteArbitrationPipeline();

            var candidateDemonstrations = new DemonstrationRegistry(
                candidateManifest.Demonstrations,
                hostCapabilities: ["WebPage", "Browser"]);

            var oldPrimary = RuntimeAssembly;
            var oldHubAssemblies = _hubRuntimeAssemblies.Values.ToArray();
            var oldCapabilities = _capabilityRuntimeAssemblies.Values.ToArray();
            var oldHub = _hubRuntime.Hub;

            _hubRuntime.ReplaceHub(candidateHub);
            Manifest = candidateManifest;
            Demonstrations = candidateDemonstrations;
            CurrentManifestExperience = startupExperience;
            PrimaryManifestExperience = primaryExperience;
            RuntimeAssembly = candidatePrimary;
            ReplaceAssemblies(_hubRuntimeAssemblies, candidateHubAssemblies);
            ReplaceAssemblies(_capabilityRuntimeAssemblies, candidateCapabilities);
            ActiveManifestPath = manifestPath;
            IsInitialized = true;
            IsDeepDive = false;
            LastManifestError = null;
            activated = true;

            DisposeRuntimeObjects(
                (oldPrimary?.Bundles ?? Array.Empty<DomainMicroBundle>()).Cast<object>()
                    .Concat(oldHubAssemblies.SelectMany(assembly => assembly.Bundles).Cast<object>())
                    .Concat(oldCapabilities.SelectMany(assembly => assembly.Bundles).Cast<object>())
                    .Concat(oldHub.LoadedBundles.Cast<object>()));
            SetState("Moniker");
        }
        catch (Exception exception)
        {
            if (!activated)
            {
                LastManifestError = exception.Message;
                DisposeCandidateRuntime(
                    candidatePrimary,
                    candidateHubAssemblies.Values,
                    candidateCapabilities.Values,
                    candidateHub?.LoadedBundles.Cast<object>() ?? Array.Empty<object>());
            }
            throw;
        }
    }

    private void ComposeManifestHubBundles(
        WebPageHostManifest manifest,
        ulong runtimeBase,
        IDictionary<string, RuntimeAssembly> assemblies)
    {
        var hubItems = manifest.Hub ?? Array.Empty<WebPageManifestHubItem>();
        for (var index = 0; index < hubItems.Count; index++)
        {
            var item = hubItems[index];
            if (item.MicroBundles is not { Count: > 0 } roots)
                continue;

            assemblies[item.Name] = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: runtimeBase + 100UL + (ulong)index,
                Bundles: roots.Select(root => root.ToRuntimeManifestEntry()).ToArray()));
        }
    }

    private void ComposeManifestCapabilities(
        WebPageHostManifest manifest,
        ulong runtimeBase,
        IDictionary<string, RuntimeAssembly> assemblies)
    {
        var capabilities = manifest.Capabilities ?? Array.Empty<WebPageManifestCapability>();
        var index = 0UL;
        foreach (var capability in capabilities)
        {
            if (capability.MicroBundles is not { Count: > 0 } roots)
                continue;

            assemblies[capability.Name] = _compositionSystem.Execute(new RuntimeManifest(
                RuntimeId: runtimeBase + 200UL + index,
                Bundles: roots.Select(root => root.ToRuntimeManifestEntry()).ToArray()));
            index++;
        }
    }

    private static void ReplaceAssemblies(
        IDictionary<string, RuntimeAssembly> destination,
        IReadOnlyDictionary<string, RuntimeAssembly> source)
    {
        destination.Clear();
        foreach (var pair in source)
            destination.Add(pair.Key, pair.Value);
    }

    private void DisposeCandidateRuntime(
        RuntimeAssembly? primary,
        IEnumerable<RuntimeAssembly> hubAssemblies,
        IEnumerable<RuntimeAssembly> capabilities,
        IEnumerable<object> hubBundles)
    {
        DisposeRuntimeObjects(
            (primary?.Bundles ?? Array.Empty<DomainMicroBundle>()).Cast<object>()
                .Concat(hubAssemblies.SelectMany(assembly => assembly.Bundles).Cast<object>())
                .Concat(capabilities.SelectMany(assembly => assembly.Bundles).Cast<object>())
                .Concat(hubBundles));
    }

    private void DisposeRuntimeObjects(IEnumerable<object> objects)
    {
        var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
        foreach (var value in objects)
        {
            if (!seen.Add(value) || value is not IDisposable disposable)
                continue;

            try
            {
                disposable.Dispose();
            }
            catch (Exception exception)
            {
                LastRuntimeCleanupError = exception.Message;
            }
        }
    }

    private static void ValidateManifestPath(string manifestPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manifestPath);
        if (manifestPath.StartsWith("//", StringComparison.Ordinal) ||
            manifestPath.StartsWith("/", StringComparison.Ordinal) ||
            manifestPath.Contains("://", StringComparison.Ordinal) ||
            manifestPath.Contains('\\') ||
            manifestPath.Contains('?') ||
            manifestPath.Contains('#') ||
            manifestPath.Split('/').Any(segment => segment is "." or "..") ||
            manifestPath.Any(char.IsControl))
        {
            throw new ArgumentException(
                "Manifest selection must be a same-origin relative asset path without traversal, query, or fragment.",
                nameof(manifestPath));
        }
    }

    public void RequestEntry()
    {
        if (Manifest is null)
            throw new InvalidOperationException("The Workshop manifest must be composed before entry.");
    }

    /// <summary>Reloads and recomposes the currently active manifest from its configured source.</summary>
    public async Task RestartFromManifestAsync(bool deepDive)
    {
        if (Manifest is null)
            throw new InvalidOperationException("The Workshop manifest must be composed before restarting.");

        await SwitchManifestAsync(ActiveManifestPath);
        IsDeepDive = deepDive;
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

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        DisposeCandidateRuntime(
            RuntimeAssembly,
            _hubRuntimeAssemblies.Values,
            _capabilityRuntimeAssemblies.Values,
            _hubRuntime.Hub.LoadedBundles.Cast<object>());
        _hubRuntimeAssemblies.Clear();
        _capabilityRuntimeAssemblies.Clear();
        RuntimeAssembly = null;
        Manifest = null;
    }
}
