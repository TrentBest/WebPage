using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Exhaustive unit coverage for MicroBundleAddress validity and identity semantics.</summary>
public sealed class MicroBundleAddressTests
{
    private static OntologySignature Ontology => new(1, 2, 3, 4, 5, 6, 7, 8, 9);

    [Fact(DisplayName = "Variant zero is valid")]
    public void VariantZeroIsValid()
        => Assert.True(new MicroBundleAddress(Ontology, 0).IsValid);

    [Fact(DisplayName = "Last reserved variant is valid")]
    public void LastReservedVariantIsValid()
        => Assert.True(new MicroBundleAddress(Ontology, int.MaxValue - 1).IsValid);

    [Fact(DisplayName = "Negative variant is invalid")]
    public void NegativeVariantIsInvalid()
        => Assert.False(new MicroBundleAddress(Ontology, -1).IsValid);

    [Fact(DisplayName = "Int maximum variant is outside capacity")]
    public void IntMaximumVariantIsOutsideCapacity()
        => Assert.False(new MicroBundleAddress(Ontology, int.MaxValue).IsValid);

    [Fact(DisplayName = "Variant capacity is int maximum")]
    public void VariantCapacityIsIntMaximum()
        => Assert.Equal((long)int.MaxValue, MicroBundleAddress.VariantCapacity);

    [Fact(DisplayName = "Create accepts valid variant")]
    public void CreateAcceptsValidVariant()
    {
        var address = MicroBundleAddress.Create(Ontology, 7);
        Assert.Equal(Ontology, address.Ontology);
        Assert.Equal(7, address.VariantId);
        Assert.True(address.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void CreateRejectsInvalidVariant(int variant)
        => Assert.Throws<ArgumentOutOfRangeException>(() => MicroBundleAddress.Create(Ontology, variant));

    [Fact(DisplayName = "Same ontology and variant produce equal addresses")]
    public void SameCoordinatesAreEqual()
        => Assert.Equal(new MicroBundleAddress(Ontology, 3), new MicroBundleAddress(Ontology, 3));

    [Fact(DisplayName = "Different variants produce different structural identities")]
    public void DifferentVariantsProduceDifferentStructuralIds()
        => Assert.NotEqual(new MicroBundleAddress(Ontology, 1).StructuralId, new MicroBundleAddress(Ontology, 2).StructuralId);

    [Fact(DisplayName = "Different ontology produces different structural identity")]
    public void DifferentOntologyProducesDifferentStructuralId()
    {
        var other = new OntologySignature(9, 8, 7, 6, 5, 4, 3, 2, 1);
        Assert.NotEqual(new MicroBundleAddress(Ontology, 1).StructuralId, new MicroBundleAddress(other, 1).StructuralId);
    }
}
