using System.Collections.Generic;
using TheSingularityWorkshop.Workshop.MicroBundles.AI;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Creates the AI contract for a spatial object without coupling the object to an
/// LLM provider. The resulting context is deliberately narrow: identity, current
/// spatial context, and the operations available in the current plan.
/// </summary>
public static class SpatialAiContextFactory
{
    public static IAiInteractable Create(SpatialInteractable item, SpatialBounds bounds)
    {
        var contextIds = new[]
        {
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Identity("spatial.plan"),
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Identity("spatial.position"),
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Identity("spatial.footprint")
        };

        var operations = new[]
        {
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Select,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveNorth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveSouth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveWest,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveEast,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Open,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.RequestAecReview
        };

        var allowed = new[]
        {
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveNorth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveSouth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveWest,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.MoveEast
        };

        return new SpatialAiInteractable(
            $"spatial.interactable.{item.Id}",
            operations,
            contextIds,
            allowed,
            program => program);
    }

    public static AiContextSnapshot BuildContext(SpatialInteractable item, SpatialBounds bounds)
    {
        var interactable = Create(item, bounds);
        var context = interactable.BuildAiContext();

        return context with
        {
            HumanContext = new Dictionary<int, string>
            {
                [TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Identity("spatial.plan")] = "WORKSHOP MASTER PLAN",
                [TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Identity("spatial.position")] = $"X={bounds.X:0.##},Y={bounds.Y:0.##}",
                [TheSingularityWorkshop.Workshop.MicroBundles.AI.WorkshopAiProtocol.Identity("spatial.footprint")] = $"W={bounds.Width:0.##},H={bounds.Height:0.##}"
            }
        };
    }
}
