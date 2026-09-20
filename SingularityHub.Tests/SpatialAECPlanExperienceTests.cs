using System;
using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Verifies that selected AEC inventory becomes a walkable plan experience.</summary>
public sealed class SpatialAECPlanExperienceTests
{
    [Fact(DisplayName = "Fictional research building receives an explicit derived code basis")]
    public void ResearchBuilding_ReceivesFictionalDerivedCode()
    {
        var type = SpatialAECOntologyCatalog.ListBuildingTypes()
            .Single(x => x.Id == "aec.scenario.fiction.high-tech-research");

        var experience = SpatialAECPlanExperienceFactory.Create(type);

        Assert.Contains("derived from", experience.CodeProfile.Basis, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("SCI-FI-DERIVED-1.0", experience.CodeProfile.Edition);
        Assert.NotEmpty(experience.Floors);
        Assert.All(experience.Floors, floor => Assert.NotEmpty(floor.Interactables));
    }

    [Fact(DisplayName = "Every canonical building type has a default walkable plan")]
    public void CanonicalBuildings_HaveDefaultPlans()
    {
        foreach (var type in SpatialAECOntologyCatalog.ListBuildingTypes()
                     .Where(x => !x.Id.StartsWith("aec.scenario.", StringComparison.Ordinal)))
        {
            var experience = SpatialAECPlanExperienceFactory.Create(type);

            Assert.NotEmpty(experience.Floors);
            Assert.NotEmpty(experience.Interactables);
            Assert.Equal(type.Id, experience.BuildingType.Id);
        }
    }

    [Fact(DisplayName = "Plan interactables expose capabilities rather than renderer-specific behavior")]
    public void Interactables_AreSemantic()
    {
        var type = SpatialAECOntologyCatalog.ListBuildingTypes()
            .Single(x => x.Id == "aec.scenario.fiction.high-tech-research");

        var experience = SpatialAECPlanExperienceFactory.Create(type);

        Assert.Contains(experience.Interactables, x =>
            x.Id == "program-capability" &&
            x.Capability.Contains("configured for", StringComparison.OrdinalIgnoreCase));

        Assert.Contains(experience.Interactables, x =>
            x.Id == "protected-core" &&
            x.Capability.Contains("protected circulation", StringComparison.OrdinalIgnoreCase));
    }
}
