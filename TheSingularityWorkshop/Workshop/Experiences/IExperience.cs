using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// Describes a user-facing Workshop experience as a portable composition of
/// Hub-recognized MicroBundles.
/// </summary>
public interface IExperience
{
    /// <summary>Stable identity for the experience.</summary>
    ulong Id { get; }

    /// <summary>User-facing name of the experience.</summary>
    string Name { get; }

    /// <summary>Version of the experience contract/content.</summary>
    BundleVersion Version { get; }

    /// <summary>Ontology coordinates describing what the experience is.</summary>
    OntologySignature Ontology { get; }

    /// <summary>MicroBundles required to compose the experience.</summary>
    IReadOnlyList<ulong> MicroBundleIds { get; }

    /// <summary>Capabilities exposed by the experience.</summary>
    IReadOnlyList<ulong> Capabilities { get; }

    /// <summary>
    /// Integer identifiers for the sensory systems provided by the experience.
    /// The experience is quantified by the number of distinct sensory systems it provides.
    /// </summary>
    IReadOnlyList<ulong> SensorySystems { get; }

    /// <summary>Gets the number of distinct sensory systems provided by the experience.</summary>
    int SenseCount => SensorySystems.Distinct().Count();

    /// <summary>
    /// Scheduler process-group identifiers used by the experience.
    /// The experience exposes them; the Hub owns their stepping and lifecycle scheduling.
    /// </summary>
    IReadOnlyList<string> ProcessingGroups { get; }
}
