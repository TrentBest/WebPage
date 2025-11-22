using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Services;


public class FSMManagerService
{
    // FSMHandle is your direct link to the live FSM instance
    private FSMHandle _fsmHandle;

    public PageStateContext Context { get; }

    // Expose the current FSM state for UI components to bind to
    public string CurrentState => _fsmHandle?.CurrentState ?? "Uninitialized";

    public FSMManagerService()
    {
        // 1. Initialize the Context/Data Bag
        Context = new PageStateContext();

        // 2. Define the FSM blueprint (Initialization, Operation, Shutdown)
        FSM_API.Create.CreateFiniteStateMachine("WebpageFSM", -1, "Update")
            // Core States: Defining the three parts of your application state
            .State("Idle", null, onUpdate: (c) => ((PageStateContext)c).Message = "Awaiting input...", null)
            .State("Clicked", onEnter: (c) => ((PageStateContext)c).Message = "Button Clicked!",null, null)
            .State("Processing", onEnter: (c) => ((PageStateContext)c).Message = "Processing...", null, null)
            .Transition("Idle", "Clicked", (c) => ((PageStateContext)c).ClickCount > 0)
            .Transition("Clicked", "Processing", (c) => true)
            .Transition("Processing", "Idle", (c) => ((PageStateContext)c).ClickCount > 10) // Simulates a reset condition         
            .BuildDefinition(); // Use BuildDefinition for API consistency

        // 3. Create the FSM instance and connect it to the Context
        // We use the "Update" processing group so the Heartbeat service will tick it.
        _fsmHandle = FSM_API.Create.CreateInstance("WebpageFSM", Context, "Update");
    }

    /// <summary>
    /// Tells the FSM instance to take one "step" or "tick" forward.
    /// </summary>
    public void Step()
    {
        // This is for manual/event-driven updates, which should still use a group update
        FSM_API.Interaction.Update("Update");
    }

    /// <summary>
    /// Forces the FSM to transition to a new state, typically triggered by a UI event.
    /// </summary>
    public void ForceTransition(string nextStateName)
    {
        _fsmHandle?.TransitionTo(nextStateName);
    }
}
