using TheSingularityWorkshop.Workshop.MicroBundles;
using Xunit;

namespace SingularityHub.Tests.MicroBundles;

/// <summary>Complete public-surface coverage for <see cref="MicroBundleManifestation"/>.</summary>
public sealed class MicroBundleManifestationTests
{
    [Fact] public void ConstructorPreservesId() { var m = new MicroBundleManifestation(7, "target", "effect"); Assert.Equal(7, m.Id); }
    [Fact] public void ConstructorPreservesTarget() { var m = new MicroBundleManifestation(7, "target", "effect"); Assert.Equal("target", m.Target); }
    [Fact] public void ConstructorPreservesEffect() { var m = new MicroBundleManifestation(7, "target", "effect"); Assert.Equal("effect", m.Effect); }
    [Fact] public void ParametersStartEmpty() { var m = new MicroBundleManifestation(7, "target", "effect"); Assert.Empty(m.Parameters); }
    [Fact] public void ParametersCanBeInitialized() { var m = new MicroBundleManifestation(7, "target", "effect") { Parameters = new Dictionary<string,int> { ["Speed"] = 12 } }; Assert.Equal(12, m.Parameters["Speed"]); }
    [Fact] public void MultipleParametersArePreserved() { var m = new MicroBundleManifestation(7, "target", "effect") { Parameters = new Dictionary<string,int> { ["A"] = 1, ["B"] = 2, ["C"] = 3 } }; Assert.Equal(3, m.Parameters.Count); Assert.Equal(1, m.Parameters["A"]); Assert.Equal(2, m.Parameters["B"]); Assert.Equal(3, m.Parameters["C"]); }
    [Fact] public void NegativeIdIsPreservedAsData() { var m = new MicroBundleManifestation(-1, "target", "effect"); Assert.Equal(-1, m.Id); }
    [Fact] public void EmptyStringsArePreserved() { var m = new MicroBundleManifestation(0, string.Empty, string.Empty); Assert.Equal(string.Empty, m.Target); Assert.Equal(string.Empty, m.Effect); }
    [Fact] public void ParameterReplacementReplacesWholeDictionary() { var first = new Dictionary<string,int> { ["A"] = 1 }; var second = new Dictionary<string,int> { ["B"] = 2 }; var m = new MicroBundleManifestation(1, "t", "e") { Parameters = first }; m.Parameters = second; Assert.DoesNotContain("A", m.Parameters.Keys); Assert.Equal(2, m.Parameters["B"]); }
}
