using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Composition;

namespace TheSingularityWorkshop.Workshop.DeepDive;

/// <summary>
/// WebPage-only educational capability supplied by a MicroBundle.
/// Shared runtime packages do not depend on this interface.
/// </summary>
public interface IDeepDiveProvider
{
    /// <summary>
    /// Builds the Deep Dive projection for the supplied Experience and its composed MicroBundles.
    /// </summary>
    WorkshopDeepDiveModel Execute(
        IExperience experience,
        IReadOnlyList<IMicroBundle> microBundles);
}
