using System;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

public sealed class ExploreExperienceMicroBundle : IDisposable, IExperience
{
    public const int BundleId = 2100;
    public const ulong ExperienceId = 3002;

    private readonly MicroBundle _lifecycle;
    private bool _disposed;

    public ExploreExperienceMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "WORKSHOP EXPLORE EXPERIENCE", new WebMicroBundleProvider());
        Navigation = new SpatialNavigationMicroBundle();
        User = new UserMicroBundle();
        Conference = new ConferenceMicroBundle();
        Forge = new FsmForgeMicroBundle();
        SoftwarePatterns = new SoftwarePatternsMicroBundle();
        SoftwarePatternsAnnotation = new SoftwarePatternsAnnotationMicroBundle(SoftwarePatterns);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;

    public SpatialNavigationMicroBundle Navigation { get; }
    public UserMicroBundle User { get; }
    public ConferenceMicroBundle Conference { get; }
    public FsmForgeMicroBundle Forge { get; }
    public SoftwarePatternsMicroBundle SoftwarePatterns { get; }
    public SoftwarePatternsAnnotationMicroBundle SoftwarePatternsAnnotation { get; }

    ulong IExperience.Id => ExperienceId;
    string IExperience.Name => "WORKSHOP EXPLORE";
    BundleVersion IExperience.Version => new(1, 0, 0);
    OntologySignature IExperience.Ontology => new(1, 1, 1, 1, 1, 1, 1, 1, checked((int)ExperienceId));
    IReadOnlyList<ulong> IExperience.MicroBundleIds => [Navigation.Id, User.Id, Conference.Id, Forge.Id, SoftwarePatterns.Id, SoftwarePatternsAnnotation.Id];
    IReadOnlyList<ulong> IExperience.Capabilities => [HubCapabilityIds.SpatialNavigation, HubCapabilityIds.VisitorIdentity, HubCapabilityIds.Conference, HubCapabilityIds.FsmForge, HubCapabilityIds.SoftwarePatterns];
    IReadOnlyList<ulong> IExperience.SensorySystems => [1];
    IReadOnlyList<string> IExperience.ProcessingGroups => ["MicroBundle_2100", "MicroBundle_2101", "MicroBundle_2102", "MicroBundle_2104", "MicroBundle_2103", "MicroBundle_2213", "MicroBundle_2214"];

    public void Update()
    {
        if (_disposed) return;
        _lifecycle.Update();
        Navigation.Update();
        User.Update();
        Conference.Update();
        Forge.Update();
        SoftwarePatterns.Update();
        SoftwarePatternsAnnotation.Update();
    }

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;
        SoftwarePatternsAnnotation.Dispose();
        SoftwarePatterns.Dispose();
        Forge.Dispose();
        Conference.Dispose();
        User.Dispose();
        Navigation.Dispose();
        _lifecycle.Dispose();
        _disposed = true;
    }
}
