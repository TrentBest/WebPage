using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.Workshop.MicroBundles.AI;

namespace TheSingularityWorkshop.Gui;

public static class WorkshopAiTerminalContext
{
    public static AiContextSnapshot BuildMapSnapshot(SpatialMapScope scope, SpatialWorkshopScene scene)
    {
        var subjectId = ProtocolAi.Identity($"spatial.map.{scope}");
        var contextIds = new[]
        {
            ProtocolAi.Identity("spatial.plan"),
            ProtocolAi.Identity("spatial.navigation"),
            ProtocolAi.Identity("spatial.approval")
        };

        var operations = new[]
        {
            ProtocolAi.Select,
            ProtocolAi.MoveNorth,
            ProtocolAi.MoveSouth,
            ProtocolAi.MoveWest,
            ProtocolAi.MoveEast,
            ProtocolAi.Open,
            ProtocolAi.RequestAecReview,
            ProtocolAi.CopyContext,
            ProtocolAi.ApplyProgram
        };

        return new AiContextSnapshot(
            subjectId,
            contextIds,
            new Dictionary<int, string>
            {
                [contextIds[0]] = scope == SpatialMapScope.Workshop
                    ? "WORKSHOP MASTER PLAN"
                    : $"{scope.ToString().ToUpperInvariant()} // UNDER CONSTRUCTION",
                [contextIds[1]] = "SPATIAL NAVIGATION",
                [contextIds[2]] = "HUMAN APPROVAL BOUNDARY"
            },
            operations,
            operations);
    }

    public static GrammarAiContract BuildMapGrammar(AiContextSnapshot snapshot)
    {
        var rules = snapshot.AvailableOperations
            .Select(operationId => new GrammarAiRule(
                operationId,
                snapshot.ContextIds,
                Array.Empty<int>()))
            .ToArray();

        return new GrammarAiContract(snapshot.SubjectId, rules);
    }

    public static string ForMap(SpatialMapScope scope, SpatialWorkshopScene scene)
    {
        var snapshot = BuildMapSnapshot(scope, scene);
        var title = scope switch
        {
            SpatialMapScope.Workshop => "MAP: WORKSHOP",
            SpatialMapScope.City => "MAP: CITY // UNDER CONSTRUCTION",
            SpatialMapScope.World => "MAP: WORLD // UNDER CONSTRUCTION",
            SpatialMapScope.SolarSystem => "MAP: SOLAR SYSTEM // UNDER CONSTRUCTION",
            _ => "MAP"
        };

        var focus = scope == SpatialMapScope.Workshop
            ? "The visitor is looking at the Workshop campus plan. This is spatial navigation, not a conventional webpage."
            : "The visitor is looking at a future spatial scope. It is intentionally under construction and should be described as such.";

        var destinations = scope == SpatialMapScope.Workshop
            ? string.Join(", ", scene.Interactables.Select(x => x.Name))
            : "the currently exposed future scope";

        var contextLines = string.Join(
            Environment.NewLine,
            snapshot.HumanContext.Select(pair => $"CONTEXT_TEXT[{pair.Key}]={pair.Value}"));

        return $"""
YOU ARE THE WORKSHOP AI TERMINAL.
PROTOCOL={ProtocolAi.ProtocolVersion}
GRAMMAR={GrammarAi.GrammarVersion}
CURRENT VIEW: {title}
{focus}
The visitor can select a building or destination. Selecting a Workshop building closes the map and walks the visitor's avatar to that building's entrance. A second click at the entrance crosses the threshold and enters the building.
The master Workshop map is read-only. Workshop campus geometry can only be proposed from the AEC Remote Office.
CURRENT DESTINATIONS: {destinations}

{contextLines}
SUBJECT={snapshot.SubjectId}
CONTEXT={string.Join(",", snapshot.ContextIds)}
OPERATIONS={string.Join(",", snapshot.AvailableOperations)}
TOKENS={string.Join(",", snapshot.AllowedProgramTokens)}

When helping the visitor, explain what they are looking at first, then describe the available actions. Do not invent capabilities that are not represented by the current context.
EXECUTABLE PROGRAM OUTPUT MUST USE PROTOCOL INTEGER IDS, NOT STRING COMMAND NAMES.
SAFE AUTOMATIC OPERATIONS: 1002 MOVE NORTH, 1003 MOVE SOUTH, 1004 MOVE WEST, 1005 MOVE EAST.
APPROVAL-DEPENDENT OPERATIONS: 1001 SELECT, 1006 OPEN, 1007 REQUEST AEC REVIEW, 1009 APPLY PROGRAM.
""";
    }
}
