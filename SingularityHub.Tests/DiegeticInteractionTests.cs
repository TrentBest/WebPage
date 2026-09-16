using TheSingularityWorkshop.Gui;
using TheSingularityWorkshop.Workshop.Experience;
using TheSingularityWorkshop.Workshop.Interaction;

namespace SingularityHub.Tests;

public sealed class DiegeticInteractionTests
{
    [Fact(DisplayName = "Spatial catalog execution enters the selected Experience")]
    public void SpatialCatalog_ExecutesEntryBehavior()
    {
        var spaces = new[]
        {
            new ExperienceSpace(
                "spaceship",
                "Singularity Spaceship Hangar",
                "VESSEL OPERATIONS",
                48, 4, 60, 15, 60, 15,
                "A hangar for player-operated vessels.",
                "VESSEL",
                "Cockpit / navigation / propulsion / systems",
                30,
                12)
        };

        string? entered = null;
        var catalog = WorkshopInteractableCatalog.Create(spaces, id => entered = id);
        var interactable = catalog["spaceship"];

        var context = new InteractionContext(
            "workshop-floor",
            InteractionScope.World,
            new HashSet<string>(StringComparer.Ordinal));

        var executed = new InteractableExecutor().TryExecute(
            interactable,
            InteractionTrigger.Click,
            context);

        Assert.True(executed);
        Assert.Equal("spaceship", entered);
    }

    [Fact(DisplayName = "Spatial catalog exposes transportation and vessel map intent")]
    public void SpatialCatalog_DeclaresDiegeticMapIntent()
    {
        var spaces = new[]
        {
            new ExperienceSpace(
                "tram-terminal",
                "Singularity City Tram",
                "CITY TRANSIT",
                18, 88, 29, 96, 29, 96,
                "A transportation Experience.",
                "TRANSIT",
                "Tram / city circulation / announcements",
                30,
                9),
            new ExperienceSpace(
                "spaceship",
                "Singularity Spaceship Hangar",
                "VESSEL OPERATIONS",
                48, 4, 60, 15, 60, 15,
                "A hangar for player-operated vessels.",
                "VESSEL",
                "Cockpit / navigation / propulsion / systems",
                30,
                12)
        };

        var catalog = WorkshopInteractableCatalog.Create(spaces);

        Assert.Equal("TRANSIT", catalog["tram-terminal"].Presentation.MapIntent.ThreadLabel);
        Assert.Equal("VESSEL", catalog["spaceship"].Presentation.MapIntent.ThreadLabel);
    }
}
