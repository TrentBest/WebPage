using Microsoft.AspNetCore.Components;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Web manifestation of the platform-neutral recursive GUI builder.
/// This is the first concrete platform family built on the shared GUI tree.
/// </summary>
public sealed class WebGuiBuilder : ICoreGuiBuilder<RenderFragment>
{
    private readonly GuiBuilder _semanticBuilder;

    private WebGuiBuilder(GuiBuilder semanticBuilder, WebGuiBuilder? parent)
    {
        _semanticBuilder = semanticBuilder;
        Parent = parent;
    }

    /// <summary>Creates a new root web builder.</summary>
    public static WebGuiBuilder Create(string kind, string id)
        => new(GuiBuilder.Create(kind, id), null);

    /// <summary>Creates a semantic panel.</summary>
    public static WebGuiBuilder Panel(string id)
        => Create("Panel", id);

    /// <summary>Creates a semantic button.</summary>
    public static WebGuiBuilder Button(string id)
        => Create("Button", id);

    /// <summary>Creates a semantic stack layout.</summary>
    public static WebGuiBuilder Stack(string id)
        => Create("Stack", id);

    /// <summary>Creates a semantic grid layout.</summary>
    public static WebGuiBuilder Grid(string id)
        => Create("Grid", id);

    /// <summary>Creates a semantic text node.</summary>
    public static WebGuiBuilder Text(string id, string text)
        => Create("Text", id).Text(text);

    /// <summary>Creates a semantic image node.</summary>
    public static WebGuiBuilder Image(string id, string source)
        => Create("Image", id).Image(source);

    /// <summary>The recursive parent builder, or <see langword="null"/> for a root.</summary>
    public object? Parent { get; }

    /// <summary>Gets the semantic node kind.</summary>
    public string Kind => _semanticBuilder.Kind;

    /// <summary>Gets the stable builder identifier.</summary>
    public string GetBuilderId() => _semanticBuilder.GetBuilderId();

    /// <summary>Sets text content.</summary>
    public WebGuiBuilder Text(string text)
    {
        _semanticBuilder.Text(text);
        return this;
    }

    /// <summary>Sets an image source.</summary>
    public WebGuiBuilder Image(string source)
    {
        _semanticBuilder.Image(source);
        return this;
    }

    /// <summary>Adds a semantic property.</summary>
    public WebGuiBuilder Property(string name, string value)
    {
        _semanticBuilder.Property(name, value);
        return this;
    }

    /// <summary>Sets a CSS class as a semantic web property.</summary>
    public WebGuiBuilder Class(string value)
        => Property("class", value);

    /// <summary>Sets a semantic style declaration for web-only presentation details.</summary>
    public WebGuiBuilder Style(string name, string value)
        => Property($"style:{name}", value);

    /// <summary>Sets a platform-neutral layout width token.</summary>
    public WebGuiBuilder Width(string value)
        => Property("layout:width", value);

    /// <summary>Sets a platform-neutral layout height token.</summary>
    public WebGuiBuilder Height(string value)
        => Property("layout:height", value);

    /// <summary>Sets a platform-neutral command identifier.</summary>
    public WebGuiBuilder Command(string commandId)
        => Property("command", commandId);

    /// <summary>
    /// Recursively composes another builder. The child is still represented by the
    /// platform-neutral <see cref="GuiNode"/> tree before Web rendering occurs.
    /// </summary>
    public WebGuiBuilder Child(
        string kind,
        string id,
        Action<WebGuiBuilder>? configure = null)
    {
        _semanticBuilder.Child(
            kind,
            id,
            child => configure?.Invoke(new WebGuiBuilder(child, this)));

        return this;
    }

    /// <summary>Builds the platform-neutral GUI tree.</summary>
    public GuiNode BuildNode() => _semanticBuilder.Build();

    /// <summary>Manifests the tree as a Blazor render fragment.</summary>
    public RenderFragment Build()
        => BlazorGuiRenderer.Render(BuildNode());

}
