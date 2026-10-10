using System.Net;
using System.Net.Http;
using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Infrastructure.Hub;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Rendering;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopCompositionTests
{
    [Fact(DisplayName = "Manifest startup composes the primary Experience and its Moniker dependency")]
    public async Task ManifestStartupComposesPrimaryExperienceAndDependency()
    {
        using var experience = CreateService();

        await experience.InitializeAsync();

        Assert.Equal("Moniker", experience.CurrentState);
        Assert.NotNull(experience.RuntimeAssembly);
        Assert.True(experience.RuntimeAssembly!.TryGetBundle<MonikerMicroBundle>(
            (ulong)MonikerMicroBundle.BundleId,
            out var moniker));
        Assert.True(experience.RuntimeAssembly.TryGetBundle<LivingGuiExperienceMicroBundle>(
            LivingGuiExperienceMicroBundle.BundleId,
            out _));
        Assert.NotNull(moniker?.Composition);
        Assert.Same(moniker!.Composition, experience.MonikerComposition);
        Assert.True(experience.CapabilityRuntimeAssemblies.TryGetValue("Rendering", out var renderingAssembly));
        Assert.True(renderingAssembly!.TryGetBundle<RenderingMicroBundle>(
            RenderingMicroBundle.BundleId,
            out var renderingBundle));
        Assert.NotNull(renderingBundle?.Protocol);
        Assert.NotNull(renderingBundle?.Grammar);
    }

    [Fact(DisplayName = "Manifest composition does not advance the PageFSM landing lifecycle")]
    public async Task ManifestCompositionDoesNotAdvanceLandingLifecycle()
    {
        using var experience = CreateService();

        await experience.InitializeAsync();
        Assert.Equal("Moniker", experience.CurrentState);

        experience.RequestEntry();

        Assert.Equal("Moniker", experience.CurrentState);
        Assert.NotNull(experience.RuntimeAssembly);
    }

    [Fact(DisplayName = "FSM_COS composes ProtocolAI and GrammarAI into an extractable exchange")]
    public void ExplicitlyComposesAiExchange()
    {
        var compositionSystem = new FsmCos(new WorkshopCompositionCatalog());

        var runtime = compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 4001UL,
            Bundles: new[]
            {
                new MicroBundleManifestEntry(AiExchangeCompositionBundle.BundleId, "0.1.0")
            }));

        var ai = runtime.TryGetBundle<AiExchangeCompositionBundle>(
            AiExchangeCompositionBundle.BundleId,
            out var aiBundle)
            ? aiBundle
            : null;

        Assert.NotNull(ai);
        Assert.NotNull(ai!.Protocol);
        Assert.NotNull(ai.Grammar);
        Assert.NotNull(ai.ExchangeText);
        Assert.Contains("PROTOCOL", ai.ExchangeText!, StringComparison.Ordinal);
        Assert.Contains("GRAMMAR", ai.ExchangeText!, StringComparison.Ordinal);
        Assert.Contains("Extract to Clipboard", ai.Composition!.Find("ai-extract").Text);
        Assert.Equal("ai.extract", ai.Composition.Find("ai-extract").Properties["command"]);
        Assert.Equal("ai.submit", ai.Composition.Find("ai-submit").Properties["command"]);
    }

    [Fact(DisplayName = "FSM_COS composes the Rendering tab and its AI semantic dependencies")]
    public void RenderingTabComposesSemanticAiBoundary()
    {
        var compositionSystem = new FsmCos(new WorkshopCompositionCatalog());

        var runtime = compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 4100UL,
            Bundles: new[]
            {
                new MicroBundleManifestEntry(RenderingMicroBundle.BundleId, "0.1.0")
            }));

        Assert.True(runtime.TryGetBundle<RenderingMicroBundle>(
            RenderingMicroBundle.BundleId,
            out var rendering));

        Assert.NotNull(rendering);
        Assert.NotNull(rendering!.Protocol);
        Assert.NotNull(rendering.Grammar);
        Assert.Contains("PERCEPTION_BAND", rendering.AiObservationText!, StringComparison.Ordinal);
        Assert.Contains("AI OBSERVATION", rendering.Composition!.Find("rendering-ai-observation").Properties["label"]);
        Assert.Contains(runtime.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.ProtocolBundleId);
        Assert.Contains(runtime.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.GrammarBundleId);
    }

    [Fact(DisplayName = "Explicit FSM_COS AI composition includes its dependency closure")]
    public void ExplicitAiCompositionIncludesClosure()
    {
        var compositionSystem = new FsmCos(new WorkshopCompositionCatalog());

        var runtime = compositionSystem.Execute(new RuntimeManifest(
            RuntimeId: 4002UL,
            Bundles: new[]
            {
                new MicroBundleManifestEntry(AiExchangeCompositionBundle.BundleId, "0.1.0")
            }));

        Assert.Contains(runtime.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.ProtocolBundleId);
        Assert.Contains(runtime.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.GrammarBundleId);
        Assert.Contains(runtime.Bundles, bundle => bundle.Id == AiExchangeCompositionBundle.BundleId);
    }

    private static WorkshopExperienceService CreateService()
    {
        var client = new HttpClient(new ManifestHandler())
        {
            BaseAddress = new Uri("https://webpage.test/")
        };

        return new WorkshopExperienceService(client, new HubRuntime(new TheSingularityWorkshop.SingularityHub.SingularityHub()));
    }

    private sealed class ManifestHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            const string manifest = """
            {
              "manifestId": "webpage-host",
              "version": "1.0.0",
              "startup": [
                { "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundles": [], "presentationSeconds": 3 }
              ],
              "hub": [
                {
                  "name": "AnyApp",
                  "kind": "Host",
                  "description": "Desktop host architecture",
                  "deploymentUrl": null,
                  "aboutUrl": "/about-us",
                  "bridgeEndpoint": null,
                  "route": "/anyapp",
                  "microBundles": []
                }
              ],
              "capabilities": [
                {
                  "name": "Rendering",
                  "microBundles": [{ "bundleId": 4100, "version": "0.1.0" }]
                }
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

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(manifest)
            });
        }
    }
}