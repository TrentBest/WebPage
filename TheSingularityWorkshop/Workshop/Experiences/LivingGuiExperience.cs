using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// The first concrete Flex Experience. The Experience is the composition boundary;
/// the Living GUI runtime remains FSM_API-driven after the Experience is installed.
/// </summary>
public sealed class LivingGuiExperience : IExperience
{
    public const ulong ExperienceId = 3002;
    public const ulong LivingGuiBundleId = LivingGuiExperienceMicroBundle.BundleId;

    public ulong Id => ExperienceId;
    public string Name => "LIVING GUI";
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, checked((int)ExperienceId));

    /// <summary>
    /// The Living GUI Experience explicitly composes its runtime MicroBundle.
    /// The MicroBundle itself declares the Moniker dependency to FSM_COS.
    /// </summary>
    public IReadOnlyList<ulong> MicroBundleIds { get; } = [LivingGuiBundleId];

    public IReadOnlyList<ulong> Capabilities { get; } = [1];
    public IReadOnlyList<ulong> SensorySystems { get; } = [1, 2, 3];
    public IReadOnlyList<string> ProcessingGroups { get; } = ["LivingGui", "PageFSM"];
}
