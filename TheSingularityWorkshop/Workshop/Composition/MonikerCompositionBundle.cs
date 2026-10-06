using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Composition;

public sealed class MonikerCompositionBundle : IMicroBundle
{
    public const ulong BundleId = 0x1001UL;

    public MonikerCompositionBundle()
    {
        Descriptor = new MicroBundleDescriptor(BundleId, "0.1.0");
    }

    public MicroBundleDescriptor Descriptor { get; }
    public ulong Id => BundleId;
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies => Array.Empty<MicroBundleDependencyRequest>();
    public GuiNode? Composition { get; private set; }

    public void Load(IMicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Composition = GuiBuilder.Create("Panel", "moniker-composition")
            .Property("composition", "Moniker")
            .Child("Text", "the", text => text.Text("THE"))
            .Child("Text", "singularity", text => text.Text("SINGULARITY"))
            .Child("Text", "workshop", text => text.Text("WORKSHOP"))
            .Build();
    }

    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
