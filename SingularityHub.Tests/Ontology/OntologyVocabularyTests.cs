using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests.Ontology;

public sealed class OntologyVocabularyTests
{
    [Fact]
    public void RetainsStringAndIntegerCoordinatesPerLayer()
    {
        var vocabulary = new OntologyVocabulary();

        var domain = vocabulary.GetOrAdd(1, "Science");
        var classToken = vocabulary.GetOrAdd(4, "Chemistry");

        Assert.Equal(domain, OntologyVocabulary.StableToken("Science"));
        Assert.Equal(classToken, OntologyVocabulary.StableToken("Chemistry"));
        Assert.NotEqual(domain, classToken);

        Assert.True(vocabulary.TryGetToken(1, "Science", out var resolvedDomain));
        Assert.Equal(domain, resolvedDomain);

        Assert.True(vocabulary.TryGetEntry(1, domain, out var retainedDomain));
        Assert.Equal("Science", retainedDomain);

        Assert.True(vocabulary.TryGetEntry(4, classToken, out var retainedClass));
        Assert.Equal("Chemistry", retainedClass);
    }

    [Fact]
    public void SameEntryResolvesToSameStableTokenAcrossLayers()
    {
        var vocabulary = new OntologyVocabulary();

        var domainToken = vocabulary.GetOrAdd(1, "Research");
        var classToken = vocabulary.GetOrAdd(4, "Research");

        Assert.Equal(OntologyVocabulary.StableToken("Research"), domainToken);
        Assert.Equal(domainToken, classToken);
    }

    [Fact]
    public void HashCollisionIsRejectedRatherThanSilentlyAliased()
    {
        const string first = "VqM4H32Y";
        const string second = "m6nxcAO6";

        Assert.Equal(
            OntologyVocabulary.StableToken(first),
            OntologyVocabulary.StableToken(second));

        var vocabulary = new OntologyVocabulary();
        vocabulary.GetOrAdd(1, first);

        var exception = Assert.Throws<InvalidOperationException>(
            () => vocabulary.GetOrAdd(1, second));

        Assert.Contains("collision", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
