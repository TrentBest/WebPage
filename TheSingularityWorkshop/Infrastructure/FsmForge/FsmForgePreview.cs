using TheSingularityWorkshop.FSM_API;
using FsmApi = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Infrastructure.FsmForge;

/// <summary>
/// A tiny context used only by the Forge preview. It gives the real FSM API a
/// valid context while keeping the preview independent of any application domain.
/// </summary>
public sealed class FsmForgePreviewContext : IStateContext
{
    public string Name { get; set; } = "ForgePreview";
    public int Context_ID => 1;
    public bool IsValid { get; set; } = true;
    public int UpdateCount { get; set; }
}

/// <summary>
/// Runs a real FSM_API definition inside the Forge so users can observe lifecycle
/// behavior before treating a scaffold or definition as a reusable product.
/// </summary>
public sealed class FsmForgePreview
{
    private static int _definitionSequence;
    private FSMHandle? _handle;
    private FsmForgePreviewContext? _context;
    private readonly List<string> _events = new();

    public string State => _handle?.CurrentState ?? "NOT STARTED";
    public int TickCount => _context?.UpdateCount ?? 0;
    public IReadOnlyList<string> Events => _events;
    public bool IsRunning => _handle?.IsValid == true;

    public void Start()
    {
        Stop();

        var suffix = Interlocked.Increment(ref _definitionSequence);
        var name = $"WorkshopForgePreview_{suffix}";
        _context = new FsmForgePreviewContext();
        _events.Clear();

        FsmApi.Create.CreateFiniteStateMachine(name, processRate: -1, processingGroup: "ForgePreview")
            .State("Idle",
                onEnter: ctx => _events.Add("OnEnter → Idle"),
                onUpdate: ctx => _events.Add("OnUpdate → Idle"),
                onExit: ctx => _events.Add("OnExit → Idle"))
            .State("Forging",
                onEnter: ctx => _events.Add("OnEnter → Forging"),
                onUpdate: ctx =>
                {
                    var preview = (FsmForgePreviewContext)ctx;
                    preview.UpdateCount++;
                    _events.Add($"OnUpdate → Forging ({preview.UpdateCount})");
                },
                onExit: ctx => _events.Add("OnExit → Forging"))
            .State("Finished",
                onEnter: ctx => _events.Add("OnEnter → Finished"),
                onUpdate: ctx => _events.Add("OnUpdate → Finished"),
                onExit: ctx => _events.Add("OnExit → Finished"))
            .WithInitialState("Idle")
            .Transition("Idle", "Forging", ctx => true)
            .Transition("Forging", "Finished", ctx => ((FsmForgePreviewContext)ctx).UpdateCount >= 3)
            .BuildDefinition();

        _handle = FsmApi.Create.CreateInstance(name, _context, "ForgePreview");
    }

    public void Tick()
    {
        if (_handle is null)
            return;

        // FSM_API exposes the public processing-group tick as Interaction.Update().
        // TickAll is an internal implementation detail and is not part of the
        // Interaction API consumed by the WebPage package.
        FsmApi.Interaction.Update("ForgePreview");
    }

    public void Stop()
    {
        if (_context is not null)
            _context.IsValid = false;

        _handle = null;
        _context = null;
    }
}
