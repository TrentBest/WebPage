using System;
using System.Runtime.InteropServices;
using TheSingularityWorkshop.Digitens;
using Xunit;

namespace SingularityHub.Tests;

public sealed class DigitenStateTests
{
    [Fact]
    public void DigitenIsAValidFsmContextWithStableNumericIdentity()
    {
        var digiten = new Digiten(42);

        Assert.True(digiten.IsValid);
        Assert.Equal("Digiten:000000000000002a", digiten.Name);
        Assert.Equal((ulong)42, digiten.State.Id);
        Assert.Equal(DigitenActivity.Idle, digiten.State.Activity);
    }

    [Fact]
    public void DeviceStateIsAReferenceFreeValueType()
    {
        Assert.True(typeof(DigitenState).IsValueType);
        Assert.False(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<DigitenState>());
        Assert.Equal(68, Marshal.SizeOf<DigitenState>());
    }

    [Fact]
    public void StateCanChangeWithoutChangingIdentity()
    {
        var digiten = new Digiten(42);
        var state = digiten.State;
        state.Activity = DigitenActivity.Working;
        state.Employment = DigitenEmploymentStatus.Employed;
        state.WorkplaceId = 7001;
        state.Hunger = 80;

        digiten.SetState(state);

        Assert.Equal((ulong)42, digiten.State.Id);
        Assert.Equal(DigitenActivity.Working, digiten.State.Activity);
        Assert.Equal(DigitenEmploymentStatus.Employed, digiten.State.Employment);
        Assert.Equal((uint)7001, digiten.State.WorkplaceId);
        Assert.Equal((byte)80, digiten.State.Hunger);
    }

    [Fact]
    public void StateCannotChangeStableIdentity()
    {
        var digiten = new Digiten(42);
        var replacement = digiten.State;
        replacement.Id = 43;

        Assert.Throws<ArgumentException>(() => digiten.SetState(replacement));
        Assert.Equal((ulong)42, digiten.State.Id);
    }

    [Fact]
    public void InvalidatedDigitenCannotBeUpdated()
    {
        var digiten = new Digiten(42);
        digiten.Invalidate();

        Assert.False(digiten.IsValid);
        Assert.Throws<InvalidOperationException>(() => digiten.SetState(digiten.State));
    }

    [Fact]
    public void ZeroIsNotAValidStableIdentity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Digiten(0));
    }
}
