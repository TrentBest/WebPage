using System.Text.Json;
using TheSingularityWorkshop.FSM_COS;

namespace TheSingularityWorkshop.Services;

/// <summary>Manifest supplied by the WebPage host. It describes startup presentation, the visitor hub, and running Experiences.</summary>
public sealed record WebPageHostManifest(
    string ManifestId,
    string Version,
    IReadOnlyList<WebPageManifestExperience> Startup,
    IReadOnlyList<WebPageManifestHubItem> Hub,
    IReadOnlyList<WebPageManifestExperience> Running,
    WebPageDeepDiveManifest DeepDive,
    IReadOnlyList<WebPageManifestCapability>? Capabilities = null);

/// <summary>One manifest-selected startup or running Experience, with explicit MicroBundle versions.</summary>
public sealed record WebPageManifestExperience(
    string Name,
    string Kind,
    IReadOnlyList<WebPageManifestBundle>? MicroBundles,
    double PresentationSeconds);

/// <summary>One versioned MicroBundle root selected by the WebPage host manifest.</summary>
public sealed record WebPageManifestBundle(ulong BundleId, string Version)
{
    public MicroBundleManifestEntry ToRuntimeManifestEntry() => new(BundleId, Version);
}

/// <summary>One manifest-defined hub destination. The host renders this data; it does not hard-code the catalog.</summary>
public sealed record WebPageManifestHubItem(
    string Name,
    string Kind,
    string Description,
    string? DeploymentUrl,
    string AboutUrl,
    string? BridgeEndpoint,
    string? Route,
    IReadOnlyList<WebPageManifestBundle>? MicroBundles = null);

/// <summary>A manifest-declared supporting capability that is composed without becoming a top-level hub destination.</summary>
public sealed record WebPageManifestCapability(
    string Name,
    IReadOnlyList<WebPageManifestBundle>? MicroBundles);

/// <summary>Manifest-defined diagnostic restart contract.</summary>
public sealed record WebPageDeepDiveManifest(
    string Experience,
    bool RestartFromManifest,
    bool Telemetry);

/// <summary>Parses and validates the host manifest without embedding concrete Experience selection in the parser.</summary>
public static class WebPageHostManifestParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static WebPageHostManifest Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        var manifest = JsonSerializer.Deserialize<WebPageHostManifest>(json, Options)
            ?? throw new InvalidOperationException("The WebPage host manifest is empty.");

        ValidateBundles(manifest.Startup.SelectMany(x => x.MicroBundles ?? Array.Empty<WebPageManifestBundle>()));
        ValidateBundles(manifest.Running.SelectMany(x => x.MicroBundles ?? Array.Empty<WebPageManifestBundle>()));
        ValidateBundles((manifest.Hub ?? Array.Empty<WebPageManifestHubItem>())
            .SelectMany(x => x.MicroBundles ?? Array.Empty<WebPageManifestBundle>()));
        ValidateBundles((manifest.Capabilities ?? Array.Empty<WebPageManifestCapability>())
            .SelectMany(x => x.MicroBundles ?? Array.Empty<WebPageManifestBundle>()));

        return manifest;
    }

    private static void ValidateBundles(IEnumerable<WebPageManifestBundle> bundles)
    {
        foreach (var bundle in bundles)
        {
            if (bundle.BundleId == 0)
                throw new InvalidOperationException("The WebPage host manifest contains a zero MicroBundle ID.");
            if (string.IsNullOrWhiteSpace(bundle.Version))
                throw new InvalidOperationException(
                    $"The WebPage host manifest does not declare a version for MicroBundle {bundle.BundleId}.");
        }
    }
}
