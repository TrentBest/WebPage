#if WORKSHOP_USE_RENDERER_SOURCE
namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Source-mode compatibility adapter for the older GuiBuilders convenience API.
/// It builds the current GUI repository's platform-neutral GuiNode through GuiBuilder;
/// it does not render controls or introduce a second presentation implementation.
/// </summary>
internal static class GuiBuilders
{
    public static Builder Panel(string id) => new("Panel", id);
    public static Builder Text(string id, string text) => new("Text", id, text);
    public static Builder TextBox(string id, string? text) => new("TextBox", id, text);
    public static Builder Button(string id, string label) => new("Button", id, label);

    internal sealed class Builder
    {
        private readonly string _kind;
        private readonly string _id;
        private readonly string? _text;
        private readonly Dictionary<string, string> _properties = new(StringComparer.Ordinal);
        private readonly List<Builder> _children = [];

        internal Builder(string kind, string id, string? text = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(kind);
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            _kind = kind;
            _id = id;
            _text = text;
        }

        public Builder Property(string name, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(value);
            _properties[name] = value;
            return this;
        }

        public Builder Child(Builder child)
        {
            ArgumentNullException.ThrowIfNull(child);
            _children.Add(child);
            return this;
        }

        public GuiNode Build()
        {
            var root = GuiBuilder.Create(_kind, _id);
            Configure(root);
            return root.Build();
        }

        private void Configure(GuiBuilder builder)
        {
            if (_text is not null)
                builder.Text(_text);

            foreach (var property in _properties)
                builder.Property(property.Key, property.Value);

            foreach (var child in _children)
                builder.Child(child._kind, child._id, child.Configure);
        }
    }
}
#endif
