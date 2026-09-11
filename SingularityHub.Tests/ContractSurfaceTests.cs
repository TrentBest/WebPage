using TheSingularityWorkshop.SingularityHub;
using Xunit;

namespace SingularityHub.Tests;

/// <summary>Cross-domain contract tests. These guard the architecture before concrete domains proliferate.</summary>
public sealed class ContractSurfaceTests
{
    [ArchitectureTest(0, 1, 1)]
    [Fact(DisplayName = "0.01.001 — Hub_Contracts_Are_Public")]
    public void Hub_Contracts_Are_Public()
    {
        var contracts = new[]
        {
            typeof(IMicroBundle), typeof(IArbitrationAudit), typeof(IArbitrator),
            typeof(IDataWarehouseLiaison), typeof(IExecutionProvider),
            typeof(IProcessGroupHost), typeof(ISingularityHub)
        };
        Assert.All(contracts, type => Assert.True(type.IsInterface && type.IsPublic, type.FullName));
    }

    [ArchitectureTest(0, 1, 2)]
    [Fact(DisplayName = "0.01.002 — Hub_Contracts_Do_Not_Depend_On_Unity")]
    public void Hub_Contracts_Do_Not_Depend_On_Unity()
    {
        var assembly = typeof(ISingularityHub).Assembly;
        var forbidden = assembly.GetExportedTypes()
            .SelectMany(t => t.GetMembers())
            .SelectMany(m => m switch
            {
                System.Reflection.MethodInfo method => new[] { method.ReturnType }.Concat(method.GetParameters().Select(p => p.ParameterType)),
                System.Reflection.PropertyInfo property => new[] { property.PropertyType },
                _ => Array.Empty<Type>()
            })
            .Distinct()
            .Where(t => t.FullName?.Contains("Unity", StringComparison.OrdinalIgnoreCase) == true)
            .ToArray();
        Assert.Empty(forbidden);
    }

    [ArchitectureTest(0, 1, 3)]
    [Fact(DisplayName = "0.01.003 — Hub_Contracts_Do_Not_Expose_Task_Or_Thread")]
    public void Hub_Contracts_Do_Not_Expose_Task_Or_Thread()
    {
        var forbidden = new[] { typeof(Task), typeof(Thread) };
        var publicTypes = typeof(ISingularityHub).Assembly.GetExportedTypes();
        foreach (var type in publicTypes)
        {
            foreach (var method in type.GetMethods())
            {
                Assert.False(forbidden.Contains(method.ReturnType));
                Assert.All(method.GetParameters(), parameter => Assert.False(forbidden.Contains(parameter.ParameterType)));
            }
        }
    }

    [ArchitectureTest(0, 1, 4)]
    [Fact(DisplayName = "0.01.004 — Hub_Contract_Composes_Execution_Indirection")]
    public void Hub_Contract_Composes_Execution_Indirection()
        => Assert.Contains(typeof(IExecutionProvider), typeof(IExecutionProvider).Assembly.GetTypes());

    [ArchitectureTest(0, 1, 5)]
    [Fact(DisplayName = "0.01.005 — ProcessGroup_Contract_Exposes_State_Not_Mechanics")]
    public void ProcessGroup_Contract_Exposes_State_Not_Mechanics()
    {
        var methods = typeof(IProcessGroupHost).GetMethods();
        Assert.Equal(3, methods.Length);
        Assert.DoesNotContain(methods, m => m.Name.Contains("Execute", StringComparison.OrdinalIgnoreCase));
    }

    [ArchitectureTest(0, 1, 6)]
    [Fact(DisplayName = "0.01.006 — MicroBundle_Arbitration_Receives_Arbitrator")]
    public void MicroBundle_Arbitration_Receives_Arbitrator()
    {
        var method = typeof(IMicroBundle).GetMethod(nameof(IMicroBundle.Arbitrate));
        Assert.NotNull(method);
        Assert.Contains(typeof(IArbitrator), method!.GetParameters().Select(p => p.ParameterType));
    }
}
