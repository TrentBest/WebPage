using System.Numerics;
using TheSingularityWorkshop.FSM_API;
using System.Linq;
using System;
using System.Collections.Generic;

using  fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Infrastructure.Demos.FoxesAndDogs
{
    public class AdvancedDemoContext : IStateContext
    {
        public static int MaxDogCount { get; private set; }
        public int SuccessCount { get; private set; } = 0;
        public FSMHandle State { get; private set; }

        public bool IsValid { get; set; } = false;
        public string Name { get; set; } = "AdvancedDemo";

        public int EnvironmentWidth { get; private set; } = 100;
        public int EngironmentHeight { get; private set; } = 100;
        public int FoxCount { get; private set; } = 15;
        public int DogCount { get; private set; } = 5;

        // FIX: Made public for access from the Razor page code
        public AdvancedDemoEnvironment environment { get; private set; }
        public bool DestinationReached { get; private set; }

        private Random random = new Random();

        public AdvancedDemoContext()
        {
            fsm_API.Internal.ResetAPI(true);

            if (!fsm_API.Interaction.Exists("AdvancedDemoFSM", "Update"))
            {
                fsm_API.Create.CreateProcessingGroup("Environment");
                fsm_API.Create.CreateProcessingGroup("Foxes");
                fsm_API.Create.CreateProcessingGroup("Dogs");

                fsm_API.Create.CreateFiniteStateMachine("AdvancedDemoFSM", -1, "Update")
                    .State("Executing", OnEnterExecuting, OnUpdateExecuting, OnExitExecuting)
                    .State("Shutdown", OnEnterShutdown, null, null)
                    .Transition("Executing", "Shutdown", ShouldShutDown)
                    .BuildDefinition();
            }

            environment = new AdvancedDemoEnvironment();
            // FIX: Assign FSMHandle to the new State property
            State = fsm_API.Create.CreateInstance("AdvancedDemoFSM", this, "Update");
            IsValid = true;
        }

        private Vector2 GetRandomPosition(AdvancedDemoContext context)
        {
            return new Vector2(
                random.Next(0, context.EnvironmentWidth),
                random.Next(0, context.EngironmentHeight));
        }

        private void OnEnterExecuting(IStateContext context)
        {
            if (context is AdvancedDemoContext adc)
            {
                adc.environment.ClearAgents();
                AdvancedDemoEnvironment ade = adc.environment;

                for (int d = 0; d < DogCount; d++)
                {
                    var pos = GetRandomPosition(adc);
                    if (ade.IsEmptyAt(pos))
                    {
                        var dog = new AdvancedDogContext(d, pos, ade);
                        ade.AddAgent(dog);
                    }
                    else { d--; }
                }

                for (int f = 0; f < FoxCount; f++)
                {
                    var pos = GetRandomPosition(adc);
                    if (ade.IsEmptyAt(pos))
                    {
                        var fox = new AdvancedFoxContext(f, pos, ade);
                        fox.Destination = fox.FindRandomDestination();
                        ade.AddAgent(fox);
                    }
                    else { f--; }
                }
            }
        }

        private void OnUpdateExecuting(IStateContext context)
        {
            fsm_API.Interaction.Update("Environment");
            fsm_API.Interaction.Update("Dogs");
            fsm_API.Interaction.Update("Foxes");

            if(context is AdvancedDemoContext adc)
            {
                foreach(var fox in adc.environment.GetAgents().OfType<AdvancedDemoContext>())
                {
                    if(fox.State.CurrentState == "Idle" && fox.DestinationReached)
                    {
                        adc.SuccessCount++;
                        fox.DestinationReached = false;
                    }
                }
            }
        }

        public void AddAgent(bool isFox, int count =1)
        {
            if (isFox)
            {
                FoxCount++;
                var pos = GetRandomPosition(this);
                if (environment.IsEmptyAt(pos))
                {
                    var fox = new AdvancedFoxContext(FoxCount, pos, environment);
                    fox.Destination = fox.FindRandomDestination();
                    environment.AddAgent(fox);
                }
            }
            else // isDog
            {
                DogCount++;
                var pos = GetRandomPosition(this);
                if (environment.IsEmptyAt(pos))
                {
                    var dog = new AdvancedDogContext(DogCount, pos, environment);
                    environment.AddAgent(dog);
                }
            }
            // No need to call StateHasChanged here; the FSM loop handles UI update.
        }

        public void AdjustEnvironmentSize(int delta)
        {
            // Simple square grid scaling for now
            EnvironmentWidth += delta;
            EngironmentHeight += delta;

            if (EnvironmentWidth < 10) EnvironmentWidth = 10;
            if (EngironmentHeight < 10) EngironmentHeight = 10;

            // Note: Agents outside the new boundary will need cleanup/repositioning 
            // in a full solution, but we ignore that complexity here for the demo.
        }

        private void OnExitExecuting(IStateContext context) { }

        private void OnEnterShutdown(IStateContext context)
        {
            if (context is AdvancedDemoContext adc)
            {
                // Cleanup logic (unchanged)
                fsm_API.Interaction.DestroyFiniteStateMachine("AdvancedDemoFSM", "Update");
                fsm_API.Interaction.DestroyFiniteStateMachine("AdvancedDemoEnvironmentFSM", "Environment");
                fsm_API.Interaction.DestroyFiniteStateMachine("AdvancedDogFSM", "Dogs");
                fsm_API.Interaction.DestroyFiniteStateMachine("AdvancedFoxFSM", "Foxes");
                fsm_API.Interaction.RemoveProcessingGroup("Environment");
                fsm_API.Interaction.RemoveProcessingGroup("Foxes");
                fsm_API.Interaction.RemoveProcessingGroup("Dogs");
                adc.IsValid = false;
            }
        }

        private bool ShouldShutDown(IStateContext context)
        {
            if (context is AdvancedDemoContext adc)
            {
                bool allFoxesAreDone = adc.environment.GetAgents()
                    .Where(a => a is AdvancedFoxContext)
                    .All(fox => fox.Status.CurrentState == "Idle" || fox.Status.CurrentState == "Mangled");

                // Removed Console.KeyAvailable as it is not available in Blazor WASM
                return allFoxesAreDone;
            }
            return false;
        }
    }
}