namespace TheSingularityWorkshop.Workshop.Chemistry;

public enum HadronKind
{
    Proton,
    Antiproton,
    LeadIon
}

public enum HadronColliderSimulationState
{
    Idle,
    Injecting,
    Accelerating,
    Colliding,
    Analyzing
}

public readonly record struct ColliderBeam(
    string Id,
    HadronKind Hadron,
    double EnergyTeV,
    double Intensity,
    double Direction,
    bool Active);

public readonly record struct CollisionEvent(
    long EventId,
    HadronKind FirstBeam,
    HadronKind SecondBeam,
    double CenterOfMassEnergyTeV,
    double EventEnergyTeV,
    double Luminosity,
    string DetectorChannel);

/// <summary>
/// Deterministic educational collider simulator. It models the control surface and
/// event stream of a hadron collider without claiming to reproduce detector physics.
/// The GUI can animate this domain model while future providers can supply richer
/// physics models.
/// </summary>
public sealed class HadronColliderSimulator
{
    private long _eventId;

    public HadronColliderSimulationState State { get; private set; } = HadronColliderSimulationState.Idle;
    public double BeamEnergyTeV { get; private set; }
    public double Luminosity { get; private set; }
    public ColliderBeam? ClockwiseBeam { get; private set; }
    public ColliderBeam? CounterClockwiseBeam { get; private set; }
    public CollisionEvent? LastCollision { get; private set; }

    public void Configure(
        HadronKind clockwiseHadron,
        HadronKind counterClockwiseHadron,
        double beamEnergyTeV,
        double luminosity)
    {
        if (beamEnergyTeV <= 0)
            throw new ArgumentOutOfRangeException(nameof(beamEnergyTeV));
        if (luminosity < 0)
            throw new ArgumentOutOfRangeException(nameof(luminosity));

        BeamEnergyTeV = beamEnergyTeV;
        Luminosity = luminosity;
        ClockwiseBeam = new ColliderBeam("beam-a", clockwiseHadron, beamEnergyTeV, 1, 1, false);
        CounterClockwiseBeam = new ColliderBeam("beam-b", counterClockwiseHadron, beamEnergyTeV, 1, -1, false);
        State = HadronColliderSimulationState.Idle;
        LastCollision = null;
    }

    public void Inject()
    {
        RequireConfigured();
        State = HadronColliderSimulationState.Injecting;
        ClockwiseBeam = ClockwiseBeam!.Value with { Active = true };
        CounterClockwiseBeam = CounterClockwiseBeam!.Value with { Active = true };
    }

    public void Accelerate()
    {
        RequireConfigured();
        if (State is not HadronColliderSimulationState.Injecting)
            throw new InvalidOperationException("Beams must be injected before acceleration.");

        State = HadronColliderSimulationState.Accelerating;
    }

    public CollisionEvent Collide(string detectorChannel = "GENERAL PURPOSE")
    {
        RequireConfigured();
        if (State is not HadronColliderSimulationState.Accelerating)
            throw new InvalidOperationException("Beams must be accelerating before collision.");

        State = HadronColliderSimulationState.Colliding;

        var collision = new CollisionEvent(
            ++_eventId,
            ClockwiseBeam!.Value.Hadron,
            CounterClockwiseBeam!.Value.Hadron,
            BeamEnergyTeV * 2,
            BeamEnergyTeV * 2,
            Luminosity,
            detectorChannel);

        LastCollision = collision;
        State = HadronColliderSimulationState.Analyzing;
        return collision;
    }

    public void Reset()
    {
        State = HadronColliderSimulationState.Idle;
        if (ClockwiseBeam is { } clockwise)
            ClockwiseBeam = clockwise with { Active = false };
        if (CounterClockwiseBeam is { } counterClockwise)
            CounterClockwiseBeam = counterClockwise with { Active = false };
        LastCollision = null;
    }

    private void RequireConfigured()
    {
        if (ClockwiseBeam is null || CounterClockwiseBeam is null)
            throw new InvalidOperationException("Configure the collider before running it.");
    }
}
