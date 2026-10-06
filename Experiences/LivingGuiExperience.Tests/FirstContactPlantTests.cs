using TheSingularityWorkshop.Services;
using Xunit;
using TheSingularityWorkshop.Workshop.Plant;
using TheSingularityWorkshop.Workshop.Sound;

namespace LivingGuiExperience.Tests;

public sealed class FirstContactPlantTests
{
    [Fact]
    public void Plant_Grows_Blooms_And_Opens_The_Hub()
    {
        using var plant = new PlantGrowthMicroBundle();
        plant.Start();

        for (var i = 0; i < 400 && !plant.IsSettled; i++)
            plant.Update();

        Assert.True(plant.IsSettled);
        Assert.Equal(6, plant.Vines.Count);
        Assert.Equal(6, plant.Panels.Count);
        Assert.All(plant.Panels, panel => Assert.Equal(1, panel.OpenProgress));
    }

    [Fact]
    public async Task ManifestStartup_ComposesMonikerDependency_ThenEntersHub()
    {
        const string manifestJson = """
        {
          "manifestId": "webpage-host",
          "version": "1.0.0",
          "startup": [{ "name": "THE SINGULARITY WORKSHOP", "kind": "Moniker", "microBundleIds": [], "presentationSeconds": 3 }],
          "running": [{ "name": "LIVING GUI", "kind": "Primary", "microBundleIds": [2102], "presentationSeconds": 0 }],
          "deepDive": { "experience": "LIVING GUI", "restartFromManifest": true, "telemetry": true }
        }
        """;

        using var httpClient = new HttpClient(new ManifestHandler(manifestJson)) { BaseAddress = new Uri("https://test.local/") };
        using var experience = new WorkshopExperienceService(
            httpClient,
            new TheSingularityWorkshop.Infrastructure.Hub.HubRuntime(
                new TheSingularityWorkshop.SingularityHub.SingularityHub()));

        await experience.InitializeAsync();

        Assert.Equal("Moniker", experience.CurrentState);
        Assert.Equal("LIVING GUI", experience.PrimaryManifestExperience!.Name);
        Assert.NotNull(experience.RuntimeAssembly);
        Assert.NotNull(experience.MonikerComposition);
        Assert.NotNull(experience.PrimaryExperienceComposition);

        experience.RequestEntry();

        Assert.Equal("Hub", experience.CurrentState);
    }

    private sealed class ManifestHandler : HttpMessageHandler
    {
        private readonly string _manifestJson;

        public ManifestHandler(string manifestJson) => _manifestJson = manifestJson;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(_manifestJson, System.Text.Encoding.UTF8, "application/json")
            });
        }
    }

    [Fact]
    public void FirstContact_Uses_An_Authored_Cavern_Environment()
    {
        var scene = SfxScene.FirstContactCavern();

        Assert.Equal("Large Open Cave", scene.Environment.Name);
        Assert.True(scene.Environment.ReverbSeconds > 0);
        Assert.Equal(4, scene.Emitters.Count);
    }
}
