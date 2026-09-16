using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.Interaction;
using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class ExperienceInteractionFlowTests
{
    [Fact]
    public void FloorPlanView_IsReturnedThroughGenericViewContract()
    {
        var space = new ExperienceSpace(
            "spaceship", "Singularity Spaceship Hangar", "VESSEL OPERATIONS",
            48, 4, 60, 15, 60, 15,
            "A hangar.", "VESSEL", "Cockpit", 30, 12);
        var state = new ExperienceViewState();

        state.SetView(new FloorPlan(space));

        var view = state.GetView<FloorPlan>();
        Assert.NotNull(view);
        Assert.Equal("spaceship", view.ExperienceId);
        Assert.Same(space, view.Space);
    }

    [Fact]
    public void InteractableHover_ExecutesHoverBehaviorWithoutEnteringExperience()
    {
        var space = new ExperienceSpace(
            "spaceship", "Singularity Spaceship Hangar", "VESSEL OPERATIONS",
            48, 4, 60, 15, 60, 15,
            "A hangar.", "VESSEL", "Cockpit", 30, 12);
        var hovered = false;
        var clicked = false;
        var interactable = WorkshopInteractableCatalog.Create(
            space,
            _ => clicked = true,
            _ => hovered = true);
        var context = new InteractionContext(
            "workshop-floor",
            InteractionScope.World,
            new HashSet<string>());
        var executor = new InteractableExecutor();

        Assert.True(executor.TryExecute(interactable, InteractionTrigger.Hover, context));
        Assert.True(hovered);
        Assert.False(clicked);

        Assert.True(executor.TryExecute(interactable, InteractionTrigger.Click, context));
        Assert.True(clicked);
    }

    [Fact]
    public void Pathfinding_ExposesTheActualWalkableInteractionDestination()
    {
        using var pathfinding = new PathfindingMicroBundle();
        var space = new ExperienceSpace(
            "engineering", "The Forge", "FSM WORKSHOP",
            66, 18, 78, 40, 78, 40,
            "Forge.", "FORGE", "FSM", 24, 18);

        Assert.True(pathfinding.TryPlan(
            50, 50,
            space.EntranceX,
            space.EntranceY,
            new[] { space.Footprint }));

        Assert.Equal(space.EntranceX, pathfinding.ResolvedTarget.X);
        Assert.Equal(space.EntranceY, pathfinding.ResolvedTarget.Y);
    }
}
