using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;

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
    {
        if (!_experiences.TryResolve(experienceId, out var experience) || experience is null)
        {
            model = null;
            return false;
        }

        var bundleIds = experience.MicroBundleIds.ToArray();
        var bundles = bundleIds
            .Select(id => _bundles.TryResolve(id, out var bundle) ? bundle : null)
            .Where(bundle => bundle is not null)
            .Cast<IMicroBundle>()
            .ToArray();

        model = new WorkshopDeepDiveModel(experience, bundles);
        return true;
    }
}

/// <summary>
/// Immutable educational projection of an Experience composition.
/// </summary>
public sealed class WorkshopDeepDiveModel
{
    public WorkshopDeepDiveModel(
        IExperience experience,
        IReadOnlyList<IMicroBundle> bundles)
    {
        Experience = experience;
        Bundles = bundles;
    }

    public IExperience Experience { get; }

    public IReadOnlyList<IMicroBundle> Bundles { get; }

    public IMicroBundle? PrimaryBundle => Bundles.FirstOrDefault();

    public int DeclaredDependencyCount =>
        Bundles.Sum(bundle => bundle.Dependencies.Count);

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
        FormatOntology(Experience.Ontology);

    private static string FormatOntology(OntologySignature ontology)
        => string.Join(" / ", Enumerable.Range(0, OntologySignature.LayerCount)
            .Select(layer => ontology[layer]));
}
