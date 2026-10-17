namespace TheSingularityWorkshop.Pages;

internal sealed record RendererBrowserMark(double X, double Y, double Radius, string Color, string Representation);

internal sealed record RendererBrowserProofMetrics(
    int EntitiesRepresented,
    int InputCommands,
    int OutputCommands,
    int OmittedCommands,
    string SelectedDetail);

/// <summary>
/// Browser-hosted diagnostic projection of the shared Renderer command-selection workload.
/// Source mode consumes the actual Renderer reducer; NuGet mode stays explicitly disconnected.
/// </summary>
internal sealed class RendererBrowserProof
{
    public static IReadOnlyList<string> DetailOptions { get; } =
        new[] { "Semantic", "Aggregate", "Distant", "Near", "Interactive" };

#if WORKSHOP_USE_RENDERER_SOURCE
    private readonly IReadOnlyList<TheSingularityWorkshop.Renderer.RenderDrawCommand> _candidates =
        TheSingularityWorkshop.Renderer.Diagnostics.RendererDiagnosticScene.CreateCommands(1_000);
    private TheSingularityWorkshop.Renderer.RendererRepresentation _detail =
        TheSingularityWorkshop.Renderer.RendererRepresentation.Interactive;

    public bool IsConnected => true;
    public string ModeLabel => "SOURCE MODE // ACTUAL RENDERER REDUCER";
    public IReadOnlyList<RendererBrowserMark> Marks { get; private set; } = Array.Empty<RendererBrowserMark>();
    public RendererBrowserProofMetrics Metrics { get; private set; } = new(0, 0, 0, 0, "Interactive");

    public RendererBrowserProof()
    {
        Refresh();
    }

    public void SetDetail(string detail)
    {
        if (!Enum.TryParse<TheSingularityWorkshop.Renderer.RendererRepresentation>(
                detail, ignoreCase: true, out var parsed) ||
            !Enum.IsDefined(parsed))
            return;

        _detail = parsed;
        Refresh();
    }

    private void Refresh()
    {
        var result = TheSingularityWorkshop.Renderer.RenderCommandReducer.Reduce(_candidates, _detail);
        Marks = result.Commands.Select(command =>
        {
            var (radius, color) = command.Representation switch
            {
                TheSingularityWorkshop.Renderer.RendererRepresentation.Semantic => (1.4, "#5f788c"),
                TheSingularityWorkshop.Renderer.RendererRepresentation.Aggregate => (2.0, "#00a8ff"),
                TheSingularityWorkshop.Renderer.RendererRepresentation.Distant => (2.6, "#00eaff"),
                TheSingularityWorkshop.Renderer.RendererRepresentation.Near => (3.4, "#ff2fff"),
                TheSingularityWorkshop.Renderer.RendererRepresentation.Interactive => (4.4, "#ffd34d"),
                _ => (2.0, "#ffffff")
            };
            return new RendererBrowserMark(
                (command.WorldTransform.M41 + 1d) * 400d,
                (command.WorldTransform.M42 + 1d) * 250d,
                radius,
                color,
                command.Representation.ToString());
        }).ToArray();

        Metrics = new RendererBrowserProofMetrics(
            result.Metrics.EntitiesRepresented,
            result.Metrics.InputCommands,
            result.Metrics.OutputCommands,
            result.Metrics.OmittedCommands,
            _detail.ToString());
    }
#else
    public bool IsConnected => false;
    public string ModeLabel => "SOURCE MODE REQUIRED // PACKAGE MODE DOES NOT LOAD THIS REDUCER";
    public IReadOnlyList<RendererBrowserMark> Marks { get; } = Array.Empty<RendererBrowserMark>();
    public RendererBrowserProofMetrics Metrics { get; } = new(0, 0, 0, 0, "Disconnected");
    public void SetDetail(string detail) { }
#endif
}
