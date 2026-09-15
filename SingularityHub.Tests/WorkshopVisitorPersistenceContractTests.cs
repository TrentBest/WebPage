using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopVisitorPersistenceContractTests
{
    [Fact(DisplayName = "Workshop visitor persistence uses the Workshop storage boundary")]
    public void VisitorPersistenceUsesWorkshopStorageBoundary()
    {
        var source = File.ReadAllText(FindRepositoryFile("TheSingularityWorkshop", "Services", "BrowserWorkshopStorage.cs"));

        Assert.Contains("IWorkshopStorage", source);
        Assert.Contains("localStorage.getItem", source);
        Assert.Contains("localStorage.setItem", source);
        Assert.Contains("localStorage.removeItem", source);
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
        throw new FileNotFoundException("Could not locate the repository source file.", Path.Combine(relativeParts));
    }
}
