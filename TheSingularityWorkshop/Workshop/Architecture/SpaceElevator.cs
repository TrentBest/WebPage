namespace TheSingularityWorkshop.Workshop.Architecture;

public enum SpaceElevatorComponentKind
{
    Anchor,
    Tether,
    Climber,
    Counterweight,
    SpaceStation
}

public readonly record struct SpaceElevatorComponent(
    string Id,
    SpaceElevatorComponentKind Kind,
    double AltitudeKilometers,
    string Label);

/// <summary>
/// Domain description of a Workshop space elevator. It is intentionally a structural
/// model rather than a vehicle simulator: the captured architecture can later drive
/// a spatial manifestation, orbital visualization, or transport simulation.
/// </summary>
public sealed record SpaceElevatorStructure(
    string Id,
    string Name,
    double TetherLengthKilometers,
    double StationAltitudeKilometers,
    IReadOnlyList<SpaceElevatorComponent> Components)
{
    public bool ReachesOrbitalStation =>
        StationAltitudeKilometers > 0 && TetherLengthKilometers >= StationAltitudeKilometers;

    public SpaceElevatorComponent? Station =>
        Components.FirstOrDefault(component => component.Kind == SpaceElevatorComponentKind.SpaceStation);
}

public static class SpaceElevatorTemplates
{
    public static SpaceElevatorStructure WorkshopOrbitalTether =>
        new(
            "space-elevator-workshop",
            "Workshop Orbital Tether",
            100_000,
            35_786,
            [
                new("ground-anchor", SpaceElevatorComponentKind.Anchor, 0, "WORKSHOP ANCHOR"),
                new("tether", SpaceElevatorComponentKind.Tether, 35_786, "ORBITAL TETHER"),
                new("climber", SpaceElevatorComponentKind.Climber, 0, "CLIMBER PLATFORM"),
                new("counterweight", SpaceElevatorComponentKind.Counterweight, 100_000, "COUNTERWEIGHT"),
                new("space-station", SpaceElevatorComponentKind.SpaceStation, 35_786, "WORKSHOP SPACE STATION")
            ]);
}
