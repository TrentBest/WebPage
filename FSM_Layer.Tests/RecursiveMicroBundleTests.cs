using TheSingularityWorkshop.FSMLayer;
using TheSingularityWorkshop.SingularityHub;

namespace FSM_Layer.Tests;

public sealed class RecursiveMicroBundleTests
{
    private static readonly OntologySignature Ontology = new(1, 2, 3, 4, 5, 6, 7, 8, 9);

    [Fact]
    public void MicroBundles_Can_Contain_MicroBundles_Recursively()
    {
        var leaf = new RecursiveMicroBundle(1, Ontology);
        var child = new RecursiveMicroBundle(2, Ontology, childMicroBundleIds: [1]);
        var root = new RecursiveMicroBundle(3, Ontology, childMicroBundleIds: [2]);

        var graph = MicroBundleCompiler.Compile(3, new Dictionary<ulong, IRecursiveMicroBundle>
        {
            [1] = leaf, [2] = child, [3] = root
        });

        Assert.Equal([1UL, 2UL, 3UL], graph.Order);
        Assert.Equal(3, graph.Bundles.Count);
        Assert.Equal(3UL, graph.RootId);
    }

    [Fact]
    public void Shared_Lesser_Bundle_Is_Compiled_Once()
    {
        var leaf = new RecursiveMicroBundle(1, Ontology);
        var left = new RecursiveMicroBundle(2, Ontology, childMicroBundleIds: [1]);
        var right = new RecursiveMicroBundle(3, Ontology, childMicroBundleIds: [1]);
        var root = new RecursiveMicroBundle(4, Ontology, childMicroBundleIds: [2, 3]);

        var graph = MicroBundleCompiler.Compile(4, new Dictionary<ulong, IRecursiveMicroBundle>
        {
            [1] = leaf, [2] = left, [3] = right, [4] = root
        });

        Assert.Equal([1UL, 2UL, 3UL, 4UL], graph.Order);
        Assert.Equal(4, graph.Bundles.Select(b => b.Id).Distinct().Count());
    }

    [Fact]
    public void Recursive_Cycle_Is_Rejected_During_Development_Compilation()
    {
        var a = new RecursiveMicroBundle(1, Ontology, childMicroBundleIds: [2]);
        var b = new RecursiveMicroBundle(2, Ontology, childMicroBundleIds: [1]);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            MicroBundleCompiler.Compile(1, new Dictionary<ulong, IRecursiveMicroBundle>
            {
                [1] = a, [2] = b
            }));

        Assert.Contains("cycle", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Missing_Child_Is_Rejected_During_Development_Compilation()
    {
        var root = new RecursiveMicroBundle(1, Ontology, childMicroBundleIds: [99]);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            MicroBundleCompiler.Compile(1, new Dictionary<ulong, IRecursiveMicroBundle> { [1] = root }));

        Assert.Contains("99", exception.Message);
    }

    [Fact]
    public void Compiled_Graph_Is_Static_Runtime_Closure()
    {
        var leaf = new RecursiveMicroBundle(1, Ontology);
        var root = new RecursiveMicroBundle(2, Ontology, childMicroBundleIds: [1]);
        var definitions = new Dictionary<ulong, IRecursiveMicroBundle> { [1] = leaf, [2] = root };

        var graph = MicroBundleCompiler.Compile(2, definitions);
        definitions.Clear();

        Assert.Equal([1UL, 2UL], graph.Order);
        Assert.Equal(2, graph.Bundles.Count);
    }
}
