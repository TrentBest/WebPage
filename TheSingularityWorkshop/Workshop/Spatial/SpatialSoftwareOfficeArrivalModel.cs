namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Pure data for the cinematic arrival at Singularity Software Inc.
/// Rendering and timing remain outside the model.
/// </summary>
public sealed class SpatialSoftwareOfficeArrivalModel
{
    public SpatialSoftwareOfficeArrivalStage Stage { get; private set; } = SpatialSoftwareOfficeArrivalStage.Rampway;
    public int ElevatorStops { get; private set; }

    public void BeginHelipadArrival() => Stage = SpatialSoftwareOfficeArrivalStage.HelicopterApproach;
    public void EnterHelicopter() => Stage = SpatialSoftwareOfficeArrivalStage.HelicopterDeparting;
    public void ArriveAtTowerHelipad() => Stage = SpatialSoftwareOfficeArrivalStage.TowerHelipad;
    public void EnterElevator() => Stage = SpatialSoftwareOfficeArrivalStage.ElevatorDescending;
    public void Ding() => ElevatorStops++;
    public void CompleteBriefing() => Stage = SpatialSoftwareOfficeArrivalStage.FacilityAccess;

    public void Reset()
    {
        Stage = SpatialSoftwareOfficeArrivalStage.Rampway;
        ElevatorStops = 0;
    }
}

public enum SpatialSoftwareOfficeArrivalStage
{
    Rampway,
    HelicopterApproach,
    HelicopterDeparting,
    TowerHelipad,
    ElevatorDescending,
    FacilityAccess
}
