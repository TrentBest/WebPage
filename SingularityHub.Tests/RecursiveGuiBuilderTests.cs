using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class RecursiveGuiBuilderTests
{
    [Fact(DisplayName = "Incremental Unit Test 05 — Gateway is a recursively composed GUI tree")]
    public void IncrementalUnitTest05_GatewayIsRecursivelyComposed()
    {
        const string avatar = "https://avatars.githubusercontent.com/u/16405167?v=4";

        var gateway = GuiBuilder
            .Create("Panel", "gateway")
            .Child("Button", "enter-workshop", button => button
                .Child("Image", "gateway-avatar", image => image
                    .Image(avatar))
                .Child("Text", "enter-workshop-label", text => text
                    .Text("Enter Workshop")))
            .Child("Warning", "system-advisory", warning => warning
                .Text("SYSTEM ADVISORY: MAXIMUM OVERDRIVE ACTIVE"))
            .Build();

        Assert.Equal("Panel", gateway.Kind);
        Assert.Equal("gateway", gateway.Id);
        Assert.Equal(2, gateway.Children.Count);

        var enter = gateway.Find("enter-workshop");
        Assert.Equal("Button", enter.Kind);
        Assert.Equal(2, enter.Children.Count);

        var avatarNode = enter.Find("gateway-avatar");
        Assert.Equal("Image", avatarNode.Kind);
        Assert.Equal(avatar, avatarNode.Source);

        var label = enter.Find("enter-workshop-label");
        Assert.Equal("Text", label.Kind);
        Assert.Equal("Enter Workshop", label.Text);

        var warning = gateway.Find("system-advisory");
        Assert.Equal("Warning", warning.Kind);
        Assert.Equal("SYSTEM ADVISORY: MAXIMUM OVERDRIVE ACTIVE", warning.Text);
    }

    [Fact(DisplayName = "Incremental Unit Test 06 — recursive children remain independent subtrees")]
    public void IncrementalUnitTest06_RecursiveChildrenRemainIndependentSubtrees()
    {
        var root = GuiBuilder
            .Create("Panel", "root")
            .Child("Panel", "left", left => left
                .Child("Button", "left-button", button => button.Text("Left")))
            .Child("Panel", "right", right => right
                .Child("Button", "right-button", button => button.Text("Right")))
            .Build();

        Assert.Equal("Left", root.Find("left-button").Text);
        Assert.Equal("Right", root.Find("right-button").Text);
        Assert.Throws<InvalidOperationException>(() => root.Find("missing"));
    }
}
