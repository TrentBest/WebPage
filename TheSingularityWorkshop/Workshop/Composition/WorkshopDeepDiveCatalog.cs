using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.DeepDive;
using CosMicroBundle = TheSingularityWorkshop.MicroBundleDomain.IMicroBundle;

namespace TheSingularityWorkshop.Workshop.Composition;

/// <summary>
/// Builds educational metadata from the same Experience and MicroBundle contracts used by composition.
/// A Deep Dive consumes this model; it does not identify individual Experiences by implementation type.
/// </summary>
public sealed class WorkshopDeepDiveCatalog
{
    private readonly WorkshopExperienceCatalog _experiences;
    private readonly WorkshopCompositionCatalog _bundles;

    public WorkshopDeepDiveCatalog(
        WorkshopExperienceCatalog experiences,
        WorkshopCompositionCatalog bundles)
    {
        _experiences = experiences;
        _bundles = bundles;
    }

    public bool TryResolve(ulong experienceId, out WorkshopDeepDiveModel? model)
        => TryResolve(experienceId, runtimeAssembly: null, requestedRootIds: null, out model);

    public bool TryResolve(
        ulong experienceId,
        RuntimeAssembly? runtimeAssembly,
        out WorkshopDeepDiveModel? model)
        => TryResolve(experienceId, runtimeAssembly, requestedRootIds: null, out model);

    /// <summary>
    /// Resolves educational metadata, preferring the actual FSM_COS assembly when the host
    /// can supply it. The catalog fallback is useful for direct links, but is not presented
    /// as proof of the runtime's resolved dependency closure.
    /// </summary>
    public bool TryResolve(
        ulong experienceId,
        RuntimeAssembly? runtimeAssembly,
        IReadOnlyList<ulong>? requestedRootIds,
        out WorkshopDeepDiveModel? model)
    {
        if (!_experiences.TryResolve(experienceId, out var experience) || experience is null)
        {
            model = null;
            return false;
        }

        var experienceRoots = experience.MicroBundleIds;
        var requestedRoots = requestedRootIds ?? experienceRoots;

        if (runtimeAssembly is not null &&
            (requestedRoots.Count == 0 ||
             requestedRoots.Any(id => !experienceRoots.Contains(id)) ||
             !requestedRoots.All(id => runtimeAssembly.TryGetBundle(id, out _))))
        {
            // A runtime must contain roots that belong to this Experience, not merely
            // any valid bundle IDs. Otherwise a different Experience could be misrepresented.
            runtimeAssembly = null;
        }

        var bundles = runtimeAssembly?.Bundles
            ?? experience.MicroBundleIds
                .Select(id => _bundles.TryResolve(id, out var bundle) ? bundle : null)
                .Where(bundle => bundle is not null)
                .Cast<CosMicroBundle>()
                .ToArray();

        foreach (var microBundle in bundles)
        {
            var provider = microBundle.TryGetProvider<IDeepDiveProvider>();
            if (provider is not null)
            {
                model = provider.Execute(experience, bundles, runtimeAssembly, requestedRoots);
                return true;
            }
        }

        model = new WorkshopDeepDiveModel(experience, bundles, runtimeAssembly, requestedRoots);
        return true;
    }
}

/// <summary>Immutable educational projection of an Experience composition.</summary>
public sealed class WorkshopDeepDiveModel
{
    public WorkshopDeepDiveModel(
        IExperience experience,
        IReadOnlyList<CosMicroBundle> bundles,
        RuntimeAssembly? runtimeAssembly = null,
        IReadOnlyList<ulong>? requestedRootIds = null)
    {
        Experience = experience;
        Bundles = bundles;
        RuntimeAssembly = runtimeAssembly;
        RequestedBundleIds = requestedRootIds ?? experience.MicroBundleIds;
    }

    public IExperience Experience { get; }
    public IReadOnlyList<CosMicroBundle> Bundles { get; }
    public RuntimeAssembly? RuntimeAssembly { get; }
    public bool IsRuntimeAssemblyBacked => RuntimeAssembly is not null;
    public IReadOnlyList<ulong> RequestedBundleIds { get; }
    public int ResolvedBundleCount => Bundles.Count;
    public CosMicroBundle? PrimaryBundle =>
        RequestedBundleIds
            .Select(id => Bundles.FirstOrDefault(bundle => bundle.Id == id))
            .FirstOrDefault(bundle => bundle is not null);

    public int DeclaredDependencyCount => PrimaryBundle?.Dependencies.Count ?? 0;

    public string DependencySummary =>
        PrimaryBundle is null
            ? "MICRO-BUNDLE NOT RESOLVED"
            : DeclaredDependencyCount == 0
                ? "NO DECLARED DEPENDENCIES"
                : $"{DeclaredDependencyCount} DECLARED DEPENDENC{(DeclaredDependencyCount == 1 ? "Y" : "IES")}";

    public string DependencyDescription =>
        PrimaryBundle is null
            ? "The Experience contract names a capability that is not currently present in the Workshop composition catalog."
            : DeclaredDependencyCount == 0
                ? "This Experience's currently declared MicroBundles do not require another MicroBundle."
                : "The MicroBundle contracts declare these requirements. FSM_COS resolves the dependency closure before the assembled runtime is handed to the host.";

    public string OntologySummary =>
        string.Join(" / ", Enumerable.Range(0, OntologySignature.LayerCount)
            .Select(layer => Experience.Ontology[layer]));
}
