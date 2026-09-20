namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Pure data for the cinematic arrival at Singularity Software Inc.
/// The flight and tower motion are sampled from a deterministic LUT microbundle.
/// </summary>
public sealed class SpatialSoftwareOfficeArrivalModel
{
    public SpatialSoftwareOfficeArrivalStage Stage { get; private set; } = SpatialSoftwareOfficeArrivalStage.Rampway;
    public int ElevatorStops { get; private set; }
    public double CinematicProgress { get; private set; }
    public SpatialHelicopterPhysicsMicroBundle Physics { get; } = new();

    public SpatialHelicopterPhysicsSample FlightPose
        => Stage == SpatialSoftwareOfficeArrivalStage.HelicopterLanding
            ? Physics.Landing(CinematicProgress)
            : Physics.Takeoff(CinematicProgress);

    public SpatialTowerSwaySample TowerSway
        => Physics.TowerSway(CinematicProgress);

    public void BeginHelipadArrival() => Stage = SpatialSoftwareOfficeArrivalStage.HelipadReady;
    public void EnterHelicopter()
    {
        CinematicProgress = 0d;
        Stage = SpatialSoftwareOfficeArrivalStage.HelicopterTakeoff;
    }

    public bool AdvanceCinematic(double seconds)
    {
        var duration = Stage switch
        {
            SpatialSoftwareOfficeArrivalStage.HelicopterTakeoff => Physics.Constants.TakeoffDurationSeconds,
            SpatialSoftwareOfficeArrivalStage.TowerReveal => Physics.Constants.TowerRevealDurationSeconds,
            SpatialSoftwareOfficeArrivalStage.HelicopterLanding => Physics.Constants.LandingDurationSeconds,
            _ => 0d
        };

        if (duration <= 0d)
            return false;

        CinematicProgress = Math.Clamp(CinematicProgress + seconds / duration, 0d, 1d);

        if (CinematicProgress < 1d)
            return false;

        CinematicProgress = 0d;

        switch (Stage)
        {
            case SpatialSoftwareOfficeArrivalStage.HelicopterTakeoff:
                Stage = SpatialSoftwareOfficeArrivalStage.TowerReveal;
                return false;

            case SpatialSoftwareOfficeArrivalStage.TowerReveal:
                Stage = SpatialSoftwareOfficeArrivalStage.HelicopterLanding;
                return false;

            case SpatialSoftwareOfficeArrivalStage.HelicopterLanding:
                Stage = SpatialSoftwareOfficeArrivalStage.TowerFloorPlan;
                return true;

            default:
                return false;
        }
    }

    public void ArriveAtTowerHelipad() => Stage = SpatialSoftwareOfficeArrivalStage.TowerFloorPlan;
    public void EnterElevator() => Stage = SpatialSoftwareOfficeArrivalStage.ElevatorDescending;
    public void Ding() => ElevatorStops++;
    public void CompleteBriefing() => Stage = SpatialSoftwareOfficeArrivalStage.FacilityAccess;

    public void Reset()
    {
        Stage = SpatialSoftwareOfficeArrivalStage.Rampway;
        ElevatorStops = 0;
        CinematicProgress = 0d;
    }
}

public enum SpatialSoftwareOfficeArrivalStage
{
    Rampway,
    HelipadReady,
    HelicopterTakeoff,
    TowerReveal,
    HelicopterLanding,
    TowerFloorPlan,
    ElevatorDescending,
    FacilityAccess
}
