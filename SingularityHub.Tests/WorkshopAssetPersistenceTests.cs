using System.Collections.Generic;
using System.Threading.Tasks;
using TheSingularityWorkshop.Services;
using TheSingularityWorkshop.Workshop.Creation;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.IO;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopAssetPersistenceTests
{
    [Fact]
    public void NamedGuiAndFsmBlueprintRoundTripThroughBinary()
    {
        var gui = GuiBuilder.Create("Panel", "root")
            .Property("class", "workshop")
            .Child("Button", "enter", button => button
                .Child("Text", "label", label => label.Text("Enter Workshop")))
            .Build();

        var fsm = new FsmBlueprint(
            "Door",
            ["Closed", "Open"],
            "Closed",
            [new FsmTransitionBlueprint("Closed", "Open", "user.activate")]);

        var asset = new WorkshopAsset("Front Door", gui, fsm);
        var restored = WorkshopAssetBinaryCodec.Deserialize(
            WorkshopAssetBinaryCodec.Serialize(asset));

        Assert.Equal("Front Door", restored.Name);
        Assert.Equal("Panel", restored.Gui.Kind);
        Assert.Equal("workshop", restored.Gui.Properties["class"]);
        Assert.Equal("Enter Workshop", restored.Gui.Find("label").Text);
        Assert.NotNull(restored.Fsm);
        Assert.Equal(["Closed", "Open"], restored.Fsm!.States);
        Assert.Equal("Closed", restored.Fsm.InitialState);
        Assert.Equal("user.activate", restored.Fsm.Transitions[0].Condition);
    }

    [Fact]
    public async Task LibraryPersistsBinaryArtifactAndRehydratesIt()
    {
        var storage = new TestStorage();
        var library = new WorkshopAssetLibrary(storage);
        var gui = GuiBuilder.Create("Panel", "root")
            .Child("Text", "title", title => title.Text("Hello Workshop"))
            .Build();

        await library.SaveAsync(new WorkshopAsset("Hello", gui));

        var names = await library.ListAsync();
        var restored = await library.LoadAsync("Hello");

        Assert.Contains("Hello", names);
        Assert.NotNull(restored);
        Assert.Equal("Hello Workshop", restored!.Gui.Find("title").Text);
        Assert.NotNull(storage.Values["workshop.asset.Hello"]);
        Assert.DoesNotContain("Hello Workshop", storage.Values["workshop.asset.Hello"]);
    }

    private sealed class TestStorage : IWorkshopStorage
    {
        public Dictionary<string, string> Values { get; } = new();

        public ValueTask<string?> GetAsync(string key)
            => ValueTask.FromResult(Values.TryGetValue(key, out var value) ? value : null);

        public ValueTask SetAsync(string key, string value)
        {
            Values[key] = value;
            return ValueTask.CompletedTask;
        }

        public ValueTask RemoveAsync(string key)
        {
            Values.Remove(key);
            return ValueTask.CompletedTask;
        }
    }
}
