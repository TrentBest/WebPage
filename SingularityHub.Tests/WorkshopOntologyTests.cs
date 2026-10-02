using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopOntologyTests
{
    [Fact]
    public void ScientificBundlesPublishDistinctCanonicalNineLayerCoordinates()
    {
        using var chemistry = new ChemistryMicroBundle();
        using var laboratory = new SingularityLaboratoryMicroBundle();

        var chemistryBundle = (TheSingularityWorkshop.Workshop.MicroBundles.IMicroBundle)chemistry;
        var laboratoryBundle = (TheSingularityWorkshop.Workshop.MicroBundles.IMicroBundle)laboratory;

        Assert.Equal(9, OntologySignature.LayerCount);
        Assert.Equal(WorkshopOntology.Chemistry(ChemistryMicroBundle.BundleId), chemistryBundle.Ontology);
        Assert.Equal(WorkshopOntology.Laboratory(SingularityLaboratoryMicroBundle.BundleId), laboratoryBundle.Ontology);
        Assert.NotEqual(chemistryBundle.Ontology, laboratoryBundle.Ontology);
        Assert.Equal(ChemistryMicroBundle.BundleId, chemistryBundle.Ontology[8]);
        Assert.Equal(SingularityLaboratoryMicroBundle.BundleId, laboratoryBundle.Ontology[8]);
    }

    [Fact]
    public void RegistryCanDiscoverScientificBundlesThroughSharedOntologyLayers()
    {
        using var chemistry = new ChemistryMicroBundle();
        using var laboratory = new SingularityLaboratoryMicroBundle();
        var registry = new MicroBundleRegistry();

        var chemistryBundle = (TheSingularityWorkshop.Workshop.MicroBundles.IMicroBundle)chemistry;
        var laboratoryBundle = (TheSingularityWorkshop.Workshop.MicroBundles.IMicroBundle)laboratory;

        Assert.True(registry.TryPublish(
            MicroBundleAddress.Create(chemistryBundle.Ontology, ChemistryMicroBundle.BundleId), chemistryBundle));
        Assert.True(registry.TryPublish(
            MicroBundleAddress.Create(laboratoryBundle.Ontology, SingularityLaboratoryMicroBundle.BundleId), laboratoryBundle));

        var science = registry.GetByLayer(1, WorkshopOntology.Domain.Science);
        var physicalScience = registry.GetByLayer(2, WorkshopOntology.Kingdom.PhysicalScience);

        Assert.Equal(2, science.Count);
        Assert.Equal(2, physicalScience.Count);
        Assert.Contains(chemistryBundle, science);
        Assert.Contains(laboratoryBundle, science);
    }

    [Fact]
    public void CanonicalCoordinatesAreDerivedFromRetainedProtocolAiVocabulary()
    {
        Assert.Equal(OntologyVocabulary.StableToken("Workshop"), WorkshopOntology.Paradigm.Workshop);
        Assert.Equal(OntologyVocabulary.StableToken("Science"), WorkshopOntology.Domain.Science);
        Assert.Equal(OntologyVocabulary.StableToken("PhysicalScience"), WorkshopOntology.Kingdom.PhysicalScience);
        Assert.Equal(OntologyVocabulary.StableToken("Chemistry"), WorkshopOntology.Class.Chemistry);
        Assert.Equal(OntologyVocabulary.StableToken("Laboratory"), WorkshopOntology.Class.Laboratory);

        var domainDescription = WorkshopOntology.DescribeLayer(1);
        var classDescription = WorkshopOntology.DescribeLayer(4);

        Assert.Contains("Science", domainDescription);
        Assert.Contains(WorkshopOntology.Domain.Science.ToString(), domainDescription);
        Assert.Contains("Chemistry", classDescription);
        Assert.Contains("Laboratory", classDescription);
    }
}
