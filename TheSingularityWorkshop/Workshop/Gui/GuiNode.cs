using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Immutable intermediate representation of a GUI element.
/// A node owns zero or more child nodes, making the GUI recursively composable
/// before any platform-specific renderer is involved.
/// </summary>
public sealed class GuiNode
{
    private readonly ReadOnlyCollection<GuiNode> _children;

    public GuiNode(
        string kind,
        string id,
        string? text = null,
        string? source = null,
        IReadOnlyDictionary<string, string>? properties = null,
        IEnumerable<GuiNode>? children = null)
    {
        if (string.IsNullOrWhiteSpace(kind))
            throw new ArgumentException("GUI node kind is required.", nameof(kind));
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("GUI node id is required.", nameof(id));

        Kind = kind;
        Id = id;
        Text = text;
        Source = source;
        Properties = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(properties ?? new Dictionary<string, string>(), StringComparer.Ordinal));
        _children = new ReadOnlyCollection<GuiNode>((children ?? Enumerable.Empty<GuiNode>()).ToList());
    }

    public string Kind { get; }
    public string Id { get; }
    public string? Text { get; }
    public string? Source { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }
    public IReadOnlyList<GuiNode> Children => _children;

    public GuiNode Find(string id)
    {
        if (Id == id)
            return this;

        foreach (var child in _children)
        {
            try
            {
                return child.Find(id);
            }
            catch (InvalidOperationException)
            {
                // Continue recursively through the remaining siblings.
            }
        }

        throw new InvalidOperationException($"GUI node '{id}' was not found beneath '{Id}'.");
    }
}
