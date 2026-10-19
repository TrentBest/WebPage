using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class ExperienceArtifactUriResolverTests
{
    [Fact]
    public void DefaultResolverKeepsManifestPathRelative()
    {
        var resolver = new ExperienceArtifactUriResolver();

        var resolved = resolver.Resolve("Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json");

        Assert.False(resolved.IsAbsoluteUri);
        Assert.Equal("Workshop/Forge/StreamingAssets/Experiences/webpage-host.manifest.json", resolved.OriginalString);
    }

    [Fact]
    public void ConfiguredHttpsBaseResolvesVersionedExperienceManifest()
    {
        var resolver = new ExperienceArtifactUriResolver(
            new Uri("https://example.blob.core.windows.net/experiences"));

        var resolved = resolver.Resolve("singularity-city/1.0.0/manifest.json");

        Assert.Equal(
            "https://example.blob.core.windows.net/experiences/singularity-city/1.0.0/manifest.json",
            resolved.AbsoluteUri);
    }

    [Theory]
    [InlineData("../manifest.json")]
    [InlineData("city/../manifest.json")]
    [InlineData("/manifest.json")]
    [InlineData("https://evil.example/manifest.json")]
    [InlineData("city/%2e%2e/manifest.json")]
    [InlineData("city/manifest.json?next=other")]
    [InlineData("city//manifest.json")]
    public void RejectsUnsafeManifestPaths(string path)
    {
        var resolver = new ExperienceArtifactUriResolver(new Uri("https://example.blob.core.windows.net/"));

        Assert.Throws<ArgumentException>(() => resolver.Resolve(path));
    }

    [Theory]
    [InlineData("http://example.blob.core.windows.net/")]
    [InlineData("https://user:secret@example.blob.core.windows.net/")]
    [InlineData("https://example.blob.core.windows.net/?token=secret")]
    [InlineData("https://example.blob.core.windows.net/#fragment")]
    public void RejectsUnsafeArtifactBaseUris(string uri)
    {
        Assert.Throws<ArgumentException>(() => new ExperienceArtifactUriResolver(new Uri(uri)));
    }
}
