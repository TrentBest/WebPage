using TheSingularityWorkshop.GrammarAi;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.ProtocolAi;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Composition;

/// <summary>
/// Composes the first host-facing ProtocolAI + GrammarAI exchange surface.
/// Transport, clipboard access, credentials, and provider calls remain host concerns.
/// </summary>
public sealed class AiExchangeCompositionBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle
{
    public const ulong BundleId = 0x1003UL;
    public const ulong ProtocolBundleId = 0x1001_0001UL;
    public const ulong GrammarBundleId = 0x1001_0002UL;

    public AiExchangeCompositionBundle()
    {
        Descriptor = new MicroBundleDescriptor(
            BundleId,
            "0.1.0",
            new[]
            {
                new MicroBundleDependency(ProtocolBundleId),
                new MicroBundleDependency(GrammarBundleId)
            });
    }

    public MicroBundleDescriptor Descriptor { get; }
    public ulong Id => BundleId;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies => new[]
    {
        MicroBundleDependencyRequest.Unconfigured(ProtocolBundleId),
        MicroBundleDependencyRequest.Unconfigured(GrammarBundleId)
    };

    public GuiNode? Composition { get; private set; }
    public ProtocolDefinition? Protocol { get; private set; }
    public GrammarDefinition? Grammar { get; private set; }
    public string? ExchangeText { get; private set; }

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Composition = GuiBuilders.Panel("ai-exchange")
            .Property("composition", "AiExchange")
            .Child(GuiBuilders.Text("ai-title", "AI EXCHANGE"))
            .Child(GuiBuilders.Text("ai-description", "Protocol identity + grammar structure, ready for clipboard or provider transport."))
            .Child(GuiBuilders.TextBox("ai-protocol", null)
                .Property("label", "Protocol")
                .Property("readonly", "true")
                .Property("multiline", "true"))
            .Child(GuiBuilders.TextBox("ai-grammar", null)
                .Property("label", "Grammar")
                .Property("readonly", "true")
                .Property("multiline", "true"))
            .Child(GuiBuilders.TextBox("ai-input", null)
                .Property("label", "LLM response")
                .Property("multiline", "true"))
            .Child(GuiBuilders.Button("ai-extract", "Extract to Clipboard")
                .Property("command", "ai.extract"))
            .Child(GuiBuilders.Button("ai-submit", "Validate / Re-enter")
                .Property("command", "ai.submit"))
            .Build();
    }

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);

        var protocolBundle = context.Bundles
            .OfType<ProtocolAiCompositionBundle>()
            .FirstOrDefault();

        var grammarBundle = context.Bundles
            .OfType<GrammarAiCompositionBundle>()
            .FirstOrDefault();

        if (protocolBundle is null || grammarBundle is null)
            return false;

        var changed = !ReferenceEquals(Protocol, protocolBundle.Protocol) ||
                      !ReferenceEquals(Grammar, grammarBundle.Grammar);

        Protocol = protocolBundle.Protocol;
        Grammar = grammarBundle.Grammar;

        if (Protocol is null || Grammar is null)
            return changed;

        ExchangeText = string.Join(
            Environment.NewLine + Environment.NewLine,
            "PROTOCOL",
            Protocol.Describe(),
            "GRAMMAR",
            Grammar.Describe());

        Composition = GuiBuilders.Panel("ai-exchange")
            .Property("composition", "AiExchange")
            .Child(GuiBuilders.Text("ai-title", "AI EXCHANGE"))
            .Child(GuiBuilders.Text("ai-description", "Copy the deterministic semantic exchange to an LLM, then paste the response back into the host."))
            .Child(GuiBuilders.TextBox("ai-protocol", Protocol.Describe())
                .Property("label", "Protocol")
                .Property("readonly", "true")
                .Property("multiline", "true"))
            .Child(GuiBuilders.TextBox("ai-grammar", Grammar.Describe())
                .Property("label", "Grammar")
                .Property("readonly", "true")
                .Property("multiline", "true"))
            .Child(GuiBuilders.TextBox("ai-input", null)
                .Property("label", "LLM response")
                .Property("multiline", "true"))
            .Child(GuiBuilders.Button("ai-extract", "Extract to Clipboard")
                .Property("command", "ai.extract"))
            .Child(GuiBuilders.Button("ai-submit", "Validate / Re-enter")
                .Property("command", "ai.submit"))
            .Build();

        return changed;
    }
}

/// <summary>ProtocolAI vocabulary contributed to the composed runtime.</summary>
public sealed class ProtocolAiCompositionBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle
{
    public const ulong BundleId = AiExchangeCompositionBundle.ProtocolBundleId;

    public ProtocolAiCompositionBundle()
    {
        Descriptor = new MicroBundleDescriptor(BundleId, "0.1.0");
    }

    public MicroBundleDescriptor Descriptor { get; }
    public ulong Id => BundleId;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies => Array.Empty<MicroBundleDependencyRequest>();
    public ProtocolDefinition Protocol { get; } = new ProtocolBuilder(0x2001UL, "WorkshopAI")
        .Define(0x2101UL, "Extract", "extract")
        .Define(0x2102UL, "Submit", "submit")
        .Define(0x2103UL, "Protocol", "protocol")
        .Define(0x2104UL, "Grammar", "grammar")
        .Build();

    public void Load(IMicroBundleLoadContext context) => ArgumentNullException.ThrowIfNull(context);

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}

/// <summary>GrammarAI structure contributed to the composed runtime.</summary>
public sealed class GrammarAiCompositionBundle : TheSingularityWorkshop.MicroBundleDomain.IMicroBundle
{
    public const ulong BundleId = AiExchangeCompositionBundle.GrammarBundleId;

    public GrammarAiCompositionBundle()
    {
        Descriptor = new MicroBundleDescriptor(
            BundleId,
            "0.1.0",
            new[] { new MicroBundleDependency(ProtocolAiCompositionBundle.BundleId) });
    }

    public MicroBundleDescriptor Descriptor { get; }
    public ulong Id => BundleId;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies => new[]
    {
        MicroBundleDependencyRequest.Unconfigured(ProtocolAiCompositionBundle.BundleId)
    };

    public GrammarDefinition Grammar { get; } = new GrammarBuilder(0x3001UL, "WorkshopAIGrammar", 0x3101UL)
        .Rule(
            0x3201UL,
            0x3101UL,
            GrammarSymbol.NonTerminal(0x3102UL))
        .Rule(
            0x3202UL,
            0x3102UL,
            GrammarSymbol.Terminal(new GrammarProtocolReference(0x2001UL, 0x2101UL)))
        .Build();

    public void Load(IMicroBundleLoadContext context) => ArgumentNullException.ThrowIfNull(context);

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
