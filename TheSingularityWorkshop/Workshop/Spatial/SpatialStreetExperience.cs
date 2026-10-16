namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Declarative first block of Singularity Street. This is a scene manifest,
/// not a claim that railway dynamics or resident AI are already simulated.
/// Geometry uses the same normalized 0-100 spatial canvas as Explore.
/// </summary>
public sealed class SpatialStreetExperience
{
    private SpatialStreetExperience(
        SpatialBounds neighborhoodBounds,
        SpatialBounds walkableStreetBounds,
        SpatialPoint startingPosition,
        IReadOnlyList<SpatialStreetPlace> places,
        IReadOnlyList<SpatialStreetRouteStop> route)
    {
        NeighborhoodBounds = neighborhoodBounds;
        WalkableStreetBounds = walkableStreetBounds;
        StartingPosition = startingPosition;
        Places = places;
        Route = route;
        Validate();
    }

    public SpatialBounds NeighborhoodBounds { get; }
    public SpatialBounds WalkableStreetBounds { get; }
    public SpatialPoint StartingPosition { get; }
    public IReadOnlyList<SpatialStreetPlace> Places { get; }
    public IReadOnlyList<SpatialStreetRouteStop> Route { get; }

    public SpatialStreetPlace Station => Places.Single(place => place.Kind == SpatialStreetPlaceKind.Station);
    public SpatialStreetPlace Train => Places.Single(place => place.Kind == SpatialStreetPlaceKind.Train);

    public static SpatialStreetExperience CreateDefault()
        => new(
            new SpatialBounds(8, 70, 84, 22),
            new SpatialBounds(12, 77, 76, 7),
            new SpatialPoint(50, 82),
            [
                new("station-neighborhood", "Neighborhood Station", SpatialStreetPlaceKind.Station,
                    new SpatialBounds(18, 74, 10, 8), "Wait for the neighborhood train."),
                new("train-little-loop", "Streetcar", SpatialStreetPlaceKind.Train,
                    new SpatialBounds(31, 78, 9, 3), "Inspect the train; route is an authored fixture."),
                new("home-corner", "Corner Home", SpatialStreetPlaceKind.Home,
                    new SpatialBounds(14, 86, 10, 5), "A resident home, reserved for future original characters."),
                new("shop-general", "Corner Shop", SpatialStreetPlaceKind.Shop,
                    new SpatialBounds(36, 86, 12, 5), "A neighborhood storefront."),
                new("park-pocket", "Pocket Park", SpatialStreetPlaceKind.Park,
                    new SpatialBounds(57, 84, 12, 6), "A small public green and meeting place."),
                new("square-neighbors", "Neighborhood Square", SpatialStreetPlaceKind.Square,
                    new SpatialBounds(72, 83, 12, 7), "A place to meet, discover, and begin a short journey.")
            ],
            [
                new("station-neighborhood", "Neighborhood Station", new SpatialPoint(23, 79), 0),
                new("park-pocket", "Pocket Park", new SpatialPoint(63, 87), 1),
                new("square-neighbors", "Neighborhood Square", new SpatialPoint(78, 86), 2),
                new("station-neighborhood", "Neighborhood Station", new SpatialPoint(23, 79), 3)
            ]);

    private void Validate()
    {
        if (Places.Count == 0) throw new InvalidOperationException("A neighborhood needs at least one place.");
        if (Places.Select(place => place.Id).Distinct(StringComparer.Ordinal).Count() != Places.Count)
            throw new InvalidOperationException("Singularity Street place IDs must be unique.");
        if (Route.Count < 2) throw new InvalidOperationException("A train route needs at least two stops.");
        if (Route.Select(stop => stop.Sequence).Distinct().Count() != Route.Count)
            throw new InvalidOperationException("Train route sequence values must be unique.");
        if (Route.Select(stop => stop.Sequence).OrderBy(sequence => sequence).Where((sequence, index) => sequence != index).Any())
            throw new InvalidOperationException("Train route sequence values must be contiguous from zero.");

        foreach (var place in Places)
        {
            if (string.IsNullOrWhiteSpace(place.Id) || string.IsNullOrWhiteSpace(place.Name))
                throw new InvalidOperationException("Every neighborhood place needs a stable ID and a name.");
            if (place.Bounds.Width <= 0 || place.Bounds.Height <= 0)
                throw new InvalidOperationException($"Place '{place.Id}' must have positive bounds.");
            if (!NeighborhoodBounds.Contains(new SpatialPoint(place.Bounds.X, place.Bounds.Y)) ||
                !NeighborhoodBounds.Contains(new SpatialPoint(place.Bounds.X + place.Bounds.Width, place.Bounds.Y + place.Bounds.Height)))
                throw new InvalidOperationException($"Place '{place.Id}' lies outside the neighborhood bounds.");
        }

        foreach (var stop in Route)
        {
            if (!Places.Any(place => place.Id == stop.PlaceId))
                throw new InvalidOperationException($"Route stop '{stop.PlaceId}' does not resolve to a neighborhood place.");
        }

        if (Station.Kind != SpatialStreetPlaceKind.Station || Train.Kind != SpatialStreetPlaceKind.Train)
            throw new InvalidOperationException("The station and train must be represented explicitly.");
    }
}

public enum SpatialStreetPlaceKind
{
    Station,
    Train,
    Home,
    Shop,
    Park,
    Square
}

public sealed record SpatialStreetPlace(
    string Id,
    string Name,
    SpatialStreetPlaceKind Kind,
    SpatialBounds Bounds,
    string InteractionDescription);

/// <summary>
/// A proposed route stop on an authored path. It is not a train motion,
/// signal, dwell-time, reservation, or collision simulation.
/// </summary>
public sealed record SpatialStreetRouteStop(
    string PlaceId,
    string Name,
    SpatialPoint Position,
    int Sequence);
