namespace TheSingularityWorkshop.SingularityHub;

/// <summary>Fixed-depth, runtime-safe ontological address.</summary>
public readonly struct OntologySignature : IEquatable<OntologySignature>
{
    public const int LayerCount = 9;
    private readonly int _l0, _l1, _l2, _l3, _l4, _l5, _l6, _l7, _l8;
    public int Version { get; }
    public uint Capabilities { get; }

    public OntologySignature(int l0, int l1, int l2, int l3, int l4, int l5, int l6, int l7, int l8, int version = 1, uint capabilities = 0)
    {
        _l0=l0; _l1=l1; _l2=l2; _l3=l3; _l4=l4; _l5=l5; _l6=l6; _l7=l7; _l8=l8;
        Version=version; Capabilities=capabilities;
    }

    public int this[int layer] => layer switch
    {
        0=>_l0, 1=>_l1, 2=>_l2, 3=>_l3, 4=>_l4, 5=>_l5, 6=>_l6, 7=>_l7, 8=>_l8,
        _=>throw new ArgumentOutOfRangeException(nameof(layer))
    };

    public ulong StructuralId
    {
        get
        {
            unchecked
            {
                ulong h=14695981039346656037UL;
                h=(h^(uint)_l0)*1099511628211UL; h=(h^(uint)_l1)*1099511628211UL;
                h=(h^(uint)_l2)*1099511628211UL; h=(h^(uint)_l3)*1099511628211UL;
                h=(h^(uint)_l4)*1099511628211UL; h=(h^(uint)_l5)*1099511628211UL;
                h=(h^(uint)_l6)*1099511628211UL; h=(h^(uint)_l7)*1099511628211UL;
                h=(h^(uint)_l8)*1099511628211UL; h=(h^(uint)Version)*1099511628211UL;
                return (h^(uint)Capabilities)*1099511628211UL;
            }
        }
    }

    public bool Equals(OntologySignature other) => _l0==other._l0&&_l1==other._l1&&_l2==other._l2&&_l3==other._l3&&_l4==other._l4&&_l5==other._l5&&_l6==other._l6&&_l7==other._l7&&_l8==other._l8&&Version==other.Version&&Capabilities==other.Capabilities;
    public override bool Equals(object? obj) => obj is OntologySignature other && Equals(other);
    public override int GetHashCode() => StructuralId.GetHashCode();
    public static bool operator ==(OntologySignature left, OntologySignature right) => left.Equals(right);
    public static bool operator !=(OntologySignature left, OntologySignature right) => !left.Equals(right);
}

public readonly record struct BundleVersion(int Major, int Minor, int Patch);

public interface IMicroBundle
{
    ulong Id { get; }
    OntologySignature Ontology { get; }
    BundleVersion Version { get; }
    IReadOnlyList<ulong> Dependencies { get; }
    bool Arbitrate(IArbitrator arbitrator, int roundIndex);
}

public readonly record struct ArbitrationEvent(int RoundIndex, ulong ActorId, ulong TargetCoordinates, string MutationType, ulong CausalParentId);

public interface IArbitrationAudit
{
    IReadOnlyList<ArbitrationEvent> Events { get; }
    void Record(ArbitrationEvent arbitrationEvent);
}

public interface IArbitrator
{
    IReadOnlyCollection<IMicroBundle> LoadedBundles { get; }
    bool LoadBundle(IMicroBundle bundle);
    int ExecuteArbitrationPipeline();
}

public readonly record struct WarehouseAddress(ulong Identity);

public interface IDataWarehouseLiaison
{
    bool TryResolve(ulong identity, out WarehouseAddress address);
}

public readonly record struct WorkDescriptor(ulong ProcessGroupId, ulong WorkId, int PathIndex);
public readonly record struct CompletionToken(ulong Value, bool Completed);

public interface IExecutionProvider
{
    CompletionToken Execute(WorkDescriptor work);
}

public enum ProcessGroupState { Registered, Active, Completed }
public readonly record struct ProcessGroupSnapshot(ulong Id, ProcessGroupState State);

public interface IProcessGroupHost
{
    bool Register(ulong id);
    bool Activate(ulong id);
    bool Complete(ulong id);
    IReadOnlyCollection<ProcessGroupSnapshot> ActiveGroups { get; }
}

public interface ISingularityHub : IArbitrator, IDataWarehouseLiaison, IProcessGroupHost
{
    IArbitrationAudit Audit { get; }
}
