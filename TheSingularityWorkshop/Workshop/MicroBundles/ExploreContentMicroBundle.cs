using System;
using System.Collections.Generic;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Nested MicroBundle tree for the Explore Experience.
///
/// This is the first extraction boundary for the large Explore surface. The page
/// remains the browser presentation host, while the content taxonomy becomes
/// independently identifiable and schedulable. Domain bundles may then be
/// migrated out of the page one capability at a time without changing the
/// visitor-facing route.
/// </summary>
public sealed class ExploreContentMicroBundle : IDisposable, IMicroBundle
{
    public const int BundleId = 2300;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public ExploreContentMicroBundle()
    {
        _lifecycle = new MicroBundle(
            BundleId,
            "EXPLORE CONTENT",
            new WebMicroBundleProvider());

        Spatial = new ExploreDomainMicroBundle(
            2310,
            "SPATIAL WORKSHOP",
            "The navigable world, visitor movement, rooms, maps, and spatial presentation.",
            _lifecycle.Id,
            new ExploreContentLeafMicroBundle(
                2311,
                "WORLD",
                "The public Workshop world and its spatial scene graph.",
                ["experience", "engineering", "creation", "unknown"],
                ["world-map", "scene", "spatial-navigation"]),
            new ExploreContentLeafMicroBundle(
                2312,
                "NAVIGATION",
                "Visitor locomotion and entrance/exit movement.",
                ["experience", "engineering", "creation", "unknown"],
                ["avatar", "movement", "entrances"]),
            new ExploreContentLeafMicroBundle(
                2313,
                "CONFERENCE",
                "Presence and conference-room participation.",
                ["conference"],
                ["presence", "membership", "remote-participants"]));

        Aec = new ExploreDomainMicroBundle(
            2320,
            "AEC",
            "AEC-oriented creation, BIM exploration, remote consultation, and generated structures.",
            _lifecycle.Id,
            new ExploreContentLeafMicroBundle(
                2321,
                "REMOTE OFFICE",
                "Spatial drafting and remote AEC consultation.",
                ["aec-remote-office", "aec-lobby"],
                ["drafting", "snap", "trade", "consultation"]),
            new ExploreContentLeafMicroBundle(
                2322,
                "BIM",
                "Ontology-backed building information exploration and editing.",
                ["aec-bim"],
                ["ontology", "rooms", "building-model"]),
            new ExploreContentLeafMicroBundle(
                2323,
                "GENERATED STRUCTURE",
                "Generated architectural structures and their plan representation.",
                ["aec-generated-structure", "aec-plan"],
                ["generation", "plan", "interactable"]));

        Laboratory = new ExploreDomainMicroBundle(
            2330,
            "SINGULARITY LABORATORY",
            "Access control, security, experiments, rendering research, and laboratory interiors.",
            _lifecycle.Id,
            new ExploreContentLeafMicroBundle(
                2331,
                "ACCESS",
                "Laboratory reception, elevators, identity checks, and floor challenges.",
                ["singularity-lab", "singularity-lab-plan"],
                ["access", "identity", "elevator", "floor-challenge"]),
            new ExploreContentLeafMicroBundle(
                2332,
                "SECURITY",
                "The physical security checkpoint and VIP route.",
                ["singularity-lab-entrance"],
                ["checkpoint", "screening", "queue", "vip-route"]),
            new ExploreContentLeafMicroBundle(
                2333,
                "HOLODECK",
                "Playable authored maze and holodeck presentation.",
                ["lab-holodeck", "maze"],
                ["maze", "leaderboard", "gameplay"]),
            new ExploreContentLeafMicroBundle(
                2334,
                "RENDERING RESEARCH",
                "The laboratory's renderer-facing substrate research station.",
                ["singularity-lab-research"],
                ["voxel-substrate", "render-intent", "camera"]),
            new ExploreContentLeafMicroBundle(
                2335,
                "SOFTWARE OFFICE",
                "A software-company experience that exposes the FSM Forge lifecycle.",
                ["singularity-software-office"],
                ["fsm-scaffold", "behavior-builder", "company"]));

        City = new ExploreDomainMicroBundle(
            2340,
            "SINGULARITY CITY",
            "Civic, residential, construction, transit, and island experiences.",
            _lifecycle.Id,
            new ExploreContentLeafMicroBundle(
                2341,
                "MANSION",
                "A mutable residential environment with civic interaction.",
                ["mansion"],
                ["architecture", "mayor", "civic-office"]),
            new ExploreContentLeafMicroBundle(
                2342,
                "CONSTRUCTION",
                "A living construction site whose definitions can become future spaces.",
                ["construction-site"],
                ["construction", "tour", "building-program"]),
            new ExploreContentLeafMicroBundle(
                2343,
                "CIVIC",
                "Government and civic participation surfaces.",
                ["singularity-capitol"],
                ["government", "digiten", "election"]),
            new ExploreContentLeafMicroBundle(
                2344,
                "TRANSIT",
                "Transit station, train model, destinations, and the transit micro-game.",
                ["singularity-transit", "singularity-station"],
                ["train", "dispatch", "boarding", "micro-game"]),
            new ExploreContentLeafMicroBundle(
                2345,
                "ISLAND",
                "A spatial island and its discoverable attractions.",
                ["singularity-island"],
                ["attractions", "travel", "world"]));

        Creation = new ExploreDomainMicroBundle(
            2350,
            "CREATION",
            "The Forge, linework, software patterns, and behavior authoring.",
            _lifecycle.Id,
            new ExploreContentLeafMicroBundle(
                2351,
                "FSM FORGE",
                "The Workshop's executable state-machine authoring surface.",
                ["forge"],
                ["fsm", "lifecycle", "runtime", "preview"]),
            new ExploreContentLeafMicroBundle(
                2352,
                "LINE WORKSHOP",
                "Renderer-neutral linework authoring and presentation.",
                ["line-lab"],
                ["linework", "style", "drawing"]),
            new ExploreContentLeafMicroBundle(
                2353,
                "SOFTWARE PATTERNS",
                "Nested creational, structural, and behavioral pattern bundles.",
                ["software-patterns"],
                ["creational", "structural", "behavioral", "annotation"]),
            new ExploreContentLeafMicroBundle(
                2354,
                "BEHAVIOR BUILDER",
                "Human-authored behavior branches and FSM lifecycle scaffolding.",
                ["singularity-software-office"],
                ["on-enter", "on-update", "on-exit", "condition"]));

        Ai = new ExploreDomainMicroBundle(
            2360,
            "AI / LANGUAGE",
            "Protocol-aware context exchange, grammar, and visitor-directed Workshop operations.",
            _lifecycle.Id,
            new ExploreContentLeafMicroBundle(
                2361,
                "WORKSHOP AI TERMINAL",
                "Context export and controlled map operations through the Workshop protocol boundary.",
                ["workshop-map", "settings"],
                ["context", "protocol", "grammar", "controlled-operations"]));

        Domains =
        [
            Spatial,
            Aec,
            Laboratory,
            City,
            Creation,
            Ai
        ];
    }

    public int Id => _lifecycle.Id;
    public IStateContext Context => _lifecycle.Context;
    public IReadOnlyList<ExploreDomainMicroBundle> Domains { get; }
    public ExploreDomainMicroBundle Spatial { get; }
    public ExploreDomainMicroBundle Aec { get; }
    public ExploreDomainMicroBundle Laboratory { get; }
    public ExploreDomainMicroBundle City { get; }
    public ExploreDomainMicroBundle Creation { get; }
    public ExploreDomainMicroBundle Ai { get; }

    public IReadOnlyList<ExploreContentLeafMicroBundle> Leaves
    {
        get
        {
            var leaves = new List<ExploreContentLeafMicroBundle>();
            foreach (var domain in Domains)
                leaves.AddRange(domain.Leaves);
            return leaves;
        }
    }

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
        foreach (var domain in Domains)
            domain.Update();
    }

    public void Invalidate()
    {
        if (_disposed) return;
        _lifecycle.Invalidate();
        foreach (var domain in Domains)
            domain.Invalidate();
    }

    public void Dispose()
    {
        if (_disposed) return;

        for (var index = Domains.Count - 1; index >= 0; index--)
            Domains[index].Dispose();

        _lifecycle.Dispose();
        _disposed = true;
    }
}

/// <summary>
/// A first-level child in the Explore content hierarchy. Each domain owns
/// focused leaf MicroBundles rather than allowing the page to become the
/// identity of an entire subsystem.
/// </summary>
public sealed class ExploreDomainMicroBundle : IDisposable, IMicroBundle
{
    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public ExploreDomainMicroBundle(
        int id,
        string name,
        string description,
        int parentId,
        params ExploreContentLeafMicroBundle[] leaves)
    {
        _lifecycle = new MicroBundle(
            id,
            name,
            new WebMicroBundleProvider(),
            parentId,
            generation: 1);

        Description = description;
        Leaves = leaves ?? throw new ArgumentNullException(nameof(leaves));

        foreach (var leaf in Leaves)
            leaf.SetParent(id);
    }

    public int Id => _lifecycle.Id;
    public IStateContext Context => _lifecycle.Context;
    public string Name => ((MicroBundleContext)_lifecycle.Context).Name;
    public string Description { get; }
    public IReadOnlyList<ExploreContentLeafMicroBundle> Leaves { get; }

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
        foreach (var leaf in Leaves)
            leaf.Update();
    }

    public void Invalidate()
    {
        if (_disposed) return;
        _lifecycle.Invalidate();
        foreach (var leaf in Leaves)
            leaf.Invalidate();
    }

    public void Dispose()
    {
        if (_disposed) return;

        for (var index = Leaves.Count - 1; index >= 0; index--)
            Leaves[index].Dispose();

        _lifecycle.Dispose();
        _disposed = true;
    }
}

/// <summary>
/// Leaf MicroBundle describing one focused Explore capability. The current
/// page still owns the presentation methods; this leaf establishes the stable
/// identity and lifecycle boundary those methods can migrate behind.
/// </summary>
public sealed class ExploreContentLeafMicroBundle : IDisposable, IMicroBundle
{
    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public ExploreContentLeafMicroBundle(
        int id,
        string name,
        string description,
        IReadOnlyList<string> roomIds,
        IReadOnlyList<string> capabilities)
    {
        _lifecycle = new MicroBundle(
            id,
            name,
            new WebMicroBundleProvider(),
            parentId: -1,
            generation: 2);

        Description = description;
        RoomIds = roomIds;
        Capabilities = capabilities;
    }

    public int Id => _lifecycle.Id;
    public IStateContext Context => _lifecycle.Context;
    public string Name => ((MicroBundleContext)_lifecycle.Context).Name;
    public string Description { get; }
    public IReadOnlyList<string> RoomIds { get; }
    public IReadOnlyList<string> Capabilities { get; }

    internal void SetParent(int parentId)
    {
        if (_lifecycle.Context is MicroBundleContext context)
            context.ParentId = parentId;
    }

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
    }

    public void Invalidate()
    {
        if (_disposed) return;
        _lifecycle.Invalidate();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _lifecycle.Dispose();
        _disposed = true;
    }
}
