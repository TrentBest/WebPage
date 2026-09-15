using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests.Ontology;

/// <summary>Complete public-surface coverage for <see cref="OntologySignature"/>.</summary>
/// <remarks>
/// Ontology coordinates are ordered Paradigm, Domain, Kingdom, Phylum, Class,
/// Order, Family, Genus, Species. Test placement describes the architectural
/// subject; it does not invent ontology values for production artifacts.
/// </remarks>
public sealed class OntologySignatureTests
{
    [ArchitectureTest(1, 1, 1)]
    [Fact(DisplayName = "1.01.001 — Ontology_Has_Exactly_Nine_Layers")]
    public void LayerCountIsNine()
        => Assert.Equal(9, OntologySignature.LayerCount);

    [ArchitectureTest(1, 1, 2)]
    [Fact(DisplayName = "1.01.002 — Ontology_Preserves_All_Nine_Coordinates")]
    public void PreservesAllNineCoordinates()
    {
        var signature = new OntologySignature(10, 20, 30, 40, 50, 60, 70, 80, 90);

        for (var layer = 0; layer < OntologySignature.LayerCount; layer++)
            Assert.Equal((layer + 1) * 10, signature[layer]);
    }

    [ArchitectureTest(1, 1, 3)]
    [Fact(DisplayName = "1.01.003 — Ontology_Rejects_Invalid_Layer_Access")]
    public void RejectsInvalidLayerAccess()
    {
        var signature = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);

        Assert.Throws<ArgumentOutOfRangeException>(() => _ = signature[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = signature[9]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = signature[int.MinValue]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = signature[int.MaxValue]);
    }

    [ArchitectureTest(1, 1, 4)]
    [Fact(DisplayName = "1.01.004 — Ontology_Exposes_Version_And_Capabilities")]
    public void ExposesVersionAndCapabilities()
    {
        var signature = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 17, 0xA5A5A5A5);

        Assert.Equal(17, signature.Version);
        Assert.Equal(0xA5A5A5A5u, signature.Capabilities);
    }

    [ArchitectureTest(1, 1, 5)]
    [Fact(DisplayName = "1.01.005 — Ontology_Equality_Uses_All_Coordinates")]
    public void EqualityUsesEveryCoordinate()
    {
        var baseline = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);

        for (var changedLayer = 0; changedLayer < OntologySignature.LayerCount; changedLayer++)
        {
            var values = Enumerable.Range(1, OntologySignature.LayerCount).ToArray();
            values[changedLayer]++;
            var changed = new OntologySignature(
                values[0], values[1], values[2], values[3], values[4],
                values[5], values[6], values[7], values[8]);

            Assert.NotEqual(baseline, changed);
            Assert.True(baseline != changed);
        }
    }

    [ArchitectureTest(1, 1, 6)]
    [Fact(DisplayName = "1.01.006 — Ontology_Equality_Includes_Version_And_Capabilities")]
    public void EqualityIncludesVersionAndCapabilities()
    {
        var baseline = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);

        Assert.NotEqual(baseline, new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 2));
        Assert.NotEqual(baseline, new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 1));
    }

    [ArchitectureTest(1, 1, 7)]
    [Fact(DisplayName = "1.01.007 — Ontology_Object_Equality_Is_Type_Safe")]
    public void ObjectEqualityIsTypeSafe()
    {
        var signature = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);

        Assert.True(signature.Equals((object)signature));
        Assert.False(signature.Equals(null));
        Assert.False(signature.Equals("not an ontology signature"));
    }

    [ArchitectureTest(1, 1, 8)]
    [Fact(DisplayName = "1.01.008 — Ontology_Equal_Values_Have_Equal_Hash_Codes")]
    public void EqualValuesHaveEqualHashCodes()
    {
        var a = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 4, 12);
        var b = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 4, 12);

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [ArchitectureTest(1, 1, 9)]
    [Fact(DisplayName = "1.01.009 — Ontology_Structural_Id_Is_Deterministic")]
    public void StructuralIdIsDeterministic()
    {
        var a = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 2, 11);
        var b = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 2, 11);

        Assert.Equal(a.StructuralId, b.StructuralId);
    }

    [ArchitectureTest(1, 1, 10)]
    [Fact(DisplayName = "1.01.010 — Ontology_Structural_Id_Tracks_Version_And_Capabilities")]
    public void StructuralIdTracksVersionAndCapabilities()
    {
        var baseline = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);

        Assert.NotEqual(baseline.StructuralId,
            new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 2).StructuralId);
        Assert.NotEqual(baseline.StructuralId,
            new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 1).StructuralId);
    }

    [ArchitectureTest(1, 1, 11)]
    [Fact(DisplayName = "1.01.011 — Ontology_Equal_Operators_Agree_With_Equality")]
    public void OperatorsAgreeWithEquality()
    {
        var a = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);
        var b = new OntologySignature(1, 2, 3, 4, 5, 6, 7, 8, 9);
        var c = new OntologySignature(9, 8, 7, 6, 5, 4, 3, 2, 1);

        Assert.True(a == b);
        Assert.False(a != b);
        Assert.False(a == c);
        Assert.True(a != c);
    }
}
