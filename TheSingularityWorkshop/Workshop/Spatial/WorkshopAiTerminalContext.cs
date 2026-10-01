using System;
using System.Linq;

namespace TheSingularityWorkshop.Gui;

public static class WorkshopAiTerminalContext
{
    public static string ForMap(SpatialMapScope scope, SpatialWorkshopScene scene)
    {
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

        return $"""
YOU ARE THE WORKSHOP AI TERMINAL.
CURRENT VIEW: {title}
{focus}
The visitor can select a building or destination. Selecting a Workshop building closes the map and walks the visitor's avatar to that building's entrance. A second click at the entrance crosses the threshold and enters the building.
The master Workshop map is read-only. Workshop campus geometry can only be proposed from the AEC Remote Office.
CURRENT DESTINATIONS: {destinations}

When helping the visitor, explain what they are looking at first, then describe the available actions. Do not invent capabilities that are not represented by the current context.
EXECUTABLE PROGRAM OUTPUT MUST USE PROTOCOL INTEGER IDS, NOT STRING COMMAND NAMES.
AVAILABLE MAP OPERATIONS: 1001 SELECT, 1002 MOVE NORTH, 1003 MOVE SOUTH, 1004 MOVE WEST, 1005 MOVE EAST, 1006 OPEN, 1007 REQUEST AEC REVIEW, 1008 COPY CONTEXT, 1009 APPLY PROGRAM.
""";
    }
}
