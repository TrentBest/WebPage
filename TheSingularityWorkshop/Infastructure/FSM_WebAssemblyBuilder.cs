using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop;
using System;

public class FSM_WebAssemblyBuilder
{
    private readonly BlazorAppFSMContext _context;
    private readonly FSMHandle _fsmHandle;

    private FSM_WebAssemblyBuilder(string[] args)
    {
        _context = new BlazorAppFSMContext(args);

        // Ensure FSM Definition Exists
        if (!FSM_API.Interaction.Exists("BlazorAppLifecycle", "Bootstrap"))
        {
            FSM_API.Create.CreateProcessingGroup("Bootstrap");
            FSM_API.Create.CreateFiniteStateMachine("BlazorAppLifecycle", -1, "Bootstrap")
                .State("Initializing", OnEnterInitializing, OnUpdateInitializing, OnExitInitializing)
                .State("Operating", OnEnterOperating, null, null)
                .State("Shutdown", null, null, null)

                .Transition("Initializing", "Operating", ctx => ((BlazorAppFSMContext)ctx).IsRegistryLoaded)
                .Transition("Operating", "Shutdown", ctx => false)

                .WithInitialState("Initializing")
                .BuildDefinition();
        }

        // Create Instance
        _fsmHandle = FSM_API.Create.CreateInstance("BlazorAppLifecycle", _context, "Bootstrap");

        
    }

    public static FSM_WebAssemblyBuilder CreateWebApp(string[] args)
    {
        return new FSM_WebAssemblyBuilder(args);
    }

    public FSM_WebAssemblyHost Build()
    {
        // UPDATED LOOP: Wait for the 'IsBuilt' flag, not just the state name.
        // This guarantees the OnEnter logic has actually successfully completed.
        int safetyBreaker = 0;
        while (!_context.IsBuilt && safetyBreaker++ < 100)
        {
            FSM_API.Interaction.Update("Bootstrap");
        }

        if (_context.Host == null)
        {
            // Detailed error reporting
            throw new InvalidOperationException(
                $"FSM failed to build Host. \n" +
                $"State: {_fsmHandle.CurrentState} \n" +
                $"IsRegistryLoaded: {_context.IsRegistryLoaded} \n" +
                $"IsBuilt: {_context.IsBuilt} \n" +
                $"SafetyBreaker: {safetyBreaker}");
        }

        // Clean up the Bootstrap FSM
        FSM_API.Interaction.DestroyInstance(_fsmHandle);

        return new FSM_WebAssemblyHost(_context.Host);
    }

    // --- FSM Actions with Error Handling ---

    private static void OnEnterInitializing(IStateContext context)
    {
        try
        {
            FSM_API.Error.OnInternalApiError += (errorMessage, exception) => {
                // Log to console, file, or your game's debug window
                Console.WriteLine($"[FSM Error]: {errorMessage}");

                if (exception != null)
                {
                    Console.WriteLine($"Exception Details: {exception}");
                }
            };
            var ctx = (BlazorAppFSMContext)context;
            ctx.Builder = WebAssemblyHostBuilder.CreateDefault(ctx.Args);

            // Setup Services (DNA)
            ctx.Builder.RootComponents.Add<App>("#app");
            ctx.Builder.RootComponents.Add<HeadOutlet>("head::after");
            ctx.Builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(ctx.Builder.HostEnvironment.BaseAddress) });

            // Important: Register the Integration services
            ctx.Builder.Services.AddSingleton<BlazorFSMIntegration>();
            

            ctx.IsRegistryLoaded = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FSM CRITICAL] Initialization Failed: {ex}");
            throw; // Re-throw to see in browser console
        }
    }

    private static void OnUpdateInitializing(IStateContext context) { /* Wait for transition */ }
    private static void OnExitInitializing(IStateContext context) { }

    private static void OnEnterOperating(IStateContext context)
    {
        try
        {
            var ctx = (BlazorAppFSMContext)context;

            if (ctx.Builder != null)
            {
                // 1. Build the Host
                ctx.Host = ctx.Builder.Build();

                // 2. Mark as built IMMEDIATELY after creation
                ctx.IsBuilt = true;

                // 3. Ignite the Engine
                // We resolve this *after* setting IsBuilt to ensure the Build() loop exits 
                // even if this specific service resolution fails (though it shouldn't).
                var engine = ctx.Host.Services.GetRequiredService<BlazorFSMIntegration>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FSM CRITICAL] Build Failed: {ex}");
            throw;
        }
    }
}

public class FSM_WebAssemblyHost
{
    private readonly WebAssemblyHost _host;
    public FSM_WebAssemblyHost(WebAssemblyHost host) => _host = host;
    public async Task RunAsync() => await _host.RunAsync();
}
