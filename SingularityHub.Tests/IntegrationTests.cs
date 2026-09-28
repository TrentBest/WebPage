using HubBundle = TheSingularityWorkshop.SingularityHub.IMicroBundle;
using HubKernel = TheSingularityWorkshop.SingularityHub.SingularityHub;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Cross-domain tests. These deliberately reveal where the concrete Workshop
/// Element has not yet become a Hub-level MicroBundle contract implementation.
/// </summary>
public sealed class IntegrationTests
{
    [ArchitectureTest(4, 1, 1)]
    [Fact(DisplayName = "4.01.001 — Workshop_Element_Is_A_MicroBundle_Domain_Object")]
    public void Workshop_Element_Is_A_MicroBundle_Domain_Object()
    {
        var element = new MicroBundle(4001, "Integration", new WebMicroBundleProvider());
        Assert.IsAssignableFrom<TheSingularityWorkshop.Workshop.MicroBundles.IMicroBundle>(element);
    }

    [ArchitectureTest(4, 1, 2)]
    [Fact(DisplayName = "4.01.002 — Workshop_Element_Can_Be_Recognized_By_Hub_Contract")]
    public void Workshop_Element_Can_Be_Recognized_By_Hub_Contract()
    {
        var element = new MicroBundle(4002, "HubElement", new WebMicroBundleProvider());
        Assert.IsAssignableFrom<HubBundle>(element);
    }

    [ArchitectureTest(4, 1, 3)]
    [Fact(DisplayName = "4.01.003 — Element_Can_Be_Loaded_By_Hub")]
    public void Element_Can_Be_Loaded_By_Hub()
    {
        var element = new MicroBundle(4003, "Loadable", new WebMicroBundleProvider());
        var bundle = Assert.IsAssignableFrom<HubBundle>(element);
        var hub = new HubKernel();
        Assert.True(hub.LoadBundle(bundle));
    }

    [ArchitectureTest(4, 1, 4)]
    [Fact(DisplayName = "4.01.004 — Element_Has_Hub_Identity")]
    public void Element_Has_Hub_Identity()
    {
        var element = new MicroBundle(4004, "Identity", new WebMicroBundleProvider());
        var bundle = Assert.IsAssignableFrom<HubBundle>(element);
        Assert.Equal((ulong)4004, bundle.Id);
    }
}
