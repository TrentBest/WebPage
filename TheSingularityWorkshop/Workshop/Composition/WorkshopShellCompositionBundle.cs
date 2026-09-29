using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Composition;

/// <summary>
/// Semantic description of the WebForge shell itself.
/// The Blazor host may render this however it chooses; FSM_COS owns the fact
/// that the shell is a composed runtime rather than a hard-coded page tree.
/// </summary>
public sealed class WorkshopShellCompositionBundle : IMicroBundle
{
    public const ulong BundleId = 0x1000UL;

    public WorkshopShellCompositionBundle()
    {
        Descriptor = new MicroBundleDescriptor(BundleId, "0.1.0");
    }

    public MicroBundleDescriptor Descriptor { get; }
    public ulong Id => BundleId;
    public IReadOnlyList<BundleRequest> Dependencies => Array.Empty<BundleRequest>();
    public GuiNode? Composition { get; private set; }

    public void Load(MicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Composition = GuiBuilders.Panel("webforge-shell")
            .Property("composition", "WebForge")
            .Child(GuiBuilders.Text("webforge-title", "THE SINGULARITY WORKSHOP"))
            .Child(GuiBuilders.Text("webforge-home", "WORKSHOP"))
            .Child(GuiBuilders.Text("webforge-explore", "EXPLORE"))
            .Child(GuiBuilders.Text("webforge-create", "CREATE"))
            .Child(GuiBuilders.Text("webforge-showcase", "SHOWCASE"))
            .Build();
    }

    public bool Arbitrate(ArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
