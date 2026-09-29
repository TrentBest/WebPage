using System.Text;
namespace TheSingularityWorkshop.Infrastructure.FsmForge;
public static class FsmForgeScaffolder
{
    public static string Generate(FsmForgeDefinition definition,string className="ForgedStateMachine")
    {
        ArgumentNullException.ThrowIfNull(definition); ArgumentException.ThrowIfNullOrWhiteSpace(className);
        var sb=new StringBuilder();
        sb.AppendLine("using TheSingularityWorkshop.FSM_API;");
        sb.AppendLine("using FsmApi = TheSingularityWorkshop.FSM_API.FSM_API;");
        sb.AppendLine();
        sb.AppendLine($"public static class {className}");
        sb.AppendLine("{");
        sb.AppendLine("    public static void Build()");
        sb.AppendLine("    {");
        sb.AppendLine($"        FsmApi.Create.CreateFiniteStateMachine(\"{className}\", processRate: -1, processingGroup: \"Forged\")");
        foreach(var state in definition.States)
        {
            sb.AppendLine($"            .State(\"{state.Name}\",");
            sb.AppendLine($"                onEnter: {state.OnInitializeMethod},");
            sb.AppendLine($"                onUpdate: {state.OnUpdateMethod},");
            sb.AppendLine($"                onExit: {state.OnExitMethod})");
        }
        if(definition.InitialState is not null)sb.AppendLine($"            .WithInitialState(\"{definition.InitialState}\")");
        foreach(var transition in definition.Transitions)sb.AppendLine($"            .Transition(\"{transition.FromState}\", \"{transition.ToState}\", {transition.MethodName})");
        sb.AppendLine("            .BuildDefinition();");
        sb.AppendLine("    }");
        sb.AppendLine();
        foreach(var state in definition.States)
        {
            sb.AppendLine($"    private static void {state.OnInitializeMethod}(IStateContext context)");
            sb.AppendLine("    {"); sb.AppendLine("        // TODO: forged initialization behavior"); sb.AppendLine("    }"); sb.AppendLine();
            sb.AppendLine($"    private static void {state.OnUpdateMethod}(IStateContext context)");
            sb.AppendLine("    {"); sb.AppendLine("        // TODO: forged update behavior"); sb.AppendLine("    }"); sb.AppendLine();
            sb.AppendLine($"    private static void {state.OnExitMethod}(IStateContext context)");
            sb.AppendLine("    {"); sb.AppendLine("        // TODO: forged exit behavior"); sb.AppendLine("    }"); sb.AppendLine();
        }
        foreach(var transition in definition.Transitions)
        {
            sb.AppendLine($"    private static bool {transition.MethodName}(IStateContext context)");
            sb.AppendLine("    {"); sb.AppendLine("        // TODO: replace the scaffold with domain logic.");
            sb.AppendLine($"        {transition.Condition.Scaffold}"); sb.AppendLine("    }"); sb.AppendLine();
        }
        sb.AppendLine("}");
        return sb.ToString();
    }
}
