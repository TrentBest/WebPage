using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialGenerativePopulationTests
{
    [Fact(DisplayName = "Incremental Unit Test 43 — Identical seeds reproduce identical populations")]
    public void IdenticalSeedsReproduceIdenticalPopulations()
    {
        var firstManifest = new SpatialPopulationManifest("ontology-east", new SpatialBounds(0, 0, 1000, 1000), 1, new SpatialSeed("ontology-v1"));
        var secondManifest = new SpatialPopulationManifest("ontology-east", new SpatialBounds(0, 0, 1000, 1000), 1, new SpatialSeed("ontology-v1"));
        var region = new SpatialBounds(100, 100, 100, 100);

        var first = SpatialSeededPopulationGenerator.Generate(firstManifest, region);
        var second = SpatialSeededPopulationGenerator.Generate(secondManifest, region);

        Assert.Equal(first, second);
        Assert.NotEmpty(first);
    }

    [Fact(DisplayName = "Incremental Unit Test 44 — Seed changes alter generated composition")]
    public void SeedChangesAlterGeneratedComposition()
    {
        var first = new SpatialPopulationManifest("mall", new SpatialBounds(0, 0, 300, 300), 1, new SpatialSeed("seed-a"));
        var second = new SpatialPopulationManifest("mall", new SpatialBounds(0, 0, 300, 300), 1, new SpatialSeed("seed-b"));
        var region = new SpatialBounds(0, 0, 300, 300);

        var firstObjects = SpatialSeededPopulationGenerator.Generate(first, region);
        var secondObjects = SpatialSeededPopulationGenerator.Generate(second, region);

        Assert.False(firstObjects.SequenceEqual(secondObjects));
    }

    [Fact(DisplayName = "Incremental Unit Test 45 — Generated interactables choose their own presentation")]
    public void GeneratedObjectsCarryPresentationMode()
    {
        var manifest = new SpatialPopulationManifest("gallery", new SpatialBounds(0, 0, 100, 100), 2, new SpatialSeed("gallery-v1"));

        var objects = SpatialSeededPopulationGenerator.Generate(manifest, new SpatialBounds(0, 0, 100, 100));

        Assert.NotEmpty(objects);
        Assert.All(objects, item => Assert.Contains(item.Presentation, new[]
        {
            SpatialPresentationMode.Popup,
            SpatialPresentationMode.Takeover,
            SpatialPresentationMode.Inline
        }));
    }

    [Fact(DisplayName = "Incremental Unit Test 46 — Functionality objects expose collectible capabilities")]
    public void FunctionalityObjectsExposeCapabilities()
    {
        var manifest = new SpatialPopulationManifest("utility", new SpatialBounds(0, 0, 500, 500), 3, new SpatialSeed("utility-v1"));

        var objects = SpatialSeededPopulationGenerator.Generate(manifest, new SpatialBounds(0, 0, 500, 500));
        var functionality = objects.FirstOrDefault(item => item.Kind == SpatialGeneratedObjectKind.Functionality);

        Assert.NotEqual(default, functionality);
        Assert.False(string.IsNullOrWhiteSpace(functionality.CapabilityId));
    }

    [Fact(DisplayName = "Incremental Unit Test 47 — Vast populations can be queried by region")]
    public void VastPopulationIsGeneratedByRegion()
    {
        var manifest = new SpatialPopulationManifest("city", new SpatialBounds(-100000, -100000, 200000, 200000), 1, new SpatialSeed("city-v1"));
        var region = new SpatialBounds(5000, 5000, 100, 100);

        var objects = SpatialSeededPopulationGenerator.Generate(manifest, region);

        Assert.NotEmpty(objects);
        Assert.All(objects, item => Assert.True(region.Contains(item.Position)));
    }
}
