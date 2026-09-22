using TheSingularityWorkshop.Services;
using Xunit;
using TheSingularityWorkshop.Workshop.Plant;
using TheSingularityWorkshop.Workshop.Sound;

namespace LivingGuiExperience.Tests;

public sealed class FirstContactPlantTests
{
    [Fact]
    public void Plant_Grows_Blooms_And_Opens_The_Hub()
    {
        using var plant = new PlantGrowthMicroBundle();
        plant.Start();

        for (var i = 0; i < 400 && !plant.IsSettled; i++)
            plant.Update();

        Assert.True(plant.IsSettled);
        Assert.Equal(6, plant.Vines.Count);
        Assert.Equal(6, plant.Panels.Count);
        Assert.All(plant.Panels, panel => Assert.Equal(1, panel.OpenProgress));
    }

    [Fact]
    public void FirstContact_Transitions_To_Conventional_Landing()
    {
        using var experience = new WorkshopExperienceService();
        experience.Initialize(returningVisitor: false);

        for (var i = 0; i < 500 && experience.CurrentState == "FirstContact"; i++)
            experience.Tick();

        Assert.Equal("Intro", experience.CurrentState);
        Assert.NotNull(experience.Plant);
        Assert.True(experience.Plant!.IsSettled);
    }

    [Fact]
    public void FirstContact_Uses_An_Authored_Cavern_Environment()
    {
        var scene = SfxScene.FirstContactCavern();

        Assert.Equal("Large Open Cave", scene.Environment.Name);
        Assert.True(scene.Environment.ReverbSeconds > 0);
        Assert.Equal(4, scene.Emitters.Count);
    }
}
