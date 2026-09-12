using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Recursive, platform-neutral GUI builder.
/// Each child is built by the same builder contract, allowing a renderer to
/// consume one tree for Blazor, WPF, Unity, Revit, or another host.
/// </summary>
public sealed class GuiBuilder
{
    private readonly NodeBuilder _root;

    private GuiBuilder(NodeBuilder root)
    {
        _root = root;
    }

    public static GuiBuilder Create(string kind, string id)
        => new(new NodeBuilder(kind, id));

    public GuiBuilder Text(string text)
    {
        _root.Text = text;
        return this;
    }

    public GuiBuilder Image(string source)
    {
        _root.Source = source;
        return this;
    }

    public GuiBuilder Property(string name, string value)
    {
        _root.Properties[name] = value;
        return this;
    }

    /// <summary>
    /// Adds a complete child subtree using the same recursive builder API.
    /// </summary>
    public GuiBuilder Child(string kind, string id, Action<GuiBuilder>? configure = null)
    {
        var child = new NodeBuilder(kind, id);
        var builder = new GuiBuilder(child);
        configure?.Invoke(builder);
        _root.Children.Add(child);
        return this;
    }

    public GuiNode Build() => _root.Build();

    private sealed class NodeBuilder
    {
        public NodeBuilder(string kind, string id)
        {
            Kind = kind;
            Id = id;
        }

        public string Kind { get; }
        public string Id { get; }
        public string? Text { get; set; }
        public string? Source { get; set; }
        public Dictionary<string, string> Properties { get; } = new(StringComparer.Ordinal);
        public List<NodeBuilder> Children { get; } = new();

        public GuiNode Build()
        {
            var children = new List<GuiNode>(Children.Count);
            foreach (var child in Children)
                children.Add(child.Build());

            return new GuiNode(Kind, Id, Text, Source, Properties, children);
        }
    }
}
