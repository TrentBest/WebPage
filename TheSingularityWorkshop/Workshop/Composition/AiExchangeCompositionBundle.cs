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

        Composition = BuildComposition(
            "Copy context to your chosen LLM, then bring its response back.");
    }

    private static GuiNode BuildComposition(string description) =>
        GuiBuilder.Create("panel", "ai-exchange")
            .Property("composition", "AiExchange")
            .Child("text", "ai-title", child => child.Text("PROTOCOLAI TERMINAL"))
            .Child("text", "ai-description", child => child.Text(description))
            .Child("button", "ai-copy-to-clipboard", child => child
                .Text("Copy to Clipboard")
                .Property("command", "ai.copy-to-clipboard"))
            .Child("button", "ai-paste-from-clipboard", child => child
                .Text("Paste from Clipboard")
                .Property("command", "ai.paste-from-clipboard"))
            .Build();

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

        Composition = BuildComposition(
            "Copy context to your chosen LLM, then bring its response back.");

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
