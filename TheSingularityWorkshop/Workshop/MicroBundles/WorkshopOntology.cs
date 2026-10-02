namespace TheSingularityWorkshop.Workshop.MicroBundles;

using TheSingularityWorkshop.SingularityHub;

/// <summary>
/// Canonical integer ontology coordinates for Workshop MicroBundles.
/// Coordinates are deliberately independent per layer: the same broad scientific
/// buckets can be shared by many bundles while deeper layers narrow discovery.
/// The ninth coordinate remains the concrete species/variant identity.
/// </summary>
public static class WorkshopOntology
{
    public static class Paradigm
    {
        public const int Workshop = 1;
    }

    public static class Domain
    {
        public const int Science = 2;
    }

    public static class Kingdom
    {
        public const int PhysicalScience = 3;
    }

    public static class Phylum
    {
        public const int Experimental = 4;
    }

    public static class Class
    {
        public const int Chemistry = 5;
        public const int Laboratory = 6;
    }

    public static class Order
    {
        public const int Elemental = 7;
        public const int ResearchFacility = 8;
    }

    public static class Family
    {
        public const int Materials = 9;
        public const int Instrumentation = 10;
    }

    public static class Genus
    {
        public const int ChemistryCapability = 11;
        public const int ScientificFacility = 12;
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
}
