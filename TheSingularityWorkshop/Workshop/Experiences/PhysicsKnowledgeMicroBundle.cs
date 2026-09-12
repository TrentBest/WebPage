using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>A MicroBundle carrying a graduated physics knowledge table.</summary>
public sealed class PhysicsKnowledgeMicroBundle : IMicroBundle
{
    public PhysicsKnowledgeMicroBundle(
        ulong id,
        string name,
        OntologySignature ontology,
        IReadOnlyList<PhysicsKnowledgeEntry> knowledge)
    {
        Id = id;
        Name = name;
        Ontology = ontology;
        Knowledge = knowledge;
    }

    public ulong Id { get; }
    public string Name { get; }
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology { get; }
    public IReadOnlyList<ulong> Dependencies { get; } = Array.Empty<ulong>();
    public IReadOnlyList<PhysicsKnowledgeEntry> Knowledge { get; }

    public bool Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        return false;
    }
}

/// <summary>
/// Knowledge MicroBundles built from the physics LUT. These bundles demonstrate that knowledge
/// can be addressed, loaded, retained, and independently expanded without changing the identity
/// of the underlying subject bundle.
/// </summary>
public static class PhysicsKnowledgeMicroBundleCatalog
{
    private static readonly (ulong Id, string Name, int Family, int Topic)[] Definitions =
    [
        (0x36000001, "Nuclear Binding", 6, 1),
        (0x36000002, "Nuclear Fission and Chain Reaction", 6, 2),
        (0x36000003, "Radioactive Decay", 6, 3),
        (0x36000004, "Nuclear Thermal Systems", 6, 4),
        (0x36000005, "Radiation Shielding", 6, 5),
        (0x37000001, "Energy Accounting", 7, 1),
        (0x37000002, "Momentum Conservation", 7, 2),
        (0x38000001, "Wave Phenomena", 7, 3),
        (0x39000001, "Dimensionless Analysis", 7, 4),
        (0x39000002, "Physical Units", 7, 5)
    ];

    public static IReadOnlyList<IMicroBundle> All { get; } = Build();

    private static IReadOnlyList<IMicroBundle> Build()
        => Definitions.Select(definition =>
            (IMicroBundle)new PhysicsKnowledgeMicroBundle(
                definition.Id,
                definition.Name,
                new OntologySignature(9, definition.Family, 1, 0, 0, 0, 0, 0, definition.Topic),
                PhysicsKnowledgeLut.Entries
                    .Where(entry => entry.MicroBundleId == definition.Id)
                    .ToArray()))
            .ToArray();
}
