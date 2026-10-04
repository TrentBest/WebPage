using TheSingularityWorkshop.Workshop;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Exhaustive unit coverage for the primary Workshop surface definitions.</summary>
public sealed class WorkshopSurfaceTests
{
    [Fact(DisplayName = "Workshop surface enum has four stable values")]
    public void SurfaceEnumHasFourStableValues()
    {
        var values = Enum.GetValues<WorkshopSurface>();
        Assert.Equal(4, values.Length);
        Assert.Equal(0, (int)WorkshopSurface.Understand);
        Assert.Equal(1, (int)WorkshopSurface.Experiences);
        Assert.Equal(2, (int)WorkshopSurface.Create);
        Assert.Equal(3, (int)WorkshopSurface.Publish);
    }

    [Fact(DisplayName = "Primary surface descriptors cover every enum value exactly once")]
    public void DescriptorsCoverEverySurfaceExactlyOnce()
    {
        var descriptors = WorkshopSurfaceDescriptor.PublicSurfaces;
        var enumValues = Enum.GetValues<WorkshopSurface>();
        Assert.Equal(enumValues.Length, descriptors.Count);
        Assert.Equal(enumValues.OrderBy(x => x), descriptors.Select(x => x.Surface).Distinct().OrderBy(x => x));
    }

    [Fact(DisplayName = "Primary surface order is understand experience create publish")]
    public void PublicSurfaceOrderIsStable()
    {
        Assert.Equal(
            new[] { WorkshopSurface.Understand, WorkshopSurface.Experiences, WorkshopSurface.Create, WorkshopSurface.Publish },
            WorkshopSurfaceDescriptor.PublicSurfaces.Select(x => x.Surface));
    }

    [Fact(DisplayName = "Every primary surface has exact title")]
    public void EveryPublicSurfaceHasExactTitle()
    {
        var expected = new Dictionary<WorkshopSurface, string>
        {
            [WorkshopSurface.Understand] = "Understand",
            [WorkshopSurface.Experiences] = "Experiences",
            [WorkshopSurface.Create] = "Create",
            [WorkshopSurface.Publish] = "Publish"
        };

        foreach (var descriptor in WorkshopSurfaceDescriptor.PublicSurfaces)
            Assert.Equal(expected[descriptor.Surface], descriptor.Title);
    }

    [Fact(DisplayName = "Every primary surface has a meaningful purpose")]
    public void EveryPublicSurfaceHasMeaningfulPurpose()
    {
        Assert.All(WorkshopSurfaceDescriptor.PublicSurfaces, descriptor =>
        {
            Assert.False(string.IsNullOrWhiteSpace(descriptor.Title));
            Assert.False(string.IsNullOrWhiteSpace(descriptor.Purpose));
            Assert.DoesNotContain("page", descriptor.Purpose, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact(DisplayName = "Descriptor value semantics include all fields")]
    public void DescriptorValueSemanticsIncludeAllFields()
    {
        var a = new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Compose");
        Assert.Equal(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Compose"));
        Assert.NotEqual(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Publish, "Create", "Compose"));
        Assert.NotEqual(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Other", "Compose"));
        Assert.NotEqual(a, new WorkshopSurfaceDescriptor(WorkshopSurface.Create, "Create", "Other"));
    }

    [Fact(DisplayName = "Primary surfaces are repeatable and stable")]
    public void PublicSurfacesAreRepeatableAndStable()
        => Assert.Equal(WorkshopSurfaceDescriptor.PublicSurfaces, WorkshopSurfaceDescriptor.PublicSurfaces);
}
