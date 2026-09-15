using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// Represents a small, independently schedulable unit of Workshop behavior.
/// A micro-bundle owns an <see cref="IStateContext"/> and delegates its visible
/// manifestation to a provider.
/// </summary>
public interface IMicroBundle
{
    /// <summary>Stable integer identity used by compact command protocols.</summary>
    int Id { get; }

    /// <summary>The runtime context carried by the bundle's FSM.</summary>
    IStateContext Context { get; }

    /// <summary>Advances the bundle's lifecycle through FSM_API.</summary>
    void Update();
}
