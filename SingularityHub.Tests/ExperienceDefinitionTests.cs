using System;
using TheSingularityWorkshop.Workshop.Creation;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Verifies the semantic boundary between Experiences and GUI definitions.</summary>
public sealed class ExperienceDefinitionTests
{
    [Fact(DisplayName = "V0.0.161 — Experience_Contains_Named_GUI_Surfaces")]
    public void ExperienceContainsNamedGuiSurfaces()
    {
        var gui = GuiBuilder.Create("Panel", "root")
            .Child("Button", "enter", button => button.Text("Enter"))
            .Build();

        var experience = new ExperienceDefinition(
            "Creation Bay",
            [new ExperienceSurfaceDefinition("main", gui)]);

        var surface = experience.FindSurface("main");

        Assert.Equal("Creation Bay", experience.Name);
        Assert.Same(gui, surface.Gui);
        Assert.Null(surface.Fsm);
    }

    [Fact(DisplayName = "V0.0.162 — Experience_GUI_Does_Not_Own_Platform_Runtime")]
    public void ExperienceGuiDoesNotOwnPlatformRuntime()
    {
        var gui = GuiBuilder.Create("Panel", "root").Build();
        var experience = new ExperienceDefinition(
            "Workshop",
            [new ExperienceSurfaceDefinition("shell", gui)]);

        Assert.IsNotAssignableFrom<ICoreGuiBuilder<GuiNode>>(
            GuiBuilder.Create("Panel", "builder"));

        Assert.Equal("Panel", experience.FindSurface("shell").Gui.Kind);
    }

    [Fact(DisplayName = "V0.0.163 — Experience_Rejects_Unknown_Surface")]
    public void ExperienceRejectsUnknownSurface()
    {
        var gui = GuiBuilder.Create("Panel", "root").Build();
        var experience = new ExperienceDefinition(
            "Workshop",
            [new ExperienceSurfaceDefinition("shell", gui)]);

        Assert.Throws<InvalidOperationException>(
            () => experience.FindSurface("missing"));
    }
}
