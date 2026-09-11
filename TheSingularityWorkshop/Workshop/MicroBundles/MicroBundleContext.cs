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

    /// <summary>
    /// FSM_API validity gate. A context is not runnable until its owning object
    /// has completed initialization and explicitly marks it valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Lifecycle invalidation is deliberately separate from FSM_API validity.
    /// It is a state-machine input, not a request to stop the FSM scheduler.
    /// </summary>
    public bool IsInvalidated { get; set; }

    /// <summary>Current lifecycle state as exposed to the manifestation provider.</summary>
    public string Phase { get; set; } = "Created";

    /// <summary>Monotonic lifecycle time supplied by the scheduler.</summary>
    public long ElapsedMilliseconds { get; set; }

    /// <summary>Optional parent bundle identity used for generative effects.</summary>
    public int ParentId { get; set; } = -1;

    /// <summary>Generation/lineage value used for generative manifestations.</summary>
    public int Generation { get; set; }
}
