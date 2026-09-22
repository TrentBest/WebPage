namespace TheSingularityWorkshop.World;

public sealed record WorkshopWorldEvent(
    string EventType,
    string LocationId,
    string DiscovererId,
    WorkshopPresentationMode PresentationMode,
    DateTimeOffset OccurredAtUtc)
{
    public static WorkshopWorldEvent CreateThreeDimensionalityUnlock(string locationId, string discovererId)
        => new("world.presentation.3d.unlock", locationId, discovererId, WorkshopPresentationMode.ThreeDimensional, DateTimeOffset.UtcNow);
}
