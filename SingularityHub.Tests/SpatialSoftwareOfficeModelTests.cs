using TheSingularityWorkshop.Workshop.Spatial;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialSoftwareOfficeModelTests
{
    [Fact]
    public void AutoNamesRemainUniqueWithoutPersistence()
    {
        var existing = new[] { "Class", "Class 2" };

        var first = SpatialUniqueNameAllocator.Allocate(null, existing, "Class");
        var second = SpatialUniqueNameAllocator.Allocate(null, [.. existing, first], "Class");

        Assert.Equal("Class 3", first);
        Assert.Equal("Class 4", second);
    }

    [Fact]
    public void RenamePreservesUniquenessAgainstSiblingObjects()
    {
        var renamed = SpatialUniqueNameAllocator.EnsureUnique(
            "Order",
            ["Order", "Order 2", "Customer"],
            "Customer");

        Assert.Equal("Order 3", renamed);
    }

    [Fact]
    public void ElevatorSupportsDirectButtonsDigitSequencesAndDirectoryPaging()
    {
        var elevator = new SpatialElevatorControlModel(
            Enumerable.Range(1, 125)
                .Select(floor => new SpatialElevatorFloor(floor, $"Floor {floor}", $"Department {floor}")));

        Assert.Equal(Enumerable.Range(1, 9), elevator.DirectFloorButtons);
        Assert.True(elevator.UsesDigitalDisplay);
        Assert.True(elevator.TrySelectDigitSequence([1, 2, 4]));
        Assert.Equal(124, elevator.SelectedFloor);
        Assert.Equal("124", elevator.Display);

        elevator.ShowDirectoryPage(2, 12);
        Assert.Equal(12, elevator.VisibleFloorDirectory.Count);
        Assert.Equal(25, elevator.VisibleFloorDirectory[0].Number);
        Assert.Equal(36, elevator.VisibleFloorDirectory[^1].Number);
    }

    [Fact]
    public void OfficeGrowsFromArtifactWithoutDuplicatingFloors()
    {
        var office = new SpatialSoftwareOfficeModel();
        var initialCount = office.Floors.Count;

        office.Artifact.Properties.Add("DisplayName");
        office.Artifact.Fields.Add("_state");
        office.Artifact.Methods.Add("Update");
        office.Artifact.CoreTypes.Add("string");
        office.Artifact.ExternalDependencies.Add("TheSingularityWorkshop.FSM_API");
        office.Artifact.Diagrams.Add(SpatialCodeDiagramKind.Class);
        office.Artifact.Diagrams.Add(SpatialCodeDiagramKind.Sequence);
        office.Artifact.IsExecutable = true;
        office.Artifact.IsDocumentable = true;
        office.Artifact.IsPublishable = true;

        office.SynchronizeFromArtifact();
        office.SynchronizeFromArtifact();

        Assert.True(office.Floors.Count > initialCount);
        Assert.Equal(1, office.Floors.Count(floor => floor.Name == "Class Diagram"));
        Assert.Equal(1, office.Floors.Count(floor => floor.Name == "Sequence Analysis"));
        Assert.Equal(SpatialOfficeEntanglement.Entangled, office.Entanglement);
    }

    [Fact]
    public void ClassDiagramDataIsFocusedOnSelectedSourceAndItsDependencies()
    {
        var office = new SpatialSoftwareOfficeModel();
        office.Artifact.SourcePath = "Example.cs";
        office.Artifact.DeclaredTypeName = "Example";
        office.Artifact.CoreTypes.Add("string");
        office.Artifact.CoreTypes.Add("bool");
        office.Artifact.ExternalDependencies.Add("TheSingularityWorkshop.FSM_API");
        office.Artifact.ExternalDependencies.Add("SingularityHub");

        office.SynchronizeFromArtifact();

        Assert.Contains("string", office.Artifact.CoreTypes);
        Assert.Contains("bool", office.Artifact.CoreTypes);
        Assert.Contains("TheSingularityWorkshop.FSM_API", office.Artifact.ExternalDependencies);
        Assert.Contains(office.Floors, floor => floor.Name == "Architecture");
    }
}

    
    [Fact]
    public void CodeDiagramKeepsSelectedFileAtTheCenterAndOnlyAddsUsedEdges()
    {
        var diagram = new SpatialCodeDiagramModel("Example.cs", "Example");
        diagram.ClassCard.Name = "Example";
        diagram.ClassCard.PublicProperties.Add("DisplayName");
        diagram.AddCoreType("string")
            .AddCoreType("bool")
            .AddDependency("TheSingularityWorkshop.FSM_API")
            .AddMethodSequence("Update", ["Read", "Transition", "Render"], "O(n)");

        Assert.Equal("Example.cs", diagram.SourcePath);
        Assert.Equal("Example", diagram.ClassCard.Name);
        Assert.Equal(2, diagram.CoreTypeCards.Count);
        Assert.Single(diagram.DependencyCards);
        Assert.Single(diagram.Sequence);
        Assert.Equal("O(n)", diagram.Complexity[0].BigO);
    }

    [Fact]
    public void OfficeTrafficCanModelAHighGameTesterSmokeBreakPopulation()
    {
        var traffic = new SpatialOfficeTrafficModel();

        for (var index = 0; index < 10; index++)
        {
            traffic.AddEmployee(new SpatialOfficeEmployee(
                $"tester-{index}",
                $"Tester {index}",
                "Game Tester",
                "Play Test",
                TakesSmokeBreaks: index < 9));
        }

        Assert.Equal(9, traffic.SmokeBreakGroup.Count);
        Assert.Equal(9, traffic.ExpectedSmokeBreakPopulation);
    }
