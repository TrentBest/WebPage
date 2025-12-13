using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Net.Http;

using TheSingularityWorkshop;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<FSMManagerService>();

        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<BlazorFSMIntegration>();
        await builder.Build().RunAsync();
    }
}


