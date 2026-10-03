using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

/// <summary>
/// File-backed WebApp composition manifest. The WebPage host reads this data and
/// translates it into the machine-oriented manifest consumed by FSM_COS.
/// </summary>
public sealed record WebPageRuntimeManifest
{
    public ulong RuntimeId { get; init; } = 1;

    [JsonPropertyName("bundles")]
    public IReadOnlyList<WebPageBundleRequest> Bundles { get; init; } = Array.Empty<WebPageBundleRequest>();
}

/// <summary>
/// One MicroBundle request from the WebApp runtime configuration file.
/// </summary>
public sealed record WebPageBundleRequest
{
    public ulong Id { get; init; }

    /// <summary>Optional base64 configuration bytes passed unchanged to FSM_COS.</summary>
    public string? Configuration { get; init; }
}
