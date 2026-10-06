using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// Instructional Experience built from the original FSM_API Fox/Dog demonstration.
/// It preserves the demonstration's purpose: independent agents, separate processing
/// groups, collision-driven state transitions, and observable runtime behavior.
/// </summary>
public sealed class FoxesAndDogsExperience : IExperience
{
    public const ulong ExperienceId = 3001;
    public const ulong BundleId = FoxesAndDogsMicroBundle.BundleId;

    public ulong Id => ExperienceId;
    public string Name => "FOXES & DOGS";
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, checked((int)ExperienceId));

    public IReadOnlyList<ulong> MicroBundleIds { get; } = [BundleId];
    public IReadOnlyList<ulong> Capabilities { get; } = [1];
    public IReadOnlyList<ulong> SensorySystems { get; } = [1, 2];
    public IReadOnlyList<string> ProcessingGroups { get; } = ["Environment", "Dogs", "Foxes", "Update"];
}
