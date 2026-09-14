using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Verifies the browser persistence boundary used by the Workshop landing page.
/// This is a source-contract test because the behavior lives at the Blazor/DOM boundary.
/// </summary>
public sealed class WorkshopVisitorPersistenceContractTests
{
    [Fact(DisplayName = "Workshop visitor persistence uses browser localStorage contract")]
    public void VisitorPersistenceUsesBrowserLocalStorageContract()
    {
        var source = File.ReadAllText(FindRepositoryFile("TheSingularityWorkshop", "Pages", "Home.razor"));

        Assert.Contains("JS.InvokeAsync<string?>(\"localStorage.getItem\", \"WorkshopVisited\")", source);
        Assert.Contains("JS.InvokeVoidAsync(\"localStorage.setItem\", \"WorkshopVisited\", \"true\")", source);
        Assert.DoesNotContain("interop.workshopVisited", source);
        Assert.DoesNotContain("interop.markWorkshopVisited", source);
    }

    private static string FindRepositoryFile(params string[] relativeParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var candidate = Path.Combine(new[] { directory.FullName }.Concat(relativeParts).ToArray());
            if (File.Exists(candidate)) return candidate;
            directory = directory.Parent;
        }

        throw new FileNotFoundException(
            "Could not locate the repository source file needed by the browser persistence contract test.",
            Path.Combine(relativeParts));
    }
}
