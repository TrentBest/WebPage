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
            { "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundleIds": [], "presentationSeconds": 3 }
          ],
          "hub": [
            { "name": "AnyApp", "kind": "Host", "description": "Desktop host architecture", "deploymentUrl": null, "aboutUrl": "/about-us", "bridgeEndpoint": null, "route": "/anyapp", "microBundleIds": [] }
          ],
          "running": [
            { "name": "LIVING GUI", "kind": "Primary", "microBundleIds": [2102], "presentationSeconds": 0 }
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
        Assert.Empty(manifest.Startup[0].MicroBundleIds);
        Assert.Single(manifest.Hub);
        Assert.Equal("AnyApp", manifest.Hub[0].Name);
        Assert.Equal("Host", manifest.Hub[0].Kind);
        Assert.Equal("/anyapp", manifest.Hub[0].Route);
        Assert.Null(manifest.Hub[0].DeploymentUrl);
        Assert.Empty(manifest.Hub[0].MicroBundleIds);
        Assert.Equal("/about-us", manifest.Hub[0].AboutUrl);
        Assert.Single(manifest.Running);
        Assert.Equal("Primary", manifest.Running[0].Kind);
        Assert.Equal(2102UL, manifest.Running[0].MicroBundleIds[0]);
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
        Assert.Empty(hub.MicroBundleIds);

        using var config = JsonDocument.Parse(File.ReadAllText(
            Path.Combine(wwwroot, "staticwebapp.config.json")));
        var fallback = config.RootElement.GetProperty("navigationFallback");
        Assert.Equal("/index.html", fallback.GetProperty("rewrite").GetString());
    }

    [Fact]
    public void PrimaryExperienceIsRequestedAsTheFsmCosRoot()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [
            { "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundleIds": [], "presentationSeconds": 3 }
          ],
          "running": [
            { "name": "LIVING GUI", "kind": "Primary", "microBundleIds": [2102], "presentationSeconds": 0 }
          ],
          "deepDive": {
            "experience": "LIVING GUI",
            "restartFromManifest": true,
            "telemetry": true
          }
        }
        """;

        var manifest = WebPageHostManifestParser.Parse(json);

        Assert.Equal(new[] { 2102UL }, manifest.Running[0].MicroBundleIds);
        Assert.Equal(2110UL, (ulong)MonikerMicroBundle.BundleId);
    }
}
