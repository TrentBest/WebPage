namespace TheSingularityWorkshop.World;

public enum WorkshopPresentationMode
{
    TwoDimensional,
    ThreeDimensional
}

public sealed record WorkshopWorldState(
    WorkshopPresentationMode PresentationMode,
    long Epoch,
    WorkshopWorldEvent? LastEvent)
{
    public static WorkshopWorldState CreateInitial()
        => new(WorkshopPresentationMode.TwoDimensional, 0, null);

    public WorkshopWorldState Activate(WorkshopWorldEvent worldEvent)
    {
        ArgumentNullException.ThrowIfNull(worldEvent);

        if (LastEvent is not null &&
            string.Equals(LastEvent.EventType, worldEvent.EventType, StringComparison.Ordinal))
            return this;

        return this with
        {
            PresentationMode = worldEvent.PresentationMode,
            Epoch = Epoch + 1,
            LastEvent = worldEvent
        };
    }
}
