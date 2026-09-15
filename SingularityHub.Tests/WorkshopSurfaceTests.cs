using TheSingularityWorkshop.Workshop;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Exhaustive unit coverage for the public Workshop surface definitions.</summary>
public sealed class WorkshopSurfaceTests
{
    [Fact(DisplayName = "Workshop surface enum has four stable values")]
    public void SurfaceEnumHasFourStableValues()
    {
        var values = Enum.GetValues<WorkshopSurface>();
        Assert.Equal(4, values.Length);
        Assert.Equal(0, (int)WorkshopSurface.WhatIsThis);
        Assert.Equal(1, (int)WorkshopSurface.Experiences);
        Assert.Equal(2, (int)WorkshopSurface.Create);
        Assert.Equal(3, (int)WorkshopSurface.Shop);
    }

    [Fact(DisplayName = "Public surface descriptors cover every enum value exactly once")]
    public void DescriptorsCoverEverySurfaceExactlyOnce()
    {
        var descriptors = WorkshopSurfaceDescriptor.PublicSurfaces;
        var enumValues = Enum.GetValues<WorkshopSurface>();
        Assert.Equal(enumValues.Length, descriptors.Count);
        Assert.Equal(enumValues.OrderBy(x => x), descriptors.Select(x => x.Surface).Distinct().OrderBy(x => x));
    }

    [Fact(DisplayName = "Public surface order is understand experience create publish")]
    public void PublicSurfaceOrderIsStable()
    {
        Assert.Equal(
            new[] { WorkshopSurface.WhatIsThis, WorkshopSurface.Experiences, WorkshopSurface.Create, WorkshopSurface.Shop },
            WorkshopSurfaceDescriptor.PublicSurfaces.Select(x => x.Surface));
    }

    [Fact(DisplayName = "Every public surface has exact title")]
    public void EveryPublicSurfaceHasExactTitle()
    {
        var expected = new Dictionary<WorkshopSurface, string>
        {
            [WorkshopSurface.WhatIsThis] = "What Is This?",
            [WorkshopSurface.Experiences] = "Experiences",
            [WorkshopSurface.Create] = "Create",
            [WorkshopSurface.Shop] = "Singularity Shop"
        };
        foreach (var descriptor in WorkshopSurfaceDescriptor.PublicSurfaces)
            Assert.Equal(expected[descriptor.Surface], descriptor.Title);
    }

    [Fact(DisplayName = "Every public surface has exact purpose")]
    public void EveryPublicSurfaceHasExactPurpose()
    {
        var expected = new Dictionary<WorkshopSurface, string>
        {
            [WorkshopSurface.WhatIsThis] = "Understand the Hub, Workshop, and composition model.",
            [WorkshopSurface.Experiences] = "Explore working things built with the Workshop.",
            [WorkshopSurface.Create] = "Compose something of your own.",
            [WorkshopSurface.Shop] = "Publish and discover reusable Workshop creations."
        };
        foreach (var descriptor in WorkshopSurfaceDescriptor.PublicSurfaces)
            Assert.Equal(expected[descriptor.Surface], descriptor.Purpose);
    }

    [Fact(DisplayName = "Descriptor value semantics include all fields")]
    public void DescriptorValueSemanticsIncludeAllFields()
    {
        var a = new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Compose");
        Assert.Equal(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Compose"));
        Assert.NotEqual(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Shop, "Create", "Compose"));
        Assert.NotEqual(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Other", "Compose"));
        Assert.NotEqual(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Other"));
    }

    [Fact(DisplayName = "Public surfaces are repeatable and stable")]
    public void PublicSurfacesAreRepeatableAndStable()
        => Assert.Equal(WorkshopSurfaceDescriptor.PublicSurfaces, WorkshopSurfaceDescriptor.PublicSurfaces);

    [Fact(DisplayName = "Public surfaces expose non-empty user-facing metadata")]
    public void PublicSurfacesExposeNonEmptyMetadata()
    {
        Assert.All(WorkshopSurfaceDescriptor.PublicSurfaces, descriptor =>
        {
            Assert.False(string.IsNullOrWhiteSpace(descriptor.Title));
            Assert.False(string.IsNullOrWhiteSpace(descriptor.Purpose));
        });
    }
}
