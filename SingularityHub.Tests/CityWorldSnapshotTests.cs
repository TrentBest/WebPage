using TheSingularityWorkshop.Workshop.City;
using Xunit;

namespace SingularityHub.Tests;

public sealed class CityWorldSnapshotTests
{
    [Fact]
    public void SnapshotPreservesRevisionAndCanonicalCoordinateConvention()
    {
        var snapshot = CityWorldSnapshot.Create("density-28874", []);

        Assert.Equal("density-28874", snapshot.Revision);
        Assert.Equal("grid-xz; +X east; +Z south; origin at grid cell (0,0)", snapshot.CoordinateConvention);
        Assert.Empty(snapshot.Entities);
    }

    [Fact]
    public void SnapshotOrdersEntitiesByStableIdWithoutChangingTheirSpatialFacts()
    {
        var northWest = new CityWorldEntity("building.1", "tower", 2.5, 3.5, 40);
        var southEast = new CityWorldEntity("building.2", "lowrise", 8, 12, 4);

        var snapshot = CityWorldSnapshot.Create("city-v1", [southEast, northWest]);

        Assert.Equal(["building.1", "building.2"], snapshot.Entities.Select(entity => entity.Id));
        Assert.Equal(2.5, snapshot.Entities[0].GridX);
        Assert.Equal(3.5, snapshot.Entities[0].GridZ);
        Assert.Equal(40, snapshot.Entities[0].Floors);
    }

    [Fact]
    public void SnapshotRejectsDuplicateEntityIdentity()
    {
        var entities = new[]
        {
            new CityWorldEntity("building.same", "tower", 1, 1, 10),
            new CityWorldEntity("building.same", "lowrise", 2, 2, 3)
        };

        var error = Assert.Throws<ArgumentException>(() => CityWorldSnapshot.Create("city-v1", entities));

        Assert.Contains("duplicated", error.Message);
    }

    [Fact]
    public void EntityRejectsNonFiniteCoordinatesAndNegativeFloors()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CityWorldEntity("building.nan", "tower", double.NaN, 0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CityWorldEntity("building.infinity", "tower", 0, double.PositiveInfinity, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CityWorldEntity("building.invalid-floors", "tower", 0, 0, -1));
    }

    [Fact]
    public void CityLabUsesTheRevisionedSnapshotForSpatialProjectionsAndStableCellIds()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        var page = File.ReadAllText(Path.Combine(root, "TheSingularityWorkshop", "Pages", "CityLab.razor"));

        Assert.Contains("CityWorldSnapshot.Create(", page);
        Assert.Contains("city-massing-v1-density-", page);
        Assert.Contains("var id = row * 1000 + col * 10 + local + 1;", page);
        Assert.True(page.Split("foreach (var building in WorldSnapshot.Entities)", StringSplitOptions.None).Length - 1 >= 3,
            "Plan, street projection, and Renderer candidate generation must consume the shared city snapshot.");
    }

}
