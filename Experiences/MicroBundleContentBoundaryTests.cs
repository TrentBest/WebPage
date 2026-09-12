using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace Experiences;

public sealed class MicroBundleContentBoundaryTests
{
    [Fact(DisplayName = "External content enters without an environment or tooling")]
    public void ExternalContent_Is_Pure_Data_Before_Wrapping()
    {
        var content = new ExternalContent(7, "application/octet-stream", "external://7");

        Assert.Equal(ContentOrigin.External, content.Origin);
        Assert.Equal(7UL, content.ContentId);
        Assert.Equal("application/octet-stream", content.MediaType);
        Assert.Equal("external://7", content.PayloadReference);
    }

    [Fact(DisplayName = "Wrapping external content introduces MicroBundle tooling")]
    public void MicroBundleEnvelope_Wraps_Content_And_Exposes_Providers()
    {
        var bundle = new MicroBundleEnvelope(
            BundleId: 42,
            ContentId: 7,
            Layer: MicroBundleLayer.AuthoringBundle,
            ProviderIds: [101, 102, 103],
            ChildBundleIds: [201, 202]);

        Assert.Equal(ContentOrigin.MicroBundle, bundle.Origin);
        Assert.True(bundle.IsWrappedExternalContent);
        Assert.True(bundle.HasTooling);
        Assert.False(bundle.IsConceptualRoot);
    }

    [Fact(DisplayName = "The onion sheds tooling toward the runtime leaf")]
    public void MicroBundleLayer_Preserves_Development_To_Runtime_Onion()
    {
        Assert.True(MicroBundleLayer.DevelopmentBundle > MicroBundleLayer.AuthoringBundle);
        Assert.True(MicroBundleLayer.AuthoringBundle > MicroBundleLayer.RuntimeBundle);
        Assert.True(MicroBundleLayer.RuntimeBundle > MicroBundleLayer.RuntimeLeaf);
    }

    [Fact(DisplayName = "A conceptual root can own the complete tooling onion")]
    public void ConceptualRoot_Can_Represent_The_Complete_Onion()
    {
        var root = new MicroBundleEnvelope(
            BundleId: 9000,
            ContentId: 0,
            Layer: MicroBundleLayer.ConceptualRoot,
            ProviderIds: [1, 2, 3, 4],
            ChildBundleIds: [42]);

        Assert.True(root.IsConceptualRoot);
        Assert.False(root.IsWrappedExternalContent);
        Assert.True(root.HasTooling);
    }
}
