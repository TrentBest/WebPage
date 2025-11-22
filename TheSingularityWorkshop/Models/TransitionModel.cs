namespace TheSingularityWorkshop.Models
{
    public class TransitionModel
    {
        // For the UI we use the actual state names for clarity
        public string FromStateName { get; set; } = "";
        public string ToStateName { get; set; } = "";
        // Store only the predicate method name
        // Method signature: Predicate<IStateContext> (Func<IStateContext, bool>)
        public string ConditionMethod { get; set; } = "AlwaysTrue";
    }
}