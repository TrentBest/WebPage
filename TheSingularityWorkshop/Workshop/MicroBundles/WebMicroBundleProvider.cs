using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// First web-facing manifestation provider. It deliberately returns semantic
/// effect names rather than manipulating the DOM directly; the host remains
/// responsible for rendering the manifestation.
/// </summary>
public sealed class WebMicroBundleProvider : IMicroBundleProvider
{
    public MicroBundleManifestation Manifest(IStateContext context)
    {
        var bundle = (MicroBundleContext)context;

        var effect = bundle.Phase switch
        {
            "Created" => "Dormant",
            "Manifesting" => "Trace",
            "Active" => "Breathe",
            "Collapsing" => "Collapse",
            "Destroyed" => "Remove",
            _ => "Dormant"
        };

        return new MicroBundleManifestation(bundle.Id, bundle.Name, effect)
        {
            Parameters = new Dictionary<string, int>
            {
                ["Generation"] = bundle.Generation,
                ["ParentId"] = bundle.ParentId,
                ["Elapsed"] = (int)Math.Min(bundle.ElapsedMilliseconds, int.MaxValue)
            }
        };
    }
}
