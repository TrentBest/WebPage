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
        Assert.Equal("WORKSHOP SPACE STATION", elevator.Station!.Value.Label);
    }
}
