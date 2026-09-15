using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Full unit coverage for the platform-neutral recursive GUI builder.</summary>
public sealed class GuiBuilderTests
{
    [Fact(DisplayName = "Create establishes root kind and identity")]
    public void CreateEstablishesRoot()
    {
        var node = GuiBuilder.Create("button", "enter").Build();
        Assert.Equal("button", node.Kind);
        Assert.Equal("enter", node.Id);
    }

    [Fact(DisplayName = "Text is retained in built node")]
    public void TextIsRetained()
        => Assert.Equal("Enter Workshop", GuiBuilder.Create("button", "enter").Text("Enter Workshop").Build().Text);

    [Fact(DisplayName = "Image source is retained in built node")]
    public void ImageIsRetained()
        => Assert.Equal("avatar.png", GuiBuilder.Create("image", "avatar").Image("avatar.png").Build().Source);

    [Fact(DisplayName = "Property is retained")]
    public void PropertyIsRetained()
    {
        var node = GuiBuilder.Create("button", "enter").Property("class", "hero").Build();
        Assert.Equal("hero", node.Properties["class"]);
    }

    [Fact(DisplayName = "Later property assignment replaces earlier value")]
    public void LaterPropertyAssignmentReplacesEarlierValue()
    {
        var node = GuiBuilder.Create("button", "enter")
            .Property("state", "cold")
            .Property("state", "hot")
            .Build();
        Assert.Equal("hot", node.Properties["state"]);
        Assert.Single(node.Properties);
    }

    [Fact(DisplayName = "Multiple properties remain independent")]
    public void MultiplePropertiesRemainIndependent()
    {
        var node = GuiBuilder.Create("button", "enter")
            .Property("width", "200")
            .Property("height", "80")
            .Build();
        Assert.Equal(2, node.Properties.Count);
        Assert.Equal("200", node.Properties["width"]);
        Assert.Equal("80", node.Properties["height"]);
    }

    [Fact(DisplayName = "Child creates recursive subtree")]
    public void ChildCreatesRecursiveSubtree()
    {
        var node = GuiBuilder.Create("panel", "root")
            .Child("button", "enter", child => child.Text("Enter"))
            .Build();
        Assert.Single(node.Children);
        Assert.Equal("button", node.Children[0].Kind);
        Assert.Equal("enter", node.Children[0].Id);
        Assert.Equal("Enter", node.Children[0].Text);
    }

    [Fact(DisplayName = "Child configuration is isolated from parent")]
    public void ChildConfigurationIsIsolated()
    {
        var node = GuiBuilder.Create("panel", "root")
            .Property("scope", "parent")
            .Child("button", "child", child => child.Property("scope", "child"))
            .Build();
        Assert.Equal("parent", node.Properties["scope"]);
        Assert.Equal("child", node.Children[0].Properties["scope"]);
    }

    [Fact(DisplayName = "Multiple children preserve insertion order")]
    public void MultipleChildrenPreserveInsertionOrder()
    {
        var node = GuiBuilder.Create("panel", "root")
            .Child("label", "first")
            .Child("button", "second")
            .Child("image", "third")
            .Build();
        Assert.Equal(new[] { "first", "second", "third" }, node.Children.Select(x => x.Id));
    }

    [Fact(DisplayName = "Nested children build at arbitrary depth")]
    public void NestedChildrenBuildAtArbitraryDepth()
    {
        var node = GuiBuilder.Create("panel", "root")
            .Child("panel", "middle", middle =>
                middle.Child("button", "leaf", leaf => leaf.Text("Leaf")))
            .Build();
        Assert.Equal("leaf", node.Find("leaf").Id);
        Assert.Equal("Leaf", node.Find("leaf").Text);
    }

    [Fact(DisplayName = "Null child configuration is permitted")]
    public void NullChildConfigurationIsPermitted()
        => Assert.Single(GuiBuilder.Create("panel", "root").Child("label", "child", null).Build().Children);

    [Fact(DisplayName = "Builder methods return same builder for fluent composition")]
    public void FluentMethodsReturnSameBuilder()
    {
        var builder = GuiBuilder.Create("button", "x");
        Assert.Same(builder, builder.Text("x"));
        Assert.Same(builder, builder.Image("x"));
        Assert.Same(builder, builder.Property("x", "y"));
        Assert.Same(builder, builder.Child("label", "y"));
    }
}
