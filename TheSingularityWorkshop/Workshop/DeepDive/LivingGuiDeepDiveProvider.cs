using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Composition;

namespace TheSingularityWorkshop.Workshop.DeepDive;

/// <summary>
/// Deep Dive provider for the current Living GUI capability.
/// The provider exists only in WebPage; other hosts can consume the same ontology and runtime
/// without acquiring this educational presentation capability.
/// </summary>
public sealed class LivingGuiDeepDiveProvider : IDeepDiveProvider
{
    public WorkshopDeepDiveModel Execute(
        IExperience experience,
        IReadOnlyList<IMicroBundle> microBundles)
        => new(experience, microBundles);
}
