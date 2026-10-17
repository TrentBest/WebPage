using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Protects the public About Us URL used by existing external links, including the Unity package.
/// The page remains a direct route; it is not dependent on a visitor unlocking a hub destination.
/// </summary>
public sealed class AboutUsCompatibilityTests
{
    [Fact]
    public void AboutUsPageKeepsItsStablePublicRouteAndExistingContent()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        var aboutUs = File.ReadAllText(Path.Combine(root, "TheSingularityWorkshop", "Pages", "AboutUs.razor"));

        Assert.StartsWith("@page \"/about-us\"", aboutUs);
        Assert.Contains("We are not building one giant application.", aboutUs);
        Assert.Contains("01 // THE PROBLEM", aboutUs);
        Assert.Contains("02 // THE ARCHITECTURE", aboutUs);
        Assert.Contains("FSM_API", aboutUs);
        Assert.Contains("FSM_COS", aboutUs);
    }

    [Fact]
    public void AboutUsRemainsReachableFromWorkshopNavigation()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
        var navigation = File.ReadAllText(Path.Combine(root, "TheSingularityWorkshop", "Layout", "NavMenu.razor"));

        Assert.Contains("href=\"/about-us\"", navigation);
        Assert.Contains(">About Us</NavLink>", navigation);
    }
}
