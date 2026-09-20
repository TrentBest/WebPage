using System.Linq;
using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Verifies that ontology interrogation ends in a real, code-based plan experience.</summary>
public sealed class SpatialAECIntentPlanIntegrationTests
{
    [Fact(DisplayName = "Fictional AEC intent produces a code-based walkable plan")]
    public void FictionalIntent_ProducesPlanExperience()
    {
        using var intent = new SpatialAECIntentModel();

        var answers = new[]
        {
            "FICTION",
            "RESEARCH",
            "FACILITY",
            "LABORATORY",
            "HIGH-TECH LABORATORY",
            "SINGLE BUILDING",
            "SCIENCE",
            "ADVANCED RESEARCH",
            "SCI-FI HIGH-TECH LABORATORY"
        };

        foreach (var answer in answers)
            intent.Answer(answer);

        var plan = intent.CreatePlanExperience();

        Assert.Equal("aec.scenario.fiction.high-tech-research", plan.BuildingType.Id);
        Assert.Equal("SCI-FI-DERIVED-1.0", plan.CodeProfile.Edition);
        Assert.Contains("derived", plan.CodeProfile.Basis, System.StringComparison.OrdinalIgnoreCase);
        Assert.NotEmpty(plan.Floors);
        Assert.All(plan.Floors, floor => Assert.NotEmpty(floor.Rooms));
        Assert.Contains(plan.Interactables, x => x.Id == "protected-core");
    }

    [Fact(DisplayName = "Ontology shopping leaves no fictional plan without a physical basis")]
    public void ScenarioPlans_ResolveToPhysicalSpecifications()
    {
        foreach (var type in SpatialAECOntologyCatalog.ListBuildingTypes()
                     .Where(x => x.Id.StartsWith("aec.scenario.")))
        {
            var plan = SpatialAECPlanExperienceFactory.Create(type);

            Assert.NotEmpty(plan.Specification.Id);
            Assert.NotEmpty(plan.CodeProfile.Basis);
            Assert.NotEmpty(plan.Floors);
        }
    }
}
