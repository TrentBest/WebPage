using System.Text.Json;

namespace TheSingularityWorkshop.Services;

/// <summary>Manifest supplied by the WebPage host. It describes startup presentation, the visitor hub, and running Experiences.</summary>
public sealed record WebPageHostManifest(
    string ManifestId,
    string Version,
    IReadOnlyList<WebPageManifestExperience> Startup,
    IReadOnlyList<WebPageManifestHubItem> Hub,
    IReadOnlyList<WebPageManifestExperience> Running,
    WebPageDeepDiveManifest DeepDive);

/// <summary>One manifest-selected startup or running Experience.</summary>
public sealed record WebPageManifestExperience(
    string Name,
    string Kind,
    IReadOnlyList<ulong> MicroBundleIds,
    double PresentationSeconds);

/// <summary>One manifest-defined hub destination. The host renders this data; it does not hard-code the catalog.</summary>
public sealed record WebPageManifestHubItem(
    string Name,
    string Kind,
    string Description,
    string? DeploymentUrl,
    string AboutUrl,
    string? BridgeEndpoint,
    string? Route);

/// <summary>Manifest-defined diagnostic restart contract.</summary>
public sealed record WebPageDeepDiveManifest(
    string Experience,
    bool RestartFromManifest,
    bool Telemetry);

/// <summary>Parses the host manifest without embedding concrete Experience selection in the parser.</summary>
public static class WebPageHostManifestParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static WebPageHostManifest Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        return JsonSerializer.Deserialize<WebPageHostManifest>(json, Options)
            ?? throw new InvalidOperationException("The WebPage host manifest is empty.");
    }
}
