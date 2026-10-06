using System.Text.Json;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.Configuration;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopManifestTests
{
    [Fact(DisplayName = "The shipped Workshop manifest is valid JSON and declares the same Hub contract")]
    public void ShippedManifestLoads()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "TheSingularityWorkshop", "wwwroot", "runtime", "workshop.manifest.json");

        var manifest = JsonSerializer.Deserialize<WorkshopManifestDocument>(
            File.ReadAllText(path),
            WorkshopManifestJson.Options);

        Assert.NotNull(manifest);
        Assert.Equal(4000UL, manifest!.RuntimeId);
        Assert.Contains(WorkshopManifestDefaults.MonikerExperienceId, manifest.Experiences.Startup);
        Assert.Equal(7, manifest.Hub.Tabs.Length);
        Assert.Equal("Experiences", manifest.Hub.Tabs[1].Label);
    }

    [Fact(DisplayName = "The Workshop default manifest loads the moniker as a startup Experience")]
    public void MonikerIsDeclaredAsStartupExperience()
    {
        var manifest = WorkshopManifestDefaults.Create();

        Assert.Contains(WorkshopManifestDefaults.MonikerExperienceId, manifest.Experiences.Startup);
        Assert.Contains(HubCapabilityIds.Moniker, manifest.Experiences.RequiredCapabilities);
        Assert.Contains(3002UL, manifest.Experiences.Running);
    }

    [Fact(DisplayName = "Hub tabs come from the manifest definition rather than a page-specific menu")]
    public void HubTabsAreManifestDefined()
    {
        var manifest = WorkshopManifestDefaults.Create();
        var hub = manifest.Hub.ToDefinition();

        Assert.Equal("THE WORKSHOP", hub.Title);
        Assert.Equal(
            new[] { "Understand", "Experiences", "Create", "Rendering", "Education", "MadMen", "Support" },
            hub.Tabs.Select(tab => tab.Label).ToArray());
        Assert.Equal(
            new[] { "/about-us", "/explore", "/create", "/rendering", "/education", "/madmen", "/support" },
            hub.Tabs.Select(tab => tab.Route).ToArray());
    }

    [Fact(DisplayName = "Entering the Workshop composes every Experience declared by the manifest phases")]
    public void ManifestExperiencesAreComposedTogether()
    {
        using var service = new WorkshopExperienceService();
        service.Initialize();

        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Gateway"; i++)
            service.Tick();

        service.RequestEntry();

        for (var i = 0; i < 200 && service.FirstContact.CurrentState != "Landing"; i++)
            service.Tick();

        Assert.Equal("LivingGui", service.CurrentState);
        Assert.Equal(
            new[] { WorkshopManifestDefaults.MonikerExperienceId, 3002UL },
            service.LoadedExperiences.Select(experience => experience.Id).ToArray());
        Assert.NotNull(service.MonikerComposition);
        Assert.Contains(
            service.RuntimeAssembly!.Bundles,
            bundle => bundle.Id == TheSingularityWorkshop.Workshop.MicroBundles.MonikerMicroBundle.BundleId);
    }
}
