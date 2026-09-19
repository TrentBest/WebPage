using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Advertising;

/// <summary>
/// Spatial/composable Experience for advertising procurement and inventory creation.
/// "Madmen" is the user-facing agency concept; the underlying domain stays generic.
/// </summary>
public sealed class MadmenExperience : IExperience
{
    public const ulong ExperienceId = 2300;

    public MadmenExperience()
    {
        Agency = new MadmenAgency(2300, "Madmen");
        MicroBundleIds = [2301, 2302, 2303, 2304];
        Capabilities = [2301, 2302, 2303, 2304];
        SensorySystems = [1, 2, 3];
        ProcessingGroups = ["Advertising", "Advertising.Market", "Advertising.Inventory"];
    }

    public MadmenAgency Agency { get; }

    public ulong Id => ExperienceId;
    public string Name => "Madmen";
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(3, 1, 1, 1, 1, 1, 1, 1, 2300);
    public IReadOnlyList<ulong> MicroBundleIds { get; }
    public IReadOnlyList<ulong> Capabilities { get; }
    public IReadOnlyList<ulong> SensorySystems { get; }
    public IReadOnlyList<string> ProcessingGroups { get; }
}
