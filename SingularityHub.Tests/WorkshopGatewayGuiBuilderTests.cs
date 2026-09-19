using Xunit;

namespace SingularityHub.Tests;

/// <summary>
/// Structural proof that the public landing gateway is expressed through the
/// recursive GUI-builder boundary rather than hand-authored Razor structure.
/// </summary>
public sealed class WorkshopGatewayGuiBuilderTests
{
    [Fact(DisplayName = "Opening gateway is built by the recursive Workshop GUI vocabulary")]
    public void OpeningGatewayUsesRecursiveGuiBuilder()
    {
        var source = File.ReadAllText(
            Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..",
                "TheSingularityWorkshop",
                "Gui",
                "WorkshopGatewayGuiBuilder.cs"));

        Assert.Contains("WorkshopGui.Button", source);
        Assert.Contains("WorkshopGui.Panel", source);
        Assert.Contains("WorkshopGui.Image", source);
        Assert.Contains(".Child(", source);
        Assert.DoesNotContain("<button", source, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<img", source, StringComparison.OrdinalIgnoreCase);

        var home = File.ReadAllText(Path.Combine(\n            AppContext.BaseDirectory,\n            "..", "..", "..", "..",\n            "TheSingularityWorkshop",\n            "Pages",\n            "Home.razor"));\n\n        Assert.DoesNotContain("<button", home, StringComparison.OrdinalIgnoreCase);\n        Assert.DoesNotContain("<img", home, StringComparison.OrdinalIgnoreCase);
    }
}
