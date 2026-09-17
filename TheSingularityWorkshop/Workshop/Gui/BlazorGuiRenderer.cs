using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Blazor manifestation of the shared recursive GUI tree.
/// The renderer is deliberately thin: semantic builders own structure and this
/// adapter owns only the translation into HTML/Blazor render instructions.
/// </summary>
public static class BlazorGuiRenderer
{
    /// <summary>Creates a Blazor render fragment for a complete GUI tree.</summary>
    public static RenderFragment Render(GuiNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return builder => RenderNode(builder, node, 0);
    }

    private static int RenderNode(RenderTreeBuilder builder, GuiNode node, int sequence)
    {
        builder.OpenElement(sequence++, ResolveTag(node.Kind));
        builder.AddAttribute(sequence++, "id", node.Id);

        var style = new StringBuilder();
        foreach (var property in node.Properties)
        {
            if (property.Key.StartsWith("style:", StringComparison.Ordinal))
            {
                if (style.Length > 0)
                    style.Append(';');

                style.Append(property.Key[6..]).Append(':').Append(property.Value);
                continue;
            }

            builder.AddAttribute(sequence++, property.Key, property.Value);
        }

        if (style.Length > 0)
            builder.AddAttribute(sequence++, "style", style.ToString());

        if (node.Source is not null)
            builder.AddAttribute(sequence++, "src", node.Source);

        if (node.Text is not null)
            builder.AddContent(sequence++, node.Text);

        foreach (var child in node.Children)
            sequence = RenderNode(builder, child, sequence);

        builder.CloseElement();
        return sequence;
    }

    private static string ResolveTag(string kind) => kind switch
    {
        "Panel" => "div",
        "Stack" => "div",
        "Grid" => "div",
        "Button" => "button",
        "Image" => "img",
        "Text" => "span",
        "Warning" => "aside",
        _ => "div"
    };
}
