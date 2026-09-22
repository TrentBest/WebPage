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
            ProtocolAi.Identity("spatial.plan"),
            ProtocolAi.Identity("spatial.position"),
            ProtocolAi.Identity("spatial.footprint")
        };

        var operations = new[]
        {
            ProtocolAi.Select,
            ProtocolAi.MoveNorth,
            ProtocolAi.MoveSouth,
            ProtocolAi.MoveWest,
            ProtocolAi.MoveEast,
            ProtocolAi.Open,
            ProtocolAi.RequestAecReview
        };

        var allowed = new[]
        {
            ProtocolAi.MoveNorth,
            ProtocolAi.MoveSouth,
            ProtocolAi.MoveWest,
            ProtocolAi.MoveEast
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
                [ProtocolAi.Identity("spatial.plan")] = "WORKSHOP MASTER PLAN",
                [ProtocolAi.Identity("spatial.position")] = $"X={bounds.X:0.##},Y={bounds.Y:0.##}",
                [ProtocolAi.Identity("spatial.footprint")] = $"W={bounds.Width:0.##},H={bounds.Height:0.##}"
            }
        };
    }
}
