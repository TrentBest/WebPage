namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Pure data describing an elevator control surface that can address a very large
/// number of floors without requiring one physical button per floor.
/// </summary>
public sealed class SpatialElevatorControlModel
{
    public const int DirectFloorButtonLimit = 9;
    public const int DigitalDisplayThreshold = 99;

    public SpatialElevatorControlModel(IEnumerable<SpatialElevatorFloor> floors)
    {
        ArgumentNullException.ThrowIfNull(floors);
        Floors = floors.OrderBy(floor => floor.Number).ToArray();

        if (Floors.Count == 0)
            throw new ArgumentException("An elevator must expose at least one floor.", nameof(floors));

        SelectedFloor = Floors[0].Number;
    }

    public IReadOnlyList<SpatialElevatorFloor> Floors { get; }

    /// <summary>Lower levels remain directly addressable as individual buttons.</summary>
    public IReadOnlyList<int> DirectFloorButtons
        => Floors.Where(floor => floor.Number >= 0 && floor.Number <= DirectFloorButtonLimit)
            .Select(floor => floor.Number)
            .ToArray();

    /// <summary>Multi-digit floors are entered as a sequence of digit presses.</summary>
    public bool UsesDigitalDisplay => Floors.Any(floor => floor.Number > DigitalDisplayThreshold);

    public int SelectedFloor { get; private set; }

    public string Display
        => UsesDigitalDisplay || SelectedFloor > DirectFloorButtonLimit
            ? SelectedFloor.ToString()
            : $"F{SelectedFloor}";

    public IReadOnlyList<SpatialElevatorFloor> VisibleFloorDirectory { get; private set; } = Array.Empty<SpatialElevatorFloor>();

    public void ShowDirectoryPage(int page, int pageSize = 12)
    {
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (page < 0) throw new ArgumentOutOfRangeException(nameof(page));

        VisibleFloorDirectory = Floors
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToArray();
    }

    public bool SelectFloor(int floor)
    {
        if (!Floors.Any(item => item.Number == floor))
            return false;

        SelectedFloor = floor;
        return true;
    }

    public int StepFloor(int direction)
    {
        if (direction == 0) return SelectedFloor;

        var index = Array.FindIndex(Floors.ToArray(), floor => floor.Number == SelectedFloor);
        var next = Math.Clamp(index + Math.Sign(direction), 0, Floors.Count - 1);
        SelectedFloor = Floors[next].Number;
        return SelectedFloor;
    }

    /// <summary>
    /// Resolves a digit sequence such as 1, 2, 4 into floor 124 when that floor exists.
    /// The sequence is intentionally data-only; a GUI builder can choose the visual input.
    /// </summary>
    public bool TrySelectDigitSequence(IEnumerable<int> digits)
    {
        ArgumentNullException.ThrowIfNull(digits);

        var values = digits.ToArray();
        if (values.Length == 0 || values.Any(digit => digit is < 0 or > 9))
            return false;

        if (values.Length > 1 && values[0] == 0)
            return false;

        var candidate = 0;
        foreach (var digit in values)
        {
            candidate = checked(candidate * 10 + digit);
        }

        return SelectFloor(candidate);
    }
}

/// <summary>A floor's human-facing directory entry.</summary>
public sealed record SpatialElevatorFloor(int Number, string Name, string Purpose);
