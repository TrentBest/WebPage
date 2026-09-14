namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Semantic fluent builder for a visual panel.
/// Panels compose recursively from the same GUI vocabulary as every other GUI.
/// </summary>
public sealed class PanelGuiBuilder : ElementBuilder
{
    internal PanelGuiBuilder(object receiver)
        : base(receiver, "section")
    {
        Style("display", "flex");
        Style("flex-direction", "column");
        Style("box-sizing", "border-box");
    }

    public PanelGuiBuilder Heading(string text, int level = 2)
    {
        Child(WorkshopGui.Element(this, $"h{Math.Clamp(level, 1, 6)}").Text(text));
        return this;
    }

    public PanelGuiBuilder Paragraph(string text)
    {
        Child(WorkshopGui.Element(this, "p").Text(text));
        return this;
    }

    public PanelGuiBuilder Content(ElementBuilder child)
    {
        Child(child);
        return this;
    }

    public new PanelGuiBuilder Attribute(string name, object? value)
    {
        base.Attribute(name, value);
        return this;
    }

    public new PanelGuiBuilder AriaLabel(string value)
    {
        base.AriaLabel(value);
        return this;
    }

    public new PanelGuiBuilder Style(string name, string value)
    {
        base.Style(name, value);
        return this;
    }
}
