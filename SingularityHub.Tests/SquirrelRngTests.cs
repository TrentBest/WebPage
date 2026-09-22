using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using TheSingularityWorkshop.Workshop.Randomness;
using Xunit;

namespace SingularityHub.Tests;

public sealed class SquirrelRngTests
{
    [Fact]
    public void Same_Position_And_Seed_Produce_The_Same_Value()
    {
        Assert.Equal(0xC895CB1Du, SquirrelRng.Noise(1));
        Assert.Equal(0xD6C56929u, SquirrelRng.Noise(1, 42));
        Assert.Equal(SquirrelRng.Noise(1234, 77), SquirrelRng.Noise(1234, 77));
        Assert.NotEqual(SquirrelRng.Noise(1234, 77), SquirrelRng.Noise(1235, 77));
    }

    [Fact]
    public void Streams_With_The_Same_Seed_Are_Reproducible()
    {
        var first = new SquirrelRng(42);
        var second = new SquirrelRng(42);

        var a = Enumerable.Range(0, 64).Select(_ => first.NextUInt()).ToArray();
        var b = Enumerable.Range(0, 64).Select(_ => second.NextUInt()).ToArray();

        Assert.Equal(a, b);
    }

    [Fact]
    public async Task Stream_Position_Is_Atomic_When_Consumed_Concurrently()
    {
        var rng = new SquirrelRng(99);
        var values = new ConcurrentBag<uint>();

        await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            for (var i = 0; i < 1_000; i++)
                values.Add(rng.NextUInt());
        })));

        Assert.Equal(8_000, values.Count);
        Assert.Equal(7_999, rng.Position);
    }
}
