namespace TheSingularityWorkshop.Workshop.Architecture;

public enum SpaceElevatorComponentKind
{
    Anchor,
    Tether,
    Climber,
    Counterweight,
    SpaceStation,
    FreightTerminal,
    PassengerTerminal,
    TramPlatform,
    PowerTransferStation,
    TransitConcourse,
    CommercialConcourse,
    SecurityOffice,
    BioSafetyStation
}

public enum SpaceElevatorTerminalKind
{
    Freight,
    Passenger
}

public readonly record struct SpaceElevatorComponent(
    string Id,
    SpaceElevatorComponentKind Kind,
    double AltitudeKilometers,
    string Label);

public readonly record struct SpaceElevatorTerminal(
    string Id,
    SpaceElevatorTerminalKind Kind,
    int HubPosition,
    string Label)
{
    public bool IsFreight => Kind == SpaceElevatorTerminalKind.Freight;
}

/// <summary>
/// Domain description of the Workshop space-elevator Experience.
/// The elevator is a transportation district, not merely a vertical tether:
/// terminals, trams, concourses, commerce, security, life-safety handling,
/// power transfer and the orbital station are all structural parts.
/// </summary>
public sealed record SpaceElevatorStructure(
    string Id,
    string Name,
    double TetherLengthKilometers,
    double StationAltitudeKilometers,
    IReadOnlyList<SpaceElevatorComponent> Components,
    IReadOnlyList<SpaceElevatorTerminal> Terminals,
    string TetherMaterial = "CARBON NANOTUBE // SPIDER-CARBON CONCEPT",
    int MidTetherPowerTransferStations = 3,
    string ClimberArchitecture = "TRI-RAIL MAGLEV SLED",
    int CommercialStorefrontCapacity = 24,
    bool CommercialLeasingEnabled = true)
{
    public bool ReachesOrbitalStation =>
        StationAltitudeKilometers > 0 && TetherLengthKilometers >= StationAltitudeKilometers;

    public SpaceElevatorComponent? Station =>
        Components.FirstOrDefault(component => component.Kind == SpaceElevatorComponentKind.SpaceStation);

    public int FreightTerminalCount => Terminals.Count(terminal => terminal.Kind == SpaceElevatorTerminalKind.Freight);
    public int PassengerTerminalCount => Terminals.Count(terminal => terminal.Kind == SpaceElevatorTerminalKind.Passenger);
    public bool HasHexagonalTerminalHub => Terminals.Count == 6 && FreightTerminalCount == 4 && PassengerTerminalCount == 2;
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
                new("ground-anchor", SpaceElevatorComponentKind.Anchor, 0, "GROUND ANCHOR // HUB CORE"),
                new("passenger-concourse", SpaceElevatorComponentKind.TransitConcourse, 0, "PASSENGER CONCOURSE"),
                new("freight-concourse", SpaceElevatorComponentKind.TransitConcourse, 0, "FREIGHT CONCOURSE"),
                new("commercial-concourse", SpaceElevatorComponentKind.CommercialConcourse, 0, "COMMERCIAL CONCOURSE // STOREFRONTS"),
                new("security-office", SpaceElevatorComponentKind.SecurityOffice, 0, "SECURITY OFFICE"),
                new("bio-safety", SpaceElevatorComponentKind.BioSafetyStation, 0, "BIOLOGICAL LIFE-SAFETY"),
                new("tether", SpaceElevatorComponentKind.Tether, 35_786, "ORBITAL TETHER"),
                new("power-transfer-1", SpaceElevatorComponentKind.PowerTransferStation, 12_000, "POWER TRANSFER // 12,000 KM"),
                new("power-transfer-2", SpaceElevatorComponentKind.PowerTransferStation, 24_000, "POWER TRANSFER // 24,000 KM"),
                new("power-transfer-3", SpaceElevatorComponentKind.PowerTransferStation, 31_000, "POWER TRANSFER // 31,000 KM"),
                new("climber", SpaceElevatorComponentKind.Climber, 0, "TRI-RAIL MAGLEV CLIMBER"),
                new("counterweight", SpaceElevatorComponentKind.Counterweight, 100_000, "COUNTERWEIGHT"),
                new("space-station", SpaceElevatorComponentKind.SpaceStation, 35_786, "WORKSHOP ORBITAL STATION")
            ],
            [
                new("freight-1", SpaceElevatorTerminalKind.Freight, 0, "FREIGHT TERMINAL 01"),
                new("freight-2", SpaceElevatorTerminalKind.Freight, 1, "FREIGHT TERMINAL 02"),
                new("freight-3", SpaceElevatorTerminalKind.Freight, 2, "FREIGHT TERMINAL 03"),
                new("freight-4", SpaceElevatorTerminalKind.Freight, 3, "FREIGHT TERMINAL 04"),
                new("passenger-1", SpaceElevatorTerminalKind.Passenger, 4, "PASSENGER TERMINAL 01"),
                new("passenger-2", SpaceElevatorTerminalKind.Passenger, 5, "PASSENGER TERMINAL 02")
            ]);
}
