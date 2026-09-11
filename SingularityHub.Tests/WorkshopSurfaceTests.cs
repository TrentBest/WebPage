using TheSingularityWorkshop.Workshop;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Layer 4 tests for the public Workshop experience surfaces.</summary>
public sealed class WorkshopSurfaceTests
{
    [ArchitectureTest(4, 2, 1)]
    [Fact(DisplayName = "4.02.001 — Public_Surface_Set_Is_Stable")]
    public void Public_Surface_Set_Is_Stable()
    {
        var surfaces = WorkshopSurfaceDescriptor.PublicSurfaces;

        Assert.Equal(4, surfaces.Count);
        Assert.Equal(WorkshopSurface.WhatIsThis, surfaces[0].Surface);
        Assert.Equal(WorkshopSurface.Experiences, surfaces[1].Surface);
        Assert.Equal(WorkshopSurface.Create, surfaces[2].Surface);
        Assert.Equal(WorkshopSurface.Shop, surfaces[3].Surface);
    }

    [ArchitectureTest(4, 2, 2)]
    [Fact(DisplayName = "4.02.002 — Public_Surface_Titles_Are_User_Facing")]
    public void Public_Surface_Titles_Are_User_Facing()
    {
        var surfaces = WorkshopSurfaceDescriptor.PublicSurfaces;

        Assert.Equal("What Is This?", surfaces[0].Title);
        Assert.Equal("Experiences", surfaces[1].Title);
        Assert.Equal("Create", surfaces[2].Title);
        Assert.Equal("Singularity Shop", surfaces[3].Title);
    }

    [ArchitectureTest(4, 2, 3)]
    [Fact(DisplayName = "4.02.003 — Every_Public_Surface_Has_A_Purpose")]
    public void Every_Public_Surface_Has_A_Purpose()
    {
        Assert.All(WorkshopSurfaceDescriptor.PublicSurfaces, surface =>
        {
            Assert.False(string.IsNullOrWhiteSpace(surface.Title));
            Assert.False(string.IsNullOrWhiteSpace(surface.Purpose));
        });
    }
}
