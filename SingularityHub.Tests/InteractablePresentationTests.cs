using TheSingularityWorkshop.Workshop.Interaction;
using Xunit;

namespace SingularityHub.Tests;

public sealed class InteractablePresentationTests
{
    [Fact]
    public void Interactable_Uses_Default_Presentation_When_None_Is_Supplied()
    {
        var interactable = new Interactable(
            "sample",
            "Sample",
            InteractionScope.World,
            new InteractionPoint(10, 10),
            new NoOpBehavior());

        Assert.Equal("INTERACTABLE", interactable.Presentation.Kicker);
        Assert.Equal("Sample", interactable.Presentation.Description);
        Assert.Equal("default", interactable.Presentation.SchematicId);
        Assert.Equal("SAMPLE", interactable.Presentation.MapIntent.ThreadLabel);
        Assert.Equal("#ffffff", interactable.Presentation.MapIntent.Accent);
    }

    [Fact]
    public void Interactable_Preserves_Composable_Presentation_Intent()
    {
        var presentation = new InteractablePresentation(
            "RESEARCH FACILITY",
            "A place for synthesis and characterization.",
            "research-facility");

        var interactable = new Interactable(
            "chemistry",
            "Chemistry Lab",
            InteractionScope.World,
            new InteractionPoint(50, 50),
            new NoOpBehavior(),
            triggers: InteractionTrigger.HoverOrClick,
            presentation: presentation);

        Assert.Same(presentation, interactable.Presentation);
        Assert.Equal("RESEARCH FACILITY", interactable.Presentation.Kicker);
        Assert.Equal("research-facility", interactable.Presentation.SchematicId);
    }

    [Fact]
    public void Interactable_Presentation_Can_Declare_Map_Thread_Without_Choosing_A_Renderer()
    {
        var map = new InteractableMapPresentation(
            "ORBITAL",
            "#ff9f43",
            LineWidth: 1.7,
            LineOpacity: .72);

        var presentation = new InteractablePresentation(
            "SPACE ELEVATOR",
            "A vertical road into orbit.",
            "space-elevator",
            Map: map);

        var interactable = new Interactable(
            "space-elevator",
            "Space Elevator",
            InteractionScope.World,
            new InteractionPoint(86, 68),
            new NoOpBehavior(),
            presentation: presentation);

        Assert.Same(map, interactable.Presentation.MapIntent);
        Assert.Equal("ORBITAL", interactable.Presentation.MapIntent.ThreadLabel);
        Assert.Equal("#ff9f43", interactable.Presentation.MapIntent.Accent);
        Assert.Equal(1.7, interactable.Presentation.MapIntent.LineWidth);
        Assert.Equal(.72, interactable.Presentation.MapIntent.LineOpacity);
    }

    private sealed class NoOpBehavior : IInteractableBehavior
    {
        public void Execute(InteractionContext context) { }
    }
}
