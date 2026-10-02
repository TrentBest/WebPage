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

        var chemistryBundle = (IMicroBundle)chemistry;
        var laboratoryBundle = (IMicroBundle)laboratory;

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

        var chemistryBundle = (IMicroBundle)chemistry;
        var laboratoryBundle = (IMicroBundle)laboratory;

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
}
