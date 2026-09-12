using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.FSMLayer;

/// <summary>
/// A development-time MicroBundle that may contain other MicroBundles recursively.
/// A parent is itself a bundle; its children are implementation material that can be
/// independently addressed and reused by other parents.
/// </summary>
public interface IRecursiveMicroBundle : IMicroBundle
{
    IReadOnlyList<ulong> ChildMicroBundleIds { get; }
}

/// <summary>
/// Generic recursive MicroBundle definition used while constructing a static runtime graph.
/// </summary>
public sealed class RecursiveMicroBundle : IRecursiveMicroBundle
{
    public RecursiveMicroBundle(
        ulong id,
        OntologySignature ontology,
        IReadOnlyList<ulong>? dependencies = null,
        IReadOnlyList<ulong>? childMicroBundleIds = null,
        BundleVersion? version = null)
    {
        Id = id;
        Ontology = ontology;
        Dependencies = dependencies ?? Array.Empty<ulong>();
        ChildMicroBundleIds = childMicroBundleIds ?? Array.Empty<ulong>();
        Version = version ?? new BundleVersion(1, 0, 0);
    }

    public ulong Id { get; }
    public OntologySignature Ontology { get; }
    public BundleVersion Version { get; }
    public IReadOnlyList<ulong> Dependencies { get; }
    public IReadOnlyList<ulong> ChildMicroBundleIds { get; }

    public bool Arbitrate(IArbitrator arbitrator, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(arbitrator);
        return false;
    }
}

/// <summary>Immutable result of development-time recursive composition.</summary>
public sealed class CompiledMicroBundleGraph
{
    internal CompiledMicroBundleGraph(ulong rootId, IReadOnlyList<IMicroBundle> bundles, IReadOnlyList<ulong> order)
    {
        RootId = rootId;
        Bundles = bundles;
        Order = order;
    }

    public ulong RootId { get; }
    public IReadOnlyList<IMicroBundle> Bundles { get; }
    public IReadOnlyList<ulong> Order { get; }
}

/// <summary>
/// Development-time compiler. Recursion is resolved once; runtime receives the flattened,
/// deterministic closure and never performs recursive discovery.
/// </summary>
public static class MicroBundleCompiler
{
    public static CompiledMicroBundleGraph Compile(
        ulong rootId,
        IReadOnlyDictionary<ulong, IRecursiveMicroBundle> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        if (!definitions.TryGetValue(rootId, out var root))
            throw new InvalidOperationException($"Root MicroBundle {rootId} is not registered.");

        var resolved = new Dictionary<ulong, IMicroBundle>();
        var visiting = new HashSet<ulong>();
        var visited = new HashSet<ulong>();
        var order = new List<ulong>();

        Visit(root, definitions, resolved, visiting, visited, order);
        return new CompiledMicroBundleGraph(rootId, order.Select(id => resolved[id]).ToArray(), order.ToArray());
    }

    private static void Visit(
        IRecursiveMicroBundle bundle,
        IReadOnlyDictionary<ulong, IRecursiveMicroBundle> definitions,
        IDictionary<ulong, IMicroBundle> resolved,
        ISet<ulong> visiting,
        ISet<ulong> visited,
        IList<ulong> order)
    {
        if (visited.Contains(bundle.Id))
            return;
        if (!visiting.Add(bundle.Id))
            throw new InvalidOperationException($"Recursive MicroBundle cycle detected at {bundle.Id}.");

        foreach (var childId in bundle.ChildMicroBundleIds)
        {
            if (!definitions.TryGetValue(childId, out var child))
                throw new InvalidOperationException(
                    $"MicroBundle {bundle.Id} contains unresolved child MicroBundle {childId}.");
            Visit(child, definitions, resolved, visiting, visited, order);
        }

        visiting.Remove(bundle.Id);
        visited.Add(bundle.Id);
        resolved[bundle.Id] = bundle;
        order.Add(bundle.Id);
    }
}
