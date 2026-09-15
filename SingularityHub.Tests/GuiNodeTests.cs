using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Full unit coverage for the immutable GUI intermediate representation.</summary>
public sealed class GuiNodeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ConstructorRejectsMissingKind(string? kind)
        => Assert.Throws<ArgumentException>(() => new GuiNode(kind!, "id"));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ConstructorRejectsMissingId(string? id)
        => Assert.Throws<ArgumentException>(() => new GuiNode("button", id!));

    [Fact(DisplayName = "Constructor preserves scalar values")]
    public void ConstructorPreservesScalarValues()
    {
        var node = new GuiNode("button", "enter", "Enter", "avatar.png");
        Assert.Equal("button", node.Kind);
        Assert.Equal("enter", node.Id);
        Assert.Equal("Enter", node.Text);
        Assert.Equal("avatar.png", node.Source);
    }

    [Fact(DisplayName = "Properties are copied from input")]
    public void PropertiesAreCopied()
    {
        var source = new Dictionary<string, string> { ["class"] = "hero" };
        var node = new GuiNode("button", "enter", properties: source);
        source["class"] = "mutated";
        Assert.Equal("hero", node.Properties["class"]);
    }

    [Fact(DisplayName = "Children are copied from input")]
    public void ChildrenAreCopied()
    {
        var child = new GuiNode("label", "child");
        var source = new List<GuiNode> { child };
        var node = new GuiNode("panel", "root", children: source);
        source.Clear();
        Assert.Single(node.Children);
        Assert.Same(child, node.Children[0]);
    }

    [Fact(DisplayName = "Missing optional values create empty collections")]
    public void OptionalCollectionsAreEmpty()
    {
        var node = new GuiNode("panel", "root");
        Assert.Empty(node.Properties);
        Assert.Empty(node.Children);
        Assert.Null(node.Text);
        Assert.Null(node.Source);
    }

    [Fact(DisplayName = "Find returns current node")]
    public void FindReturnsCurrentNode()
    {
        var node = new GuiNode("panel", "root");
        Assert.Same(node, node.Find("root"));
    }

    [Fact(DisplayName = "Find returns direct child")]
    public void FindReturnsDirectChild()
    {
        var child = new GuiNode("button", "child");
        var node = new GuiNode("panel", "root", children: new[] { child });
        Assert.Same(child, node.Find("child"));
    }

    [Fact(DisplayName = "Find searches recursively")]
    public void FindSearchesRecursively()
    {
        var leaf = new GuiNode("label", "leaf");
        var middle = new GuiNode("panel", "middle", children: new[] { leaf });
        var root = new GuiNode("panel", "root", children: new[] { middle });
        Assert.Same(leaf, root.Find("leaf"));
    }

    [Fact(DisplayName = "Find continues through sibling subtrees")]
    public void FindContinuesThroughSiblingSubtrees()
    {
        var first = new GuiNode("panel", "first", children: new[] { new GuiNode("label", "not-it") });
        var target = new GuiNode("button", "target");
        var root = new GuiNode("panel", "root", children: new[] { first, target });
        Assert.Same(target, root.Find("target"));
    }

    [Fact(DisplayName = "Find throws when id is absent")]
    public void FindThrowsWhenAbsent()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new GuiNode("panel", "root").Find("missing"));
        Assert.Contains("missing", exception.Message);
        Assert.Contains("root", exception.Message);
    }

    [Fact(DisplayName = "Find does not mutate node")]
    public void FindDoesNotMutateNode()
    {
        var node = new GuiNode("panel", "root", children: new[] { new GuiNode("label", "child") });
        _ = node.Find("child");
        Assert.Single(node.Children);
        Assert.Equal("child", node.Children[0].Id);
    }
}
