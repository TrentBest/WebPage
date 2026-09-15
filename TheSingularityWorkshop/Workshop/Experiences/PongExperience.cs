using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// Concrete Workshop Experience for Pong.
/// Idle mode selects this Experience; it does not define what Pong is.
/// The Experience is the composition boundary and points at the Pong MicroBundle.
/// </summary>
public sealed class PongExperience : IExperience
{
    public const ulong ExperienceId = 3001;
    public const ulong PongBundleId = PongMicroBundle.BundleId;

    public ulong Id => ExperienceId;
    public string Name => "PONG";
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, checked((int)ExperienceId));
    public IReadOnlyList<ulong> MicroBundleIds { get; } = [PongBundleId];
    public IReadOnlyList<ulong> Capabilities { get; } = [1];
    public IReadOnlyList<ulong> SensorySystems { get; } = [1, 2];
    public IReadOnlyList<string> ProcessingGroups { get; } = ["MicroBundle_2001"];
}
