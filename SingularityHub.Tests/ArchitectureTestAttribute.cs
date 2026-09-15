using System.Reflection;

namespace SingularityHub.Tests;

/// <summary>
/// Assigns a stable architectural address to a test.
/// X = architectural layer, Y = contract/test group, Z = test case.
/// The address is metadata and does not impose runner execution order.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ArchitectureTestAttribute : Attribute
{
    public ArchitectureTestAttribute(int layer, int group, int test)
    {
        if (layer < 0) throw new ArgumentOutOfRangeException(nameof(layer));
        if (group < 0) throw new ArgumentOutOfRangeException(nameof(group));
        if (test < 0) throw new ArgumentOutOfRangeException(nameof(test));

        Layer = layer;
        Group = group;
        Test = test;
    }

    public int Layer { get; }
    public int Group { get; }
    public int Test { get; }

    public string Address => $"{Layer}.{Group:00}.{Test:000}";

    public static string GetAddress(MethodInfo method)
        => method.GetCustomAttribute<ArchitectureTestAttribute>()?.Address
           ?? throw new InvalidOperationException($"{method.DeclaringType?.FullName}.{method.Name} is missing ArchitectureTest metadata.");
}
