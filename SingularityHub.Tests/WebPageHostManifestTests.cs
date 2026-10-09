using System.Text.Json;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WebPageHostManifestTests
{
    [Fact]
    public void ManifestParserPreservesStartupAndPrimaryExperience()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [
            { "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundles": [], "presentationSeconds": 3 }
          ],
          "hub": [
            { "name": "AnyApp", "kind": "Host", "description": "Desktop host architecture", "deploymentUrl": null, "aboutUrl": "/about-us", "bridgeEndpoint": null, "route": "/anyapp", "microBundles": [] }
          ],
          "capabilities": [
            { "name": "Rendering", "microBundles": [{ "bundleId": 4100, "version": "0.1.0" }] }
          ],
          "running": [
            { "name": "LIVING GUI", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": "1.0.0" }], "presentationSeconds": 0 }
          ],
          "deepDive": {
            "experience": "LIVING GUI",
            "restartFromManifest": true,
            "telemetry": true
          }
        }
        """;

        var manifest = WebPageHostManifestParser.Parse(json);

        Assert.Equal("webpage-host", manifest.ManifestId);
        Assert.Single(manifest.Startup);
        Assert.Equal("Moniker", manifest.Startup[0].Kind);
        Assert.Empty(manifest.Startup[0].MicroBundles);
        Assert.Single(manifest.Hub);
        Assert.Equal("AnyApp", manifest.Hub[0].Name);
        Assert.Equal("Host", manifest.Hub[0].Kind);
        Assert.Equal("/anyapp", manifest.Hub[0].Route);
        Assert.Null(manifest.Hub[0].DeploymentUrl);
        Assert.Empty(manifest.Hub[0].MicroBundles);
        Assert.Equal("/about-us", manifest.Hub[0].AboutUrl);
        Assert.NotNull(manifest.Capabilities);
        Assert.Equal(4100UL, Assert.Single(manifest.Capabilities!).MicroBundles![0].BundleId);
        Assert.Equal("0.1.0", manifest.Capabilities![0].MicroBundles![0].Version);
        Assert.Single(manifest.Running);
        Assert.Equal("Primary", manifest.Running[0].Kind);
        Assert.Equal(2102UL, manifest.Running[0].MicroBundles![0].BundleId);
        Assert.Equal("1.0.0", manifest.Running[0].MicroBundles![0].Version);
        Assert.True(manifest.DeepDive.RestartFromManifest);
        Assert.True(manifest.DeepDive.Telemetry);
    }

    [Fact]
    public void PublicHostAssetsExposeOnlyTheAnyAppHubAndSupportClientRoutes()
    {
        var repositoryRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", ".."));
        var wwwroot = Path.Combine(repositoryRoot, "TheSingularityWorkshop", "wwwroot");
        var manifestJson = File.ReadAllText(Path.Combine(
            wwwroot, "Workshop", "Forge", "StreamingAssets", "Experiences", "webpage-host.manifest.json"));
        var manifest = WebPageHostManifestParser.Parse(manifestJson);

        var hub = Assert.Single(manifest.Hub);
        Assert.Equal("AnyApp", hub.Name);
        Assert.Equal("/anyapp", hub.Route);
        Assert.Empty(hub.MicroBundles);

        Assert.NotNull(manifest.Capabilities);
        var rendering = Assert.Single(manifest.Capabilities!);
        Assert.Equal("Rendering", rendering.Name);
        Assert.Equal(4100UL, rendering.MicroBundles![0].BundleId);
        Assert.Equal("0.1.0", rendering.MicroBundles[0].Version);

        using var config = JsonDocument.Parse(File.ReadAllText(
            Path.Combine(wwwroot, "staticwebapp.config.json")));
        var fallback = config.RootElement.GetProperty("navigationFallback");
        Assert.Equal("/index.html", fallback.GetProperty("rewrite").GetString());
    }

    [Fact]
    public void ManifestParserRejectsUnversionedMicroBundleRoots()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [],
          "hub": [],
          "running": [
            { "name": "LIVING GUI", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": " " }], "presentationSeconds": 0 }
          ],
          "deepDive": { "experience": "LIVING GUI", "restartFromManifest": true, "telemetry": true }
        }
        """;

        Assert.Throws<InvalidOperationException>(() => WebPageHostManifestParser.Parse(json));
    }

    [Fact]
    public void PrimaryExperienceIsRequestedAsTheFsmCosRoot()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [
            { "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundles": [], "presentationSeconds": 3 }
          ],
          "running": [
            { "name": "LIVING GUI", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": "1.0.0" }], "presentationSeconds": 0 }
          ],
          "deepDive": {
            "experience": "LIVING GUI",
            "restartFromManifest": true,
            "telemetry": true
          }
        }
        """;

        var manifest = WebPageHostManifestParser.Parse(json);

        Assert.Equal(2102UL, Assert.Single(manifest.Running[0].MicroBundles!).BundleId);
        Assert.Equal("1.0.0", manifest.Running[0].MicroBundles![0].Version);
        Assert.Equal(2110UL, (ulong)MonikerMicroBundle.BundleId);
    }
}
