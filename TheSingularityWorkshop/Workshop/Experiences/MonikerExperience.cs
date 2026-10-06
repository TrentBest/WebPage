using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// The configured Workshop moniker Experience. The manifest loads this Experience
/// during startup so branding is a composed runtime capability rather than page markup.
/// </summary>
public sealed class MonikerExperience : IExperience
{
    public const ulong ExperienceId = 2001;
    public const ulong MonikerBundleId = MonikerMicroBundle.BundleId;

    public ulong Id => ExperienceId;
    public string Name => MonikerMicroBundle.DefaultText;
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, checked((int)ExperienceId));
    public IReadOnlyList<ulong> MicroBundleIds { get; } = [MonikerBundleId];
    public IReadOnlyList<ulong> Capabilities { get; } = [HubCapabilityIds.Moniker];
    public IReadOnlyList<ulong> SensorySystems { get; } = [1];
    public IReadOnlyList<string> ProcessingGroups { get; } = ["WorkshopMoniker"];
}
