using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Recursive, platform-neutral GUI builder.
/// Each child is built by the same builder contract, allowing a renderer to
/// consume one tree for Blazor, WPF, Unity, Revit, or another host.
/// </summary>
public sealed class GuiBuilder : ICoreGuiBuilder<GuiNode>
{
    private readonly NodeBuilder _root;

    private GuiBuilder(NodeBuilder root, GuiBuilder? parent = null)
    {
        _root = root;
        Parent = parent;
    }

    /// <summary>The semantic kind represented by this builder.</summary>
    public string Kind => _root.Kind;

    /// <summary>The stable semantic identifier represented by this builder.</summary>
    public string GetBuilderId() => _root.Id;

    /// <summary>The recursive parent builder, or <see langword="null"/> for a root.</summary>
    public GuiBuilder? Parent { get; }

    /// <summary>
    /// Explicit interface projection of the recursive parent.
    /// The public property remains strongly typed for fluent GUI composition,
    /// while the shared contract exposes the WPF-aligned object parent.
    /// </summary>
    object? ICoreGuiBuilder<GuiNode>.Parent => Parent;

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
        var builder = new GuiBuilder(child, this);
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
