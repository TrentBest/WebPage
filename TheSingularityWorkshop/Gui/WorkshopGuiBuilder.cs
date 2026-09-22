using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>Workshop-owned fluent vocabulary for recursive GUI construction.</summary>
public static class WorkshopGui
{
    public static ElementBuilder Element(object receiver, string tagName) => new(receiver, tagName);
    public static PanelGuiBuilder Panel(object receiver) => new(receiver);
    public static MultiPanelGuiBuilder MultiPanel(object receiver) => new(receiver);
    public static ButtonGuiBuilder Button(object receiver) => new(receiver);
    public static ElementBuilder Image(object receiver) => Element(receiver, "img");
}

/// <summary>Recursive fluent description of a Blazor element.</summary>
public class ElementBuilder
{
    private readonly object _receiver;
    private readonly string _tagName;
    private readonly RenderFragment? _rawFragment;
    private readonly List<(string Name, object? Value)> _attributes = new();
    private readonly List<RenderFragment> _children = new();
    private readonly Dictionary<string, string> _styles = new(StringComparer.Ordinal);

    internal ElementBuilder(object receiver, string tagName) { _receiver = receiver; _tagName = tagName; }
    private ElementBuilder(RenderFragment rawFragment) { _receiver = new object(); _tagName = string.Empty; _rawFragment = rawFragment; }

    /// <summary>Allows an existing RenderFragment-producing builder to cross back into the recursive vocabulary.</summary>
    public static implicit operator ElementBuilder(RenderFragment fragment)
    {
        ArgumentNullException.ThrowIfNull(fragment);
        return new ElementBuilder(fragment);
    }

    public ElementBuilder Attribute(string name, object? value) { _attributes.Add((name, value)); return this; }
    public ElementBuilder AriaLabel(string value) => Attribute("aria-label", value);
    public ElementBuilder Title(string value) => Attribute("title", value);
    public ElementBuilder TabIndex(int value) => Attribute("tabindex", value);
    public ElementBuilder Class(string className) => Attribute("class", className);
    public ElementBuilder Style(string name, string value) { _styles[name] = value; return this; }
    public ElementBuilder Position(double x, double y) => Style("position", "absolute").Style("left", $"{x:0.##}%").Style("top", $"{y:0.##}%").Style("transform", "translate(-50%, -50%)");
    public ElementBuilder Text(string text) { _children.Add(builder => builder.AddContent(0, text)); return this; }
    public ElementBuilder Child(ElementBuilder child) { _children.Add(child.Build()); return this; }
    public ElementBuilder Child(RenderFragment child) { _children.Add(child); return this; }
    public ElementBuilder Content(ElementBuilder child) => Child(child);
    public ElementBuilder OnClick(Action action) => Attribute("onclick", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnClick(Func<Task> action) => Attribute("onclick", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnClick(Action<MouseEventArgs> action) => Attribute("onclick", EventCallback.Factory.Create<MouseEventArgs>(_receiver, action));
    public ElementBuilder OnKeyDown(Action<KeyboardEventArgs> action) => Attribute("onkeydown", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnWheel(Action<WheelEventArgs> action) => Attribute("onwheel", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnMouseEnter(Action action) => Attribute("onmouseenter", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnMouseLeave(Action action) => Attribute("onmouseleave", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnMouseMove(Action<MouseEventArgs> action) => Attribute("onmousemove", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnMouseDown(Action<MouseEventArgs> action) => Attribute("onmousedown", EventCallback.Factory.Create(_receiver, action));
    public ElementBuilder OnMouseUp(Action<MouseEventArgs> action) => Attribute("onmouseup", EventCallback.Factory.Create(_receiver, action));

    /// <summary>Prevents a child interaction from bubbling into a parent command surface.</summary>
    public ElementBuilder StopPropagation(string eventName) => Attribute($"{eventName}:stopPropagation", true);

    public ElementBuilder PreventDefault(string eventName) => Attribute($"{eventName}:preventDefault", true);
    public ElementBuilder BuildChildren(params ElementBuilder[] children) { foreach (var child in children) Child(child); return this; }

    public RenderFragment Build()
    {
        if (_rawFragment is not null) return _rawFragment;
        return builder =>
        {
            // RenderTreeBuilder sequence numbers describe source locations, not runtime
            // invocation order. The fluent builder is intentionally loop-driven, so using
            // a counter here makes the generated tree unstable whenever a conditional or
            // collection changes between renders. That can corrupt Blazor's diff traversal.
            // Keep the loop operations anchored to their source locations instead.
            builder.OpenElement(0, _tagName);

            if (_attributes.Count > 0)
            {
                builder.AddMultipleAttributes(1, _attributes.Select(attribute =>
                    new KeyValuePair<string, object?>(attribute.Name, attribute.Value)));
            }

            if (_styles.Count > 0)
                builder.AddAttribute(2, "style", string.Join(";", _styles.Select(x => $"{x.Key}:{x.Value}")));

            foreach (var child in _children)
                builder.AddContent(3, child);

            builder.CloseElement();
        };
    }
}
