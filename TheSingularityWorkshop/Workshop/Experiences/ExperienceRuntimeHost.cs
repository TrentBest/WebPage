namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// Retains the resolved MicroBundle closure for each active Experience.
/// Definitions remain in the global registries; this host owns the compact runtime set.
/// </summary>
public sealed class ExperienceRuntimeHost
{
    private readonly ExperienceLoader _loader;
    private readonly Dictionary<ulong, ExperienceLoadResult> _active = new();

    public ExperienceRuntimeHost(ExperienceLoader loader)
    {
        _loader = loader ?? throw new ArgumentNullException(nameof(loader));
    }

    public IReadOnlyCollection<ExperienceLoadResult> ActiveExperiences => _active.Values;

    public bool IsActive(ulong experienceId) => _active.ContainsKey(experienceId);

    public bool TryGet(ulong experienceId, out ExperienceLoadResult? loaded)
        => _active.TryGetValue(experienceId, out loaded);

    public ExperienceLoadResult Activate(ulong experienceId, bool requireIdle = false)
    {
        var result = _loader.Load(experienceId, requireIdle);
        if (!result.IsReady || result.Experience is null)
            return result;

        _active[experienceId] = result;
        return result;
    }

    public bool Deactivate(ulong experienceId) => _active.Remove(experienceId);
}
