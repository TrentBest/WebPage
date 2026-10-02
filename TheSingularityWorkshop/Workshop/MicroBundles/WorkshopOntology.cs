namespace TheSingularityWorkshop.Workshop.MicroBundles;

using TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Canonical nine-layer ontology coordinates for Workshop MicroBundles.
/// Human-readable entries are registered through the ProtocolAI-backed vocabulary
/// and become deterministic integer runtime coordinates. The ninth coordinate
/// remains the concrete species/variant identity.
/// </summary>
public static class WorkshopOntology
{
    private static readonly OntologyVocabulary Vocabulary = new();

    public static class Paradigm
    {
        public static readonly int Workshop = Vocabulary.GetOrAdd(0, "Workshop");
    }

    public static class Domain
    {
        public static readonly int Science = Vocabulary.GetOrAdd(1, "Science");
    }

    public static class Kingdom
    {
        public static readonly int PhysicalScience = Vocabulary.GetOrAdd(2, "PhysicalScience");
    }

    public static class Phylum
    {
        public static readonly int Experimental = Vocabulary.GetOrAdd(3, "Experimental");
    }

    public static class Class
    {
        public static readonly int Chemistry = Vocabulary.GetOrAdd(4, "Chemistry");
        public static readonly int Laboratory = Vocabulary.GetOrAdd(4, "Laboratory");
    }

    public static class Order
    {
        public static readonly int Elemental = Vocabulary.GetOrAdd(5, "Elemental");
        public static readonly int ResearchFacility = Vocabulary.GetOrAdd(5, "ResearchFacility");
    }

    public static class Family
    {
        public static readonly int Materials = Vocabulary.GetOrAdd(6, "Materials");
        public static readonly int Instrumentation = Vocabulary.GetOrAdd(6, "Instrumentation");
    }

    public static class Genus
    {
        public static readonly int ChemistryCapability = Vocabulary.GetOrAdd(7, "ChemistryCapability");
        public static readonly int ScientificFacility = Vocabulary.GetOrAdd(7, "ScientificFacility");
    }

    public static OntologySignature Chemistry(int species = 2215) =>
        new(
            Paradigm.Workshop,
            Domain.Science,
            Kingdom.PhysicalScience,
            Phylum.Experimental,
            Class.Chemistry,
            Order.Elemental,
            Family.Materials,
            Genus.ChemistryCapability,
            species);

    public static OntologySignature Laboratory(int species = 2216) =>
        new(
            Paradigm.Workshop,
            Domain.Science,
            Kingdom.PhysicalScience,
            Phylum.Experimental,
            Class.Laboratory,
            Order.ResearchFacility,
            Family.Instrumentation,
            Genus.ScientificFacility,
            species);

    /// <summary>Returns ProtocolAI's retained semantic definition for one ontology layer.</summary>
    public static string DescribeLayer(int layer) => Vocabulary.DescribeLayer(layer);
}
