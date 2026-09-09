using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace TheSingularityWorkshop.Gui;

/// <summary>
/// Small fluent entry point for Workshop-owned GUI construction.
///
/// The builder deliberately wraps Blazor's RenderTreeBuilder instead of making
/// RenderTreeBuilder the vocabulary of the application. The Workshop can grow
/// semantic operations here as the living GUI develops.
/// </summary>
public static class WorkshopGui
{
    public static ElementBuilder Element(string tagName)
        => new(tagName);

    public static ElementBuilder Panel()
        => Element("div");

    public static ElementBuilder Button()
        => Element("button");

    public static ElementBuilder Image()
        => Element("img");
}

/// <summary>
/// Fluent description of a small Blazor element tree.
/// </summary>
public sealed class ElementBuilder
{
    private readonly string _tagName;
    private readonly List<(string Name, object? Value)> _attributes = new();
    private readonly List<RenderFragment> _children = new();

    internal ElementBuilder(string tagName)
    {
        _tagName = tagName;
    }

    public ElementBuilder Class(string className)
    {
        _attributes.Add(("class", className));
        return this;
    }

    public ElementBuilder Attribute(string name, object? value)
    {
        _attributes.Add((name, value));
        return this;
    }

    public ElementBuilder Text(string text)
    {
        _children.Add(builder => builder.AddContent(0, text));
        return this;
    }

    public ElementBuilder Child(ElementBuilder child)
    {
        _children.Add(child.Build());
        return this;
    }

    public ElementBuilder Child(RenderFragment child)
    {
        _children.Add(child);
        return this;
    }

    /// <summary>
    /// Produces the Blazor render fragment at the boundary of the Workshop GUI
    /// abstraction. Application code should normally remain in fluent semantics.
    /// </summary>
    public RenderFragment Build() => builder =>
    {
        builder.OpenElement(0, _tagName);

        var sequence = 1;
        foreach (var attribute in _attributes)
        {
            builder.AddAttribute(sequence++, attribute.Name, attribute.Value);
        }

        foreach (var child in _children)
        {
            builder.AddContent(sequence++, child);
        }

        builder.CloseElement();
    };
}
