using TheSingularityWorkshop.FSM_COS;
using TheSingularityWorkshop.Workshop.Composition;
using TheSingularityWorkshop.Workshop.Plant;

namespace TheSingularityWorkshop.Services;

public sealed class WorkshopExperienceService : IDisposable
{
    public event Action? StateChanged;
    private readonly FirstContactFsm _firstContact = new();
    private readonly IFsmCos _compositionSystem;
    private bool _disposed;

    public string CurrentState { get; private set; } = "Intro";
    public bool IsFirstVisit { get; private set; }
    public bool ShowUnity => false;
    public bool IsInitialized { get; private set; }
    public FlexExperienceDefinition? SelectedFlexExperience { get; private set; }
    public FirstContactFsm FirstContact => _firstContact;
    public RuntimeAssembly? RuntimeAssembly { get; private set; }
    public GuiNode? MonikerComposition => RuntimeAssembly?.Bundles
        .OfType<MonikerCompositionBundle>()
        .Select(bundle => bundle.Composition)
        .FirstOrDefault(composition => composition is not null);

    public WorkshopExperienceService(IFsmCos compositionSystem)
    {
        _compositionSystem = compositionSystem ?? throw new ArgumentNullException(nameof(compositionSystem));
    }

    public void Initialize(bool returningVisitor = false)
    {
        if(IsInitialized)return;
        IsInitialized=true; IsFirstVisit=!returningVisitor;
        RuntimeAssembly=_compositionSystem.Execute(new RuntimeManifest(
            RuntimeId:1UL,
            Bundles:new[]{BundleRequest.Unconfigured(MonikerCompositionBundle.BundleId)}));
        CurrentState="FirstContact";
        _firstContact.Start();
        StateChanged?.Invoke();
    }
    public void MarkVisited(){}
    public void RequestEntry()
    {
        if(CurrentState!="FirstContact"||_firstContact.CurrentState!="Gateway")return;
        SelectedFlexExperience=FlexExperienceCatalog.SelectDefault();
        _firstContact.RequestEntry(); StateChanged?.Invoke();
    }
    public void Tick()
    {
        if(_disposed||!IsInitialized||CurrentState!="FirstContact")return;
        _firstContact.Update();
        if(_firstContact.CurrentState=="HubGrowth"&&_firstContact.StateTicks>=30)_firstContact.SetHubReady();
        if(_firstContact.IsLanding)SetState("Intro");
    }
    public void MarkUnityStarted(){}
    public void SetCriticalMassReached()=>SetState("CriticalMass");
    public void SetState(string state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        if(CurrentState==state)return;
        CurrentState=state; StateChanged?.Invoke();
    }
    public void Dispose()
    {
        if(_disposed)return;
        _firstContact.Dispose(); _disposed=true;
    }
}