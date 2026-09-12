using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>Stable semantic families used by the first ontology-backed MicroBundle library.</summary>
public static class OntologyFamilies
{
    public const int SoftwareAbstractions = 1;
    public const int DigitalLogic = 2;
    public const int Physics = 9;

    public const int Newtonian = 1;
    public const int ElectricityAndMagnetism = 2;
    public const int Thermodynamics = 3;
    public const int Hydrodynamics = 4;
    public const int Plasmadynamics = 5;
}

/// <summary>Concrete authored MicroBundle whose identity is determined by its integer ontology address.</summary>
public sealed class OntologyMicroBundle : IMicroBundle
{
    public OntologyMicroBundle(ulong id, string name, OntologySignature ontology, IReadOnlyList<ulong>? dependencies = null)
    {
        Id = id;
        Name = name;
        Ontology = ontology;
        Dependencies = dependencies ?? Array.Empty<ulong>();
    }

    public ulong Id { get; }
    public string Name { get; }
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology { get; }
    public IReadOnlyList<ulong> Dependencies { get; }

    public bool Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        return false;
    }
}

/// <summary>
/// Initial broad ontology inventory. These are deliberately small, independently addressable
/// MicroBundles rather than one giant physics package. New domains can be appended without
/// changing existing integer identities.
/// </summary>
public static class OntologyMicroBundleCatalog
{
    public static IReadOnlyList<IMicroBundle> All { get; } = Build();

    private static IReadOnlyList<IMicroBundle> Build()
    {
        var bundles = new List<IMicroBundle>();
        AddSoftwareAbstractions(bundles);
        AddDigitalLogic(bundles);
        AddPhysics(bundles);
        return bundles;
    }

    private static void AddSoftwareAbstractions(List<IMicroBundle> b)
    {
        b.Add(Create(0x10000001, "Type Systems", OntologyFamilies.SoftwareAbstractions, 1, 1, 1));
        b.Add(Create(0x10000002, "Object Models", OntologyFamilies.SoftwareAbstractions, 1, 1, 2));
        b.Add(Create(0x10000003, "Functional Composition", OntologyFamilies.SoftwareAbstractions, 1, 1, 3));
        b.Add(Create(0x10000004, "Dependency Injection", OntologyFamilies.SoftwareAbstractions, 1, 1, 4));
        b.Add(Create(0x10000005, "State Machines", OntologyFamilies.SoftwareAbstractions, 1, 1, 5));
        b.Add(Create(0x10000006, "Schedulers", OntologyFamilies.SoftwareAbstractions, 1, 1, 6));
        b.Add(Create(0x10000007, "Serialization", OntologyFamilies.SoftwareAbstractions, 1, 1, 7));
        b.Add(Create(0x10000008, "Query Systems", OntologyFamilies.SoftwareAbstractions, 1, 1, 8));
        b.Add(Create(0x10000009, "Distributed Systems", OntologyFamilies.SoftwareAbstractions, 1, 1, 9));
        b.Add(Create(0x1000000A, "Resource Lifecycles", OntologyFamilies.SoftwareAbstractions, 1, 1, 10));
    }

    private static void AddDigitalLogic(List<IMicroBundle> b)
    {
        b.Add(Create(0x20000001, "Boolean Algebra", OntologyFamilies.DigitalLogic, 1, 1, 1));
        b.Add(Create(0x20000002, "Logic Gates", OntologyFamilies.DigitalLogic, 1, 1, 2));
        b.Add(Create(0x20000003, "Combinational Logic", OntologyFamilies.DigitalLogic, 1, 1, 3));
        b.Add(Create(0x20000004, "Sequential Logic", OntologyFamilies.DigitalLogic, 1, 1, 4));
        b.Add(Create(0x20000005, "Registers", OntologyFamilies.DigitalLogic, 1, 1, 5));
        b.Add(Create(0x20000006, "Counters", OntologyFamilies.DigitalLogic, 1, 1, 6));
        b.Add(Create(0x20000007, "Finite State Logic", OntologyFamilies.DigitalLogic, 1, 1, 7));
        b.Add(Create(0x20000008, "Memory Logic", OntologyFamilies.DigitalLogic, 1, 1, 8));
        b.Add(Create(0x20000009, "Arithmetic Logic", OntologyFamilies.DigitalLogic, 1, 1, 9));
        b.Add(Create(0x2000000A, "Clocking", OntologyFamilies.DigitalLogic, 1, 1, 10));
    }

    private static void AddPhysics(List<IMicroBundle> b)
    {
        AddPhysicsFamily(b, OntologyFamilies.Newtonian, 0x31000000, "Newtonian");
        AddPhysicsFamily(b, OntologyFamilies.ElectricityAndMagnetism, 0x32000000, "Electricity and Magnetism");
        AddPhysicsFamily(b, OntologyFamilies.Thermodynamics, 0x33000000, "Thermodynamics");
        AddPhysicsFamily(b, OntologyFamilies.Hydrodynamics, 0x34000000, "Hydrodynamics");
        AddPhysicsFamily(b, OntologyFamilies.Plasmadynamics, 0x35000000, "Plasmadynamics");
    }

    private static void AddPhysicsFamily(List<IMicroBundle> b, int family, ulong prefix, string name)
    {
        var topics = name switch
        {
            "Newtonian" => new[] { "Motion", "Forces", "Momentum", "Energy", "Gravity" },
            "Electricity and Magnetism" => new[] { "Charge", "Fields", "Circuits", "Induction", "Waves" },
            "Thermodynamics" => new[] { "Temperature", "Heat", "Entropy", "Equilibrium", "Phase Change" },
            "Hydrodynamics" => new[] { "Pressure", "Flow", "Viscosity", "Turbulence", "Buoyancy" },
            _ => new[] { "Ionization", "Magnetic Confinement", "Plasma Waves", "Debye Shielding", "Magnetohydrodynamics" }
        };

        for (var i = 0; i < topics.Length; i++)
        {
            // The topic token is globally unique at the ninth ontology layer. Encoding the
            // family into the token prevents the same 1..5 topic slot from colliding across
            // the five physics families.
            var topicToken = (family * 100) + (i + 1);
            b.Add(Create(
                prefix + (ulong)(i + 1),
                $"{name}: {topics[i]}",
                OntologyFamilies.Physics,
                family,
                1,
                0,
                0,
                0,
                0,
                0,
                topicToken));
        }
    }

    private static IMicroBundle Create(ulong id, string name, params int[] layers)
    {
        var ontology = new OntologySignature(
            layers.ElementAtOrDefault(0), layers.ElementAtOrDefault(1), layers.ElementAtOrDefault(2),
            layers.ElementAtOrDefault(3), layers.ElementAtOrDefault(4), layers.ElementAtOrDefault(5),
            layers.ElementAtOrDefault(6), layers.ElementAtOrDefault(7), layers.ElementAtOrDefault(8));
        return new OntologyMicroBundle(id, name, ontology);
    }
}