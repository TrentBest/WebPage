#pragma warning disable BL0006

using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Rendering;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WebGuiBuilderManifestTests
{
    [Fact(DisplayName = "Web builder manifests a recursive tree through Blazor")]
    public void WebBuilderManifestsRecursiveTree()
    {
        var builder = WebGuiBuilder
            .Panel("workshop")
            .Class("workshop-panel")
            .Style("display", "flex")
            .Child("Button", "enter", button => button
                .Class("workshop-button")
                .Child("Text", "label", text => text.Text("Enter Workshop")))
            .Child("Warning", "advisory", warning =>
                warning.Text("SYSTEM ADVISORY"));

        var node = builder.BuildNode();

        Assert.Equal("Panel", node.Kind);
        Assert.Equal("workshop", node.Id);
        Assert.Equal("workshop-panel", node.Properties["class"]);
        Assert.Equal("flex", node.Properties["style:display"]);
        Assert.Equal("Enter Workshop", node.Find("label").Text);
        Assert.Equal("SYSTEM ADVISORY", node.Find("advisory").Text);

        var renderTree = new RenderTreeBuilder();
        builder.Build()(renderTree);

        var frames = renderTree.GetFrames().Array;
        Assert.Contains(frames, frame => frame.FrameType == RenderTreeFrameType.Element && frame.ElementName == "div");
        Assert.Contains(frames, frame => frame.FrameType == RenderTreeFrameType.Element && frame.ElementName == "button");
        Assert.Contains(frames, frame => frame.FrameType == RenderTreeFrameType.Text && frame.TextContent == "Enter Workshop");
    }

    [Fact(DisplayName = "Web builder preserves recursive parent relationship")]
    public void WebBuilderPreservesParentRelationship()
    {
        WebGuiBuilder? capturedChild = null;

        var root = WebGuiBuilder.Panel("root")
            .Child("Panel", "child", child => capturedChild = child);

        Assert.Null(root.Parent);
        Assert.NotNull(capturedChild);
        Assert.Same(root, capturedChild!.Parent);
    }

    [Fact(DisplayName = "Web manifest exposes the initial shared GUI vocabulary")]
    public void WebManifestExposesInitialVocabulary()
    {
        Assert.True(WebGuiManifest.Supports("Panel"));
        Assert.True(WebGuiManifest.Supports("Button"));
        Assert.True(WebGuiManifest.Supports("Image"));
        Assert.True(WebGuiManifest.Supports("Text"));
        Assert.True(WebGuiManifest.Supports("Warning"));
        Assert.True(WebGuiManifest.Supports("Stack"));
        Assert.True(WebGuiManifest.Supports("Grid"));
        Assert.False(WebGuiManifest.Supports("NotAWorkshopControl"));
    }

    [Fact(DisplayName = "V0.0.159 — Web GUI builder manifestation is established")]
    public void V0_0_159_WebGuiBuilderManifestation()
        => Assert.Equal("0.0.159", "0.0.159");
}

#pragma warning restore BL0006
