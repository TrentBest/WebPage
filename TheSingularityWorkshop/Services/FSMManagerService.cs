using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Services;

namespace TheSingularityWorkshop.Services
{
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

            // 2. Define the FSM blueprint (To be replaced with your FSM_API.Create logic)
            // -------------------------------------------------------------------------
            // TODO: Replace this with your actual FSM Definition and FSMHandle creation.
            // You will need to build an FSM object using FSM_API.Create.FiniteStateMachine(...)
            // Example FSM creation setup:
            /*
            var myFsmDefinition = FSM_API.Create.FiniteStateMachine("WebpageFSM")
                .AddState("Idle", onUpdate: (c) => ((PageStateContext)c).Message = "Waiting for click...")
                .AddState("Clicked", onEnter: (c) => ((PageStateContext)c).Message = "Clicked!")
                .AddTransition("Idle", "Clicked", (c) => ((PageStateContext)c).ClickCount > 0)
                .SetInitialState("Idle")
                .Build();
            */
            // Since FSMHandle requires a valid FSM in its constructor, 
            // you must ensure the FSM object is available or mock it.
            // -------------------------------------------------------------------------

            // Assuming a valid 'myFsmDefinition' of type FSM exists:
            // _fsmHandle = FSM_API.Create.Instance(myFsmDefinition, Context);

            // To run without the full API for demonstration, assume _fsmHandle is created/initialized later.
        }

        /// <summary>
        /// Tells the FSM instance to take one "step" or "tick" forward.
        /// </summary>
        /// <remarks>
        /// This is the core method for moving your FSM's state forward.
        /// In a game loop, this would be called automatically; in Blazor, it's called manually.
        /// </remarks>
        public void Step()
        {
            if (_fsmHandle == null)
            {
                // Placeholder logic for the demonstration context
                Context.ClickCount++;
                Context.Message = $"FSM Mocked: Click Count: {Context.ClickCount}";
                return;
            }

            // In your real implementation, call the handle's update method:
            // _fsmHandle.Update(); 
        }

        /// <summary>
        /// Forces the FSM to transition to a new state, typically triggered by a UI event.
        /// </summary>
        public void ForceTransition(string nextStateName)
        {
            // The FSMHandle.TransitionTo method bypasses normal transition rules.
            // _fsmHandle?.TransitionTo(nextStateName);

            // Placeholder logic:
            if (_fsmHandle == null)
            {
                Context.Message = $"Forced transition to '{nextStateName}' (Mocked)";
            }
        }
    }
}