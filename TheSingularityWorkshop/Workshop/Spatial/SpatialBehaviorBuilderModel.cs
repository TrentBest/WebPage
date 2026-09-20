namespace TheSingularityWorkshop.Workshop.Spatial;

/// <summary>
/// Pure semantic representation of an FSM behavior body. The user manipulates
/// context types and data cards; source syntax is a later projection.
/// </summary>
public sealed class SpatialBehaviorBuilderModel
{
    private readonly List<SpatialBehaviorContextBranch> _branches = [];

    public SpatialBehaviorBuilderModel(string methodName)
    {
        MethodName = string.IsNullOrWhiteSpace(methodName) ? "OnUpdate" : methodName;
        AddContextBranch("UserSelectedType", "ust");
    }

    public string MethodName { get; }
    public IReadOnlyList<SpatialBehaviorContextBranch> ContextBranches => _branches;
    public IReadOnlyList<string> LifecycleMethods { get; } = ["OnEnter", "OnUpdate", "OnExit", "Condition"];

    public SpatialBehaviorContextBranch AddContextBranch(string contextType, string variableName)
    {
        var type = string.IsNullOrWhiteSpace(contextType) ? "ContextType" : contextType.Trim();
        var variable = string.IsNullOrWhiteSpace(variableName) ? "contextValue" : variableName.Trim();
        var branch = new SpatialBehaviorContextBranch(_branches.Count == 0, type, variable);
        _branches.Add(branch);
        return branch;
    }

    public SpatialBehaviorContextBranch AddElseIf(string contextType, string variableName)
        => AddContextBranch(contextType, variableName);
}

public sealed record SpatialBehaviorContextBranch(bool IsFirstBranch, string ContextType, string VariableName)
{
    public string Keyword => IsFirstBranch ? "if" : "else if";
    public string Signature => $"{Keyword}(context is {ContextType} {VariableName})";
}
