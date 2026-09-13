using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests;

public sealed class IncrementalVersion071Tests
{
    [ArchitectureTest(0, 0, 71)]
    [Fact(DisplayName = "0.00.071 — Pong_Physics_Is_Advanced_By_FSM_API")]
    public void PongPhysicsIsAdvancedByFsmApi()
    {
        var pong = new PongMicroBundle();
        var initialX = pong.BallX;

        pong.Update();

        Assert.NotEqual(initialX, pong.BallX);
        Assert.Equal("PLAYING", pong.GameState);
    }
}
