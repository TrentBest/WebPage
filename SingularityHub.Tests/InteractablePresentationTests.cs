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

    private sealed class NoOpBehavior : IInteractableBehavior
    {
        public void Execute(InteractionContext context) { }
    }
}
