using System.Numerics;
using TheSingularityWorkshop.FSM_API;
using System.Collections.Generic;
using System.Linq;
using System;

using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Infrastructure.Demos.FoxesAndDogs
{
    public class AdvancedDogContext : IAdvancedAgent
    {
        // IAdvancedAgent properties
        public float SightRange { get; set; } = 3;
        public Vector2 Position { get; set; }
        public FSMHandle Status { get; }
        public List<IAdvancedAgent> VisibleAgents { get; } = new List<IAdvancedAgent>();
        public List<IAdvancedAgent> CollidingAgents { get; } = new List<IAdvancedAgent>();
        public bool IsValid { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public int Speed { get; private set; } = 1; // Default speed for dogs

        // Made nullable/initialized to comply with NRTs
        private IAdvancedAgent? Chasing { get; set; }

        private AdvancedDemoEnvironment Environment { get; }

        public AdvancedDogContext(int id, Vector2 pos, AdvancedDemoEnvironment environment)
        {
            Position = pos;
            if (!fsm_API.Interaction.Exists("AdvancedDogFSM", "Dogs"))
            {
                fsm_API.Create.CreateFiniteStateMachine("AdvancedDogFSM", -1, "Dogs")
                    .State("Sleeping", null, null, null)
                    .State("Idle", OnEnterIdle, null, null)
                    .State("Chasing", OnEnterChasing, OnUpdateChasing, OnExitChasing)
                    .State("Mangling", OnEnterMangling, null, null)
                    .Transition("Sleeping", "Idle", ShouldWake)
                    .Transition("Idle", "Chasing", ShouldChase)
                    .Transition("Chasing", "Mangling", IsManglingFox)
                    .Transition("Mangling", "Idle", (ctx) => true)
                    .Transition("Idle", "Sleeping", ShouldSleep)
                    .BuildDefinition();
            }
            Status = fsm_API.Create.CreateInstance("AdvancedDogFSM", this, "Dogs");
            Environment = environment;
            Name = $"Dog[{id}]";
            IsValid = true;
        }

        private void OnEnterIdle(IStateContext context)
        {
            if (context is AdvancedDogContext dog)
            {
                dog.Speed = 1;
                // Calls the method in the same namespace
                dog.Chasing = dog.Environment.GetNearestFox(dog);
            }
        }

        private void OnEnterChasing(IStateContext context)
        {
            if (context is AdvancedDogContext dog)
            {
                dog.Speed = 3;
                // Console.WriteLine removed
            }
        }

        private void OnUpdateChasing(IStateContext context)
        {
            if (context is AdvancedDogContext dog)
            {
                if (dog.Chasing == null) return;

                Vector2 direction = Vector2.Normalize(dog.Chasing.Position - dog.Position);
                dog.Position += direction * dog.Speed;
                // Console.WriteLine removed
            }
        }

        private void OnExitChasing(IStateContext context)
        {
            if (context is AdvancedDogContext dog)
            {

                dog.Speed = 1;
                dog.Chasing = null;
            }
        }

        private void OnEnterMangling(IStateContext context)
        {
            if (context is AdvancedDogContext dog)
            {
                // Console.WriteLine removed
                dog.Chasing?.Status.TransitionTo("Mangled");
            }
        }

        private bool ShouldWake(IStateContext context) => context is AdvancedDogContext dog && dog.CollidingAgents.Count > 0;
        private bool ShouldChase(IStateContext context) => context is AdvancedDogContext dog && dog.VisibleAgents.Any(agent => agent is AdvancedFoxContext);

        private bool IsManglingFox(IStateContext context)
        {
            if (context is AdvancedDogContext dog)
            {
                if (dog.Chasing == null) return false;
                return Vector2.Distance(dog.Position, dog.Chasing.Position) < 0.5f;
            }
            return false;
        }

        private bool ShouldSleep(IStateContext context) => context is AdvancedDogContext dog && !dog.VisibleAgents.Any(agent => agent is AdvancedFoxContext);
    }
}