namespace TheSingularityWorkshop.Gui;

/// <summary>
/// A deterministic, presentation-independent passenger journey through a transit system.
/// The host supplies simulation time; this model never starts its own clock.
/// </summary>
public sealed record SpatialTransitJourney(
    string Id,
    string PassengerId,
    string OriginPlatformId,
    string DestinationId,
    double RequestedAtSeconds,
    double PickupSeconds,
    double BoardingSeconds,
    double TravelSeconds,
    double ArrivalDwellSeconds)
{
    public SpatialTransitJourneySnapshot SnapshotAt(double elapsedSeconds)
    {
        if (string.IsNullOrWhiteSpace(Id) ||
            string.IsNullOrWhiteSpace(PassengerId) ||
            string.IsNullOrWhiteSpace(OriginPlatformId) ||
            string.IsNullOrWhiteSpace(DestinationId))
            throw new InvalidOperationException("A transit journey requires stable journey, passenger, origin, and destination identities.");

        if (!double.IsFinite(RequestedAtSeconds) || RequestedAtSeconds < 0 ||
            !double.IsFinite(PickupSeconds) || PickupSeconds < 0 ||
            !double.IsFinite(BoardingSeconds) || BoardingSeconds < 0 ||
            !double.IsFinite(TravelSeconds) || TravelSeconds <= 0 ||
            !double.IsFinite(ArrivalDwellSeconds) || ArrivalDwellSeconds < 0)
            throw new InvalidOperationException("Transit journey timing must be finite and non-negative; travel time must be positive.");

        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < RequestedAtSeconds)
            throw new ArgumentOutOfRangeException(nameof(elapsedSeconds), "Snapshot time must be finite and at or after the request time.");

        var journeyElapsed = elapsedSeconds - RequestedAtSeconds;
        var boardingAt = PickupSeconds;
        var departureAt = boardingAt + BoardingSeconds;
        var arrivalAt = departureAt + TravelSeconds;
        var completeAt = arrivalAt + ArrivalDwellSeconds;

        var status = journeyElapsed < boardingAt
            ? SpatialTransitJourneyStatus.Waiting
            : journeyElapsed < departureAt
                ? SpatialTransitJourneyStatus.Boarding
                : journeyElapsed < arrivalAt
                    ? SpatialTransitJourneyStatus.InTransit
                    : journeyElapsed < completeAt
                        ? SpatialTransitJourneyStatus.Arrived
                        : SpatialTransitJourneyStatus.Completed;

        return new SpatialTransitJourneySnapshot(
            Id,
            PassengerId,
            OriginPlatformId,
            DestinationId,
            status,
            journeyElapsed,
            Math.Clamp(journeyElapsed / completeAt, 0d, 1d));
    }
}

/// <summary>Observable stages of a passenger's trip; arrival and trip completion are distinct.</summary>
public enum SpatialTransitJourneyStatus
{
    Waiting,
    Boarding,
    InTransit,
    Arrived,
    Completed
}

/// <summary>Immutable state derived from a journey manifest and caller-owned simulation time.</summary>
public readonly record struct SpatialTransitJourneySnapshot(
    string JourneyId,
    string PassengerId,
    string OriginPlatformId,
    string DestinationId,
    SpatialTransitJourneyStatus Status,
    double ElapsedSeconds,
    double Progress);
