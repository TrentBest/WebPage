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
            { "name": "Rendering", "kind": "MicroBundle", "description": "Rendering research", "deploymentUrl": null, "aboutUrl": "/rendering", "bridgeEndpoint": null, "route": "/rendering", "microBundleIds": [4100] }
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
        Assert.Equal("Rendering", manifest.Hub[0].Name);
        Assert.Equal("MicroBundle", manifest.Hub[0].Kind);
        Assert.Equal("/rendering", manifest.Hub[0].Route);
        Assert.Null(manifest.Hub[0].DeploymentUrl);
        Assert.Equal(new[] { 4100UL }, manifest.Hub[0].MicroBundleIds);
        Assert.Equal("/rendering", manifest.Hub[0].AboutUrl);
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
