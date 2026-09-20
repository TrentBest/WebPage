namespace TheSingularityWorkshop.Gui;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A semantic room requirement used to generate an architectural proposal.
/// Quantity is data; the renderer decides how that data becomes visible.
/// </summary>
public sealed record SpatialRoomProgram(
    string Id,
    string Name,
    int Quantity,
    double WidthFeet,
    double DepthFeet,
    double HeightFeet,
    bool IndividuallyResizable = true,
    string SourceNote = "")
{
    public double AreaSquareFeet => WidthFeet * DepthFeet;

    public SpatialRoomProgram Scale(double factor)
    {
        if (factor <= 0)
            throw new ArgumentOutOfRangeException(nameof(factor));

        return this with
        {
            WidthFeet = WidthFeet * factor,
            DepthFeet = DepthFeet * factor
        };
    }

    public SpatialRoomProgram Resize(double widthFeet, double depthFeet)
    {
        if (widthFeet <= 0 || depthFeet <= 0)
            throw new ArgumentOutOfRangeException(nameof(widthFeet), "Room dimensions must be positive.");

        return this with { WidthFeet = widthFeet, DepthFeet = depthFeet };
    }
}

/// <summary>
/// A building proposal is a data composition that can be rendered in plan,
/// inspected by room, and later projected into elevation or richer geometry.
/// </summary>
public sealed record SpatialBuildingProgram(
    string Id,
    string Name,
    IReadOnlyList<SpatialRoomProgram> Rooms,
    double GlobalScale = 1d,
    string Evidence = "")
{
    public int RoomCount => Rooms.Sum(room => room.Quantity);

    public double FinishedRoomAreaSquareFeet
        => Rooms.Sum(room => room.AreaSquareFeet * room.Quantity);

    public SpatialBuildingProgram ScaleAll(double factor)
    {
        if (factor <= 0)
            throw new ArgumentOutOfRangeException(nameof(factor));

        return this with
        {
            GlobalScale = GlobalScale * factor,
            Rooms = Rooms.Select(room => room.Scale(factor)).ToArray()
        };
    }

    public SpatialBuildingProgram ResizeRoom(string roomId, double widthFeet, double depthFeet)
    {
        var room = Rooms.FirstOrDefault(x => string.Equals(x.Id, roomId, StringComparison.OrdinalIgnoreCase));
        if (room is null)
            throw new KeyNotFoundException($"No room program '{roomId}' exists.");

        if (!room.IndividuallyResizable)
            throw new InvalidOperationException($"Room '{roomId}' does not permit individual resizing.");

        return this with
        {
            Rooms = Rooms.Select(item =>
                string.Equals(item.Id, roomId, StringComparison.OrdinalIgnoreCase)
                    ? item.Resize(widthFeet, depthFeet)
                    : item).ToArray()
        };
    }

    public SpatialRoomProgram ResolveRoom(string roomId)
        => Rooms.FirstOrDefault(x => string.Equals(x.Id, roomId, StringComparison.OrdinalIgnoreCase))
           ?? throw new KeyNotFoundException($"No room program '{roomId}' exists.");
}

/// <summary>
/// Seed programs for the AEC proof. These are explicit baselines, not claims that
/// one dimension is universal for every home or jurisdiction.
/// </summary>
public static class SpatialBuildingProgramCatalog
{
    public static SpatialBuildingProgram ResidentialBaseline
        => new(
            "residential.baseline",
            "Residential Baseline",
            [
                new(
                    "bedroom",
                    "Bedroom",
                    3,
                    11,
                    11,
                    9,
                    true,
                    "Illustrative 11 x 11 x 9 baseline supplied for the AEC interaction proof."),
                new("kitchen", "Kitchen", 1, 12, 14, 9, true),
                new("living", "Living Room", 1, 16, 18, 9, true),
                new("bathroom", "Bathroom", 2, 8, 10, 9, true),
                new("circulation", "Circulation", 1, 5, 20, 9, true)
            ],
            Evidence: "Program seed. Validate dimensions against jurisdiction, building type, accessibility, code, and project-specific requirements.");

    public static SpatialBuildingProgram RemoteOffice
        => new(
            "aec.remote-office",
            "AEC Remote Office",
            [
                new("reception", "Reception", 1, 12, 14, 9, true),
                new("conference", "Conference Room", 1, 16, 22, 9, true),
                new("open-office", "Open Office", 1, 28, 30, 9, true),
                new("director", "Director Office", 1, 14, 16, 9, true),
                new("model-library", "Model Library", 1, 16, 18, 9, true),
                new("collaboration", "Collaboration Room", 2, 12, 14, 9, true),
                new("support", "Support", 1, 10, 12, 9, true)
            ],
            Evidence: "Conceptual AEC office program; dimensions are authoring seeds rather than construction requirements.");
}
