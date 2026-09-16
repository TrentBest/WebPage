using TheSingularityWorkshop.Gui;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Incremental Unit Test 24 — linework is data first and renderer second.</summary>
public sealed class SpatialLineworkTests
{
    [Fact(DisplayName = "Incremental Unit Test 24 — linework FSM enters drawing and commits endpoint pairs")]
    public void SpatialLinework_DrawsAndCommitsLine()
    {
        using var linework = new SpatialLinework();

        Assert.Equal("Idle", linework.State);

        linework.BeginLine(new SpatialPoint(10, 20));
        Assert.Equal("Drawing", linework.State);

        linework.ContinueLine(new SpatialPoint(40, 55));
        var line = linework.EndLine();

        Assert.Equal("Idle", linework.State);
        Assert.Single(linework.Lines);
        Assert.Equal(new SpatialPoint(10, 20), line.Start);
        Assert.Equal(new SpatialPoint(40, 55), line.End);
    }

    [Fact(DisplayName = "Incremental Unit Test 24 — linework encodes one endpoint pair per RGBA texture texel")]
    public void SpatialLinework_EncodesTextureBuffer()
    {
        using var linework = new SpatialLinework();
        linework.BeginLine(new SpatialPoint(1, 2));
        linework.ContinueLine(new SpatialPoint(3, 4));
        linework.EndLine();

        linework.BeginLine(new SpatialPoint(10, 20));
        linework.ContinueLine(new SpatialPoint(30, 40));
        linework.EndLine();

        var buffer = linework.TextureBuffer;

        Assert.Equal(2, buffer.Width);
        Assert.Equal(1, SpatialLineTextureBuffer.Height);
        Assert.Equal(new SpatialLineTextureTexel(1, 2, 3, 4), buffer.Texels[0]);
        Assert.Equal(new SpatialLineTextureTexel(10, 20, 30, 40), buffer.Texels[1]);
    }
}
