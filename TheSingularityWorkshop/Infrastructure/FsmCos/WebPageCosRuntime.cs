using System.Net.Http.Json;
using System.Text.Json;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using CosEngine = TheSingularityWorkshop.FSM_COS.FsmCos;

namespace TheSingularityWorkshop.Infrastructure.FsmCos;

public sealed class WebPageCosRuntime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly CosEngine _cos;

    public WebPageCosRuntime(HttpClient httpClient, WebPageMicroBundleCatalog catalog)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cos = new CosEngine(catalog ?? throw new ArgumentNullException(nameof(catalog)));
    }

    public RuntimeAssembly? Assembly { get; private set; }

    public async Task<RuntimeAssembly> LoadAsync(string manifestPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("A manifest path is required.", nameof(manifestPath));

        var document = await _httpClient.GetFromJsonAsync<WebPageRuntimeManifest>(
            manifestPath, JsonOptions, cancellationToken);

        if (document is null)
            throw new InvalidOperationException($"Manifest '{manifestPath}' returned no data.");
        if (document.RuntimeId == 0)
            throw new InvalidOperationException("The WebApp runtime manifest must specify a non-zero runtime ID.");

        var requests = document.Bundles.Select(ToBundleRequest).ToArray();
        Assembly = _cos.Execute(new RuntimeManifest(document.RuntimeId, requests));
        return Assembly;
    }

    private static MicroBundleDependencyRequest ToBundleRequest(WebPageBundleRequest request)
    {
        if (request.Id == 0)
            throw new InvalidOperationException("A WebApp MicroBundle request must specify a non-zero ID.");

        var configuration = string.IsNullOrWhiteSpace(request.Configuration)
            ? ReadOnlyMemory<byte>.Empty
            : Convert.FromBase64String(request.Configuration);

        return new MicroBundleDependencyRequest(request.Id, configuration);
    }
}
