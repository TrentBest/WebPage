namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Captures the user's intended AEC structure before conventional drafting begins.
/// The interrogation walks the nine ontology layers from broad reality/fiction context
/// down to the concrete building type, then assembles a navigable spatial concept.
/// </summary>
public sealed class SpatialAECIntentModel
{
    public static readonly IReadOnlyList<string> LayerNames =
    [
        "Paradigm", "Domain", "Kingdom", "Phylum", "Class",
        "Order", "Family", "Genus", "Species"
    ];

    private readonly List<string?> _answers = new(new string?[9]);

    public int CurrentLayer { get; private set; }

    public IReadOnlyList<string?> Answers => _answers;

    public string? CurrentAnswer => _answers[CurrentLayer];

    public bool IsComplete => CurrentLayer >= LayerNames.Count;

    public string BuildingType { get; private set; } = "UNRESOLVED STRUCTURE";

    public string Summary
        => string.Join(" / ", _answers.Where(x => !string.IsNullOrWhiteSpace(x)));

    public IReadOnlyList<string> OptionsForCurrentLayer
        => CurrentLayer switch
        {
            0 => ["REALITY", "FICTION"],
            1 => ["BUILT ENVIRONMENT", "INDUSTRIAL", "CIVIC", "RESIDENTIAL", "RESEARCH"],
            2 => ["FACILITY", "CAMPUS", "INFRASTRUCTURE", "WORKPLACE"],
            3 => ["LABORATORY", "OFFICE", "FACTORY", "HOSPITAL", "WAREHOUSE"],
            4 => ["RESEARCH FACILITY", "HIGH-TECH LABORATORY", "PRODUCTION LABORATORY", "FIELD LAB"],
            5 => ["SINGLE BUILDING", "MULTI-BUILDING", "VERTICAL FACILITY", "CAMPUS FACILITY"],
            6 => ["SCIENCE", "ENGINEERING", "MEDICAL", "ENERGY", "AEROSPACE"],
            7 => ["ADVANCED RESEARCH", "APPLIED RESEARCH", "DEVELOPMENT", "TESTING"],
            8 => ["SCI-FI HIGH-TECH LABORATORY", "RESEARCH LABORATORY", "ENGINEERING LABORATORY", "CUSTOM FACILITY"],
            _ => []
        };

    public void Answer(string value)
    {
        if (IsComplete) return;
        if (!OptionsForCurrentLayer.Contains(value, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"'{value}' is not a valid answer for {LayerNames[CurrentLayer]}.", nameof(value));

        _answers[CurrentLayer] = value.ToUpperInvariant();
        CurrentLayer++;

        if (IsComplete)
            BuildingType = ResolveBuildingType();
    }

    public void Reset()
    {
        for (var i = 0; i < _answers.Count; i++) _answers[i] = null;
        CurrentLayer = 0;
        BuildingType = "UNRESOLVED STRUCTURE";
    }

    public SpatialAECGeneratedStructure Generate(double squareUnits = 2400)
    {
        if (!IsComplete)
            throw new InvalidOperationException("The AEC intent must reach the concrete building type before generation.");

        var normalized = Math.Max(400, squareUnits);
        var footprint = Math.Sqrt(normalized);
        var floorCount = _answers[5] switch
        {
            "VERTICAL FACILITY" => 6,
            "CAMPUS FACILITY" => 3,
            "MULTI-BUILDING" => 2,
            _ => 1
        };

        var rooms = new List<SpatialAECGeneratedRoom>
        {
            new("lobby", "Arrival / Reception", footprint * .16, footprint * .16),
            new("conference", "Conference / Briefing", footprint * .14, footprint * .12),
            new("research", "Research Core", footprint * .28, footprint * .24),
            new("fabrication", "Fabrication / Prototyping", footprint * .22, footprint * .18),
            new("support", "Building Support", footprint * .12, footprint * .16),
            new("systems", "MEP / Systems", footprint * .10, footprint * .24)
        };

        return new SpatialAECGeneratedStructure(
            BuildingType,
            normalized,
            floorCount,
            Math.Round(footprint, 1),
            rooms,
            Summary);
    }

    private string ResolveBuildingType()
    {
        if (_answers[0] == "FICTION" &&
            _answers[4] == "HIGH-TECH LABORATORY" &&
            _answers[6] == "SCIENCE" &&
            _answers[7] == "ADVANCED RESEARCH")
            return "SCI-FI HIGH-TECH RESEARCH LABORATORY";

        return _answers[8] ?? "CUSTOM FACILITY";
    }
}

/// <summary>Generated spatial concept produced from an AEC intent interrogation.</summary>
public sealed record SpatialAECGeneratedStructure(
    string BuildingType,
    double SquareUnits,
    int Floors,
    double ApproximateSideLength,
    IReadOnlyList<SpatialAECGeneratedRoom> Rooms,
    string OntologySummary);

public sealed record SpatialAECGeneratedRoom(
    string Id,
    string Name,
    double Width,
    double Depth);
