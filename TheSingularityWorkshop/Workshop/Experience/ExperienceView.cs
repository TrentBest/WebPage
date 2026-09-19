namespace TheSingularityWorkshop.Workshop.Experience;

/// <summary>Renderer-neutral view currently presented by an Experience.</summary>
public interface IExperienceView
{
}

/// <summary>
/// The Workshop's intentional close-up view of an Experience.
/// Entering an interactable does not replace the Workshop with an arbitrary
/// page; the Workshop asks the Experience for its floor plan and renders that.
/// </summary>
public sealed record FloorPlan(ExperienceSpace Space) : IExperienceView
{
    public string ExperienceId => Space.Id;
}

/// <summary>Small renderer-neutral view state used by an Experience host.</summary>
public sealed class ExperienceViewState
{
    private IExperienceView? _current;

    public IExperienceView? Current => _current;

    public void SetView(IExperienceView view)
        => _current = view ?? throw new ArgumentNullException(nameof(view));

    public T? GetView<T>() where T : class, IExperienceView
        => _current as T;

    public void Clear() => _current = null;
}
