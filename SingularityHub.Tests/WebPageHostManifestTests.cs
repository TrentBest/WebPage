using System.Text.Json;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Infrastructure.Hub;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Experiences;
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

    [Theory]
    [InlineData("2.0.0")]
    [InlineData("not-a-version")]
    public void ManifestParserRejectsUnsupportedSchemaVersions(string version)
    {
        var json = $$"""
        {
          "manifestId": "webpage-host",
          "version": "{{version}}",
          "startup": [{ "name": "Moniker", "kind": "Moniker", "microBundles": [], "presentationSeconds": 0 }],
          "hub": [],
          "running": [{ "name": "Primary", "kind": "Primary", "microBundles": [], "presentationSeconds": 0 }],
          "deepDive": { "experience": "Primary", "restartFromManifest": true, "telemetry": false }
        }
        """;

        var exception = Assert.Throws<InvalidOperationException>(() => WebPageHostManifestParser.Parse(json));
        Assert.Contains("Supported major version is 1", exception.Message);
    }

    [Fact]
    public void PublicHostAssetsExposeManifestDrivenWorkshopDestinationsAndClientRoutes()
    {
        var repositoryRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", ".."));
        var wwwroot = Path.Combine(repositoryRoot, "TheSingularityWorkshop", "wwwroot");
        var manifestJson = File.ReadAllText(Path.Combine(
            wwwroot, "Workshop", "Forge", "StreamingAssets", "Experiences", "webpage-host.manifest.json"));
        var manifest = WebPageHostManifestParser.Parse(manifestJson);

        Assert.Equal(2101UL, Assert.Single(manifest.Startup[0].MicroBundles!).BundleId);
        Assert.Equal(new[] { "Welcome", "Singularity Laboratory", "Singularity Architecture", "Movement Lab", "Replay Lab", "Singularity Studios" }, manifest.Hub.Select(item => item.Name));
        Assert.DoesNotContain(manifest.Hub, item => item.Name == "AnyApp");
        Assert.DoesNotContain(manifest.Hub, item => item.Name == "Support");

        var welcome = Assert.Single(manifest.Hub, item => item.Name == "Welcome");
        Assert.Equal("Welcome", welcome.Kind);
        Assert.Equal("/", welcome.Route);
        Assert.Empty(welcome.MicroBundles!);

        var demonstrations = Assert.Single(manifest.Hub, item => item.Name == "Singularity Laboratory");
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

        var replayLab = Assert.Single(manifest.Hub, item => item.Name == "Replay Lab");
        Assert.Equal("ExperienceGallery", replayLab.Kind);
        Assert.Equal("/replay-lab", replayLab.Route);
        Assert.Empty(replayLab.MicroBundles!);

        var studios = Assert.Single(manifest.Hub, item => item.Name == "Singularity Studios");
        Assert.Equal("ExperienceGallery", studios.Kind);
        Assert.Equal("/singularity-studios", studios.Route);
        Assert.Empty(studios.MicroBundles!);

        var soldierField = Assert.Single(manifest.Demonstrations!, item => item.Id == "soldier-field-webgpu");
        Assert.Equal("/experiences/soldier-field/index.html", soldierField.EmbeddedExperienceUrl);

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
    public void CompositionCatalogCreatesIsolatedBundleInstancesForSeparateRuntimeCompositions()
    {
        var catalog = new WorkshopCompositionCatalog();

        Assert.True(catalog.TryResolve(2102, "1.0.0", out var first));
        Assert.True(catalog.TryResolve(2102, "1.0.0", out var second));

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotSame(first, second);

        if (first is IDisposable firstDisposable)
            firstDisposable.Dispose();
        if (second is IDisposable secondDisposable)
            secondDisposable.Dispose();
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
    public void EmbeddedDemonstrationsRejectExternalAndProtocolRelativeUrls()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [{ "name": "MONIKER", "kind": "Moniker", "microBundles": [], "presentationSeconds": 3 }],
          "running": [{ "name": "LIVING GUI", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": "1.0.0" }], "presentationSeconds": 0 }],
          "demonstrations": [{
            "id": "unsafe-embed",
            "title": "Unsafe embed",
            "summary": "Test",
            "whyItMatters": "Test",
            "status": "Test",
            "whatItProves": "Test",
            "whatItDoesNotProve": "Test",
            "launchMode": "Test",
            "runInstructions": "Test",
            "sourceUrl": "https://example.com/source",
            "requirements": "Test",
            "embeddedExperienceUrl": "//example.com/remote.html"
          }],
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
    [Fact]
    public async Task SwitchingManifestActivatesNewCompositionAndInvalidManifestKeepsActiveRuntime()
    {
        const string manifestA = """
        {
          "manifestId": "showcase-a",
          "version": "1.0.0",
          "startup": [{ "name": "MONIKER A", "kind": "Moniker", "microBundles": [{ "bundleId": 2101, "version": "1.0.0" }], "presentationSeconds": 3 }],
          "hub": [{ "name": "Welcome A", "kind": "Welcome", "description": "A", "deploymentUrl": null, "aboutUrl": "/about-us", "bridgeEndpoint": null, "route": "/a", "microBundles": [] }],
          "running": [{ "name": "LIVING GUI A", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": "1.0.0" }], "presentationSeconds": 0 }],
          "deepDive": { "experience": "LIVING GUI A", "restartFromManifest": true, "telemetry": true }
        }
        """;
        const string manifestB = """
        {
          "manifestId": "showcase-b",
          "version": "1.0.0",
          "startup": [{ "name": "MONIKER B", "kind": "Moniker", "microBundles": [{ "bundleId": 2101, "version": "1.0.0" }], "presentationSeconds": 3 }],
          "hub": [{ "name": "Welcome B", "kind": "Welcome", "description": "B", "deploymentUrl": null, "aboutUrl": "/about-us", "bridgeEndpoint": null, "route": "/b", "microBundles": [] }],
          "running": [{ "name": "LIVING GUI B", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": "1.0.0" }], "presentationSeconds": 0 }],
          "deepDive": { "experience": "LIVING GUI B", "restartFromManifest": true, "telemetry": true }
        }
        """;

        using var client = new HttpClient(new ManifestHandler(new Dictionary<string, string>
        {
            ["manifest-a.json"] = manifestA,
            ["manifest-b.json"] = manifestB,
            ["invalid.json"] = "{ invalid json"
        }))
        {
            BaseAddress = new Uri("https://localhost/")
        };
        var hubKernel = new HubKernel();
        var hub = new HubRuntime(hubKernel);
        using var executionHost = new FSMManagerService(hubKernel);
        var originalPage = executionHost.Page;
        using var service = new WorkshopExperienceService(
            client, hub, executionHost, new WorkshopExperienceCatalog());

        await service.SwitchManifestAsync("manifest-a.json");
        var firstAssembly = service.RuntimeAssembly;
        var firstExecutionRuntime = executionHost.ExecutionRuntime;
        var firstHub = hub.Hub;
        Assert.Equal(3002UL, service.SelectedExperience!.Id);
        Assert.True(firstExecutionRuntime.IsManifestBound);
        Assert.Same(originalPage, firstExecutionRuntime.Page);
        Assert.Same(originalPage.Context, firstExecutionRuntime.Context);
        Assert.Equal(originalPage.CurrentState, firstExecutionRuntime.CurrentState);
        Assert.Equal(firstAssembly!.RuntimeId, firstExecutionRuntime.RuntimeAssembly!.RuntimeId);
        Assert.Equal("showcase-a", service.Manifest!.ManifestId);
        Assert.Equal("/a", Assert.Single(service.Manifest.Hub).Route);

        await service.SwitchManifestAsync("manifest-b.json");
        Assert.Equal("showcase-b", service.Manifest!.ManifestId);
        Assert.Equal("/b", Assert.Single(service.Manifest.Hub).Route);
        Assert.NotSame(firstAssembly, service.RuntimeAssembly);
        Assert.Same(firstHub, hub.Hub);
        Assert.Equal("manifest-b.json", service.ActiveManifestPath);
        Assert.NotSame(firstExecutionRuntime, executionHost.ExecutionRuntime);
        Assert.True(executionHost.ExecutionRuntime.IsManifestBound);
        Assert.Equal(service.RuntimeAssembly!.RuntimeId, executionHost.ExecutionRuntime.RuntimeAssembly!.RuntimeId);
        Assert.Same(originalPage, executionHost.ExecutionRuntime.Page);
        Assert.Equal("LIVING GUI", executionHost.ExecutionRuntime.Experience!.Name);

        var beforeReset = service.RuntimeAssembly;
        await service.RestartFromManifestAsync(deepDive: true);
        Assert.Equal("showcase-b", service.Manifest!.ManifestId);
        Assert.NotSame(beforeReset, service.RuntimeAssembly);
        Assert.True(service.IsDeepDive);

        var activeAssembly = service.RuntimeAssembly;
        var activeExecutionRuntime = executionHost.ExecutionRuntime;
        await Assert.ThrowsAnyAsync<Exception>(() => service.SwitchManifestAsync("invalid.json"));
        Assert.Equal("showcase-b", service.Manifest!.ManifestId);
        Assert.Same(activeAssembly, service.RuntimeAssembly);
        Assert.Same(activeExecutionRuntime, executionHost.ExecutionRuntime);
        Assert.Equal("manifest-b.json", service.ActiveManifestPath);
        Assert.NotNull(service.LastManifestError);
    }

    [Theory]
    [InlineData("text/html", false)]
    [InlineData("application/vnd.example+json", true)]
    public async Task ManifestSwitchValidatesJsonContentType(string contentType, bool accepted)
    {
        const string manifestJson = """
        {
          "manifestId": "content-type-test",
          "version": "1.0.0",
          "startup": [{ "name": "MONIKER", "kind": "Moniker", "microBundles": [], "presentationSeconds": 0 }],
          "hub": [],
          "running": [{ "name": "LIVING GUI", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": "1.0.0" }], "presentationSeconds": 0 }],
          "deepDive": { "experience": "LIVING GUI", "restartFromManifest": true, "telemetry": false }
        }
        """;

        using var client = new HttpClient(new ManifestHandler(
            new Dictionary<string, string> { ["manifest.json"] = manifestJson },
            contentType))
        {
            BaseAddress = new Uri("https://localhost/")
        };
        var hubKernel = new HubKernel();
        using var executionHost = new FSMManagerService(hubKernel);
        using var service = new WorkshopExperienceService(
            client, new HubRuntime(hubKernel), executionHost, new WorkshopExperienceCatalog());

        if (accepted)
        {
            await service.SwitchManifestAsync("manifest.json");
            Assert.Equal("content-type-test", service.Manifest!.ManifestId);
        }
        else
        {
            await Assert.ThrowsAsync<InvalidDataException>(
                () => service.SwitchManifestAsync("manifest.json"));
            Assert.Null(service.Manifest);
            Assert.Contains("must use a JSON content type", service.LastManifestError);
        }
    }

    [Fact]
    public async Task ManifestSwitchRejectsExternalAndTraversalPathsBeforeFetching()
    {
        using var client = new HttpClient(new ManifestHandler(new Dictionary<string, string>()))
        {
            BaseAddress = new Uri("https://localhost/")
        };
        var hubKernel = new HubKernel();
        using var executionHost = new FSMManagerService(hubKernel);
        using var service = new WorkshopExperienceService(
            client, new HubRuntime(hubKernel), executionHost, new WorkshopExperienceCatalog());

        await Assert.ThrowsAsync<ArgumentException>(() => service.SwitchManifestAsync("https://example.com/manifest.json"));
        await Assert.ThrowsAsync<ArgumentException>(() => service.SwitchManifestAsync("../manifest.json"));
    }

    [Fact]
    public void ManifestParserRejectsDuplicateDemonstrationIdsIgnoringCase()
    {
        const string json = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [{ "name": "MONIKER", "kind": "Moniker", "microBundles": [], "presentationSeconds": 3 }],
          "running": [{ "name": "LIVING GUI", "kind": "Primary", "microBundles": [{ "bundleId": 2102, "version": "1.0.0" }], "presentationSeconds": 0 }],
          "demonstrations": [
            {
              "id": "same-proof",
              "title": "First proof",
              "summary": "Test",
              "whyItMatters": "Test",
              "status": "Test",
              "whatItProves": "Test",
              "whatItDoesNotProve": "Test",
              "launchMode": "Test",
              "runInstructions": "Test",
              "sourceUrl": "https://example.com/first",
              "requirements": "Test"
            },
            {
              "id": "SAME-PROOF",
              "title": "Second proof",
              "summary": "Test",
              "whyItMatters": "Test",
              "status": "Test",
              "whatItProves": "Test",
              "whatItDoesNotProve": "Test",
              "launchMode": "Test",
              "runInstructions": "Test",
              "sourceUrl": "https://example.com/second",
              "requirements": "Test"
            }
          ],
          "deepDive": { "experience": "LIVING GUI", "restartFromManifest": true, "telemetry": true }
        }
        """;

        var exception = Assert.Throws<InvalidOperationException>(
            () => WebPageHostManifestParser.Parse(json));

        Assert.Contains("SAME-PROOF", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ManifestBoundExecutionAdapterRejectsRuntimeMissingRequiredExperienceBundle()
    {
        using var page = new PageFSM();
        var assembly = new FsmCos(new WorkshopCompositionCatalog()).Execute(
            new RuntimeManifest(990, new[] { new MicroBundleManifestEntry(2101, "1.0.0") }));

        try
        {
            Assert.Throws<InvalidOperationException>(() =>
                new PageFsmExecutionRuntime(page, new LivingGuiExperience(), assembly));
        }
        finally
        {
            foreach (var disposable in assembly.Bundles.OfType<IDisposable>())
                disposable.Dispose();
        }
    }

    [Fact]
    public void ManifestBoundExecutionAdapterTicksAndObservesTheSamePageRuntime()
    {
        using var page = new PageFSM();
        var assembly = new FsmCos(new WorkshopCompositionCatalog()).Execute(
            new RuntimeManifest(991, new[] { new MicroBundleManifestEntry(2102, "1.0.0") }));
        var adapter = new PageFsmExecutionRuntime(page, new LivingGuiExperience(), assembly);

        try
        {
            Assert.True(adapter.IsManifestBound);
            Assert.Same(page, adapter.Page);
            Assert.Same(page.Context, adapter.Context);
            var ticksBeforeRequest = page.Context.TotalTicks;

            adapter.RequestEnter();
            var ticksAfterRequest = page.Context.TotalTicks;
            adapter.Tick();
            var observation = adapter.Observe();

            Assert.True(page.Context.EnterRequested);
            Assert.True(ticksAfterRequest >= ticksBeforeRequest);
            Assert.Equal(ticksAfterRequest + 1, observation.TotalTicks);
            Assert.Equal(page.CurrentState, observation.PageState);
            Assert.Equal(assembly.RuntimeId, adapter.RuntimeAssembly!.RuntimeId);
        }
        finally
        {
            foreach (var disposable in assembly.Bundles.OfType<IDisposable>())
                disposable.Dispose();
        }
    }

    private sealed class ManifestHandler(IReadOnlyDictionary<string, string> manifests, string contentType = "application/json") : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath.TrimStart('/') ?? string.Empty;
            if (!manifests.TryGetValue(path, out var content))
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));

            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, contentType)
            });
        }
    }

}
