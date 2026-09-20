namespace TheSingularityWorkshop.Gui;

using TheSingularityWorkshop.Workshop.Spatial;

/// <summary>Visual data-card presentation for FSM lifecycle behavior.</summary>
public static class SpatialBehaviorBuilderGuiBuilder
{
    public static ElementBuilder Build(object receiver, SpatialBehaviorBuilderModel model, Action addElseIf, Action exit)
    {
        var root = WorkshopGui.Panel(receiver)
            .Style("position", "fixed").Style("inset", "0").Style("overflow", "auto")
            .Style("background", "#02050a").Style("color", "#fff")
            .Style("font-family", "Consolas, 'Courier New', monospace");

        root.Content(WorkshopGui.Element(receiver, "div").Style("max-width", "1000px").Style("margin", "5vh auto").Style("padding", "2rem")
            .Content(WorkshopGui.Element(receiver, "div").Style("color", "#00eaff").Style("font-size", ".55rem").Style("letter-spacing", ".2em").Text("BEHAVIOR DEPARTMENT // SEMANTIC EDITOR"))
            .Content(WorkshopGui.Element(receiver, "h1").Style("margin", ".5rem 0 1rem").Text(model.MethodName))
            .Content(Lifecycle(receiver, model))
            .Content(WorkshopGui.Element(receiver, "div").Style("display", "flex").Style("gap", "1rem").Style("flex-wrap", "wrap").BuildChildren(model.ContextBranches.Select(branch => BranchCard(receiver, branch)).ToArray()))
            .Content(WorkshopGui.Element(receiver, "div").Style("margin-top", "1.2rem").Style("padding", "1rem").Style("border", "1px dashed #00eaff55").Style("background", "#00eaff08")
                .Content(WorkshopGui.Element(receiver, "div").Style("color", "#8ba8b2").Style("font-family", "system-ui,sans-serif").Style("font-size", ".75rem").Text("The body of each condition is a nested data card. Selecting another context type adds another compatible else-if branch rather than forcing the user to type syntax.")))
            .Content(WorkshopGui.Button(receiver).Label("+ ADD ELSE IF CONTEXT").Style("margin-top", "1rem").Style("padding", ".7rem 1rem").Style("border", "1px solid #ffd34d77").Style("background", "#ffd34d0c").Style("color", "#ffd34d").Style("font-family", "inherit").OnClick(addElseIf))
            .Content(WorkshopGui.Button(receiver).Label("← RETURN TO FACILITY").Style("margin-top", "2rem").Style("padding", ".7rem 1rem").Style("border", "1px solid #ff38d177").Style("background", "#ff38d10c").Style("color", "#ff38d1").Style("font-family", "inherit").OnClick(exit)));

        return root;
    }

    private static ElementBuilder Lifecycle(object r, SpatialBehaviorBuilderModel model)
        => WorkshopGui.Element(r, "div").Style("display", "grid").Style("grid-template-columns", "repeat(auto-fit,minmax(130px,1fr))").Style("gap", ".5rem").Style("margin-bottom", "1.5rem")
            .BuildChildren(model.LifecycleMethods.Select(method => WorkshopGui.Element(r, "div").Style("padding", ".6rem").Style("border", "1px solid #00eaff33").Style("background", "#00eaff08").Style("color", "#8ba8b2").Text(method)).ToArray());

    private static ElementBuilder BranchCard(object r, SpatialBehaviorContextBranch branch)
        => WorkshopGui.Element(r, "div").Style("min-width", "280px").Style("flex", "1 1 280px").Style("border", $"1px solid {(branch.IsFirstBranch ? "#00eaff" : "#ffd34d")}55").Style("background", "#071019").Style("padding", "1rem")
            .Content(WorkshopGui.Element(r, "div").Style("color", branch.IsFirstBranch ? "#00eaff" : "#ffd34d").Style("font-size", ".5rem").Style("letter-spacing", ".14em").Text(branch.Keyword.ToUpperInvariant()))
            .Content(WorkshopGui.Element(r, "div").Style("margin-top", ".7rem").Style("padding", ".65rem").Style("border", "1px solid #ffffff18").Style("background", "#0006").Text(branch.Signature))
            .Content(WorkshopGui.Element(r, "div").Style("margin-top", ".7rem").Style("min-height", "110px").Style("border", "1px dashed #ffffff22").Style("padding", ".7rem").Style("color", "#667b83").Text("{ body data card }"));
}
