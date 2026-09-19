namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Compact nine-layer ontological coordinate for a runtime capability.</summary>
public readonly struct OntologySignature : IEquatable<OntologySignature>
{
    /// <summary>Number of ontology layers.</summary>
    public const int LayerCount = 9;
    private readonly int _l0, _l1, _l2, _l3, _l4, _l5, _l6, _l7, _l8;
    /// <summary>Schema version of this signature.</summary>
    public int Version { get; }
    /// <summary>Compact capability flags associated with the signature.</summary>
    public uint Capabilities { get; }

    /// <summary>Creates an ontology signature from its nine layers and optional version/capabilities.</summary>
    public OntologySignature(int l0, int l1, int l2, int l3, int l4, int l5, int l6, int l7, int l8, int version = 1, uint capabilities = 0)
    {
        _l0=l0; _l1=l1; _l2=l2; _l3=l3; _l4=l4; _l5=l5; _l6=l6; _l7=l7; _l8=l8; Version=version; Capabilities=capabilities;
    }
    /// <summary>Gets a layer by zero-based index.</summary>
    public int this[int layer] => layer switch { 0=>_l0,1=>_l1,2=>_l2,3=>_l3,4=>_l4,5=>_l5,6=>_l6,7=>_l7,8=>_l8,_=>throw new ArgumentOutOfRangeException(nameof(layer)) };
    /// <summary>Gets the deterministic compact structural identity.</summary>
    public ulong StructuralId { get { unchecked { ulong h=14695981039346656037UL; h=(h^(uint)_l0)*1099511628211UL; h=(h^(uint)_l1)*1099511628211UL; h=(h^(uint)_l2)*1099511628211UL; h=(h^(uint)_l3)*1099511628211UL; h=(h^(uint)_l4)*1099511628211UL; h=(h^(uint)_l5)*1099511628211UL; h=(h^(uint)_l6)*1099511628211UL; h=(h^(uint)_l7)*1099511628211UL; h=(h^(uint)_l8)*1099511628211UL; h=(h^(uint)Version)*1099511628211UL; return (h^(uint)Capabilities)*1099511628211UL; } } }
    /// <inheritdoc/>
    public bool Equals(OntologySignature other) => _l0==other._l0&&_l1==other._l1&&_l2==other._l2&&_l3==other._l3&&_l4==other._l4&&_l5==other._l5&&_l6==other._l6&&_l7==other._l7&&_l8==other._l8&&Version==other.Version&&Capabilities==other.Capabilities;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is OntologySignature other && Equals(other);
    /// <inheritdoc/>
    public override int GetHashCode() => StructuralId.GetHashCode();
    /// <summary>Compares two ontology signatures for equality.</summary>
    public static bool operator ==(OntologySignature left, OntologySignature right) => left.Equals(right);
    /// <summary>Compares two ontology signatures for inequality.</summary>
    public static bool operator !=(OntologySignature left, OntologySignature right) => !left.Equals(right);
}

/// <summary>Semantic version for a MicroBundle or Experience.</summary>
public readonly record struct BundleVersion(int Major, int Minor, int Patch);
/// <summary>Categories of recorded arbitration mutation.</summary>
public enum MutationType { StructuralMutation, PropertyInjection, DependencyResolution }

/// <summary>Reusable capability with a once-only load phase followed by bounded arbitration.</summary>
/// <summary>Reusable capability with a once-only load phase followed by bounded arbitration.</summary>
public interface IMicroBundle
{
    /// <summary>Stable bundle identity.</summary>
    ulong Id { get; }
    /// <summary>Ontological location of the bundle.</summary>
    OntologySignature Ontology { get; }
    /// <summary>Current bundle version.</summary>
    BundleVersion Version { get; }
    /// <summary>Stable identities of required bundles.</summary>
    IReadOnlyList<ulong> Dependencies { get; }
    /// <summary>Receives its arbitrator exactly once during installation.</summary>
    void LoadBundle(IArbitrator arbitrator);
    /// <summary>Performs one logical arbitration round after installation.</summary>
    bool Arbitrate(IArbitrator arbitrator, int roundIndex);
}
/// <summary>Runtime composition boundary for an Experience.</summary>
public interface IExperience
{
    ulong Id { get; }
    /// <summary>Human-readable Experience name.</summary>
    string Name { get; }
    BundleVersion Version { get; }
    OntologySignature Ontology { get; }
    /// <summary>MicroBundles explicitly composed into the Experience.</summary>
    IReadOnlyList<ulong> MicroBundleIds { get; }
    /// <summary>Capabilities exposed by the Experience.</summary>
    IReadOnlyList<ulong> Capabilities { get; }
    /// <summary>Sensory systems used by the Experience.</summary>
    IReadOnlyList<ulong> SensorySystems { get; }
    /// <summary>Distinct sensory-system count.</summary>
    int SenseCount => SensorySystems.Distinct().Count();
    /// <summary>Processing groups associated with the Experience.</summary>
    IReadOnlyList<string> ProcessingGroups { get; }
}
/// <summary>Immutable audit record for one arbitration mutation.</summary>
public readonly record struct ArbitrationEvent(int RoundIndex, ulong ActorId, ulong TargetCoordinates, MutationType MutationType, ulong CausalParentId);
/// <summary>Audit sink for arbitration events.</summary>
public interface IArbitrationAudit { IReadOnlyList<ArbitrationEvent> Events { get; } void Record(ArbitrationEvent arbitrationEvent); }
/// <summary>Coordinates MicroBundle discovery, installation, and bounded arbitration.</summary>
public interface IArbitrator
{
    /// <summary>Bundles that completed installation.</summary>
    IReadOnlyCollection<IMicroBundle> LoadedBundles { get; }
    /// <summary>Bundles registered and available for installation.</summary>
    IReadOnlyCollection<IMicroBundle> AvailableBundles { get; }
    /// <summary>Registers a bundle as available without installing it.</summary>
    bool RegisterBundle(IMicroBundle bundle);
    /// <summary>Installs a bundle and invokes its once-only load hook.</summary>
    bool LoadBundle(IMicroBundle bundle);
    /// <summary>Attempts to resolve and install an available bundle.</summary>
    bool TryLoadBundle(ulong bundleId);
    /// <summary>Attempts to resolve an installed bundle.</summary>
    bool TryGetLoadedBundle(ulong bundleId, out IMicroBundle? bundle);
    /// <summary>Runs the bounded arbitration pipeline.</summary>
    int ExecuteArbitrationPipeline();
}
/// <summary>Stable address into a data warehouse.</summary>
public readonly record struct WarehouseAddress(ulong Identity);
/// <summary>Resolves compact identities into warehouse addresses.</summary>
public interface IDataWarehouseLiaison { bool TryResolve(ulong identity, out WarehouseAddress address); }
/// <summary>Compact description of scheduled work.</summary>
public readonly record struct WorkDescriptor(ulong ProcessGroupId, ulong WorkId, int PathIndex);
/// <summary>Result token for scheduled work.</summary>
public readonly record struct CompletionToken(ulong Value, bool Completed);
/// <summary>Executes scheduled work descriptors.</summary>
public interface IExecutionProvider { CompletionToken Execute(WorkDescriptor work); }
/// <summary>Lifecycle state of a process group.</summary>
public enum ProcessGroupState { Registered, Active, Completed }
/// <summary>Snapshot of a process group's lifecycle state.</summary>
public readonly record struct ProcessGroupSnapshot(ulong Id, ProcessGroupState State);
/// <summary>Registers and advances process groups.</summary>
public interface IProcessGroupHost { bool Register(ulong id); bool Activate(ulong id); bool Complete(ulong id); IReadOnlyCollection<ProcessGroupSnapshot> ActiveGroups { get; } }

/// <summary>Common Hub boundary for arbitration, scheduling, data, and routing.</summary>
public interface ISingularityHub : IArbitrator, IDataWarehouseLiaison, IProcessGroupHost
{
    /// <summary>Gets the arbitration audit.</summary>
    IArbitrationAudit Audit { get; }
    /// <summary>Gets the Hub routing table.</summary>
    ISingularityRouting Routing { get; }
}

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
