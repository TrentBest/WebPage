using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Semantic fluent builder for interactive buttons.
/// A button is a GUI primitive, not a CSS construct.
/// </summary>
public sealed class ButtonGuiBuilder : ElementBuilder
{
    internal ButtonGuiBuilder(object receiver)
        : base(receiver, "button")
    {
        Attribute("type", "button");
    }

    public ButtonGuiBuilder Label(string text)
    {
        Text(text);
        return this;
    }

    public ButtonGuiBuilder PositionAt(double x, double y)
    {
        Position(x, y);
        return this;
    }

    public new ButtonGuiBuilder Attribute(string name, object? value)
    {
        base.Attribute(name, value);
        return this;
    }

    public new ButtonGuiBuilder AriaLabel(string value)
    {
        base.AriaLabel(value);
        return this;
    }

    public new ButtonGuiBuilder Title(string value)
    {
        base.Title(value);
        return this;
    }

    public new ButtonGuiBuilder Style(string name, string value)
    {
        base.Style(name, value);
        return this;
    }

    public new ButtonGuiBuilder OnClick(Action action)
    {
        base.OnClick(action);
        return this;
    }

    public new ButtonGuiBuilder OnClick(Func<Task> action)
    {
        base.OnClick(action);
        return this;
    }

    public new ButtonGuiBuilder OnClick(Action<MouseEventArgs> action)
    {
        base.OnClick(action);
        return this;
    }
}
