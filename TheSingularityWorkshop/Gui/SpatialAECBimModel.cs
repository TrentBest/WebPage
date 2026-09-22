using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Editable conference-room BIM model. The model is intentionally semantic first:
/// rooms are atomic reusable parts, floors are slices, and ontology content is
/// searched before it is placed into the active floor.
/// </summary>
public sealed class SpatialAECBimModel
{
    private readonly List<int> _floors = [];
    private readonly List<SpatialAECBimRoom> _rooms = [];
    private readonly List<SpatialOntologyContent> _results = [];

    private SpatialAECBimModel(string buildingId, string buildingName)
    {
        BuildingId = buildingId;
        BuildingName = buildingName;
        for (var floor = 1; floor <= 4; floor++)
            _floors.Add(floor);
    }

    public string BuildingId { get; }
    public string BuildingName { get; }
    public IReadOnlyList<int> Floors => _floors;
    public int ActiveFloor { get; private set; } = 1;
    public string SearchQuery { get; private set; } = "";
    public IReadOnlyList<SpatialAECBimRoom> Rooms => _rooms;
    public IReadOnlyList<SpatialOntologyContent> SearchResults => _results;

    public static SpatialAECBimModel CreateDefault(string buildingId, string? buildingName = null)
    {
        var model = new SpatialAECBimModel(
            buildingId,
            buildingName ?? buildingId);

        model._rooms.AddRange(
        [
            new("room-entry", "Entry", "CIRCULATION", 1, 0, 0, 12, 8, ["Reality", "Utility"]),
            new("room-reception", "Reception", "PUBLIC", 1, 12, 0, 14, 8, ["Reality", "Utility"]),
            new("room-conference", "Conference", "COLLABORATION", 1, 26, 0, 18, 12, ["Reality", "Utility"]),
            new("room-systems", "Systems", "MEP", 1, 0, 8, 12, 8, ["Reality", "Utility"])
        ]);

        model.Search("");
        return model;
    }

    public void SetActiveFloor(int floor)
        => ActiveFloor = Floors.Contains(floor) ? floor : ActiveFloor;

    public void Search(string? query)
    {
        SearchQuery = query?.Trim() ?? "";
        _results.Clear();

        foreach (var content in SpatialOntologyContentCatalog.All)
        {
            if (SearchQuery.Length == 0 ||
                content.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                content.Tags.Any(tag => tag.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)))
            {
                _results.Add(content);
            }
        }
    }

    public void AddRoom(string contentId)
    {
        var content = SpatialOntologyContentCatalog.All
            .FirstOrDefault(item => item.Id.Equals(contentId, StringComparison.OrdinalIgnoreCase));

        if (content.Equals(default(SpatialOntologyContent))) return;

        var index = _rooms.Count(room => room.Floor == ActiveFloor) + 1;
        _rooms.Add(new SpatialAECBimRoom(
            $"room-{content.Id}-{ActiveFloor}-{index}",
            content.Name,
            content.Category,
            ActiveFloor,
            4 + (index % 3) * 6,
            4 + (index / 3) * 6,
            content.Width,
            content.Depth,
            content.Tags));
    }

    public void DuplicateRoom(string roomId)
    {
        var source = _rooms.FirstOrDefault(room => room.Id == roomId);
        if (source.Equals(default(SpatialAECBimRoom))) return;

        var index = _rooms.Count(room => room.Floor == source.Floor) + 1;
        _rooms.Add(source with
        {
            Id = $"{source.Id}-copy-{index}",
            Name = $"{source.Name} Copy {index}",
            X = source.X + 3,
            Y = source.Y + 3
        });
    }

    public void RemoveRoom(string roomId)
        => _rooms.RemoveAll(room => room.Id == roomId);

    public IReadOnlyList<SpatialAECBimRoom> RoomsOnOrBelowActiveFloor
        => _rooms.Where(room => room.Floor <= ActiveFloor).ToArray();

    public IReadOnlyList<SpatialAECBimRoom> ActiveRooms
        => _rooms.Where(room => room.Floor == ActiveFloor).ToArray();
}

public readonly record struct SpatialAECBimRoom(
    string Id,
    string Name,
    string Category,
    int Floor,
    double X,
    double Y,
    double Width,
    double Depth,
    IReadOnlyList<string> Tags);

public readonly record struct SpatialOntologyContent(
    string Id,
    string Name,
    string Category,
    double Width,
    double Depth,
    IReadOnlyList<string> Tags);

public static class SpatialOntologyContentCatalog
{
    public static IReadOnlyList<SpatialOntologyContent> All =>
    [
        new("office-idler", "Idler Demonstration Room", "IDLer", 10, 8, ["Idler", "Utility", "Demonstration"]),
        new("flex-demo", "Flex Demonstration", "FLEX", 8, 8, ["Flex", "Demonstration", "Experience"]),
        new("conference-suite", "Conference Suite", "COLLABORATION", 14, 10, ["Collaboration", "Reality", "Utility"]),
        new("mechanical-core", "Mechanical Core", "SYSTEMS", 10, 10, ["MEP", "Mechanical", "Utility"]),
        new("lab-bay", "Laboratory Bay", "SCIENCE", 12, 12, ["Science", "Reality", "Experiment"]),
        new("reception-node", "Reception Node", "PUBLIC", 8, 6, ["Public", "Utility", "Idler"])
    ];
}
