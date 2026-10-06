using System.Text.Json;
using System.Text.Json.Serialization;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.Configuration;

/// <summary>
/// Host-owned Workshop manifest. It declares the Experience composition phases and
/// the Hub surfaces that the presentation host should expose.
/// </summary>
public sealed record WorkshopManifestDocument
{
    public ulong RuntimeId { get; init; } = 4000;
    public WorkshopExperienceManifest Experiences { get; init; } = new();
    public WorkshopHubManifest Hub { get; init; } = new();
}

/// <summary>Ordered Experience composition declared by the Workshop manifest.</summary>
public sealed record WorkshopExperienceManifest
{
    public ulong[] Startup { get; init; } = [];
    public ulong[] Transitioning { get; init; } = [];
    public ulong[] Running { get; init; } = [];
    public ulong[] RequiredCapabilities { get; init; } = [];

    public ExperienceManifest ToRuntimeManifest()
        => new(Startup, Transitioning, Running, RequiredCapabilities);
}

/// <summary>One configured Hub level and its manifest-defined tabs.</summary>
public sealed record WorkshopHubManifest
{
    public ulong Id { get; init; } = 1000;
    public string Path { get; init; } = "/";
    public string Title { get; init; } = "THE WORKSHOP";
    public WorkshopHubTabManifest[] Tabs { get; init; } = [];

    public SingularityHubDefinition ToDefinition()
        => new(
            Id,
            Path,
            Title,
            Tabs.Select(tab => tab.ToDefinition()));
}

/// <summary>Serializable representation of a Hub tab.</summary>
public sealed record WorkshopHubTabManifest
{
    public ulong Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public int Order { get; init; }
    public HubTabTargetKind TargetKind { get; init; } = HubTabTargetKind.Tool;
    public string Route { get; init; } = string.Empty;
    public ulong? TargetId { get; init; }
    public HubTabAcquisition? Acquisition { get; init; }
    public string? Icon { get; init; }
    public string? Tone { get; init; }

    public HubTabDefinition ToDefinition()
        => new(Id, Label, Order, TargetKind, Route, TargetId, Acquisition, Icon, Tone);
}

/// <summary>Shared JSON options for the Workshop host manifest.</summary>
public static class WorkshopManifestJson
{
    public static JsonSerializerOptions Options { get; } = Create();

    private static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}

/// <summary>
/// Holds the manifest loaded for the current WebPage host. Tests may use the
/// built-in default while the browser host loads the file-backed manifest.
/// </summary>
public sealed class WorkshopManifestStore
{
    public WorkshopManifestDocument Manifest { get; private set; } = WorkshopManifestDefaults.Create();

    public void Set(WorkshopManifestDocument manifest)
        => Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
}

/// <summary>Loads the host manifest from the WebPage static configuration surface.</summary>
public sealed class WorkshopManifestLoader(HttpClient httpClient)
{
    private const string ManifestPath = "runtime/workshop.manifest.json";

    public async Task<WorkshopManifestDocument> LoadAsync(CancellationToken cancellationToken = default)
    {
        var manifest = await httpClient.GetFromJsonAsync<WorkshopManifestDocument>(
            ManifestPath,
            WorkshopManifestJson.Options,
            cancellationToken);

        return manifest ?? throw new InvalidOperationException(
            $"The Workshop manifest '{ManifestPath}' was empty.");
    }
}

/// <summary>Safe in-process default used only before a file-backed manifest is loaded.</summary>
public static class WorkshopManifestDefaults
{
    public const ulong MonikerExperienceId = 2001;

    public static WorkshopManifestDocument Create()
        => new()
        {
            RuntimeId = 4000,
            Experiences = new WorkshopExperienceManifest
            {
                Startup = [MonikerExperienceId],
                Running = [LivingGuiExperienceId],
                RequiredCapabilities = [HubCapabilityIds.Moniker]
            },
            Hub = new WorkshopHubManifest
            {
                Id = 1000,
                Path = "/",
                Title = "THE WORKSHOP",
                Tabs =
                [
                    new() { Id = 1, Label = "Understand", Order = 0, Route = "/about-us", TargetId = 5001, Icon = "info", Tone = "magenta" },
                    new() { Id = 2, Label = "Experiences", Order = 1, Route = "/explore", TargetKind = HubTabTargetKind.Experience, TargetId = 3002, Icon = "eye", Tone = "cyan" },
                    new() { Id = 3, Label = "Create", Order = 2, Route = "/create", TargetId = 5003, Icon = "wrench", Tone = "yellow" },
                    new() { Id = 4, Label = "Rendering", Order = 3, Route = "/rendering", TargetId = 5004, Icon = "globe", Tone = "cyan" },
                    new() { Id = 5, Label = "Education", Order = 4, Route = "/education", TargetId = 5005, Tone = "green" },
                    new() { Id = 6, Label = "MadMen", Order = 5, Route = "/madmen", TargetId = 5006, Tone = "orange" },
                    new() { Id = 7, Label = "Support", Order = 6, Route = "/support", TargetId = 5007, Icon = "heart", Tone = "red" }
                ]
            }
        };

    private const ulong LivingGuiExperienceId = 3002;
}
