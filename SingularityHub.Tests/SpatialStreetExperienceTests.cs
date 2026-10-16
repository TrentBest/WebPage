namespace SingularityHub.Tests;

using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

public sealed class SpatialStreetExperienceTests
{
    [Fact]
    public void DefaultNeighborhood_HasOriginalPlacesAndExplicitStationTrain()
    {
        var street = SpatialStreetExperience.CreateDefault();

        Assert.Equal(6, street.Places.Count);
        Assert.Equal("station-neighborhood", street.Station.Id);
        Assert.Equal("train-little-loop", street.Train.Id);
        Assert.Contains(street.Places, place => place.Kind == SpatialStreetPlaceKind.Home);
        Assert.Contains(street.Places, place => place.Kind == SpatialStreetPlaceKind.Shop);
        Assert.Contains(street.Places, place => place.Kind == SpatialStreetPlaceKind.Park);
        Assert.Contains(street.Places, place => place.Kind == SpatialStreetPlaceKind.Square);
    }

    [Fact]
    public void DefaultRoute_ResolvesEveryStopAndReturnsToStation()
    {
        var street = SpatialStreetExperience.CreateDefault();

        Assert.Equal(4, street.Route.Count);
        Assert.Equal(new[] { 0, 1, 2, 3 }, street.Route.Select(stop => stop.Sequence));
        Assert.All(street.Route, stop => Assert.Contains(street.Places, place => place.Id == stop.PlaceId));
        Assert.Equal(street.Route[0].PlaceId, street.Route[^1].PlaceId);
        Assert.Equal("station-neighborhood", street.Route[0].PlaceId);
    }

    [Fact]
    public void DefaultNeighborhood_FitsProposedSouthExpanseAndStartsInsideWalkableStreet()
    {
        var street = SpatialStreetExperience.CreateDefault();

        Assert.True(street.NeighborhoodBounds.Contains(new SpatialPoint(8, 70)));
        Assert.True(street.NeighborhoodBounds.Contains(new SpatialPoint(92, 92)));
        Assert.True(street.WalkableStreetBounds.Contains(street.StartingPosition));
        Assert.All(street.Places, place =>
        {
            Assert.True(street.NeighborhoodBounds.Contains(new SpatialPoint(place.Bounds.X, place.Bounds.Y)));
            Assert.True(street.NeighborhoodBounds.Contains(new SpatialPoint(
                place.Bounds.X + place.Bounds.Width,
                place.Bounds.Y + place.Bounds.Height)));
        });
    }
}
