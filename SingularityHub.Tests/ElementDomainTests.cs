using System.Reflection;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Layer 2: Element. These tests define the Atom → Element → Provider boundary.
/// No implementation is hidden in the tests; the suite intentionally exposes
/// missing architectural contracts as failures.
/// </summary>
public sealed class ElementDomainTests : IDisposable
{
    public ElementDomainTests() => Reset();

    [ArchitectureTest(2, 1, 1)]
    [Fact(DisplayName = "2.01.001 — NewElement_IsCreated")]
    public void Test_2_01_001_NewElement_IsCreated()
    {
        var element = CreateElement(2001);

        Assert.NotNull(element);
        Assert.Equal(2001, element.Id);
        Assert.IsType<MicroBundleContext>(element.Context);
    }

    [ArchitectureTest(2, 1, 2)]
    [Fact(DisplayName = "2.01.002 — Element_Owns_An_Atom")]
    public void Test_2_01_002_Element_Owns_An_Atom()
    {
        var element = CreateElement(2002);

        var atomMember = element.GetType()
            .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .SingleOrDefault(m => m.Name is "Atom" or "_atom" or "_fsm");

        Assert.NotNull(atomMember);
        var atomType = atomMember switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            _ => null
        };

        Assert.NotNull(atomType);
        Assert.True(typeof(FSMHandle).IsAssignableFrom(atomType!) || atomType == typeof(FSM),
            $"Element Atom member must be an FSM_API Atom handle/FSM, but was {atomType}.");
    }

    [ArchitectureTest(2, 1, 3)]
    [Fact(DisplayName = "2.01.003 — Atom_Starts_In_Defined_State")]
    public void Test_2_01_003_Atom_Starts_In_Defined_State()
    {
        var element = CreateElement(2003);
        var context = Assert.IsType<MicroBundleContext>(element.Context);

        Assert.Equal("Created", context.Phase);
        Assert.Equal(0, context.ElapsedMilliseconds);
        Assert.True(context.IsValid);
        Assert.Null(element.Manifestation);
    }

    [ArchitectureTest(2, 1, 4)]
    [Fact(DisplayName = "2.01.004 — Atom_Advances_Exactly_One_Transition")]
    public void Test_2_01_004_Atom_Advances_Exactly_One_Transition()
    {
        var element = CreateElement(2004);
        var context = Assert.IsType<MicroBundleContext>(element.Context);

        element.Update();

        Assert.Equal("Manifesting", context.Phase);
        Assert.Equal(1, context.ElapsedMilliseconds);

        element.Update();

        Assert.Equal("Active", context.Phase);
        Assert.Equal(2, context.ElapsedMilliseconds);
    }

    [ArchitectureTest(2, 1, 5)]
    [Fact(DisplayName = "2.01.005 — Element_Reflects_Atom_State")]
    public void Test_2_01_005_Element_Reflects_Atom_State()
    {
        var element = CreateElement(2005);
        var context = Assert.IsType<MicroBundleContext>(element.Context);

        Assert.Equal("Created", context.Phase);

        element.Update();
        Assert.Equal("Manifesting", context.Phase);

        element.Update();
        Assert.Equal("Active", context.Phase);
    }

    [ArchitectureTest(2, 1, 6)]
    [Fact(DisplayName = "2.01.006 — Provider_Cannot_Advance_Atom")]
    public void Test_2_01_006_Provider_Cannot_Advance_Atom()
    {
        var element = CreateElement(2006);
        var provider = new WebMicroBundleProvider();
        var context = Assert.IsType<MicroBundleContext>(element.Context);

        var manifestation = provider.Manifest(context);

        Assert.Equal("Created", context.Phase);
        Assert.Equal("Dormant", manifestation.Effect);
    }

    [ArchitectureTest(2, 1, 7)]
    [Fact(DisplayName = "2.01.007 — Two_Elements_Are_Isolated")]
    public void Test_2_01_007_Two_Elements_Are_Isolated()
    {
        var first = CreateElement(2007);
        var second = CreateElement(2008);
        var firstContext = Assert.IsType<MicroBundleContext>(first.Context);
        var secondContext = Assert.IsType<MicroBundleContext>(second.Context);

        first.Update();

        Assert.Equal("Manifesting", firstContext.Phase);
        Assert.Equal(1, firstContext.ElapsedMilliseconds);
        Assert.Equal("Created", secondContext.Phase);
        Assert.Equal(0, secondContext.ElapsedMilliseconds);
        Assert.Null(second.Manifestation);

        second.Update();

        Assert.Equal("Manifesting", secondContext.Phase);
        Assert.Equal(1, secondContext.ElapsedMilliseconds);
    }

    [ArchitectureTest(2, 1, 8)]
    [Fact(DisplayName = "2.01.008 — Reset_Cleanup_Isolates_Subsequent_Tests")]
    public void Test_2_01_008_Reset_Cleanup_Isolates_Subsequent_Tests()
    {
        _ = CreateElement(2009);
        Reset();

        var fresh = CreateElement(2010);
        var context = Assert.IsType<MicroBundleContext>(fresh.Context);

        Assert.Equal("Created", context.Phase);
        Assert.Equal(0, context.ElapsedMilliseconds);
        Assert.Null(fresh.Manifestation);
    }

    public void Dispose() => Reset();

    private static MicroBundle CreateElement(int id)
        => new(id, $"Element_{id}", new WebMicroBundleProvider());

    private static void Reset() => FSM_API.Internal.ResetAPI(true);
}
