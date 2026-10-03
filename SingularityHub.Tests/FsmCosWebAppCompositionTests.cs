using TheSingularityWorkshop.Infrastructure.FsmCos;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Executable proof that the WebApp vertical slice composes through FSM_COS
/// rather than constructing its selected MicroBundle directly.
/// </summary>
public sealed class FsmCosWebAppCompositionTests
{
    [Fact]
    public void WebPageCatalogResolvesPongThroughFsmCos()
    {
        var catalog = new WebPageMicroBundleCatalog();
        var cos = new FsmCos(catalog);

        var assembly = cos.Execute(
            new RuntimeManifest(
                1,
                new[] { MicroBundleDependencyRequest.Unconfigured(PongMicroBundle.BundleId) }));

        Assert.Single(assembly.Bundles);
        Assert.True(assembly.TryGetBundle<PongMicroBundle>(
            PongMicroBundle.BundleId,
            out var pong));
        Assert.NotNull(pong);
        Assert.Equal((ulong)PongMicroBundle.BundleId, pong!.Id);
        Assert.Equal((ulong)PongMicroBundle.BundleId, pong.Descriptor.Id);
    }
}
