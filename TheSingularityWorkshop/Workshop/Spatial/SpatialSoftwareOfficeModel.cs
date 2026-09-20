namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Pure data model for the software-development office as a living manifestation of
/// the artifact being built. It deliberately contains no rendering, serialization, or
/// persistence concerns.
/// </summary>
public sealed class SpatialSoftwareOfficeModel
{
    private readonly List<SpatialSoftwareOfficeFloor> _floors = [];
    private readonly List<SpatialOfficeEmployee> _employees = [];

    public SpatialSoftwareOfficeModel(string? requestedName = null)
    {
        Name = SpatialUniqueNameAllocator.Allocate(requestedName, [], "Software Office 001");
        AddFoundationalFloors();
    }

    public string Name { get; private set; }

    public IReadOnlyList<SpatialSoftwareOfficeFloor> Floors => _floors;

    public IReadOnlyList<SpatialOfficeEmployee> Employees => _employees;

    public SpatialCodeArtifactModel Artifact { get; } = new();

    public SpatialOfficeEntanglement Entanglement { get; private set; } = SpatialOfficeEntanglement.Unentangled;

    public void Rename(string requestedName, IEnumerable<string> siblingOfficeNames)
    {
        Name = SpatialUniqueNameAllocator.EnsureUnique(requestedName, siblingOfficeNames, Name);
    }

    /// <summary>Adds a floor only when its semantic purpose is not already represented.</summary>
    public SpatialSoftwareOfficeFloor AddFloor(string requestedName, string purpose)
    {
        var name = SpatialUniqueNameAllocator.Allocate(
            requestedName,
            _floors.Select(floor => floor.Name),
            "New Floor");

        var floor = new SpatialSoftwareOfficeFloor(_floors.Count + 1, name, purpose);
        _floors.Add(floor);
        return floor;
    }

    /// <summary>
    /// Grows the office from semantic capabilities. Repeated calls are idempotent:
    /// the building reflects the artifact without producing duplicate departments.
    /// </summary>
    public void SynchronizeFromArtifact()
    {
        if (Artifact.Properties.Count > 0 || Artifact.Fields.Count > 0)
            EnsureFloor("Structure", "Types, properties, fields, relationships, and class structure.");

        if (Artifact.Methods.Count > 0)
            EnsureFloor("Behavior", "Methods, parameters, statements, expressions, FSMs, and commands.");

        if (Artifact.CoreTypes.Count > 0 || Artifact.ExternalDependencies.Count > 0)
            EnsureFloor("Architecture", "Core data types, packages, assemblies, and dependency relationships.");

        if (Artifact.Diagrams.Contains(SpatialCodeDiagramKind.Class))
            EnsureFloor("Class Diagram", "Semantic class visualization centered on the selected source artifact.");

        if (Artifact.Diagrams.Contains(SpatialCodeDiagramKind.Sequence))
            EnsureFloor("Sequence Analysis", "Current-class execution sequence and calculated O-notation.");

        if (Artifact.Diagrams.Contains(SpatialCodeDiagramKind.Collaboration))
            EnsureFloor("Collaboration", "Object collaboration and responsibility relationships.");

        if (Artifact.Diagrams.Contains(SpatialCodeDiagramKind.UseCase))
            EnsureFloor("Use Case", "LLM-assisted use-case discovery; under construction until semantic population exists.");

        if (Artifact.IsExecutable)
            EnsureFloor("Play Test", "Execute the artifact, observe it, and validate behavior.");

        if (Artifact.IsDocumentable)
            EnsureFloor("Documentation", "Generate and maintain documentation from the same semantic artifact.");

        if (Artifact.IsPublishable)
            EnsureFloor("Publishing", "Prepare the artifact for its target package, application, or deployment surface.");

        UpdateEntanglement();
    }

    public SpatialElevatorControlModel CreateElevator()
        => new(_floors.Select(floor => new SpatialElevatorFloor(floor.Number, floor.Name, floor.Purpose)));

    public void AddEmployee(SpatialOfficeEmployee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        _employees.Add(employee);
    }

    private void AddFoundationalFloors()
    {
        AddFloor("Reception", "Intent routing, guest registration, project orientation, and workspace selection.");
        AddFloor("Architecture", "Understand what is being built before editing its pieces.");
        AddFloor("Structure", "Types, data, properties, fields, and semantic relationships.");
        AddFloor("Behavior", "Methods, state, commands, and executable behavior.");
        AddFloor("Visualization", "Images, meshes, spatial artifacts, GUI, and presentation.");
        AddFloor("Play Test", "Run the artifact and inspect what it actually does.");
        AddFloor("Documentation", "Explain the artifact from its semantic source.");
        AddFloor("Publishing", "Package and publish the resulting artifact.");
        AddFloor("Roof — Smoke Break", "A social rooftop where office traffic naturally appears and disappears.");
    }

    private void EnsureFloor(string name, string purpose)
    {
        if (_floors.Any(floor => string.Equals(floor.Name, name, StringComparison.OrdinalIgnoreCase)))
            return;

        AddFloor(name, purpose);
    }

    private void UpdateEntanglement()
    {
        var hasStructure = Artifact.Properties.Count > 0 || Artifact.Fields.Count > 0 || Artifact.Methods.Count > 0;
        var hasExecution = Artifact.IsExecutable;
        var hasPlayTest = _floors.Any(floor => floor.Name == "Play Test");

        Entanglement = hasStructure && hasExecution && hasPlayTest
            ? SpatialOfficeEntanglement.Entangled
            : hasStructure
                ? SpatialOfficeEntanglement.Forming
                : SpatialOfficeEntanglement.Unentangled;
    }
}

/// <summary>The semantic artifact currently represented by the office.</summary>
public sealed class SpatialCodeArtifactModel
{
    public string? SourcePath { get; set; }
    public string? DeclaredTypeName { get; set; }

    public List<string> CoreTypes { get; } = [];
    public List<string> ExternalDependencies { get; } = [];
    public List<string> Properties { get; } = [];
    public List<string> Fields { get; } = [];
    public List<string> Methods { get; } = [];
    public HashSet<SpatialCodeDiagramKind> Diagrams { get; } = [];

    public bool IsExecutable { get; set; }
    public bool IsDocumentable { get; set; }
    public bool IsPublishable { get; set; }
}

/// <summary>The four planned code-relationship diagram surfaces.</summary>
public enum SpatialCodeDiagramKind
{
    Class,
    Sequence,
    Collaboration,
    UseCase
}

/// <summary>Lifecycle state for the moment the physical office becomes coupled to the artifact.</summary>
public enum SpatialOfficeEntanglement
{
    Unentangled,
    Forming,
    Entangled
}

/// <summary>Minimal pure-data employee used by the office population layer.</summary>
public sealed record SpatialOfficeEmployee(
    string Id,
    string Name,
    string Role,
    string Department,
    bool TakesSmokeBreaks);
/// <summary>One semantic floor in the generated office.</summary>
public sealed record SpatialSoftwareOfficeFloor(int Number, string Name, string Purpose);
