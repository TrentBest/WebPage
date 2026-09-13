using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests.MicroBundles;

/// <summary>Complete public-surface and lifecycle coverage for <see cref="MicroBundleContext"/>.</summary>
public sealed class MicroBundleContextTests
{
    [Fact] public void ConstructorPreservesIdentity() { var c = new MicroBundleContext(42, "Widget"); Assert.Equal(42, c.Id); Assert.Equal("Widget", c.Name); }
    [Fact] public void ConstructorStartsInvalid() { var c = new MicroBundleContext(1, "x"); Assert.False(c.IsValid); }
    [Fact] public void ConstructorStartsUninvalidated() { var c = new MicroBundleContext(1, "x"); Assert.False(c.IsInvalidated); }
    [Fact] public void ConstructorStartsCreated() { var c = new MicroBundleContext(1, "x"); Assert.Equal("Created", c.Phase); }
    [Fact] public void ConstructorStartsAtZeroElapsed() { var c = new MicroBundleContext(1, "x"); Assert.Equal(0L, c.ElapsedMilliseconds); }
    [Fact] public void ConstructorUsesNoParent() { var c = new MicroBundleContext(1, "x"); Assert.Equal(-1, c.ParentId); }
    [Fact] public void ConstructorStartsGenerationZero() { var c = new MicroBundleContext(1, "x"); Assert.Equal(0, c.Generation); }
    [Fact] public void NameCanChange() { var c = new MicroBundleContext(1, "x"); c.Name = "y"; Assert.Equal("y", c.Name); }
    [Fact] public void ValidityCanBeEnabled() { var c = new MicroBundleContext(1, "x"); c.IsValid = true; Assert.True(c.IsValid); }
    [Fact] public void ValidityCanBeDisabled() { var c = new MicroBundleContext(1, "x") { IsValid = true }; c.IsValid = false; Assert.False(c.IsValid); }
    [Fact] public void InvalidationCanBeEnabled() { var c = new MicroBundleContext(1, "x"); c.IsInvalidated = true; Assert.True(c.IsInvalidated); }
    [Fact] public void PhaseCanChange() { var c = new MicroBundleContext(1, "x"); c.Phase = "Active"; Assert.Equal("Active", c.Phase); }
    [Fact] public void ElapsedTimeCanAdvance() { var c = new MicroBundleContext(1, "x"); c.ElapsedMilliseconds = 123; Assert.Equal(123L, c.ElapsedMilliseconds); }
    [Fact] public void ParentCanBeAssigned() { var c = new MicroBundleContext(1, "x"); c.ParentId = 99; Assert.Equal(99, c.ParentId); }
    [Fact] public void GenerationCanBeAssigned() { var c = new MicroBundleContext(1, "x"); c.Generation = 7; Assert.Equal(7, c.Generation); }
    [Fact] public void StateInputsAreIndependent() { var c = new MicroBundleContext(1, "x") { IsValid = true, IsInvalidated = true, Phase = "Collapsing", ElapsedMilliseconds = 8, ParentId = 2, Generation = 3 }; Assert.True(c.IsValid); Assert.True(c.IsInvalidated); Assert.Equal("Collapsing", c.Phase); Assert.Equal(8L, c.ElapsedMilliseconds); Assert.Equal(2, c.ParentId); Assert.Equal(3, c.Generation); }
}
