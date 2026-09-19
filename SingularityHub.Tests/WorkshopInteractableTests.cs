using System.Collections.Generic;
using TheSingularityWorkshop.Workshop.Interaction;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopInteractableTests
{
    [Fact]
    public void WorkshopInteractable_Implements_Core_Interaction_Contract_And_Declares_Sign()
    {
        var sign = new WorkshopSign(
            "forge-sign",
            "THE FORGE",
            "FSM WORKSHOP",
            "State, lifecycle, and executable machinery.");

        IInteractable interactable = new WorkshopInteractable(
            "forge",
            "The Forge",
            InteractionScope.World,
            new InteractionPoint(10, 20),
            NoOpBehavior.Instance,
            sign,
            triggers: InteractionTrigger.HoverOrClick);

        Assert.IsType<WorkshopInteractable>(interactable);
        Assert.True(((WorkshopInteractable)interactable).HasSign);
        Assert.Equal("THE FORGE", ((WorkshopInteractable)interactable).Sign!.Title);
        Assert.True(interactable.Supports(InteractionTrigger.Click));
    }

    [Fact]
    public void WorkshopSign_Preserves_The_Face_Content()
    {
        var sign = new WorkshopSign(
            "research-sign",
            "RESEARCH LABORATORY",
            "PARTICLE PHYSICS",
            "Chemistry / materials / collider simulation");

        Assert.Equal("research-sign", sign.Id);
        Assert.Equal("RESEARCH LABORATORY", sign.Title);
        Assert.Equal(2, sign.Lines.Length);
        Assert.Equal("PARTICLE PHYSICS", sign.Lines[0]);
    }

    [Fact]
    public void Workshop_And_NonWorkshop_Interactables_Share_The_Same_Executor()
    {
        var workshopBehavior = new RecordingBehavior();
        var genericBehavior = new RecordingBehavior();
        var context = new InteractionContext("world", InteractionScope.World, new HashSet<string>());

        IInteractable workshop = new WorkshopInteractable(
            "sign",
            "Sign",
            InteractionScope.World,
            new InteractionPoint(0, 0),
            workshopBehavior);
        IInteractable generic = new Interactable(
            "excel-cell",
            "Excel Cell",
            InteractionScope.Surface,
            new InteractionPoint(0, 0),
            genericBehavior);

        var executor = new InteractableExecutor();
        Assert.True(executor.TryExecute(workshop, InteractionTrigger.Click, context));
        Assert.True(executor.TryExecute(generic, InteractionTrigger.Click, context));
        Assert.Equal(1, workshopBehavior.ExecutionCount);
        Assert.Equal(1, genericBehavior.ExecutionCount);
    }

    private sealed class NoOpBehavior : IInteractableBehavior
    {
        public static readonly NoOpBehavior Instance = new();
        public void Execute(InteractionContext context) { }
    }

    private sealed class RecordingBehavior : IInteractableBehavior
    {
        public int ExecutionCount { get; private set; }
        public void Execute(InteractionContext context) => ExecutionCount++;
    }
}
