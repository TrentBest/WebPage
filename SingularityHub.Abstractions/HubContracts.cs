namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Global execution phase selected by the composition manifest and owned by the Hub host.</summary>
public enum ExperienceExecutionPhase
{
    Startup,
    Transitioning,
    Running
}

/// <summary>Stable runtime capability identifiers shared across independently compiled Experiences.</summary>
public static class HubCapabilityIds
{
    /// <summary>Capability identifying an Experience that can provide a user-defined Moniker presentation.</summary>
    public const ulong Moniker = 0x4D4F4E494B455201UL;
}

/// <summary>
/// Ordered composition instructions for an Experience host.
/// The lists are execution order, not discovery hints: when a manifest is supplied,
/// the host follows this composition instead of reconstructing a default trail.
/// </summary>
public readonly record struct ExperienceManifest(
    IReadOnlyList<ulong> Startup,
    IReadOnlyList<ulong> Transitioning,
    IReadOnlyList<ulong> Running,
    IReadOnlyList<ulong> RequiredCapabilities)
{
    /// <summary>Returns whether the manifest explicitly requires a capability.</summary>
    public bool RequiresCapability(ulong capabilityId) => RequiredCapabilities.Contains(capabilityId);

    /// <summary>Returns whether an Experience identity appears in any phase.</summary>
    public bool ContainsExperience(ulong experienceId)
        => Startup.Contains(experienceId)
        || Transitioning.Contains(experienceId)
        || Running.Contains(experienceId);
}

/// <summary>
/// Optional presentation surface supplied by an Experience. Presentation and removal
/// are capabilities of the Experience, so a Moniker is not constrained to one animation.
/// </summary>
public interface IExperiencePresentation
{
    IReadOnlyList<ulong> PresentationMicroBundleIds { get; }
    IReadOnlyList<ulong> RemovalMicroBundleIds { get; }
}
