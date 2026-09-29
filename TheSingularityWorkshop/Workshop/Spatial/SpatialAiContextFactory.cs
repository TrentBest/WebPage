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
    private const string ProtocolType = "TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi";

    public static IAiInteractable Create(SpatialInteractable item, SpatialBounds bounds)
    {
        var contextIds = new[]
        {
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Identity("spatial.plan"),
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Identity("spatial.position"),
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Identity("spatial.footprint")
        };

        var operations = new[]
        {
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Select,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveNorth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveSouth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveWest,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveEast,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Open,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.RequestAecReview
        };

        var allowed = new[]
        {
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveNorth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveSouth,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveWest,
            TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.MoveEast
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
                [TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Identity("spatial.plan")] = "WORKSHOP MASTER PLAN",
                [TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Identity("spatial.position")] = $"X={bounds.X:0.##},Y={bounds.Y:0.##}",
                [TheSingularityWorkshop.Workshop.MicroBundles.AI.ProtocolAi.Identity("spatial.footprint")] = $"W={bounds.Width:0.##},H={bounds.Height:0.##}"
            }
        };
    }
}
