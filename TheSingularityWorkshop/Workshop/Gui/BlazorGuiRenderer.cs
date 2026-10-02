using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Blazor manifestation of a platform-neutral <see cref="GuiNode"/> tree.
/// The builder remains renderer-independent; this type is the adapter that turns
/// semantic GUI nodes into Blazor render instructions.
/// </summary>
public static class BlazorGuiRenderer
{
    /// <summary>
    /// Creates a Blazor render fragment for the complete recursive GUI tree.
    /// </summary>
    public static RenderFragment Render(GuiNode node, Action<string>? onCommand = null)
    {
        ArgumentNullException.ThrowIfNull(node);
        return builder => RenderNode(builder, node, 0, onCommand);
    }

    private static int RenderNode(RenderTreeBuilder builder, GuiNode node, int sequence, Action<string>? onCommand)
    {
        builder.OpenElement(sequence++, ResolveTag(node.Kind));
        builder.AddAttribute(sequence++, "id", node.Id);

        if (!string.IsNullOrWhiteSpace(node.Source))
            builder.AddAttribute(sequence++, "src", node.Source);

        foreach (var property in node.Properties)
        {
            if (property.Key == "command" && onCommand is not null && node.Kind == "Button")
                builder.AddAttribute(sequence++, "onclick", EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(builder, () => onCommand(property.Value)));
            else if (property.Key != "command")
                builder.AddAttribute(sequence++, property.Key, property.Value);
        }

        if (node.Text is not null)
            builder.AddContent(sequence++, node.Text);

        foreach (var child in node.Children)
            sequence = RenderNode(builder, child, sequence, onCommand);

        builder.CloseElement();
        return sequence;
    }

    private static string ResolveTag(string kind) => kind switch
    {
        "Panel" => "div",
        "Button" => "button",
        "Image" => "img",
        "Text" => "span",
        "Warning" => "aside",
        _ => "div"
    };
}
