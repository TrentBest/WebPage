#if WORKSHOP_USE_RENDERER_SOURCE
using TheSingularityWorkshop.Renderer;
#endif

namespace TheSingularityWorkshop.Pages;

/// <summary>
/// Optional live bridge to the actual Renderer FSM when WebPage is built against sibling repository sources.
/// The default NuGet-mode app does not pretend the unpublished Renderer project is loaded.
/// </summary>
internal sealed class DigitenRendererProbe : IDisposable
{
#if WORKSHOP_USE_RENDERER_SOURCE
    private readonly RendererComputationContext _context = new("webpage-digiten-preview");
    private readonly RendererComputationMachine _machine;

    public DigitenRendererProbe()
    {
        _machine = new RendererComputationMachine(_context);
        _machine.Advance();
    }

    public bool IsConnected => true;
    public string ActiveRepresentation => _machine.CurrentRepresentation.ToString();

    public void Request(string representation)
    {
        if (!Enum.TryParse<RepresentationTarget>(representation, ignoreCase: true, out var target))
            return;

        _context.DesiredRepresentation = target;
        _machine.Advance();
    }

    public void Dispose() => _machine.Dispose();
#else
    public bool IsConnected => false;
    public string ActiveRepresentation => "SOURCE MODE REQUIRED";
    public void Request(string representation) { }
    public void Dispose() { }
#endif
}
