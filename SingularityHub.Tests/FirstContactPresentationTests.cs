using TheSingularityWorkshop.Services;
using Xunit;

namespace SingularityHub.Tests;

public sealed class FirstContactPresentationTests
{
    [Fact(DisplayName = "First contact crossfades the question 180 degrees out of phase")]
    public void FirstContactQuestionCrossfadeIsPhaseOpposed()
    {
        using var fsm = new FirstContactFsm();
        fsm.Start();

        Assert.Equal(0d, fsm.StatementOpacity);
        Assert.Equal(0d, fsm.QuestionOpacity);

        while (fsm.CurrentState != "Question")
            fsm.Update();

        Assert.Equal("Question", fsm.CurrentState);
        Assert.InRange(fsm.StatementOpacity, .9d, 1d);
        Assert.InRange(fsm.QuestionOpacity, 0d, .1d);

        while (fsm.StateTicks < 30)
            fsm.Update();

        Assert.Equal(0d, fsm.StatementOpacity);
        Assert.Equal(1d, fsm.QuestionOpacity);

        fsm.Update();

        Assert.Equal("Question", fsm.CurrentState);
        Assert.InRange(fsm.QuestionOpacity, 0d, 1d);
        Assert.True(fsm.QuestionOpacity < 1d);

        while (fsm.CurrentState != "Gateway")
            fsm.Update();

        Assert.Equal("Gateway", fsm.CurrentState);
        Assert.Equal(0d, fsm.StatementOpacity);
        Assert.Equal(0d, fsm.QuestionOpacity);
    }
}
