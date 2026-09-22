namespace TheSingularityWorkshop.Workshop.Rendering;

/// <summary>
/// CPU-side construction truth for voxel geometry. Occupied cells are data only;
/// exterior faces are the boundary from which a renderer may construct mesh.
/// </summary>
public sealed class VoxelSubstrate
{
    private readonly byte[] _cells;

    /// <summary>Creates an empty voxel volume.</summary>
    public VoxelSubstrate(int width, int height, int depth)
    {
        if (width <= 0 || height <= 0 || depth <= 0)
            throw new ArgumentOutOfRangeException();

        Width = width;
        Height = height;
        Depth = depth;
        _cells = new byte[checked(width * height * depth)];
    }

    /// <summary>Gets the volume width in micro-cubes.</summary>
    public int Width { get; }

    /// <summary>Gets the volume height in micro-cubes.</summary>
    public int Height { get; }

    /// <summary>Gets the volume depth in micro-cubes.</summary>
    public int Depth { get; }

    /// <summary>Gets the number of occupied micro-cubes.</summary>
    public int OccupiedCount { get; private set; }

    /// <summary>Gets or sets whether a micro-cube is material.</summary>
    public bool this[int x, int y, int z]
    {
        get => Contains(x, y, z) && _cells[Index(x, y, z)] != 0;
        set
        {
            if (!Contains(x, y, z))
                return;

            var index = Index(x, y, z);
            var previous = _cells[index] != 0;

            if (previous == value)
                return;

            _cells[index] = value ? (byte)1 : (byte)0;
            OccupiedCount += value ? 1 : -1;
        }
    }

    /// <summary>Fills the complete volume with material.</summary>
    public void Fill()
    {
        Array.Fill(_cells, (byte)1);
        OccupiedCount = _cells.Length;
    }

    /// <summary>Removes material inside a spherical carving tool.</summary>
    public void RemoveSphere(double centerX, double centerY, double centerZ, double radius)
    {
        if (radius < 0)
            throw new ArgumentOutOfRangeException(nameof(radius));

        var radiusSquared = radius * radius;

        for (var z = Math.Max(0, (int)Math.Floor(centerZ - radius)); z <= Math.Min(Depth - 1, (int)Math.Ceiling(centerZ + radius)); z++)
        for (var y = Math.Max(0, (int)Math.Floor(centerY - radius)); y <= Math.Min(Height - 1, (int)Math.Ceiling(centerY + radius)); y++)
        for (var x = Math.Max(0, (int)Math.Floor(centerX - radius)); x <= Math.Min(Width - 1, (int)Math.Ceiling(centerX + radius)); x++)
        {
            var dx = x - centerX;
            var dy = y - centerY;
            var dz = z - centerZ;

            if (dx * dx + dy * dy + dz * dz <= radiusSquared)
                this[x, y, z] = false;
        }
    }

    /// <summary>Determines whether an occupied cube touches empty space.</summary>
    public bool IsExterior(int x, int y, int z)
    {
        if (!this[x, y, z])
            return false;

        return !this[x + 1, y, z] ||
               !this[x - 1, y, z] ||
               !this[x, y + 1, z] ||
               !this[x, y - 1, z] ||
               !this[x, y, z + 1] ||
               !this[x, y, z - 1];
    }

    /// <summary>Enumerates the exposed cube faces from which a mesh can be constructed.</summary>
    public IEnumerable<VoxelSurfaceFace> EnumerateExteriorFaces()
    {
        for (var z = 0; z < Depth; z++)
        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
        {
            if (!this[x, y, z])
                continue;

            if (!this[x + 1, y, z]) yield return new(x, y, z, VoxelFaceDirection.PositiveX);
            if (!this[x - 1, y, z]) yield return new(x, y, z, VoxelFaceDirection.NegativeX);
            if (!this[x, y + 1, z]) yield return new(x, y, z, VoxelFaceDirection.PositiveY);
            if (!this[x, y - 1, z]) yield return new(x, y, z, VoxelFaceDirection.NegativeY);
            if (!this[x, y, z + 1]) yield return new(x, y, z, VoxelFaceDirection.PositiveZ);
            if (!this[x, y, z - 1]) yield return new(x, y, z, VoxelFaceDirection.NegativeZ);
        }
    }

    private bool Contains(int x, int y, int z)
        => x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth;

    private int Index(int x, int y, int z)
        => x + Width * (y + Height * z);
}

/// <summary>Identifies which side of an occupied micro-cube contributes an exterior face.</summary>
public enum VoxelFaceDirection
{
    NegativeX,
    PositiveX,
    NegativeY,
    PositiveY,
    NegativeZ,
    PositiveZ
}

/// <summary>Identifies one exterior face of one occupied micro-cube.</summary>
public readonly record struct VoxelSurfaceFace(int X, int Y, int Z, VoxelFaceDirection Direction);
