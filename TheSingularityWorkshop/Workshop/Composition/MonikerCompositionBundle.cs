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
    public IReadOnlyList<BundleRequest> Dependencies => Array.Empty<BundleRequest>();
    public GuiNode? Composition { get; private set; }

    public void Load(MicroBundleLoadContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Composition = GuiBuilder.Create("Panel", "moniker-composition")
            .Property("composition", "Moniker")
            .Child("Text", "the", text => text.Text("THE"))
            .Child("Text", "singularity", text => text.Text("SINGULARITY"))
            .Child("Text", "workshop", text => text.Text("WORKSHOP"))
            .Build();
    }

    public bool Arbitrate(ArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        return false;
    }
}
