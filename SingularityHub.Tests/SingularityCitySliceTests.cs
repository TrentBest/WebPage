using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SingularityCitySliceTests
{
    [Fact]
    public void CivicDirectoryHasStableUniqueIdentitiesAndAResolvableMayor()
    {
        var officials = SingularityCivicDigitenCatalog.Officials;

        Assert.Equal(11, officials.Count);
        Assert.Equal(officials.Count, officials.Select(official => official.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal("Mayor Mara", SingularityCivicDigitenCatalog.Mayor.Name);
        Assert.Equal("Mayor Mara", SingularityCivicDigitenCatalog.Resolve("DIGITEN.MAYOR").Name);
        Assert.Contains(SingularityCivicDigitenCatalog.Mayor.CommitteeAssignments, assignment => assignment == "Executive Priorities");
    }

    [Fact]
    public void MansionFeaturesHaveStableIdsAndStayInsideTheNormalizedPlan()
    {
        var features = SingularityMansionArchitecture.Features;

        Assert.Equal(9, features.Count);
        Assert.Equal(features.Count, features.Select(feature => feature.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());

        foreach (var feature in features)
        {
            Assert.True(feature.X >= 0 && feature.Y >= 0, $"{feature.Id} starts outside the plan.");
            Assert.True(feature.Width > 0 && feature.Height > 0, $"{feature.Id} has no visible area.");
            Assert.True(feature.X + feature.Width <= 100, $"{feature.Id} exceeds the plan width.");
            Assert.True(feature.Y + feature.Height <= 100, $"{feature.Id} exceeds the plan height.");
        }

        Assert.Equal(4, SingularityMansionArchitecture.Alcoves.Count);
        Assert.Equal("Grand Entrance", SingularityMansionArchitecture.Resolve("front-entry").Name);
    }

    [Fact]
    public void PresenceContractSeparatesLiveAvatarFromDurableWorldState()
    {
        Assert.Contains("CONNECTED = VISIBLE AVATAR", SingularityDigitenPresenceRules.PresenceContract);
        Assert.Contains("DISCONNECTED = REMOVE LIVE AVATAR", SingularityDigitenPresenceRules.PresenceContract);
        Assert.Contains("PERSISTED WORLD STATE = RETAIN DURABLE ROLES AND WORLD DATA", SingularityDigitenPresenceRules.PresenceContract);
    }
}
