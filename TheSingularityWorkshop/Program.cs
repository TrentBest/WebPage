using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization; // Required for Auth
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

using TheSingularityWorkshop;
using TheSingularityWorkshop.Services;
// Alias locally just in case
using FSM_API = TheSingularityWorkshop.FSM_API.FSM_API;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // 1. HTTP Client
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // 2. AUTHENTICATION (The Missing Piece)
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, StaticWebAppAuthenticationStateProvider>();

        // 3. Domain Services
        builder.Services.AddScoped<FSMManagerService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<BlazorFSMIntegration>();

        await builder.Build().RunAsync();
    }
}