namespace TheSingularityWorkshop.Models
{
    public class FSMDefinition
    {
        public string Name { get; set; } = "NewFSM";
        // Changed to string to simplify input handling for special values like "Infinity"
        public string ProcessRate { get; set; } = "1.0";
        public string ProcessGroup { get; set; } = "Default";
        public List<StateModel> States { get; set; } = new List<StateModel>();
        public List<TransitionModel> Transitions { get; set; } = new List<TransitionModel>();
    }
}