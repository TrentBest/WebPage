using System.Collections.Generic;
using TheSingularityWorkshop.Workshop.Interaction;
using Xunit;

namespace SingularityHub.Tests;

public sealed class InteractableFilterTests
{
    [Fact]
    public void Filter_Exposes_Only_The_Runnable_Current_Surface()
    {
        var desk = new Interactable(
            "desk",
            "Research Desk",
            InteractionScope.Surface,
            new InteractionPoint(20, 30),
            new RecordingBehavior());

        var notebook = new Interactable(
            "notebook",
            "Notebook",
            InteractionScope.Surface,
            new InteractionPoint(21, 30),
            new RecordingBehavior(),
            new SurfaceScopeFilter());

        var drawer = new Interactable(
            "drawer-a",
            "Drawer A",
            InteractionScope.Container,
            new InteractionPoint(20, 31),
            new RecordingBehavior(),
            new SurfaceScopeFilter());

        desk.AddChild(notebook).AddChild(drawer);

        var context = new InteractionContext("desk", InteractionScope.Surface, new HashSet<string>());
        var runnable = new InteractableFilter().GetRunnable(desk.Children, context);

        Assert.Single(runnable);
        Assert.Equal("notebook", runnable[0].Id);
    }

    [Fact]
    public void Drawer_Contents_Are_Not_Runnable_Until_The_Drawer_Is_Open()
    {
        var drawer = new Interactable(
            "drawer-a",
            "Drawer A",
            InteractionScope.Container,
            new InteractionPoint(20, 31),
            new RecordingBehavior());

        var reagent = new Interactable(
            "reagent-x",
            "Reagent X",
            InteractionScope.Contents,
            new InteractionPoint(20, 31),
            new RecordingBehavior(),
            new OpenContainerFilter());
        drawer.AddChild(reagent);

        var filter = new InteractableFilter();
        var closed = filter.GetRunnable(
            drawer.Children,
            new InteractionContext("drawer-a", InteractionScope.Contents, new HashSet<string>()));
        var open = filter.GetRunnable(
            drawer.Children,
            new InteractionContext("drawer-a", InteractionScope.Contents, new HashSet<string> { "drawer-a" }));

        Assert.Empty(closed);
        Assert.Single(open);
        Assert.Equal("reagent-x", open[0].Id);
    }

    [Fact]
    public void Interactable_Supports_Hover_And_Click_Independently()
    {
        var hoverOnly = new Interactable(
            "hover",
            "Hover Action",
            InteractionScope.World,
            new InteractionPoint(10, 10),
            new RecordingBehavior(),
            triggers: InteractionTrigger.Hover);
        var both = new Interactable(
            "both",
            "Hover Or Click",
            InteractionScope.World,
            new InteractionPoint(10, 10),
            new RecordingBehavior(),
            triggers: InteractionTrigger.HoverOrClick);

        Assert.True(hoverOnly.Supports(InteractionTrigger.Hover));
        Assert.False(hoverOnly.Supports(InteractionTrigger.Click));
        Assert.True(both.Supports(InteractionTrigger.Hover));
        Assert.True(both.Supports(InteractionTrigger.Click));
    }

    [Fact]
    public void InteractableExecutor_Runs_Behavior_Only_For_Supported_Eligible_Trigger()
    {
        var behavior = new RecordingBehavior();
        var interactable = new Interactable(
            "sample",
            "Sample",
            InteractionScope.World,
            new InteractionPoint(10, 10),
            behavior,
            triggers: InteractionTrigger.HoverOrClick);
        var context = new InteractionContext("world", InteractionScope.World, new HashSet<string>());

        var executor = new InteractableExecutor();

        Assert.True(executor.TryExecute(interactable, InteractionTrigger.Hover, context));
        Assert.Equal(1, behavior.ExecutionCount);
        Assert.True(executor.TryExecute(interactable, InteractionTrigger.Click, context));
        Assert.Equal(2, behavior.ExecutionCount);
    }

    private sealed class RecordingBehavior : IInteractableBehavior
    {
        public int ExecutionCount { get; private set; }

        public void Execute(InteractionContext context) => ExecutionCount++;
    }
}
