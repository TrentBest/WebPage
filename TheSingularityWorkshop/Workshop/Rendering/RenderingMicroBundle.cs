using TheSingularityWorkshop.GrammarAi;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.ProtocolAi;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Rendering;

/// <summary>
/// Host-facing Rendering capability. It keeps rendering semantics independent from
/// the browser while declaring an optional semantic AI exchange through the canonical
/// ProtocolAi + GrammarAi composition bundle.
/// </summary>
public sealed class RenderingMicroBundle : IMicroBundle
{
    public const ulong BundleId = 4100UL;

    public RenderingMicroBundle()
    {
        Descriptor = new MicroBundleDescriptor(
            BundleId,
            "0.1.0",
            new[]
            {
                new MicroBundleDependency(AiExchangeCompositionBundle.BundleId)
            });
    }

    public MicroBundleDescriptor Descriptor { get; }
    public ulong Id => BundleId;

    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
    [
        MicroBundleDependencyRequest.Unconfigured(AiExchangeCompositionBundle.BundleId)
    ];

    public RenderingIntent Intent { get; private set; } =
        new("workshop-scene", .5d, RenderingCameraBehavior.Frame);

    public RenderingPerceptionModel Perception { get; } = new();

    public ProtocolDefinition? Protocol { get; private set; }
    public GrammarDefinition? Grammar { get; private set; }
    public string? AiObservationText { get; private set; }
    public GuiNode? Composition { get; private set; }

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Composition = BuildComposition();
    }

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);

        var aiExchange = context.Bundles
            .OfType<AiExchangeCompositionBundle>()
            .FirstOrDefault();

        if (aiExchange is null)
            return false;

        var changed = !ReferenceEquals(Protocol, aiExchange.Protocol) ||
                      !ReferenceEquals(Grammar, aiExchange.Grammar);

        Protocol = aiExchange.Protocol;
        Grammar = aiExchange.Grammar;

        if (Protocol is null || Grammar is null)
            return changed;

        AiObservationText = DescribeObservation(
            "workshop-scene",
            Perception.Resolve(3d),
            new[]
            {
                "The Workshop renderer is active.",
                "A semantic scene is available to the observer.",
                "The observer has not been given privileged knowledge of hidden world state."
            });

        Composition = BuildComposition();
        return changed;
    }

    /// <summary>
    /// Produces the semantic description that can become the AI's observation input.
    /// This is deliberately descriptive rather than a hidden world-state dump.
    /// </summary>
    public string DescribeObservation(
        string targetId,
        RenderingPerceptionBand band,
        IReadOnlyList<string> visibleFacts)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetId);
        ArgumentNullException.ThrowIfNull(band);
        ArgumentNullException.ThrowIfNull(visibleFacts);

        var facts = visibleFacts.Count == 0
            ? "No visible facts were reported."
            : string.Join(" ", visibleFacts);

        return string.Join(
            Environment.NewLine,
            $"TARGET: {targetId}",
            $"PERCEPTION_BAND: {band.Name}",
            $"DETAIL_BUDGET_MS: {band.UpdateInterval.TotalMilliseconds:0}",
            $"OBSERVATION: {facts}");
    }

    private GuiNode BuildComposition()
    {
        var panel = GuiBuilders.Panel("rendering")
            .Property("composition", "Rendering")
            .Child(GuiBuilders.Text("rendering-title", "RENDERING"))
            .Child(GuiBuilders.Text(
                "rendering-description",
                "Semantic world -> camera/context -> perception band -> manifestation."));

        if (!string.IsNullOrWhiteSpace(AiObservationText))
        {
            panel.Child(
                GuiBuilders.TextBox("rendering-ai-observation", AiObservationText)
                    .Property("label", "AI OBSERVATION")
                    .Property("readonly", "true")
                    .Property("multiline", "true"));
        }

        return panel.Build();
    }
}
