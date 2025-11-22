namespace TheSingularityWorkshop.Models
{
    public class StateModel
    {
        public string Name { get; set; } = "NewState";
        // Store only the method name, to be selected from a list/dropdown
        // Method signature: Action<IStateContext>
        public string OnEnterMethod { get; set; } = "NoOp";
        public string OnUpdateMethod { get; set; } = "NoOp";
        public string OnExitMethod { get; set; } = "NoOp";
    }
}