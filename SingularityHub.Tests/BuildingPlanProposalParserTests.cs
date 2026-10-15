using TheSingularityWorkshop.Workshop.Architecture;
using Xunit;

namespace SingularityHub.Tests;

public sealed class BuildingPlanProposalParserTests
{
    [Fact]
    public void ParsesAndValidatesACompletePlanProposal()
    {
        const string json = """
        {
          "kind": "building-plan-proposal",
          "plan": {
            "id": "candidate.house",
            "name": "Candidate House",
            "width": 10,
            "height": 10,
            "rooms": [
              { "id": "entry", "name": "Entry", "x": 0, "y": 0, "width": 3, "height": 3, "use": "Entry" }
            ],
            "walls": [
              { "id": "north", "x1": 0, "y1": 0, "x2": 10, "y2": 0, "thickness": 0.12 }
            ],
            "openings": [
              { "id": "front-door", "wallId": "north", "offset": 1, "width": 1, "kind": "HingedDoor", "hinge": "Start", "swing": "IntoRoom", "openFraction": 1 }
            ],
            "fixtures": []
          }
        }
        """;

        var result = BuildingPlanProposalParser.Parse(json);

        Assert.True(result.IsValid, result.Message);
        Assert.NotNull(result.Plan);
        Assert.Equal("candidate.house", result.Plan!.Id);
        Assert.Single(result.Plan.Rooms);
        Assert.Single(result.Plan.Openings);
    }

    [Fact]
    public void RejectsProposalWithAnOpeningThatReferencesAMissingWall()
    {
        const string json = """
        {
          "kind": "building-plan-proposal",
          "plan": {
            "id": "candidate.house",
            "name": "Candidate House",
            "width": 10,
            "height": 10,
            "rooms": [],
            "walls": [],
            "openings": [
              { "id": "front-door", "wallId": "missing", "offset": 1, "width": 1, "kind": "HingedDoor" }
            ],
            "fixtures": []
          }
        }
        """;

        var result = BuildingPlanProposalParser.Parse(json);

        Assert.False(result.IsValid);
        Assert.Null(result.Plan);
        Assert.Contains("missing wall", result.Message);
        Assert.Contains("working plan was not changed", result.Message);
    }

    [Fact]
    public void RejectsWrongProposalKindAndMalformedJson()
    {
        var wrongKind = BuildingPlanProposalParser.Parse("""{"kind":"other","plan":{}}""");
        var malformed = BuildingPlanProposalParser.Parse("{ not json");

        Assert.False(wrongKind.IsValid);
        Assert.Null(wrongKind.Plan);
        Assert.False(malformed.IsValid);
        Assert.Null(malformed.Plan);
    }
}
