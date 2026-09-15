namespace TheSingularityWorkshop.Workshop.Architecture;

public enum ArchitecturalCellKind
{
    Empty,
    Wall,
    Door,
    Room,
    Stair,
    Elevator,
    Ladder
}

public enum ArchitecturalVerticalConnectorKind
{
    Stair,
    Elevator,
    Ladder
}

public readonly record struct ArchitecturalCell(int X, int Y, ArchitecturalCellKind Kind)
{
    public bool IsVerticalConnector => Kind is ArchitecturalCellKind.Stair
        or ArchitecturalCellKind.Elevator
        or ArchitecturalCellKind.Ladder;
}

public readonly record struct ArchitecturalVerticalConnector(
    string Id,
    ArchitecturalVerticalConnectorKind Kind,
    int X,
    int Y,
    int FromFloor,
    int ToFloor,
    string Label);

public sealed record ArchitecturalFloorPlan(
    string Id,
    string Name,
    int Width,
    int Height,
    IReadOnlyList<ArchitecturalCell> Cells,
    IReadOnlyList<ArchitecturalVerticalConnector> VerticalConnectors)
{
    public ArchitecturalCell? CellAt(int x, int y)
    {
        foreach (var cell in Cells)
        {
            if (cell.X == x && cell.Y == y)
                return cell;
        }

        return null;
    }

    public ArchitecturalFloorPlan WithCell(int x, int y, ArchitecturalCellKind kind)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            throw new ArgumentOutOfRangeException();

        var cells = Cells.Where(cell => cell.X != x || cell.Y != y).ToList();
        if (kind != ArchitecturalCellKind.Empty)
            cells.Add(new ArchitecturalCell(x, y, kind));

        var connectors = VerticalConnectors
            .Where(connector => connector.X != x || connector.Y != y)
            .ToArray();

        return this with { Cells = cells, VerticalConnectors = connectors };
    }

    public ArchitecturalFloorPlan AddVerticalConnector(ArchitecturalVerticalConnector connector)
    {
        if (connector.X < 0 || connector.Y < 0 || connector.X >= Width || connector.Y >= Height)
            throw new ArgumentOutOfRangeException(nameof(connector));

        var connectors = VerticalConnectors
            .Where(existing => existing.Id != connector.Id
                && (existing.X != connector.X || existing.Y != connector.Y))
            .Append(connector)
            .ToArray();
        return this with { VerticalConnectors = connectors };
    }

    public ArchitecturalStructure Capture(string structureId, string structureName)
        => new(structureId, structureName, [this]);
}

public sealed record ArchitecturalStructure(
    string Id,
    string Name,
    IReadOnlyList<ArchitecturalFloorPlan> Floors)
{
    public int TotalCellCount => Floors.Sum(floor => floor.Cells.Count);
    public int VerticalConnectorCount => Floors.Sum(floor => floor.VerticalConnectors.Count);
}

/// <summary>
/// Mutable authoring surface for the Architectural Shop. The editor is deliberately
/// domain data first: the GUI builder paints this state, but the floor plan itself is
/// independent of Blazor and can later become an ExperienceSpace/3D structure.
/// </summary>
public sealed class ArchitecturalShopEditor
{
    public ArchitecturalFloorPlan Plan { get; private set; } = ArchitecturalFloorPlanTemplates.Blank;
    public ArchitecturalStructure? CapturedStructure { get; private set; }
    public ArchitecturalCellKind SelectedTool { get; private set; } = ArchitecturalCellKind.Wall;

    public void SelectTool(ArchitecturalCellKind tool) => SelectedTool = tool;

    public void Apply(int x, int y)
    {
        Plan = Plan.WithCell(x, y, SelectedTool);

        if (SelectedTool is ArchitecturalCellKind.Stair
            or ArchitecturalCellKind.Elevator
            or ArchitecturalCellKind.Ladder)
        {
            var kind = SelectedTool switch
            {
                ArchitecturalCellKind.Stair => ArchitecturalVerticalConnectorKind.Stair,
                ArchitecturalCellKind.Elevator => ArchitecturalVerticalConnectorKind.Elevator,
                _ => ArchitecturalVerticalConnectorKind.Ladder
            };
            var id = $"{kind.ToString().ToLowerInvariant()}-{x}-{y}";
            Plan = Plan.AddVerticalConnector(new ArchitecturalVerticalConnector(
                id, kind, x, y, 0, 1, kind.ToString().ToUpperInvariant()));
        }

        CapturedStructure = null;
    }

    public void Load(ArchitecturalFloorPlan plan)
    {
        Plan = plan;
        CapturedStructure = null;
    }

    public ArchitecturalStructure Capture()
    {
        CapturedStructure = Plan.Capture($"structure-{Plan.Id}", Plan.Name);
        return CapturedStructure;
    }
}

public static class ArchitecturalFloorPlanTemplates
{
    public static ArchitecturalFloorPlan Blank =>
        new("blank", "New Floor Plan", 20, 14, [], []);

    public static ArchitecturalFloorPlan OpenWorkshop => Build(
        "open-workshop", "Open Workshop", 20, 14,
        [
            (1, 1, ArchitecturalCellKind.Wall), (2, 1, ArchitecturalCellKind.Wall), (3, 1, ArchitecturalCellKind.Wall),
            (4, 1, ArchitecturalCellKind.Wall), (5, 1, ArchitecturalCellKind.Wall), (6, 1, ArchitecturalCellKind.Wall),
            (7, 1, ArchitecturalCellKind.Wall), (8, 1, ArchitecturalCellKind.Wall), (9, 1, ArchitecturalCellKind.Wall),
            (10, 1, ArchitecturalCellKind.Wall), (11, 1, ArchitecturalCellKind.Wall), (12, 1, ArchitecturalCellKind.Wall),
            (13, 1, ArchitecturalCellKind.Wall), (14, 1, ArchitecturalCellKind.Wall), (15, 1, ArchitecturalCellKind.Wall),
            (16, 1, ArchitecturalCellKind.Wall), (17, 1, ArchitecturalCellKind.Wall), (18, 1, ArchitecturalCellKind.Wall),
            (1, 12, ArchitecturalCellKind.Wall), (2, 12, ArchitecturalCellKind.Wall), (3, 12, ArchitecturalCellKind.Wall),
            (4, 12, ArchitecturalCellKind.Wall), (5, 12, ArchitecturalCellKind.Wall), (6, 12, ArchitecturalCellKind.Wall),
            (7, 12, ArchitecturalCellKind.Wall), (8, 12, ArchitecturalCellKind.Wall), (9, 12, ArchitecturalCellKind.Wall),
            (10, 12, ArchitecturalCellKind.Door), (11, 12, ArchitecturalCellKind.Wall), (12, 12, ArchitecturalCellKind.Wall),
            (13, 12, ArchitecturalCellKind.Wall), (14, 12, ArchitecturalCellKind.Wall), (15, 12, ArchitecturalCellKind.Wall),
            (16, 12, ArchitecturalCellKind.Wall), (17, 12, ArchitecturalCellKind.Wall), (18, 12, ArchitecturalCellKind.Wall),
            (3, 3, ArchitecturalCellKind.Room), (4, 3, ArchitecturalCellKind.Room), (3, 4, ArchitecturalCellKind.Room),
            (4, 4, ArchitecturalCellKind.Room), (15, 3, ArchitecturalCellKind.Room), (16, 3, ArchitecturalCellKind.Room),
            (15, 4, ArchitecturalCellKind.Room), (16, 4, ArchitecturalCellKind.Room),
            (17, 9, ArchitecturalCellKind.Stair), (18, 9, ArchitecturalCellKind.Elevator),
            (2, 9, ArchitecturalCellKind.Ladder)
        ],
        [
            new("stair-17-9", ArchitecturalVerticalConnectorKind.Stair, 17, 9, 0, 1, "STAIR"),
            new("elevator-18-9", ArchitecturalVerticalConnectorKind.Elevator, 18, 9, 0, 2, "ELEVATOR"),
            new("ladder-2-9", ArchitecturalVerticalConnectorKind.Ladder, 2, 9, 0, 1, "LADDER")
        ]);

    public static ArchitecturalFloorPlan Gallery => Build(
        "gallery", "Gallery", 20, 14,
        [
            (1, 1, ArchitecturalCellKind.Wall), (2, 1, ArchitecturalCellKind.Wall), (3, 1, ArchitecturalCellKind.Wall),
            (4, 1, ArchitecturalCellKind.Wall), (5, 1, ArchitecturalCellKind.Wall), (6, 1, ArchitecturalCellKind.Wall),
            (7, 1, ArchitecturalCellKind.Wall), (8, 1, ArchitecturalCellKind.Wall), (9, 1, ArchitecturalCellKind.Wall),
            (10, 1, ArchitecturalCellKind.Door), (11, 1, ArchitecturalCellKind.Wall), (12, 1, ArchitecturalCellKind.Wall),
            (13, 1, ArchitecturalCellKind.Wall), (14, 1, ArchitecturalCellKind.Wall), (15, 1, ArchitecturalCellKind.Wall),
            (16, 1, ArchitecturalCellKind.Wall), (17, 1, ArchitecturalCellKind.Wall), (18, 1, ArchitecturalCellKind.Wall),
            (5, 4, ArchitecturalCellKind.Room), (6, 4, ArchitecturalCellKind.Room), (7, 4, ArchitecturalCellKind.Room),
            (12, 4, ArchitecturalCellKind.Room), (13, 4, ArchitecturalCellKind.Room), (14, 4, ArchitecturalCellKind.Room),
            (9, 9, ArchitecturalCellKind.Stair), (10, 9, ArchitecturalCellKind.Stair),
            (15, 9, ArchitecturalCellKind.Elevator)
        ],
        [
            new("stair-9-9", ArchitecturalVerticalConnectorKind.Stair, 9, 9, 0, 1, "STAIR"),
            new("stair-10-9", ArchitecturalVerticalConnectorKind.Stair, 10, 9, 0, 1, "STAIR"),
            new("elevator-15-9", ArchitecturalVerticalConnectorKind.Elevator, 15, 9, 0, 3, "ELEVATOR")
        ]);

    public static ArchitecturalFloorPlan TowerCore => Build(
        "tower-core", "Tower Core", 20, 14,
        [
            (1, 1, ArchitecturalCellKind.Wall), (2, 1, ArchitecturalCellKind.Wall), (3, 1, ArchitecturalCellKind.Wall),
            (4, 1, ArchitecturalCellKind.Wall), (5, 1, ArchitecturalCellKind.Wall), (6, 1, ArchitecturalCellKind.Wall),
            (7, 1, ArchitecturalCellKind.Wall), (8, 1, ArchitecturalCellKind.Wall), (9, 1, ArchitecturalCellKind.Wall),
            (10, 1, ArchitecturalCellKind.Wall), (11, 1, ArchitecturalCellKind.Wall), (12, 1, ArchitecturalCellKind.Wall),
            (13, 1, ArchitecturalCellKind.Wall), (14, 1, ArchitecturalCellKind.Wall), (15, 1, ArchitecturalCellKind.Wall),
            (16, 1, ArchitecturalCellKind.Wall), (17, 1, ArchitecturalCellKind.Wall), (18, 1, ArchitecturalCellKind.Wall),
            (8, 5, ArchitecturalCellKind.Stair), (9, 5, ArchitecturalCellKind.Stair),
            (10, 5, ArchitecturalCellKind.Elevator), (11, 5, ArchitecturalCellKind.Elevator),
            (9, 9, ArchitecturalCellKind.Ladder), (10, 9, ArchitecturalCellKind.Room)
        ],
        [
            new("stair-8-5", ArchitecturalVerticalConnectorKind.Stair, 8, 5, 0, 1, "STAIR"),
            new("stair-9-5", ArchitecturalVerticalConnectorKind.Stair, 9, 5, 0, 1, "STAIR"),
            new("elevator-10-5", ArchitecturalVerticalConnectorKind.Elevator, 10, 5, 0, 6, "ELEVATOR"),
            new("elevator-11-5", ArchitecturalVerticalConnectorKind.Elevator, 11, 5, 0, 6, "ELEVATOR"),
            new("ladder-9-9", ArchitecturalVerticalConnectorKind.Ladder, 9, 9, 0, 1, "LADDER")
        ]);

    private static ArchitecturalFloorPlan Build(
        string id,
        string name,
        int width,
        int height,
        IEnumerable<(int X, int Y, ArchitecturalCellKind Kind)> cells,
        IReadOnlyList<ArchitecturalVerticalConnector> connectors)
        => new(id, name, width, height,
            cells.Select(cell => new ArchitecturalCell(cell.X, cell.Y, cell.Kind)).ToArray(),
            connectors);
}
