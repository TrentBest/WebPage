namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Pure semantic data for the code diagrams attached to a selected source artifact.
/// The diagram is centered on one source file and exposes only the types and external
/// dependencies actually used by that artifact.
/// </summary>
public sealed class SpatialCodeDiagramModel
{
    public SpatialCodeDiagramModel(string sourcePath, string declaredTypeName)
    {
        SourcePath = string.IsNullOrWhiteSpace(sourcePath) ? throw new ArgumentException("A source path is required.", nameof(sourcePath)) : sourcePath;
        DeclaredTypeName = string.IsNullOrWhiteSpace(declaredTypeName) ? throw new ArgumentException("A declared type name is required.", nameof(declaredTypeName)) : declaredTypeName;
    }

    public string SourcePath { get; }
    public string DeclaredTypeName { get; }

    public SpatialCodeClassCard ClassCard { get; } = new();

    /// <summary>Only core data types actually used by the selected source file.</summary>
    public List<SpatialCodeTypeCard> CoreTypeCards { get; } = [];

    /// <summary>Only packages, assemblies, or DLLs actually used by the selected source file.</summary>
    public List<SpatialCodeDependencyCard> DependencyCards { get; } = [];

    public List<SpatialCodeMethodSequence> Sequence { get; } = [];

    public List<SpatialComplexityEntry> Complexity { get; } = [];

    public SpatialCodeDiagramModel AddCoreType(string typeName)
    {
        if (!CoreTypeCards.Any(card => string.Equals(card.Name, typeName, StringComparison.Ordinal)))
            CoreTypeCards.Add(new SpatialCodeTypeCard(typeName));

        return this;
    }

    public SpatialCodeDiagramModel AddDependency(string name)
    {
        if (!DependencyCards.Any(card => string.Equals(card.Name, name, StringComparison.Ordinal)))
            DependencyCards.Add(new SpatialCodeDependencyCard(name));

        return this;
    }

    public SpatialCodeDiagramModel AddMethodSequence(string methodName, IReadOnlyList<string> steps, string complexity)
    {
        Sequence.Add(new SpatialCodeMethodSequence(methodName, steps));
        Complexity.Add(new SpatialComplexityEntry(methodName, complexity));
        return this;
    }
}

/// <summary>The default UML class card is deliberately compact: the type name is the center.</summary>
public sealed class SpatialCodeClassCard
{
    public string? Name { get; set; }
    public string Kind { get; set; } = "class";
    public List<string> PublicProperties { get; } = [];
    public List<string> Fields { get; } = [];
    public List<string> Methods { get; } = [];
}

/// <summary>A colored semantic card for a core runtime data type.</summary>
public sealed record SpatialCodeTypeCard(string Name);

/// <summary>A semantic card for an external package, assembly, or DLL.</summary>
public sealed record SpatialCodeDependencyCard(string Name);

/// <summary>A sequence row focused on one method of the selected source artifact.</summary>
public sealed record SpatialCodeMethodSequence(string MethodName, IReadOnlyList<string> Steps);

/// <summary>Calculated complexity displayed beneath the sequence diagram.</summary>
public sealed record SpatialComplexityEntry(string Subject, string BigO);
