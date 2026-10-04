using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Experiences;

namespace TheSingularityWorkshop.Workshop.Composition;

/// <summary>
/// Resolves Workshop Experience contracts by stable identity so discovery and educational
/// surfaces do not need page-specific type checks.
/// </summary>
public sealed class WorkshopExperienceCatalog
{
    private readonly IReadOnlyDictionary<ulong, IExperience> _experiences;

    public WorkshopExperienceCatalog()
    {
        var livingGui = new LivingGuiExperience();

        _experiences = new Dictionary<ulong, IExperience>
        {
            [livingGui.Id] = livingGui
        };
    }

    /// <summary>Attempts to resolve an Experience by its stable identity.</summary>
    public bool TryResolve(ulong experienceId, out IExperience? experience)
        => _experiences.TryGetValue(experienceId, out experience);

    /// <summary>Returns Experiences currently discoverable by the Workshop host.</summary>
    public IReadOnlyCollection<IExperience> Experiences => _experiences.Values;
}
