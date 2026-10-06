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
            { "name": "AnyApp", "kind": "DesktopHost", "description": "Desktop proving ground", "deploymentUrl": "https://github.com/TrentBest/AnyApp/releases", "aboutUrl": "/about-us", "bridgeEndpoint": "http://127.0.0.1:48156/" }
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
        Assert.Equal("/about-us", manifest.Hub[0].AboutUrl);
        Assert.Single(manifest.Running);
        Assert.Equal("Primary", manifest.Running[0].Kind);
        Assert.Equal(2102UL, manifest.Running[0].MicroBundleIds[0]);
        Assert.True(manifest.DeepDive.RestartFromManifest);
        Assert.True(manifest.DeepDive.Telemetry);
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
