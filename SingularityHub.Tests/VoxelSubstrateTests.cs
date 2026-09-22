using TheSingularityWorkshop.Workshop.Rendering;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Proves that voxel occupancy, carving, and exterior surface discovery remain separate concerns.</summary>
public sealed class VoxelSubstrateTests
{
    [Fact]
    public void Filled_Cube_Produces_Only_Exterior_Faces()
    {
        var volume = new VoxelSubstrate(3, 3, 3);
        volume.Fill();

        Assert.Equal(27, volume.OccupiedCount);
        Assert.Equal(54, volume.EnumerateExteriorFaces().Count());
    }

    [Fact]
    public void Removing_A_Cube_Changes_Data_And_Exposes_Interior_Surface()
    {
        var volume = new VoxelSubstrate(3, 3, 3);
        volume.Fill();

        volume[1, 1, 1] = false;

        Assert.Equal(26, volume.OccupiedCount);
        Assert.Equal(60, volume.EnumerateExteriorFaces().Count());
    }

    [Fact]
    public void Sphere_Tool_Removes_Micro_Cubes_Without_Creating_Render_Data()
    {
        var volume = new VoxelSubstrate(9, 9, 9);
        volume.Fill();

        volume.RemoveSphere(4, 4, 4, 2);

        Assert.True(volume.OccupiedCount < 729);
        Assert.True(volume.EnumerateExteriorFaces().Any());
    }
}
