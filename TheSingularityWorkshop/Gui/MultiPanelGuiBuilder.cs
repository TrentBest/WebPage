namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Semantic builder for a collection of panels.
/// It is intentionally recursive: every panel remains a GUI builder in its own right.
/// </summary>
public sealed class MultiPanelGuiBuilder : ElementBuilder
{
    internal MultiPanelGuiBuilder(object receiver)
        : base(receiver, "div")
    {
    }

    public MultiPanelGuiBuilder Panel(Action<PanelGuiBuilder> configure)
    {
        var panel = WorkshopGui.Panel(this);
        configure(panel);
        Child(panel);
        return this;
    }

    public MultiPanelGuiBuilder Content(ElementBuilder child)
    {
        Child(child);
        return this;
    }

    public new MultiPanelGuiBuilder Style(string name, string value)
    {
        base.Style(name, value);
        return this;
    }
}
