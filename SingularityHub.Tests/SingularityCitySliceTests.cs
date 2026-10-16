using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityCitySliceTests
{
    [Fact]
    public void CivicDirectoryHasStableUniqueIdentitiesAndAResolvableMayor()
    {
        var officials = SingularityCivicDigitenCatalog.Officials;

        Assert.Equal(11, officials.Count);
        Assert.Equal(officials.Count, officials.Select(official => official.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal("Mayor Mara", SingularityCivicDigitenCatalog.Mayor.Name);
        Assert.Equal("Mayor Mara", SingularityCivicDigitenCatalog.Resolve("DIGITEN.MAYOR").Name);
        Assert.Contains(SingularityCivicDigitenCatalog.Mayor.CommitteeAssignments, assignment => assignment == "Executive Priorities");
    }

    [Fact]
    public void CityHasStableSoccerArenaIdentityInsideWorldBounds()
    {
        var structures = SingularityCityCatalog.Structures;
        var arena = Assert.Single(structures, structure => structure.Id == "city-soccer-arena");

        Assert.Equal("Singularity Soccer Arena", arena.Name);
        Assert.Equal(SpatialCityStructureKind.Culture, arena.Kind);
        Assert.True(arena.Bounds.X >= 0 && arena.Bounds.Y >= 0, "The arena starts outside the city plan.");
        Assert.True(arena.Bounds.Width > 0 && arena.Bounds.Height > 0, "The arena must have a visible footprint.");
        Assert.True(arena.Bounds.X + arena.Bounds.Width <= SingularityCityCatalog.WorldBounds.Width, "The arena exceeds the city width.");
        Assert.True(arena.Bounds.Y + arena.Bounds.Height <= SingularityCityCatalog.WorldBounds.Height, "The arena exceeds the city height.");
        Assert.Contains(structures, structure => structure.Id == "city-transit-hub");
        Assert.Contains("Soccer MicroBundle", arena.FutureFunction);
    }

    [Fact]
    public void MansionFeaturesHaveStableIdsAndStayInsideTheNormalizedPlan()
    {
        var features = SingularityMansionArchitecture.Features;

        Assert.Equal(9, features.Count);
        Assert.Equal(features.Count, features.Select(feature => feature.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());

        foreach (var feature in features)
        {
            Assert.True(feature.X >= 0 && feature.Y >= 0, $"{feature.Id} starts outside the plan.");
            Assert.True(feature.Width > 0 && feature.Height > 0, $"{feature.Id} has no visible area.");
            Assert.True(feature.X + feature.Width <= 100, $"{feature.Id} exceeds the plan width.");
            Assert.True(feature.Y + feature.Height <= 100, $"{feature.Id} exceeds the plan height.");
        }

        Assert.Equal(4, SingularityMansionArchitecture.Alcoves.Count);
        Assert.Equal("Grand Entrance", SingularityMansionArchitecture.Resolve("front-entry").Name);
    }

    [Fact]
    public void PresenceContractSeparatesLiveAvatarFromDurableWorldState()
    {
        Assert.Contains("CONNECTED = VISIBLE AVATAR", SingularityDigitenPresenceRules.PresenceContract);
        Assert.Contains("DISCONNECTED = REMOVE LIVE AVATAR", SingularityDigitenPresenceRules.PresenceContract);
        Assert.Contains("PERSISTED WORLD STATE = RETAIN DURABLE ROLES AND WORLD DATA", SingularityDigitenPresenceRules.PresenceContract);
    }

    [Fact]
    public void AerialTrafficHasStableUniqueVehiclesAndExplicitAltitudeLanes()
    {
        var manifest = SpatialAirTrafficManifest.CreateDefault();

        Assert.Equal("singularity-air-traffic", manifest.Id);
        Assert.Equal(4, manifest.Vehicles.Count);
        Assert.Equal(
            manifest.Vehicles.Count,
            manifest.Vehicles.Select(vehicle => vehicle.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Contains(manifest.Vehicles, vehicle => vehicle.AltitudeBand == 1);
        Assert.Contains(manifest.Vehicles, vehicle => vehicle.AltitudeBand == 2);
        Assert.Contains(manifest.Vehicles, vehicle => vehicle.AltitudeBand == 3);
        Assert.Equal("air-vehicle-001", manifest.FindVehicle("AIR-VEHICLE-001")?.Id);
    }

    [Fact]
    public void AerialTrafficSnapshotsAreDeterministicAndAdvanceAlongDeclaredRoutes()
    {
        var manifest = SpatialAirTrafficManifest.CreateDefault();

        var first = manifest.SnapshotAt(12);
        var repeat = manifest.SnapshotAt(12);
        var later = manifest.SnapshotAt(18);

        Assert.Equal(first, repeat);
        Assert.Equal(first.Count, later.Count);

        var vehicleAtFirst = Assert.Single(first, snapshot => snapshot.VehicleId == "air-vehicle-001");
        var vehicleLater = Assert.Single(later, snapshot => snapshot.VehicleId == "air-vehicle-001");

        Assert.NotEqual(vehicleAtFirst.Position, vehicleLater.Position);
        Assert.InRange(vehicleAtFirst.RouteProgress, 0, 1);
        Assert.InRange(vehicleLater.RouteProgress, 0, 1);
        Assert.Equal(1, vehicleAtFirst.AltitudeBand);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void AerialTrafficRejectsInvalidElapsedTime(double elapsedSeconds)
    {
        var manifest = SpatialAirTrafficManifest.CreateDefault();

        Assert.Throws<ArgumentOutOfRangeException>(() => manifest.SnapshotAt(elapsedSeconds));
    }
}
