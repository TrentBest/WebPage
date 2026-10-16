using Microsoft.AspNetCore.Components;
using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Components;

public partial class WelcomeExperience : IDisposable
{
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        DestroyMachine();
    }
}

internal sealed class WelcomeLabContext : IStateContext
{
    internal static int NextContextId;
    public string Name { get; set; } = "WelcomeLab";
    public int Context_ID { get; set; }
    public bool IsValid { get; set; } = true;
    public bool StartRequested { get; set; }
    public int UpdateCount { get; set; }
}
