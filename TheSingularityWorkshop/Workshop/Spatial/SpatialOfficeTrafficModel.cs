namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Pure-data traffic policy for a living office. It does not simulate nicotine use;
/// it describes when a social rooftop is eligible to receive employee traffic.
/// </summary>
public sealed class SpatialOfficeTrafficModel
{
    private readonly List<SpatialOfficeEmployee> _employees = [];

    public double SmokeBreakParticipationRatio { get; set; } = .90;

    public IReadOnlyList<SpatialOfficeEmployee> Employees => _employees;

    public IReadOnlyList<SpatialOfficeEmployee> SmokeBreakGroup
        => _employees
            .Where(employee => employee.TakesSmokeBreaks)
            .ToArray();

    public void AddEmployee(SpatialOfficeEmployee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        _employees.Add(employee);
    }

    /// <summary>Returns the configured population ratio without inventing individual identities.</summary>
    public int ExpectedSmokeBreakPopulation
        => (int)Math.Round(_employees.Count * Math.Clamp(SmokeBreakParticipationRatio, 0d, 1d), MidpointRounding.AwayFromZero);
}
