using System.Text.Json;
using System.Text.Json.Serialization;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>
/// Workshop Experience for authoring and serializing REST capability recipes.
/// The Blazor surface is only a manifestation; the recipe itself is portable data.
/// </summary>
public sealed class RestCapabilityLabExperience : IExperience
{
    public const ulong ExperienceId = 3010;

    private readonly List<RestCapabilityOperation> _operations =
    [
        new("GetCatalog", "GET", "/catalog")
    ];

    public ulong Id => ExperienceId;
    public string Name => "REST CAPABILITY LAB";
    public BundleVersion Version => new(1, 0, 0);
    public OntologySignature Ontology => new(0, 0, 0, 0, 0, 0, 0, 0, checked((int)ExperienceId));
    public IReadOnlyList<ulong> MicroBundleIds { get; } = [4010];
    public IReadOnlyList<ulong> Capabilities { get; } = [1];
    public IReadOnlyList<ulong> SensorySystems { get; } = [1];
    public IReadOnlyList<string> ProcessingGroups { get; } = ["MicroBundle_4010"];

    public string ApiName { get; set; } = "Workshop Catalog API";
    public string BaseUri { get; set; } = "https://api.example.com";
    public IReadOnlyList<RestCapabilityOperation> Operations => _operations;

    public void AddOperation(string name, string method, string path)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Operation name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(method)) throw new ArgumentException("HTTP method is required.", nameof(method));
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Path is required.", nameof(path));

        _operations.Add(new RestCapabilityOperation(name.Trim(), method.Trim().ToUpperInvariant(), NormalizePath(path)));
    }

    public void RemoveOperation(int index)
    {
        if ((uint)index >= (uint)_operations.Count) return;
        _operations.RemoveAt(index);
    }

    public string Serialize()
        => JsonSerializer.Serialize(
            new RestCapabilityRecipe(
                ApiName.Trim(),
                BaseUri.Trim().TrimEnd('/'),
                _operations),
            JsonOptions);

    public void Reset()
    {
        ApiName = "Workshop Catalog API";
        BaseUri = "https://api.example.com";
        _operations.Clear();
        _operations.Add(new RestCapabilityOperation("GetCatalog", "GET", "/catalog"));
    }

    private static string NormalizePath(string path)
    {
        var normalized = path.Trim();
        return normalized.StartsWith('/') ? normalized : "/" + normalized;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public sealed record RestCapabilityRecipe(
    string Name,
    string BaseUri,
    IReadOnlyList<RestCapabilityOperation> Operations);

public sealed record RestCapabilityOperation(
    string Name,
    string Method,
    string Path);
