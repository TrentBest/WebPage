using TheSingularityWorkshop.Workshop.Architecture;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpaceElevatorTests
{
    [Fact]
    public void WorkshopOrbitalTether_ReachesStationAndContainsRequiredInfrastructure()
    {
        var elevator = SpaceElevatorTemplates.WorkshopOrbitalTether;

        Assert.True(elevator.ReachesOrbitalStation);
        Assert.Equal(35_786, elevator.StationAltitudeKilometers);
        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.Anchor);
        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.Tether);
        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.Climber);
        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.Counterweight);
        Assert.Equal("WORKSHOP ORBITAL STATION", elevator.Station!.Value.Label);
    }

    [Fact]
    public void WorkshopOrbitalTether_HasHexagonalFourFreightTwoPassengerHub()
    {
        var elevator = SpaceElevatorTemplates.WorkshopOrbitalTether;

        Assert.True(elevator.HasHexagonalTerminalHub);
        Assert.Equal(4, elevator.FreightTerminalCount);
        Assert.Equal(2, elevator.PassengerTerminalCount);
        Assert.Equal(6, elevator.Terminals.Count);
        Assert.All(elevator.Terminals, terminal => Assert.InRange(terminal.HubPosition, 0, 5));
    }

    [Fact]
    public void WorkshopOrbitalTether_DeclaresPowerTransferAndTriRailClimber()
    {
        var elevator = SpaceElevatorTemplates.WorkshopOrbitalTether;

        Assert.Equal(3, elevator.MidTetherPowerTransferStations);
        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.PowerTransferStation);
        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.SecurityOffice);
        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.BioSafetyStation);
        Assert.Contains("TRI-RAIL", elevator.ClimberArchitecture);
        Assert.Contains("CARBON NANOTUBE", elevator.TetherMaterial);
    }

    [Fact]
    public void WorkshopOrbitalTether_ProvidesCommercialConcourseAndLeasingCapacity()
    {
        var elevator = SpaceElevatorTemplates.WorkshopOrbitalTether;

        Assert.Contains(elevator.Components, component => component.Kind == SpaceElevatorComponentKind.CommercialConcourse);
        Assert.True(elevator.CommercialLeasingEnabled);
        Assert.True(elevator.CommercialStorefrontCapacity >= 1);
    }
}
