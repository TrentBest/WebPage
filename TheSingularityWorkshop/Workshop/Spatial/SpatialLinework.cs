using TheSingularityWorkshop.FSM_API;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Presentation-independent linework substrate for a spatial scene.
/// One line is encoded as one RGBA texture texel: X1, Y1, X2, Y2.
/// This keeps the data suitable for a future WebGL/WebGPU renderer while the
/// current perception layer can still render the same buffer as SVG.
/// </summary>
public sealed class SpatialLinework : IDisposable
{
    private const string ProcessingGroup = "SpatialLinework";
    private const string DefinitionName = "SpatialLineworkFSM";

    private readonly LineworkContext _context = new();
    private readonly List<SpatialLine> _lines = [];
    private FSMHandle? _fsm;
    private bool _disposed;

    /// <summary>Creates an empty linework controller with its FSM lifecycle.</summary>
    public SpatialLinework()
    {
        BuildFsm();
        _fsm = FSM_API.Create.CreateInstance(DefinitionName, _context, ProcessingGroup);
    }

    /// <summary>Current linework lifecycle state.</summary>
    public string State => _fsm?.CurrentState ?? "Idle";

    /// <summary>Immutable view of the current line primitives.</summary>
    public IReadOnlyList<SpatialLine> Lines => _lines;

    /// <summary>GPU-oriented texture buffer representation of the current lines.</summary>
    public SpatialLineTextureBuffer TextureBuffer => SpatialLineTextureBuffer.FromLines(_lines);

    /// <summary>Starts a line at a world-space point.</summary>
    public void BeginLine(SpatialPoint start)
    {
        ThrowIfDisposed();
        _context.StartX = start.X;
        _context.StartY = start.Y;
        _context.EndX = start.X;
        _context.EndY = start.Y;
        _context.DrawingRequested = true;
        UpdateFsm();
    }

    /// <summary>Moves the active line endpoint without committing a new primitive.</summary>
    public void ContinueLine(SpatialPoint end)
    {
        ThrowIfDisposed();
        if (State != "Drawing") return;
        _context.EndX = end.X;
        _context.EndY = end.Y;
        UpdateFsm();
    }

    /// <summary>Commits the active line to the line buffer.</summary>
    public SpatialLine EndLine()
    {
        ThrowIfDisposed();
        if (State != "Drawing")
            throw new InvalidOperationException("Linework is not currently drawing.");

        var line = new SpatialLine(
            _lines.Count,
            new SpatialPoint(_context.StartX, _context.StartY),
            new SpatialPoint(_context.EndX, _context.EndY));

        _lines.Add(line);
        _context.DrawingRequested = false;
        UpdateFsm();
        return line;
    }

    /// <summary>Clears all committed lines and returns the controller to idle.</summary>
    public void Clear()
    {
        ThrowIfDisposed();
        _lines.Clear();
        _context.DrawingRequested = false;
        _context.ClearRequested = true;
        UpdateFsm();
        _context.ClearRequested = false;
    }

    /// <summary>Updates the FSM once, allowing the linework lifecycle to advance.</summary>
    public void Update()
    {
        ThrowIfDisposed();
        UpdateFsm();
    }

    private void BuildFsm()
    {
        if (!FSM_API.Interaction.Exists(DefinitionName, ProcessingGroup))
        {
            FSM_API.Create.CreateProcessingGroup(ProcessingGroup);
            FSM_API.Create.CreateFiniteStateMachine(DefinitionName, -1, ProcessingGroup)
                .State("Idle", null, null, null)
                .State("Drawing", null, null, null)
                .Transition("Idle", "Drawing", context => ((LineworkContext)context).DrawingRequested)
                .Transition("Drawing", "Idle", context => !((LineworkContext)context).DrawingRequested)
                .BuildDefinition();
        }
    }

    private void UpdateFsm()
    {
        FSM_API.Interaction.Update(ProcessingGroup);
        if (_context.ClearRequested)
            _context.DrawingRequested = false;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _fsm?.Dispose();
        _lines.Clear();
    }

    private sealed class LineworkContext : IStateContext
    {
        public string Name { get; set; } = ProcessingGroup;
        public bool IsValid { get; set; } = true;
        public bool DrawingRequested { get; set; }
        public bool ClearRequested { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
    }
}

/// <summary>A single controllable world-space line primitive.</summary>
public readonly record struct SpatialLine(int Index, SpatialPoint Start, SpatialPoint End);

/// <summary>RGBA float texel containing one line's two endpoints: X1, Y1, X2, Y2.</summary>
public readonly record struct SpatialLineTextureTexel(float R, float G, float B, float A);

/// <summary>
/// Flat texture-shaped line buffer. Width equals line count and height is one.
/// A GPU backend can upload the texels directly to a floating-point texture.
/// </summary>
public sealed class SpatialLineTextureBuffer
{
    private SpatialLineTextureBuffer(IReadOnlyList<SpatialLineTextureTexel> texels)
    {
        Texels = texels;
    }

    public int Width => Texels.Count;
    public const int Height = 1;
    public IReadOnlyList<SpatialLineTextureTexel> Texels { get; }

    /// <summary>Encodes every line as one RGBA float texel.</summary>
    public static SpatialLineTextureBuffer FromLines(IReadOnlyList<SpatialLine> lines)
        => new(lines.Select(line => new SpatialLineTextureTexel(
            (float)line.Start.X,
            (float)line.Start.Y,
            (float)line.End.X,
            (float)line.End.Y)).ToArray());
}
