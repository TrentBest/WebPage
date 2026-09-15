using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using TheSingularityWorkshop;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Infrastructure.Hub;
using TheSingularityWorkshop.SingularityHub;
using TheSingularityWorkshop.Workshop.IO;

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
        builder.Services.AddScoped<WorkshopExperienceService>();
        builder.Services.AddScoped<IWorkshopStorage, BrowserWorkshopStorage>();

        // The WebPage hosts the concrete Hub but owns no Hub mechanics.
        builder.Services.AddSingleton<SingularityHub>();
        builder.Services.AddSingleton<HubRuntime>();

        await builder.Build().RunAsync();
    }
}