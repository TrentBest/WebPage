using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.Interaction;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Creates the renderer-neutral interactable descriptions used by spatial
/// Workshop surfaces. This is intentionally separate from any renderer so the
/// same presentation metadata can drive the world and its map.
/// </summary>
public static class WorkshopInteractableCatalog
{
    public static IReadOnlyDictionary<string, IInteractable> Create(
        IEnumerable<ExperienceSpace> spaces)
        => Create(spaces, null);

    public static IReadOnlyDictionary<string, IInteractable> Create(
        IEnumerable<ExperienceSpace> spaces,
        Action<string>? onInteract)
    {
        ArgumentNullException.ThrowIfNull(spaces);
        return spaces.ToDictionary(space => space.Id, space => Create(space, onInteract));
    }

    public static IInteractable Create(ExperienceSpace space)
        => Create(space, null);

    public static IInteractable Create(
        ExperienceSpace space,
        Action<string>? onInteract)
    {
        ArgumentNullException.ThrowIfNull(space);

        var (schematic, thread, accent) = space.Id switch
        {
            "engineering" => ("forge", "MACHINERY", "#00eaff"),
            "creation" => ("creation-bay", "CREATION", "#52e05a"),
            "experience" => ("research-facility", "EXPERIENCE", "#ff70d9"),
            "architecture" => ("architectural-shop", "ARCHITECTURE", "#ffd34d"),
            "research" => ("research-facility", "RESEARCH", "#b58cff"),
            "space-elevator" => ("space-elevator", "ORBITAL", "#ff9f43"),
            "tram-terminal" => ("tram", "TRANSIT", "#52e0ff"),
            "spaceship" => ("spaceship", "VESSEL", "#ff70d9"),
            "unknown" => ("unknown", "UNKNOWN", "#ff5577"),
            _ => ("default", "WORKSHOP", "#ffffff")
        };

        var behavior = onInteract is null
            ? NoOpBehavior.Instance
            : new CallbackBehavior(() => onInteract(space.Id));

        return new WorkshopInteractable(
            space.Id,
            space.Name,
            InteractionScope.World,
            new InteractionPoint(space.EntranceX, space.EntranceY),
            behavior,
            new WorkshopSign(
                $"{space.Id}-sign",
                space.Name,
                space.Kind,
                space.Description,
                $"CAPABILITY // {space.Capability}"),
            triggers: InteractionTrigger.HoverOrClick,
            presentation: new InteractablePresentation(
                space.Kind,
                space.Description,
                schematic,
                Map: new InteractableMapPresentation(thread, accent)));
    }

    private sealed class CallbackBehavior : IInteractableBehavior
    {
        private readonly Action _action;

        public CallbackBehavior(Action action) => _action = action;

        public void Execute(InteractionContext context) => _action();
    }

    private sealed class NoOpBehavior : IInteractableBehavior
    {
        public static readonly NoOpBehavior Instance = new();
        public void Execute(InteractionContext context) { }
    }
}
