using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SpatialDigitenPresenceTests
{
    [Fact]
    public void ConnectedUserBecomesALiveDigitenAvatar()
    {
        var service = new SpatialDigitenPresenceService();

        var presence = service.Connect("user-1", "Visitor One", new SpatialPoint(50.5, 50.5));

        Assert.Equal("avatar:user-1", presence.DigitenId);
        Assert.Single(service.Connected);
        Assert.True(service.TryGet("user-1", out var current));
        Assert.Equal("Visitor One", current.DisplayName);
    }

    [Fact]
    public void DisconnectRemovesLiveAvatarWithoutChangingPersistentIdentityModel()
    {
        var service = new SpatialDigitenPresenceService();
        service.Connect("user-1", "Visitor One", new SpatialPoint(50.5, 50.5));

        Assert.True(service.Disconnect("user-1"));
        Assert.Empty(service.Connected);
        Assert.False(service.TryGet("user-1", out _));
    }
}
