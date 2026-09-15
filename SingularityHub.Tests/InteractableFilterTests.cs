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

    private sealed class RecordingBehavior : IInteractableBehavior
    {
        public void Execute(InteractionContext context) { }
    }
}
