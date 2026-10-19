using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using TheSingularityWorkshop.World;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using TheSingularityWorkshop;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Infrastructure.Hub;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.IO;
using TheSingularityWorkshop.Workshop.Composition;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, StaticWebAppAuthenticationStateProvider>();

        builder.Services.AddScoped<FSMManagerService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<BlazorFSMIntegration>();
        // Optional public, read-only artifact origin. Configure only a base URI here;
        // never place storage account keys or write-capable SAS tokens in browser config.
        var experienceArtifactBaseUriValue = builder.Configuration["ExperienceArtifacts:BaseUri"];
        Uri? experienceArtifactBaseUri = string.IsNullOrWhiteSpace(experienceArtifactBaseUriValue)
            ? null
            : new Uri(experienceArtifactBaseUriValue, UriKind.Absolute);
        var experienceArtifactManifestPath = builder.Configuration["ExperienceArtifacts:ManifestPath"];
        builder.Services.AddScoped(sp => new WorkshopExperienceService(
            sp.GetRequiredService<HttpClient>(),
            sp.GetRequiredService<HubRuntime>(),
            sp.GetRequiredService<FSMManagerService>(),
            sp.GetRequiredService<WorkshopExperienceCatalog>(),
            experienceArtifactBaseUri,
            experienceArtifactManifestPath));
        builder.Services.AddSingleton<WorkshopExperienceCatalog>();
        builder.Services.AddSingleton<WorkshopCompositionCatalog>();
        builder.Services.AddSingleton<WorkshopDeepDiveCatalog>();
        builder.Services.AddScoped<WorkshopWorldClient>();
        builder.Services.AddScoped<IWorkshopStorage, BrowserWorkshopStorage>();
        builder.Services.AddScoped<WorkshopAssetLibrary>();

        // The WebPage hosts the concrete Hub but owns no Hub mechanics.
        // Hub state belongs to one browser session. Scoped lifetimes are required for
        // independent active manifests and prevent one visitor's runtime from leaking to another.
        builder.Services.AddScoped<SingularityHub>();
        builder.Services.AddScoped<HubRuntime>();

        await builder.Build().RunAsync();
    }
}
