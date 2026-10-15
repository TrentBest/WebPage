using System.Text.Json;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.Composition;
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
        Assert.Empty(manifest.Startup[0].MicroBundles!);
        Assert.Single(manifest.Hub);
        Assert.Equal("AnyApp", manifest.Hub[0].Name);
        Assert.Equal("Host", manifest.Hub[0].Kind);
        Assert.Equal("/anyapp", manifest.Hub[0].Route);
        Assert.Null(manifest.Hub[0].DeploymentUrl);
        Assert.Empty(manifest.Hub[0].MicroBundles!);
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
    public void PublicHostAssetsExposeOnlyTheThreeManifestDrivenWorkshopDestinationsAndClientRoutes()
    {
        var repositoryRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", ".."));
        var wwwroot = Path.Combine(repositoryRoot, "TheSingularityWorkshop", "wwwroot");
        var manifestJson = File.ReadAllText(Path.Combine(
            wwwroot, "Workshop", "Forge", "StreamingAssets", "Experiences", "webpage-host.manifest.json"));
        var manifest = WebPageHostManifestParser.Parse(manifestJson);

        Assert.Equal(new[] { "Welcome", "Demonstrations", "Singularity Architecture", "Movement Lab" }, manifest.Hub.Select(item => item.Name));
        Assert.DoesNotContain(manifest.Hub, item => item.Name == "AnyApp");
        Assert.DoesNotContain(manifest.Hub, item => item.Name == "Support");

        var welcome = Assert.Single(manifest.Hub, item => item.Name == "Welcome");
        Assert.Equal("Welcome", welcome.Kind);
        Assert.Equal("/", welcome.Route);
        Assert.Empty(welcome.MicroBundles!);

        var demonstrations = Assert.Single(manifest.Hub, item => item.Name == "Demonstrations");
        Assert.Equal("ExperienceGallery", demonstrations.Kind);
        Assert.Equal("/?tab=demonstrations", demonstrations.Route);
        Assert.Empty(demonstrations.MicroBundles!);

        var architecture = Assert.Single(manifest.Hub, item => item.Name == "Singularity Architecture");
        Assert.Equal("CreationStudio", architecture.Kind);
        Assert.Equal("/architecture", architecture.Route);
        Assert.Empty(architecture.MicroBundles!);

        var movementLab = Assert.Single(manifest.Hub, item => item.Name == "Movement Lab");
        Assert.Equal("ExperienceGallery", movementLab.Kind);
        Assert.Equal("/movement-lab", movementLab.Route);
        Assert.Empty(movementLab.MicroBundles!);

        Assert.NotNull(manifest.Capabilities);
        Assert.Equal(new[] { "Rendering", "ProtocolAi Terminal" }, manifest.Capabilities!.Select(capability => capability.Name));
        var rendering = Assert.Single(manifest.Capabilities!, capability => capability.Name == "Rendering");
        Assert.Equal(4100UL, rendering.MicroBundles![0].BundleId);
        Assert.Equal("0.1.0", rendering.MicroBundles[0].Version);
        var terminal = Assert.Single(manifest.Capabilities!, capability => capability.Name == "ProtocolAi Terminal");
        Assert.Equal(AiExchangeCompositionBundle.BundleId, terminal.MicroBundles![0].BundleId);
        Assert.Equal("0.1.0", terminal.MicroBundles[0].Version);

        using var config = JsonDocument.Parse(File.ReadAllText(
            Path.Combine(wwwroot, "staticwebapp.config.json")));
        var fallback = config.RootElement.GetProperty("navigationFallback");
        Assert.Equal("/index.html", fallback.GetProperty("rewrite").GetString());
    }

    [Fact]
    public void FsmCosComposesVersionedPrimaryRootAndCanonicalMonikerDependency()
    {
        var catalog = new WorkshopCompositionCatalog();
        var assembly = new FsmCos(catalog).Execute(
            new RuntimeManifest(
                900,
                new[] { new MicroBundleManifestEntry(2102, "1.0.0") }));

        Assert.Equal(new ulong[] { 2110, 2102 }, assembly.Bundles.Select(bundle => bundle.Id));
        Assert.True(assembly.TryGetBundle<LivingGuiExperienceMicroBundle>(2102, out _));
        Assert.True(assembly.TryGetBundle<MonikerMicroBundle>(2110, out var moniker));
        Assert.False(catalog.TryResolve(2102, "9.9.9", out _));
        Assert.Throws<InvalidOperationException>(() => new FsmCos(catalog).Execute(
            new RuntimeManifest(
                901,
                new[] { new MicroBundleManifestEntry(2102, "9.9.9") })));

        moniker!.Dispose();
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
