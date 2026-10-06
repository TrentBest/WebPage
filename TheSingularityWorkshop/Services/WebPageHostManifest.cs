using System.Text.Json;

namespace TheSingularityWorkshop.Services;

/// <summary>Manifest supplied by the WebPage host. It describes startup order and preload intent.</summary>
public sealed record WebPageHostManifest(
    string ManifestId,
    string Version,
    IReadOnlyList<WebPageManifestExperience> Startup,
    IReadOnlyList<WebPageManifestExperience> Preload,
    WebPageDeepDiveManifest DeepDive);

/// <summary>One manifest-selected startup or preload Experience.</summary>
public sealed record WebPageManifestExperience(
    string Name,
    string Kind,
    IReadOnlyList<ulong> MicroBundleIds,
    double PresentationSeconds);

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
