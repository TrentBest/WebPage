using Microsoft.AspNetCore.Components;
using TheSingularityWorkshop.Workshop.Architecture;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Presents the Architectural Shop as a drafting table rather than a form.
/// The floor plan is drawn as line-work/SVG primitives through the fluent GUI
/// builder. The editor owns the architectural data; this builder only manifests it.
/// </summary>
public static class ArchitecturalShopGuiBuilder
{
    private const string Cyan = "#00eaff";
    private const string Magenta = "#ff2cff";
    private const string Green = "#52e05a";
    private const string White = "#ffffff";
    private const string Muted = "#6e8993";
    private const string Ink = "#01040a";

    public static ElementBuilder Build(
        object receiver,
        ArchitecturalShopEditor editor,
        Action<ArchitecturalCellKind> selectTool,
        Action<int, int> applyCell,
        Action<ArchitecturalFloorPlan> loadTemplate,
        Action capture,
        Action exitShop)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "relative")
            .Style("height", "100vh")
            .Style("min-height", "0")
            .Style("overflow", "hidden")
            .Style("background", Ink)
            .Style("color", White)
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(DraftingSurface(receiver, editor, applyCell));
        root.Content(Header(receiver, editor));
        root.Content(ToolRail(receiver, editor, selectTool));
        root.Content(TemplateRail(receiver, loadTemplate));
        root.Content(CapturePanel(receiver, editor, capture));
        root.Content(ExitButton(receiver, exitShop));
        return root;
    }

    private static ElementBuilder Header(object receiver, ArchitecturalShopEditor editor)
        => WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "3%").Style("top", "3%")
            .Style("z-index", "10").Style("background", "rgba(1,4,10,.9)")
            .Style("border", $"1px solid {Cyan}66").Style("padding", ".7rem 1rem")
            .Content(WorkshopGui.Element(receiver, "div")
                .Style("font-size", ".45rem").Style("letter-spacing", ".28em").Style("color", Cyan)
                .Text("WORKSHOP // ARCHITECTURAL SHOP"))
            .Content(WorkshopGui.Element(receiver, "strong")
                .Style("display", "block").Style("margin-top", ".25rem")
                .Style("font-size", "clamp(1rem,2.2vw,1.7rem)").Style("letter-spacing", ".08em")
                .Text(editor.Plan.Name))
            .Content(WorkshopGui.Element(receiver, "small")
                .Style("display", "block").Style("margin-top", ".25rem").Style("color", Muted)
                .Text($"{editor.Plan.Width} × {editor.Plan.Height} GRID // {editor.Plan.Cells.Count} MARKS // {editor.Plan.VerticalConnectors.Count} VERTICAL CONNECTORS"));

    private static ElementBuilder DraftingSurface(
        object receiver,
        ArchitecturalShopEditor editor,
        Action<int, int> applyCell)
    {
        var panel = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "18%").Style("right", "18%")
            .Style("top", "12%").Style("bottom", "9%")
            .Style("display", "flex").Style("align-items", "center")
            .Style("justify-content", "center").Style("background", "#020a12")
            .Style("border", $"1px solid {Cyan}55")
            .Style("box-shadow", "inset 0 0 60px rgba(0,234,255,.035)");

        var svg = WorkshopGui.Element(receiver, "svg")
            .Attribute("viewBox", $"0 0 {editor.Plan.Width} {editor.Plan.Height}")
            .Attribute("role", "img")
            .Attribute("aria-label", "Architectural floor plan drafting surface")
            .Style("width", "100%").Style("height", "100%")
            .Style("max-width", "900px").Style("max-height", "700px")
            .Style("cursor", "crosshair");

        for (var x = 0; x < editor.Plan.Width; x++)
        {
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", x).Attribute("y1", 0).Attribute("x2", x).Attribute("y2", editor.Plan.Height)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".09").Attribute("stroke-width", ".025"));
        }

        for (var y = 0; y < editor.Plan.Height; y++)
        {
            svg.Child(WorkshopGui.Element(receiver, "line")
                .Attribute("x1", 0).Attribute("y1", y).Attribute("x2", editor.Plan.Width).Attribute("y2", y)
                .Attribute("stroke", Cyan).Attribute("stroke-opacity", ".09").Attribute("stroke-width", ".025"));
        }

        foreach (var cell in editor.Plan.Cells)
            svg.Child(Cell(receiver, cell, applyCell));

        svg.Child(WorkshopGui.Element(receiver, "rect")
            .Attribute("x", ".2").Attribute("y", ".2")
            .Attribute("width", editor.Plan.Width - .4).Attribute("height", editor.Plan.Height - .4)
            .Attribute("fill", "none").Attribute("stroke", Cyan).Attribute("stroke-opacity", ".6")
            .Attribute("stroke-width", ".08").Attribute("pointer-events", "none"));

        panel.Content(svg);
        return panel;
    }

    private static ElementBuilder Cell(
        object receiver,
        ArchitecturalCell cell,
        Action<int, int> applyCell)
    {
        var (fill, stroke, label) = cell.Kind switch
        {
            ArchitecturalCellKind.Wall => ("rgba(0,234,255,.18)", Cyan, ""),
            ArchitecturalCellKind.Door => ("rgba(82,224,90,.22)", Green, "D"),
            ArchitecturalCellKind.Room => ("rgba(255,44,255,.13)", Magenta, ""),
            ArchitecturalCellKind.Stair => ("rgba(255,255,255,.16)", White, "S"),
            ArchitecturalCellKind.Elevator => ("rgba(0,234,255,.28)", Cyan, "E"),
            ArchitecturalCellKind.Ladder => ("rgba(255,44,255,.24)", Magenta, "L"),
            _ => ("transparent", "transparent", "")
        };

        var rect = WorkshopGui.Element(receiver, "rect")
            .Attribute("x", cell.X).Attribute("y", cell.Y)
            .Attribute("width", "1").Attribute("height", "1")
            .Attribute("fill", fill).Attribute("stroke", stroke).Attribute("stroke-width", ".055")
            .Attribute("aria-label", $"{cell.Kind} at {cell.X},{cell.Y}")
            .OnClick(_ => applyCell(cell.X, cell.Y));

        if (string.IsNullOrEmpty(label))
            return rect;

        var group = WorkshopGui.Element(receiver, "g").Child(rect);
        group.Child(WorkshopGui.Element(receiver, "text")
            .Attribute("x", cell.X + .5).Attribute("y", cell.Y + .68)
            .Attribute("text-anchor", "middle").Attribute("font-size", ".45")
            .Attribute("fill", stroke).Attribute("pointer-events", "none")
            .Text(label));
        return group;
    }

    private static ElementBuilder ToolRail(
        object receiver,
        ArchitecturalShopEditor editor,
        Action<ArchitecturalCellKind> selectTool)
    {
        var rail = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "2%").Style("top", "25%")
            .Style("z-index", "10").Style("width", "14%")
            .Style("background", "rgba(1,4,10,.92)").Style("border", $"1px solid {Magenta}55")
            .Style("padding", ".65rem");

        rail.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".42rem").Style("letter-spacing", ".18em").Style("color", Magenta)
            .Text("DRAFTING TOOLS"));

        foreach (var tool in new[]
        {
            ArchitecturalCellKind.Wall,
            ArchitecturalCellKind.Door,
            ArchitecturalCellKind.Room,
            ArchitecturalCellKind.Stair,
            ArchitecturalCellKind.Elevator,
            ArchitecturalCellKind.Ladder,
            ArchitecturalCellKind.Empty
        })
        {
            rail.Content(WorkshopGui.Button(receiver)
                .Label(tool == ArchitecturalCellKind.Empty ? "ERASE" : tool.ToString().ToUpperInvariant())
                .Style("width", "100%").Style("margin-top", ".35rem").Style("padding", ".42rem")
                .Style("border", $"1px solid {(editor.SelectedTool == tool ? Green : Cyan)}66")
                .Style("border-radius", "0").Style("background", "transparent")
                .Style("color", editor.SelectedTool == tool ? Green : Cyan)
                .Style("font-family", "inherit").Style("font-size", ".4rem")
                .Style("letter-spacing", ".1em").Style("cursor", "pointer")
                .OnClick(() => selectTool(tool)));
        }

        return rail;
    }

    private static ElementBuilder TemplateRail(
        object receiver,
        Action<ArchitecturalFloorPlan> loadTemplate)
    {
        var rail = WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("right", "2%").Style("top", "25%")
            .Style("z-index", "10").Style("width", "14%")
            .Style("background", "rgba(1,4,10,.92)").Style("border", $"1px solid {Cyan}55")
            .Style("padding", ".65rem");

        rail.Content(WorkshopGui.Element(receiver, "div")
            .Style("font-size", ".42rem").Style("letter-spacing", ".18em").Style("color", Cyan)
            .Text("STARTING PLANS"));

        foreach (var plan in new[]
        {
            ArchitecturalFloorPlanTemplates.OpenWorkshop,
            ArchitecturalFloorPlanTemplates.Gallery,
            ArchitecturalFloorPlanTemplates.TowerCore
        })
        {
            rail.Content(WorkshopGui.Button(receiver)
                .Label(plan.Name.ToUpperInvariant())
                .Style("width", "100%").Style("margin-top", ".35rem").Style("padding", ".42rem")
                .Style("border", $"1px solid {Cyan}66").Style("border-radius", "0")
                .Style("background", "transparent").Style("color", Cyan)
                .Style("font-family", "inherit").Style("font-size", ".4rem")
                .Style("letter-spacing", ".08em").Style("cursor", "pointer")
                .OnClick(() => loadTemplate(plan)));
        }

        return rail;
    }

    private static ElementBuilder CapturePanel(
        object receiver,
        ArchitecturalShopEditor editor,
        Action capture)
    {
        var status = editor.CapturedStructure is null
            ? "DRAFT EXISTS IN MEMORY — CAPTURE IT AS A STRUCTURE WHEN READY."
            : $"CAPTURED // {editor.CapturedStructure.TotalCellCount} MARKS // {editor.CapturedStructure.VerticalConnectorCount} VERTICAL CONNECTORS";

        return WorkshopGui.Panel(receiver)
            .Style("position", "absolute").Style("left", "50%").Style("bottom", "2%")
            .Style("z-index", "10").Style("transform", "translateX(-50%)")
            .Style("display", "flex").Style("align-items", "center").Style("gap", ".8rem")
            .Style("background", "rgba(1,4,10,.94)").Style("border", $"1px solid {Green}66")
            .Style("padding", ".45rem .7rem")
            .Content(WorkshopGui.Element(receiver, "span")
                .Style("font-size", ".38rem").Style("letter-spacing", ".08em").Style("color", Muted)
                .Text(status))
            .Content(WorkshopGui.Button(receiver)
                .Label("CAPTURE STRUCTURE")
                .Style("padding", ".45rem .7rem").Style("border", $"1px solid {Green}99")
                .Style("border-radius", "0").Style("background", "transparent").Style("color", Green)
                .Style("font-family", "inherit").Style("font-size", ".4rem")
                .Style("letter-spacing", ".1em").Style("cursor", "pointer")
                .OnClick(capture));
    }

    private static ElementBuilder ExitButton(object receiver, Action exitShop)
        => WorkshopGui.Button(receiver)
            .Label("EXIT // ARCHITECTURAL SHOP")
            .Style("position", "absolute").Style("right", "2%").Style("bottom", "2%")
            .Style("z-index", "10").Style("padding", ".5rem .8rem")
            .Style("border", $"1px solid {Cyan}88").Style("border-radius", "0")
            .Style("background", Ink).Style("color", Cyan)
            .Style("font-family", "inherit").Style("font-size", ".4rem")
            .Style("letter-spacing", ".12em").Style("cursor", "pointer")
            .OnClick(exitShop);
}
