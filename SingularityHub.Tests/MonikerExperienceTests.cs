using System.Net;
using System.Net.Http;
using TheSingularityWorkshop.Infrastructure.Hub;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class MonikerExperienceTests
{
    [Fact(DisplayName = "Manifest names Moniker as startup presentation and Living GUI as primary Experience")]
    public void ManifestSeparatesStartupAndPrimaryExperience()
    {
        var manifest = WebPageHostManifestParser.Parse("""
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
        """);

        Assert.Equal("Moniker", manifest.Startup[0].Kind);
        Assert.Equal(3, manifest.Startup[0].PresentationSeconds);
        Assert.Equal("Primary", manifest.Running[0].Kind);
        Assert.Equal(2102UL, manifest.Running[0].MicroBundleIds[0]);
    }

    [Fact(DisplayName = "Primary Experience resolves the canonical Moniker dependency and enters the Hub")]
    public async Task PrimaryExperienceResolvesCanonicalMonikerDependency()
    {
        using var service = CreateService();

        await service.InitializeAsync();

        Assert.Equal("Moniker", service.CurrentState);
        Assert.NotNull(service.MonikerComposition);

        service.RequestEntry();

        Assert.Equal("Hub", service.CurrentState);
        Assert.NotNull(service.PrimaryExperienceComposition);
    }

    [Fact(DisplayName = "The primary Experience hands off to the Hub after startup presentation")]
    public async Task PrimaryExperienceRemainsActiveAfterStartupPresentation()
    {
        using var service = CreateService();

        await service.InitializeAsync();
        service.RequestEntry();

        Assert.Equal("Hub", service.CurrentState);
        Assert.True(service.RuntimeAssembly!.TryGetBundle<MonikerMicroBundle>(
            (ulong)MonikerMicroBundle.BundleId,
            out _));
        Assert.True(service.RuntimeAssembly.TryGetBundle<LivingGuiExperienceMicroBundle>(
            LivingGuiExperienceMicroBundle.BundleId,
            out _));
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

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(manifest)
            });
        }
    }
}