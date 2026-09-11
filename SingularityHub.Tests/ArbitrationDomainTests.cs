using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 5: arbitration. These tests make the homeostasis and audit laws executable.</summary>
public sealed class ArbitrationDomainTests
{
    [ArchitectureTest(5, 1, 1)]
    [Fact(DisplayName = "5.01.001 — Audit_Starts_Empty")]
    public void Audit_Starts_Empty()
        => Assert.Empty(new ArbitrationAudit().Events);

    [ArchitectureTest(5, 1, 2)]
    [Fact(DisplayName = "5.01.002 — Audit_Preserves_Event_Order")]
    public void Audit_Preserves_Event_Order()
    {
        var audit = new ArbitrationAudit();
        audit.Record(new ArbitrationEvent(0, 2, 20, MutationType.StructuralMutation, 0));
        audit.Record(new ArbitrationEvent(1, 1, 10, MutationType.PropertyInjection, 2));
        Assert.Equal(2, audit.Events.Count);
        Assert.Equal((ulong)2, audit.Events[0].ActorId);
        Assert.Equal((ulong)1, audit.Events[1].ActorId);
    }

    [ArchitectureTest(5, 1, 3)]
    [Fact(DisplayName = "5.01.003 — Audit_Preserves_Causal_Parent")]
    public void Audit_Preserves_Causal_Parent()
    {
        var audit = new ArbitrationAudit();
        var expected = new ArbitrationEvent(3, 5, 9, MutationType.DependencyResolution, 4);
        audit.Record(expected);
        Assert.Equal(expected, Assert.Single(audit.Events));
    }

    [ArchitectureTest(5, 1, 4)]
    [Fact(DisplayName = "5.01.004 — MutationTypes_Are_Explicit")]
    public void MutationTypes_Are_Explicit()
    {
        Assert.Equal(3, Enum.GetValues<MutationType>().Length);
        Assert.Contains(MutationType.StructuralMutation, Enum.GetValues<MutationType>());
        Assert.Contains(MutationType.PropertyInjection, Enum.GetValues<MutationType>());
        Assert.Contains(MutationType.DependencyResolution, Enum.GetValues<MutationType>());
    }

    [ArchitectureTest(5, 1, 5)]
    [Fact(DisplayName = "5.01.005 — Bundle_Contract_Contains_Identity_Ontology_Version_Dependencies")]
    public void Bundle_Contract_Contains_Identity_Ontology_Version_Dependencies()
    {
        var members = typeof(IMicroBundle).GetProperties().Select(p => p.Name).ToHashSet();
        Assert.Subset(new HashSet<string> { "Id", "Ontology", "Version", "Dependencies" }, members);
    }

    [ArchitectureTest(5, 1, 6)]
    [Fact(DisplayName = "5.01.006 — Arbitrator_Exposes_Loaded_Bundles")]
    public void Arbitrator_Exposes_Loaded_Bundles()
        => Assert.NotNull(typeof(IArbitrator).GetProperty(nameof(IArbitrator.LoadedBundles)));

    [ArchitectureTest(5, 1, 7)]
    [Fact(DisplayName = "5.01.007 — Hub_Composes_Audit_Contract")]
    public void Hub_Composes_Audit_Contract()
        => Assert.NotNull(typeof(ISingularityHub).GetProperty(nameof(ISingularityHub.Audit)));
}
