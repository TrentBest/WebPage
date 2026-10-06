using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WebPageHostManifestTests
{
    [Fact]
    public void ManifestParserPreservesStartupOrderAndPreloadIntent()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [
            { "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundleIds": [2110], "presentationSeconds": 3 },
            { "name": "WORKSHOP HUB", "kind": "Hub", "microBundleIds": [], "presentationSeconds": 0 }
          ],
          "preload": [
            { "name": "WORKSHOP HUB", "kind": "Hub", "microBundleIds": [], "presentationSeconds": 0 }
          ],
          "deepDive": {
            "experience": "WORKSHOP HUB",
            "restartFromManifest": true,
            "telemetry": true
          }
        }
        """;

        var manifest = WebPageHostManifestParser.Parse(json);

        Assert.Equal("webpage-host", manifest.ManifestId);
        Assert.Equal("Moniker", manifest.Startup[0].Kind);
        Assert.Equal(2110UL, manifest.Startup[0].MicroBundleIds[0]);
        Assert.Equal("Hub", manifest.Startup[1].Kind);
        Assert.True(manifest.DeepDive.RestartFromManifest);
        Assert.True(manifest.DeepDive.Telemetry);
    }

    [Fact]
    public void ManifestDefinesHubAsTheWebPageExperienceAfterMoniker()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [
            { "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundleIds": [2110], "presentationSeconds": 3 },
            { "name": "WORKSHOP HUB", "kind": "Hub", "microBundleIds": [], "presentationSeconds": 0 }
          ],
          "preload": [
            { "name": "WORKSHOP HUB", "kind": "Hub", "microBundleIds": [], "presentationSeconds": 0 }
          ],
          "deepDive": {
            "experience": "WORKSHOP HUB",
            "restartFromManifest": true,
            "telemetry": true
          }
        }
        """;

        var manifest = WebPageHostManifestParser.Parse(json);

        Assert.Equal("Moniker", manifest.Startup[0].Kind);
        Assert.Equal("Hub", manifest.Startup[1].Kind);
        Assert.Equal("WORKSHOP HUB", manifest.DeepDive.Experience);
        Assert.Equal(3, manifest.Startup[0].PresentationSeconds);
    }
}
