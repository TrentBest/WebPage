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

        while (fsm.CurrentState != "FirstFadingSecondComingIn")
            fsm.Update();

        Assert.Equal("FirstFadingSecondComingIn", fsm.CurrentState);
        Assert.InRange(fsm.StatementOpacity, .9d, 1d);
        Assert.InRange(fsm.QuestionOpacity, 0d, .1d);

        fsm.Update();

        Assert.Equal("FirstFadingSecondComingIn", fsm.CurrentState);
        Assert.InRange(fsm.StatementOpacity, 0d, 1d);
        Assert.InRange(fsm.QuestionOpacity, 0d, 1d);
        Assert.Equal(
            1d,
            fsm.StatementOpacity + fsm.QuestionOpacity,
            6);

        while (fsm.CurrentState != "SecondOnly")
            fsm.Update();

        Assert.Equal("SecondOnly", fsm.CurrentState);
        Assert.Equal(0d, fsm.StatementOpacity);
        Assert.Equal(1d, fsm.QuestionOpacity);

        fsm.Update();

        Assert.Equal("SecondOnly", fsm.CurrentState);
        Assert.Equal(0d, fsm.StatementOpacity);
        Assert.InRange(fsm.QuestionOpacity, 0d, 1d);
        Assert.True(fsm.QuestionOpacity < 1d);

        while (fsm.CurrentState != "Gateway")
            fsm.Update();

        Assert.Equal("Gateway", fsm.CurrentState);
        Assert.Equal(0d, fsm.StatementOpacity);
        Assert.Equal(0d, fsm.QuestionOpacity);
    }
}
