using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using TheSingularityWorkshop.FSM_API;


public class BlazorAppFSMContext : IStateContext
{
    public string Name { get; set; } = "BlazorAppFSMContext";
    public bool IsValid { get; set; } = true;

    // Input
    public string[] Args { get; set; }

    // The Organism Stages
    public WebAssemblyHostBuilder? Builder { get; set; }
    public WebAssemblyHost? Host { get; set; }

    // State Flags
    public bool IsRegistryLoaded { get; set; }
    public bool IsBuilt { get; set; }

    public BlazorAppFSMContext(string[] args)
    {
        Args = args;
    }
}
