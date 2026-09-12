using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>Marks an Experience that is explicitly admitted to the Idle host.</summary>
public interface IIdleExperience : IExperience;

/// <summary>Global authored MicroBundle definitions indexed by integer identity and every ontology layer.</summary>
public sealed class MicroBundleRegistry
{
    private readonly Dictionary<ulong, IMicroBundle> _byId = new();
    private readonly Dictionary<ulong, List<IMicroBundle>> _byOntology = new();
    private readonly Dictionary<int, Dictionary<int, List<IMicroBundle>>> _byLayer =
        Enumerable.Range(0, OntologySignature.LayerCount)
            .ToDictionary(layer => layer, _ => new Dictionary<int, List<IMicroBundle>>());

    public int Count => _byId.Count;

    public void Register(IMicroBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);
        if (_byId.ContainsKey(bundle.Id))
            throw new InvalidOperationException($"MicroBundle {bundle.Id} is already registered.");

        _byId.Add(bundle.Id, bundle);
        var ontologyId = bundle.Ontology.StructuralId;
        if (!_byOntology.TryGetValue(ontologyId, out var exactMatches))
        {
            exactMatches = new List<IMicroBundle>();
            _byOntology.Add(ontologyId, exactMatches);
        }
        exactMatches.Add(bundle);

        for (var layer = 0; layer < OntologySignature.LayerCount; layer++)
        {
            var token = bundle.Ontology[layer];
            var index = _byLayer[layer];
            if (!index.TryGetValue(token, out var matches))
            {
                matches = new List<IMicroBundle>();
                index.Add(token, matches);
            }
            matches.Add(bundle);
        }
    }

    public bool TryGet(ulong id, out IMicroBundle? bundle) => _byId.TryGetValue(id, out bundle);

    public IReadOnlyList<IMicroBundle> FindByOntology(OntologySignature ontology)
        => _byOntology.TryGetValue(ontology.StructuralId, out var matches) ? matches : Array.Empty<IMicroBundle>();

    public IReadOnlyList<IMicroBundle> FindByOntologyLayer(int layer, int token)
    {
        if (layer is < 0 or >= OntologySignature.LayerCount)
            throw new ArgumentOutOfRangeException(nameof(layer));
        return _byLayer[layer].TryGetValue(token, out var matches)
            ? matches
            : Array.Empty<IMicroBundle>();
    }
}

/// <summary>Global Experience definitions available to the Hub.</summary>
public sealed class ExperienceRegistry
{
    private readonly Dictionary<ulong, IExperience> _experiences = new();
    public int Count => _experiences.Count;

    public void Register(IExperience experience)
    {
        ArgumentNullException.ThrowIfNull(experience);
        if (_experiences.ContainsKey(experience.Id))
            throw new InvalidOperationException($"Experience {experience.Id} is already registered.");
        _experiences.Add(experience.Id, experience);
    }

    public bool TryGet(ulong id, out IExperience? experience) => _experiences.TryGetValue(id, out experience);
}

/// <summary>Outcome of admitting an Experience into a Hub runtime.</summary>
public sealed record ExperienceLoadResult(
    bool IsLoadable,
    bool IsIdleCompatible,
    IExperience? Experience,
    IReadOnlyList<IMicroBundle> Bundles,
    IReadOnlyList<ulong> DependencyOrder,
    IReadOnlyList<string> Errors)
{
    public bool IsReady => IsLoadable;
}

/// <summary>Resolves, validates, arbitrates and returns the compact active MicroBundle closure.</summary>
public sealed class ExperienceLoader
{
    private const int MaxArbitrationRounds = 10;
    private readonly ExperienceRegistry _experiences;
    private readonly MicroBundleRegistry _bundles;

    public ExperienceLoader(ExperienceRegistry experiences, MicroBundleRegistry bundles)
    {
        _experiences = experiences ?? throw new ArgumentNullException(nameof(experiences));
        _bundles = bundles ?? throw new ArgumentNullException(nameof(bundles));
    }

    public ExperienceLoadResult Load(ulong experienceId, bool requireIdle = false)
    {
        if (!_experiences.TryGet(experienceId, out var experience) || experience is null)
            return Failure(null, false, "Experience is not registered.");

        var idleCompatible = experience is IIdleExperience;
        if (experience.MicroBundleIds.Count == 0)
            return Failure(experience, idleCompatible, "Experience has no MicroBundles.");
        if (experience.SensorySystems.Distinct().Count() != experience.SenseCount)
            return Failure(experience, idleCompatible, "Experience SenseCount does not match its distinct sensory systems.");
        if (experience.ProcessingGroups.Count == 0)
            return Failure(experience, idleCompatible, "Experience exposes no processing groups.");
        if (requireIdle && !idleCompatible)
            return Failure(experience, false, "Experience is not admitted to Idle mode.");

        var errors = new List<string>();
        var resolved = new Dictionary<ulong, IMicroBundle>();
        foreach (var id in experience.MicroBundleIds)
        {
            if (!_bundles.TryGet(id, out var bundle) || bundle is null)
            {
                errors.Add($"MicroBundle {id} is not registered.");
                continue;
            }
            if (!SharesParentOntology(experience.Ontology, bundle.Ontology))
                errors.Add($"MicroBundle {id} does not inherit the Experience ontology through layer 8.");
            resolved[id] = bundle;
        }

        var order = new List<ulong>();
        var visiting = new HashSet<ulong>();
        var visited = new HashSet<ulong>();
        foreach (var id in experience.MicroBundleIds)
            Visit(id, resolved, visiting, visited, order, errors);

        if (errors.Count > 0)
            return new ExperienceLoadResult(false, idleCompatible, experience, resolved.Values.ToArray(), order, errors);

        var arbitrator = new RuntimeArbitrator();
        foreach (var id in order)
            arbitrator.LoadBundle(resolved[id]);
        for (var round = 0; round < MaxArbitrationRounds; round++)
            if (arbitrator.ExecuteArbitrationPipeline() == 0)
                break;

        return new ExperienceLoadResult(true, idleCompatible, experience,
            order.Select(id => resolved[id]).ToArray(), order, Array.Empty<string>());
    }

    private static bool SharesParentOntology(OntologySignature experience, OntologySignature bundle)
    {
        for (var layer = 0; layer < OntologySignature.LayerCount - 1; layer++)
            if (experience[layer] != bundle[layer])
                return false;
        return true;
    }

    private static void Visit(ulong id, IReadOnlyDictionary<ulong, IMicroBundle> resolved,
        HashSet<ulong> visiting, HashSet<ulong> visited, List<ulong> order, List<string> errors)
    {
        if (visited.Contains(id) || !resolved.TryGetValue(id, out var bundle))
            return;
        if (!visiting.Add(id))
        {
            errors.Add($"MicroBundle dependency cycle detected at {id}.");
            return;
        }
        foreach (var dependency in bundle.Dependencies)
        {
            if (!resolved.ContainsKey(dependency))
                errors.Add($"MicroBundle {id} depends on unresolved MicroBundle {dependency}.");
            else
                Visit(dependency, resolved, visiting, visited, order, errors);
        }
        visiting.Remove(id);
        visited.Add(id);
        order.Add(id);
    }

    private static ExperienceLoadResult Failure(IExperience? experience, bool idleCompatible, string error)
        => new(false, idleCompatible, experience, Array.Empty<IMicroBundle>(), Array.Empty<ulong>(), [error]);

    private sealed class RuntimeArbitrator : IArbitrator
    {
        private readonly List<IMicroBundle> _loaded = new();
        public IReadOnlyCollection<IMicroBundle> LoadedBundles => _loaded;
        public bool LoadBundle(IMicroBundle bundle)
        {
            if (_loaded.Any(existing => existing.Id == bundle.Id))
                return false;
            _loaded.Add(bundle);
            return true;
        }
        public int ExecuteArbitrationPipeline()
        {
            var changed = 0;
            foreach (var bundle in _loaded)
                if (bundle.Arbitrate(this, 0))
                    changed++;
            return changed == 0 ? 0 : 1;
        }
    }
}
