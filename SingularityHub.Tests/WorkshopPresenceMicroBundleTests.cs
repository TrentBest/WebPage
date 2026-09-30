using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class WorkshopPresenceMicroBundleTests
{
    [Fact(DisplayName = "Workshop presence exposes deterministic remote participants")]
    public void PresenceStartsWithSimulatedParticipants()
    {
        using var presence = new WorkshopPresenceMicroBundle();

        Assert.Equal((ulong)WorkshopPresenceMicroBundle.BundleId, presence.Id);
        Assert.Equal(WorkshopPresenceSource.Simulated, presence.Source);
        Assert.Equal(4, presence.Participants.Count);
        Assert.All(presence.Participants, participant => Assert.Equal(WorkshopPresenceKind.Simulated, participant.Kind));
    }

    [Fact(DisplayName = "Workshop presence can accept connected participants without changing the presentation contract")]
    public void PresenceCanSwapToConnectedParticipants()
    {
        using var presence = new WorkshopPresenceMicroBundle();

        presence.SetConnectedParticipants(
        [
            new("connected-01", "Alice", 40, 40, WorkshopPresenceKind.Connected),
            new("connected-02", "Bob", 60, 60, WorkshopPresenceKind.Connected)
        ]);

        Assert.Equal(WorkshopPresenceSource.Connected, presence.Source);
        Assert.Equal(new[] { "Alice", "Bob" }, presence.Participants.Select(x => x.DisplayName));
        Assert.All(presence.Participants, participant => Assert.Equal(WorkshopPresenceKind.Connected, participant.Kind));
    }

    [Fact(DisplayName = "Workshop presence simulation can be restored")]
    public void PresenceCanRestoreSimulation()
    {
        using var presence = new WorkshopPresenceMicroBundle();
        presence.SetConnectedParticipants([new("connected-01", "Alice", 40, 40, WorkshopPresenceKind.Connected)]);

        presence.ResetSimulation();

        Assert.Equal(WorkshopPresenceSource.Simulated, presence.Source);
        Assert.Equal(4, presence.Participants.Count);
    }
}
