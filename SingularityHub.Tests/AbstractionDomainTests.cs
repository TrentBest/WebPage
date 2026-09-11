using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 1: immutable contracts and value objects beneath Hub domains.</summary>
public sealed class AbstractionDomainTests
{
    [ArchitectureTest(1, 1, 1)]
    [Fact(DisplayName = "1.01.001 — Ontology_Has_Exactly_Nine_Layers")]
    public void Ontology_Has_Exactly_Nine_Layers()
        => Assert.Equal(9, OntologySignature.LayerCount);

    [ArchitectureTest(1, 1, 2)]
    [Fact(DisplayName = "1.01.002 — Ontology_Equality_Uses_All_Layers")]
    public void Ontology_Equality_Uses_All_Layers()
    {
        var baseline = new OntologySignature(1,2,3,4,5,6,7,8,9);
        for (var changedLayer = 0; changedLayer < 9; changedLayer++)
        {
            var values = Enumerable.Range(1, 9).ToArray();
            values[changedLayer]++;
            var changed = new OntologySignature(values[0],values[1],values[2],values[3],values[4],values[5],values[6],values[7],values[8]);
            Assert.NotEqual(baseline, changed);
        }
    }

    [ArchitectureTest(1, 1, 3)]
    [Fact(DisplayName = "1.01.003 — Ontology_Rejects_Invalid_Layer_Access")]
    public void Ontology_Rejects_Invalid_Layer_Access()
    {
        var signature = new OntologySignature(1,2,3,4,5,6,7,8,9);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = signature[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = signature[9]);
    }

    [ArchitectureTest(1, 1, 4)]
    [Fact(DisplayName = "1.01.004 — Ontology_Provides_Deterministic_Structural_Id")]
    public void Ontology_Provides_Deterministic_Structural_Id()
    {
        var a = new OntologySignature(1,2,3,4,5,6,7,8,9, 2, 11);
        var b = new OntologySignature(1,2,3,4,5,6,7,8,9, 2, 11);
        Assert.Equal(a.StructuralId, b.StructuralId);
    }

    [ArchitectureTest(1, 1, 5)]
    [Fact(DisplayName = "1.01.005 — Ontology_Version_And_Capabilities_Affect_Identity")]
    public void Ontology_Version_And_Capabilities_Affect_Identity()
    {
        var baseline = new OntologySignature(1,2,3,4,5,6,7,8,9);
        Assert.NotEqual(baseline, new OntologySignature(1,2,3,4,5,6,7,8,9, 2));
        Assert.NotEqual(baseline, new OntologySignature(1,2,3,4,5,6,7,8,9, 1, 1));
    }

    [ArchitectureTest(1, 1, 6)]
    [Fact(DisplayName = "1.01.006 — BundleVersion_Is_Value_Semantic")]
    public void BundleVersion_Is_Value_Semantic()
        => Assert.Equal(new BundleVersion(1,2,3), new BundleVersion(1,2,3));

    [ArchitectureTest(1, 1, 7)]
    [Fact(DisplayName = "1.01.007 — WorkDescriptor_Is_Value_Semantic")]
    public void WorkDescriptor_Is_Value_Semantic()
        => Assert.Equal(new WorkDescriptor(1,2,3), new WorkDescriptor(1,2,3));

    [ArchitectureTest(1, 1, 8)]
    [Fact(DisplayName = "1.01.008 — CompletionToken_Represents_Completion")]
    public void CompletionToken_Represents_Completion()
    {
        var incomplete = new CompletionToken(10, false);
        var complete = new CompletionToken(10, true);
        Assert.False(incomplete.Completed);
        Assert.True(complete.Completed);
        Assert.NotEqual(incomplete, complete);
    }

    [ArchitectureTest(1, 1, 9)]
    [Fact(DisplayName = "1.01.009 — WarehouseAddress_Is_Value_Semantic")]
    public void WarehouseAddress_Is_Value_Semantic()
        => Assert.Equal(new WarehouseAddress(42), new WarehouseAddress(42));
}
