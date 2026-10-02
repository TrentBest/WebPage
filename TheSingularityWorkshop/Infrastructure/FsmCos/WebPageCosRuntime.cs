using System.Text.Json;
using TheSingularityWorkshop.FSM_COS;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

/// <summary>
/// WebApp runtime boundary between file-backed composition and FSM_COS.
///
/// WebPage is responsible for loading configuration and presenting the result;
/// FSM_COS is responsible for dependency loading, arbitration, convergence, and
/// creation of the RuntimeAssembly.
/// </summary>
public sealed class WebPageCosRuntime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly FsmCos _cos;

    /// <summary>Initializes a WebPage FSM_COS runtime.</summary>
    public WebPageCosRuntime(HttpClient httpClient, WebPageMicroBundleCatalog catalog)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cos = new FsmCos(catalog ?? throw new ArgumentNullException(nameof(catalog)));
    }

    /// <summary>The most recently composed runtime assembly.</summary>
    public RuntimeAssembly? Assembly { get; private set; }

    /// <summary>
    /// Loads a WebApp manifest file and composes exactly the bundles requested by it.
    /// </summary>
    public async Task<RuntimeAssembly> LoadAsync(
        string manifestPath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("A manifest path is required.", nameof(manifestPath));

        var document = await _httpClient.GetFromJsonAsync<WebPageRuntimeManifest>(
            manifestPath,
            JsonOptions,
            cancellationToken);

        if (document is null)
            throw new InvalidOperationException($"Manifest '{manifestPath}' returned no data.");

        if (document.RuntimeId == 0)
            throw new InvalidOperationException("The WebApp runtime manifest must specify a non-zero runtime ID.");

        var requests = document.Bundles.Select(ToBundleRequest).ToArray();
        var runtimeManifest = new RuntimeManifest(document.RuntimeId, requests);

        Assembly = _cos.Execute(runtimeManifest);
        return Assembly;
    }

    private static BundleRequest ToBundleRequest(WebPageBundleRequest request)
    {
        if (request.Id == 0)
            throw new InvalidOperationException("A WebApp MicroBundle request must specify a non-zero ID.");

        var configuration = string.IsNullOrWhiteSpace(request.Configuration)
            ? ReadOnlyMemory<byte>.Empty
            : Convert.FromBase64String(request.Configuration);

        return new BundleRequest(request.Id, configuration);
    }
}
