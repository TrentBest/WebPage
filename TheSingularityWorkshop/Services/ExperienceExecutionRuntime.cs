using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Services;

/// <summary>
/// Host execution seam between a manifest-selected Experience and its current runtime.
/// It owns tick routing and observation selection without claiming that composition
/// alone makes the RuntimeAssembly an executable lifecycle.
/// </summary>
public interface IExperienceExecutionRuntime
{
    PageFSM Page { get; }
    PageStateContext Context { get; }
    IExperience? Experience { get; }
    RuntimeAssembly? RuntimeAssembly { get; }
    bool IsManifestBound { get; }
    string ExecutionOwner { get; }
    string CurrentState { get; }
    string LivingGuiState { get; }
    string LivingGuiActivePhase { get; }
    bool LivingGuiIsValid { get; }
    TimeSpan MonikerPresentationDuration { get; }
    void RequestEnter();
    void SignalLivingGuiPopulated();
    void SignalMonikerReady();
    void SignalGravityReleased();
    void SignalLivingGuiFallen();
    void SignalNavigationReady();
    void Shutdown();
    void Tick();
    PageFSM.RuntimeObservation Observe();
}

/// <summary>
/// Compatibility adapter used until the Experience package exposes an executable
/// lifecycle contract. Binding requires every MicroBundle declared by the Experience
/// to be present in the composed RuntimeAssembly.
/// </summary>
public sealed class PageFsmExecutionRuntime : IExperienceExecutionRuntime
{
    public PageFSM Page { get; }
    public PageStateContext Context => Page.Context;
    public IExperience? Experience { get; }
    public RuntimeAssembly? RuntimeAssembly { get; }
    public bool IsManifestBound => Experience is not null && RuntimeAssembly is not null;

    public string ExecutionOwner => IsManifestBound
        ? $"Manifest-bound host adapter: {Experience!.Name} → preserved PageFSM"
        : "WebPage compatibility PageFSM";

    public PageFsmExecutionRuntime(
        PageFSM page,
        IExperience? experience = null,
        RuntimeAssembly? runtimeAssembly = null)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
        if ((experience is null) != (runtimeAssembly is null))
            throw new ArgumentException("Experience identity and composed RuntimeAssembly must be supplied together.");

        if (experience is not null && runtimeAssembly is not null)
        {
            var composedBundleIds = runtimeAssembly.Bundles.Select(bundle => bundle.Id).ToHashSet();
            var missingBundleIds = experience.MicroBundleIds
                .Where(bundleId => !composedBundleIds.Contains(bundleId))
                .ToArray();
            if (missingBundleIds.Length > 0)
                throw new InvalidOperationException(
                    $"Experience {experience.Id} cannot bind to RuntimeAssembly {runtimeAssembly.RuntimeId}; " +
                    $"required MicroBundles are missing: {string.Join(", ", missingBundleIds)}.");

            Experience = experience;
            RuntimeAssembly = runtimeAssembly;
        }
    }

    public string CurrentState => Page.CurrentState;
    public string LivingGuiState => Page.LivingGuiState;
    public string LivingGuiActivePhase => Page.LivingGuiActivePhase;
    public bool LivingGuiIsValid => Page.LivingGuiIsValid;
    public TimeSpan MonikerPresentationDuration => Page.MonikerPresentationDuration;

    public void RequestEnter() => Page.RequestEnter();
    public void SignalLivingGuiPopulated() => Page.SignalLivingGuiPopulated();
    public void SignalMonikerReady() => Page.SignalMonikerReady();
    public void SignalGravityReleased() => Page.SignalGravityReleased();
    public void SignalLivingGuiFallen() => Page.SignalLivingGuiFallen();
    public void SignalNavigationReady() => Page.SignalNavigationReady();
    public void Shutdown() => Page.Shutdown();
    public void Tick() => Page.Update();

    public PageFSM.RuntimeObservation Observe() => Page.ObserveRuntime();
}
