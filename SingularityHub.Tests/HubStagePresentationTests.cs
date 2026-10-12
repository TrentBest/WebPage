using System;
using System.IO;
using Xunit;

namespace SingularityHub.Tests;

public sealed class HubStagePresentationTests
{
    [Fact(DisplayName = "Hub arrival overlays navigation without shrinking the moniker stage")]
    public void HubArrivalKeepsMonikerStageFullWidth()
    {
        var root = FindRepositoryRoot();
        var layoutPath = Path.Combine(root, "TheSingularityWorkshop", "Layout", "MainLayout.razor");
        var layoutCssPath = Path.Combine(root, "TheSingularityWorkshop", "Layout", "MainLayout.razor.css");
        var navPath = Path.Combine(root, "TheSingularityWorkshop", "Layout", "NavMenu.razor");

        var layout = File.ReadAllText(layoutPath);
        var layoutCss = File.ReadAllText(layoutCssPath);
        var nav = File.ReadAllText(navPath);

        Assert.DoesNotContain(
            ".workshop-page.workshop-hub-split article.workshop-content { flex: 1 1 0%; width: 0;",
            layout,
            StringComparison.Ordinal);
        Assert.Contains(
            ".workshop-page.workshop-hub-split > .workshop-sidebar {",
            layoutCss,
            StringComparison.Ordinal);
        Assert.Contains(
            ".workshop-page.workshop-hub-split .workshop-content {",
            layoutCss,
            StringComparison.Ordinal);
        Assert.Contains("private bool collapseNavMenu = true;", nav, StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(
                    directory.FullName,
                    "TheSingularityWorkshop",
                    "Layout",
                    "MainLayout.razor")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate the WebPage repository root from the test output directory.");
    }
}
