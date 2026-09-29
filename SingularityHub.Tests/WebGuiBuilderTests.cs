using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Rendering;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WebGuiBuilderTests
{
    [Fact(DisplayName = "WebGuiBuilder composes against the shared GUI semantic tree")]
    public void WebGuiBuilderBuildsSharedSemanticTree()
    {
        var root = WebGuiBuilder.Panel("workshop")
            .Child("Panel", "header", header => header.Child("Text", "title", title => title.Text("The Singularity Workshop")))
            .Child("Button", "enter", enter => enter.Child("Text", "label", label => label.Text("Enter Workshop")))
            .BuildNode();
        Assert.Equal("Panel", root.Kind);
        Assert.Equal("workshop", root.Id);
        Assert.Equal("Panel", root.Find("header").Kind);
        Assert.Equal("The Singularity Workshop", root.Find("title").Text);
        Assert.Equal("Button", root.Find("enter").Kind);
        Assert.Equal("Enter Workshop", root.Find("label").Text);
    }

    [Fact(DisplayName = "WebGuiBuilder preserves recursive builder identity")]
    public void WebGuiBuilderPreservesIdentity()
    {
        var root = WebGuiBuilder.Panel("root").Child("Panel", "child");
        Assert.Equal("Panel", root.Kind);
        Assert.Equal("root", root.GetBuilderId());
        Assert.Null(root.Parent);
        var node = root.BuildNode();
        Assert.Equal("child", node.Children[0].Id);
    }

    [Fact(DisplayName = "WebGuiBuilder semantic tree survives Blazor manifestation")]
    public void WebGuiBuilderManifestsThroughBlazor()
    {
        var root = WebGuiBuilder.Panel("root")
            .Child("Button", "enter", button => button.Child("Text", "label", label => label.Text("Enter Workshop")));
        var renderTree = new RenderTreeBuilder();
        root.Build()(renderTree);
        var frames = renderTree.GetFrames().Array;
        Assert.Contains(frames, frame => frame.FrameType == RenderTreeFrameType.Element && frame.ElementName == "div");
        Assert.Contains(frames, frame => frame.FrameType == RenderTreeFrameType.Attribute && frame.AttributeName == "id" && (string?)frame.AttributeValue == "root");
        Assert.Contains(frames, frame => frame.FrameType == RenderTreeFrameType.Element && frame.ElementName == "button");
        Assert.Contains(frames, frame => frame.FrameType == RenderTreeFrameType.Text && frame.TextContent == "Enter Workshop");
    }
}
