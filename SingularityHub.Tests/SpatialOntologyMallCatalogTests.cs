using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialOntologyMallCatalogTests
{
    [Fact(DisplayName = "Ontology Mall catalog presents Experiences and MicroBundles separately")]
    public void OntologyMall_SeparatesExperiencesAndMicroBundles()
    {
        var catalog = SpatialOntologyMallCatalog.CreateDefault();

        Assert.NotEmpty(catalog.OfKind(SpatialOntologyMallKind.Experience));
        Assert.NotEmpty(catalog.OfKind(SpatialOntologyMallKind.MicroBundle));
        Assert.Contains(catalog.Entries, x => x.Id == "microbundle.sailing");
        Assert.Contains(catalog.Entries, x => x.Id == "microbundle.solar-system");
    }

    [Fact(DisplayName = "Ontology Mall catalog reserves domains for future vehicle Experiences")]
    public void OntologyMall_ReservesAirMaritimeAndSpaceDomains()
    {
        var catalog = SpatialOntologyMallCatalog.CreateDefault();

        Assert.Contains(catalog.Entries, x => x.Id == "domain.air");
        Assert.Contains(catalog.Entries, x => x.Id == "domain.maritime");
        Assert.Contains(catalog.Entries, x => x.Id == "domain.space");
    }
}
