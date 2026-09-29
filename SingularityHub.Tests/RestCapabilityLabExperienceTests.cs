using System.Text.Json;
using TheSingularityWorkshop.Workshop.Experiences;

namespace SingularityHub.Tests;

public sealed class RestCapabilityLabExperienceTests
{
    [Fact]
    public void SerializesCapabilityRecipeWithoutRuntimeResponse()
    {
        var experience = new RestCapabilityLabExperience();

        experience.AddOperation("GetItem", "GET", "catalog/{id}");

        var json = experience.Serialize();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.Equal("Workshop Catalog API", root.GetProperty("Name").GetString());
        Assert.Equal("https://api.example.com", root.GetProperty("BaseUri").GetString());
        Assert.Equal(2, root.GetProperty("Operations").GetArrayLength());
        Assert.Equal("/catalog/{id}", root.GetProperty("Operations")[1].GetProperty("Path").GetString());
        Assert.False(json.Contains("response", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NormalizesOperationMethodAndPath()
    {
        var experience = new RestCapabilityLabExperience();

        experience.AddOperation("Create", "post", "items");

        var operation = Assert.Single(experience.Operations.Where(x => x.Name == "Create"));

        Assert.Equal("POST", operation.Method);
        Assert.Equal("/items", operation.Path);
    }

    [Fact]
    public void RemoveOperationDoesNotBreakRecipe()
    {
        var experience = new RestCapabilityLabExperience();

        experience.AddOperation("GetItem", "GET", "/catalog/{id}");
        experience.RemoveOperation(0);

        Assert.Single(experience.Operations);
        Assert.Equal("GetCatalog", experience.Operations[0].Name);
    }
}
