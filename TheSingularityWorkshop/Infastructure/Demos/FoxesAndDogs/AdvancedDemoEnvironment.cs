using System.Numerics;
using TheSingularityWorkshop.FSM_API;
using System.Collections.Generic;
using System.Linq;
using System; // For Math.Abs

using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Infrastructure.Demos.FoxesAndDogs
{
    public class AdvancedDemoEnvironment : IStateContext
    {
        public int EnvironmentWidth { get; private set; } = 100;
        public int EngironmentHeight { get; private set; } = 100;

        private List<IAdvancedAgent> _agents = new List<IAdvancedAgent>();

        public AdvancedDemoEnvironment()
        {
            if (!fsm_API.Interaction.Exists("AdvancedDemoEnvironmentFSM", "Environment"))
            {
                fsm_API.Create.CreateFiniteStateMachine("AdvancedDemoEnvironmentFSM", -1, "Environment")
                    .State("Existing", null, OnUpdateExisting, null)
                    .BuildDefinition();
            }
            fsm_API.Create.CreateInstance("AdvancedDemoEnvironmentFSM", this, "Environment");
            Name = "AdvancedDemoEnvironment";
            IsValid = true;
        }

        private void OnUpdateExisting(IStateContext context)
        {
            if (context is AdvancedDemoEnvironment env)
            {
                foreach (var agent in env._agents)
                {
                    agent.CollidingAgents.Clear();
                    agent.VisibleAgents.Clear();
                }

                for (int i = 0; i < env._agents.Count; i++)
                {
                    var currentAgent = env._agents[i];
                    for (int j = i + 1; j < env._agents.Count; j++)
                    {
                        var otherAgent = env._agents[j];

                        // Collision logic (copied from original file)
                        if (Vector2.Distance(currentAgent.Position, otherAgent.Position) < 0.5f
                            && currentAgent.Status.CurrentState != "Jumping"
                            && otherAgent.Status.CurrentState != "Jumping")
                        {
                            currentAgent.CollidingAgents.Add(otherAgent);
                            otherAgent.CollidingAgents.Add(currentAgent);
                        }

                        // Vision logic (copied from original file)
                        if (ManhattanDistance(currentAgent.Position, otherAgent.Position) <= currentAgent.SightRange)
                        {
                            currentAgent.VisibleAgents.Add(otherAgent);
                        }
                        if (ManhattanDistance(currentAgent.Position, otherAgent.Position) <= otherAgent.SightRange)
                        {
                            otherAgent.VisibleAgents.Add(currentAgent);
                        }
                    }
                }
            }
        }

        public void ClearAgents() => _agents.Clear();

        public List<IAdvancedAgent> GetAgents() => _agents.ToList();

        public void AddAgent(IAdvancedAgent agent) => _agents.Add(agent);

        public bool IsEmptyAt(Vector2 pos)
        {
            return !_agents.Any(s => Vector2.Distance(s.Position, pos) < 0.5f);
        }

        // Must cast to the correct context type (IAdvancedAgent)
        public IAdvancedAgent? GetNearestFox(AdvancedDogContext dog)
        {
            return _agents.Where(s => s is AdvancedFoxContext).OrderBy(s => ManhattanDistance(s.Position, dog.Position)).FirstOrDefault();
        }

        private float ManhattanDistance(Vector2 position1, Vector2 position2)
        {
            float dx = Math.Abs(position1.X - position2.X);
            float dy = Math.Abs(position1.Y - position2.Y);
            return dx + dy;
        }

        public bool IsValid { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}