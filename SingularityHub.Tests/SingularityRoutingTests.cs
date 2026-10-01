using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Exhaustive unit coverage for the Hub-owned in-memory routing implementation.</summary>
public sealed class SingularityRoutingTests
{
    [Fact(DisplayName = "Routing starts empty")]
    public void StartsEmpty() => Assert.Empty(new SingularityRouting().Routes);

    [Fact(DisplayName = "Valid route registers and resolves case-insensitively")]
    public void ValidRouteRegistersAndResolvesCaseInsensitively()
    {
        var routing = new SingularityRouting();
        var route = new SingularityRoute(101, "/Workshop", "WORKSHOP", 5001);

        Assert.True(routing.Register(route));
        Assert.Single(routing.Routes);
        Assert.True(routing.TryResolve("/workshop", out var resolved));
        Assert.Equal(route, resolved);
    }

    [Fact(DisplayName = "Duplicate path is rejected")]
    public void DuplicatePathIsRejected()
    {
        var routing = new SingularityRouting();
        Assert.True(routing.Register(new SingularityRoute(1, "/same", "A", 10)));
        Assert.False(routing.Register(new SingularityRoute(2, "/same", "B", 20)));
        Assert.Single(routing.Routes);
    }

    [Theory]
    [InlineData(0, "/x", "X", 1)]
    [InlineData(1, "", "X", 1)]
    [InlineData(1, "   ", "X", 1)]
    [InlineData(1, "/x", "X", 0)]
    public void InvalidRouteIsRejected(ulong routeId, string path, string tab, ulong experienceId)
        => Assert.Throws<ArgumentException>(() => new SingularityRouting().Register(new SingularityRoute(routeId, path, tab, experienceId)));

    [Fact(DisplayName = "Null route path is rejected by validation")]
    public void NullRoutePathIsRejected()
        => Assert.Throws<ArgumentException>(() => new SingularityRouting().Register(new SingularityRoute(1, null!, "X", 1)));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyPathCannotResolve(string path)
    {
        Assert.False(new SingularityRouting().TryResolve(path, out _));
    }

    [Fact]
    public void NullPathCannotResolve()
        => Assert.False(new SingularityRouting().TryResolve(null!, out _));

    [Fact(DisplayName = "Unknown path cannot resolve")]
    public void UnknownPathCannotResolve()
        => Assert.False(new SingularityRouting().TryResolve("/missing", out _));

    [Fact(DisplayName = "Remove deletes existing route")]
    public void RemoveDeletesExistingRoute()
    {
        var routing = new SingularityRouting();
        routing.Register(new SingularityRoute(1, "/x", "X", 2));
        Assert.True(routing.Remove("/x"));
        Assert.Empty(routing.Routes);
        Assert.False(routing.TryResolve("/x", out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RemoveReturnsFalseForEmptyPath(string path)
        => Assert.False(new SingularityRouting().Remove(path));

    [Fact]
    public void RemoveReturnsFalseForNullPath()
        => Assert.False(new SingularityRouting().Remove(null!));

    [Fact(DisplayName = "Removing unknown route returns false")]
    public void RemovingUnknownRouteReturnsFalse()
        => Assert.False(new SingularityRouting().Remove("/missing"));
}
