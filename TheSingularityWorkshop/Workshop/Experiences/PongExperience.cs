using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// The canonical Pong experience. The experience is assembled from independently
/// identifiable MicroBundles; presentation is supplied by the active host.
/// </summary>
public sealed class PongExperience : IIdleExperience
{
    public const ulong ExperienceId = 0x504F4E47UL;

    private static readonly ulong[] BundleIds =
    [
        PongMicroBundleIds.ArenaAsset,
        PongMicroBundleIds.PaddleAsset,
        PongMicroBundleIds.BallAsset,
        PongMicroBundleIds.ScoreAsset,
        PongMicroBundleIds.VisualPresentation,
        PongMicroBundleIds.SoundPresentation,
        PongMicroBundleIds.InputBehavior,
        PongMicroBundleIds.BallPhysicsBehavior,
        PongMicroBundleIds.ScoreBehavior
    ];

    private static readonly ulong[] CapabilityIds =
    [PongCapabilityIds.Visual, PongCapabilityIds.Sound, PongCapabilityIds.Input, PongCapabilityIds.Behavior];
    private static readonly ulong[] SensorySystemIds = [PongSenseIds.Sight, PongSenseIds.Hearing];
    private static readonly string[] Groups =
    ["Experience.Pong.Input", "Experience.Pong.Physics", "Experience.Pong.Presentation"];

    public ulong Id => ExperienceId;
    public string Name => "PONG";
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(7, 1, 2, 3, 4, 5, 6, 8, 42);
    public IReadOnlyList<ulong> MicroBundleIds => BundleIds;
    public IReadOnlyList<ulong> Capabilities => CapabilityIds;
    public IReadOnlyList<ulong> SensorySystems => SensorySystemIds;
    public int SenseCount => SensorySystems.Distinct().Count();
    public IReadOnlyList<string> ProcessingGroups => Groups;
    public static IReadOnlyList<IMicroBundle> MicroBundles => PongMicroBundleCatalog.All;
}

/// <summary>Stable integer identities for the Pong MicroBundles.</summary>
public static class PongMicroBundleIds
{
    public const ulong ArenaAsset = 0x504F4E4701UL;
    public const ulong PaddleAsset = 0x504F4E4702UL;
    public const ulong BallAsset = 0x504F4E4703UL;
    public const ulong ScoreAsset = 0x504F4E4704UL;
    public const ulong VisualPresentation = 0x504F4E4711UL;
    public const ulong SoundPresentation = 0x504F4E4712UL;
    public const ulong InputBehavior = 0x504F4E4721UL;
    public const ulong BallPhysicsBehavior = 0x504F4E4722UL;
    public const ulong ScoreBehavior = 0x504F4E4723UL;
}

/// <summary>Stable integer capability identities used by Pong.</summary>
public static class PongCapabilityIds
{
    public const ulong Visual = 0x1001UL;
    public const ulong Sound = 0x1002UL;
    public const ulong Input = 0x1003UL;
    public const ulong Behavior = 0x1004UL;
}

/// <summary>Stable integer sensory-system identities used by Pong.</summary>
public static class PongSenseIds
{
    public const ulong Sight = 1;
    public const ulong Hearing = 2;
}

/// <summary>Separates authored assets from executable behavior and presentation bundles.</summary>
public enum PongMicroBundleKind
{
    Asset,
    Presentation,
    Behavior
}

/// <summary>
/// Concrete Pong MicroBundle. Its identity, ontology, version, dependencies and
/// arbitration boundary are runtime data; a host decides how the bundle is manifested.
/// </summary>
public sealed class PongMicroBundle : IMicroBundle
{
    public PongMicroBundle(ulong id, string name, PongMicroBundleKind kind, IReadOnlyList<ulong> dependencies)
    {
        Id = id;
        Name = name;
        Kind = kind;
        Dependencies = dependencies;
    }

    public ulong Id { get; }
    public string Name { get; }
    public PongMicroBundleKind Kind { get; }
    public OntologySignature Ontology => new(7, 1, 2, 3, 4, 5, 6, 8, unchecked((int)Id));
    public BundleVersion Version => new(1, 0, 0);
    public IReadOnlyList<ulong> Dependencies { get; }

    public bool Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        return true;
    }
}

/// <summary>Canonical MicroBundle catalog for the Pong experience.</summary>
public static class PongMicroBundleCatalog
{
    private static readonly PongMicroBundle[] Definitions =
    [
        new(PongMicroBundleIds.ArenaAsset, "Arena", PongMicroBundleKind.Asset, []),
        new(PongMicroBundleIds.PaddleAsset, "Paddle", PongMicroBundleKind.Asset, []),
        new(PongMicroBundleIds.BallAsset, "Ball", PongMicroBundleKind.Asset, []),
        new(PongMicroBundleIds.ScoreAsset, "Score", PongMicroBundleKind.Asset, []),
        new(PongMicroBundleIds.VisualPresentation, "Visual Presentation", PongMicroBundleKind.Presentation,
            [PongMicroBundleIds.ArenaAsset, PongMicroBundleIds.PaddleAsset, PongMicroBundleIds.BallAsset, PongMicroBundleIds.ScoreAsset]),
        new(PongMicroBundleIds.SoundPresentation, "Sound Presentation", PongMicroBundleKind.Presentation,
            [PongMicroBundleIds.BallAsset]),
        new(PongMicroBundleIds.InputBehavior, "Up Down Input", PongMicroBundleKind.Behavior, []),
        new(PongMicroBundleIds.BallPhysicsBehavior, "Ball Physics", PongMicroBundleKind.Behavior,
            [PongMicroBundleIds.BallAsset, PongMicroBundleIds.ArenaAsset]),
        new(PongMicroBundleIds.ScoreBehavior, "Score Behavior", PongMicroBundleKind.Behavior,
            [PongMicroBundleIds.ScoreAsset, PongMicroBundleIds.BallPhysicsBehavior])
    ];

    public static IReadOnlyList<PongMicroBundle> All => Definitions;
}
