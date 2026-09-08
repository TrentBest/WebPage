using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Shared state bag for a micro-bundle. It deliberately implements
/// <see cref="IStateContext"/> so FSM_API remains the lifecycle authority.
/// </summary>
public sealed class MicroBundleContext : IStateContext
{
    public MicroBundleContext(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; set; }
    public bool IsValid { get; set; } = true;

    /// <summary>Current lifecycle state as exposed to the manifestation provider.</summary>
    public string Phase { get; set; } = "Created";

    /// <summary>Monotonic lifecycle time supplied by the scheduler.</summary>
    public long ElapsedMilliseconds { get; set; }

    /// <summary>Optional parent bundle identity used for generative effects.</summary>
    public int ParentId { get; set; } = -1;

    /// <summary>Generation/lineage value used by reproductive manifestations.</summary>
    public int Generation { get; set; }
}
