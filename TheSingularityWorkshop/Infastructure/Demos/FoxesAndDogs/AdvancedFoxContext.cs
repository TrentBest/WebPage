using System.Numerics;
using TheSingularityWorkshop.FSM_API;
using System.Collections.Generic;
using System.Linq;
using System;

using fsm_API = TheSingularityWorkshop.FSM_API.FSM_API;

namespace TheSingularityWorkshop.Infrastructure.Demos.FoxesAndDogs
{
    // Changed access modifier from internal to public to be used by AdvancedDemoContext
    public class AdvancedFoxContext : IFoxAgent
    {
        private AdvancedDemoEnvironment Environment { get; }
        private Random random = new Random();
        private Vector2 JumpStart;
        private Vector2 JumpEnd;
        private int Speed { get; set; } = 1;

        // IAdvancedAgent Properties
        public Vector2 Position { get; set; }
        public Vector2 Destination { get; set; }
        public float SightRange { get; set; } = 2;
        public List<IAdvancedAgent> VisibleAgents { get; } = new List<IAdvancedAgent>();
        public List<IAdvancedAgent> CollidingAgents { get; } = new List<IAdvancedAgent>();
        public FSMHandle Status { get; }
        public bool IsValid { get; set; } = true;
        public string Name { get; set; } = string.Empty;

        public AdvancedFoxContext(int id, Vector2 pos, AdvancedDemoEnvironment environment)
        {
            this.Position = pos;
            this.Environment = environment;
            this.Name = $"Fox[{id}]";

            if (!fsm_API.Interaction.Exists("AdvancedFoxFSM", "Foxes"))
            {
                fsm_API.Create.CreateFiniteStateMachine("AdvancedFoxFSM", -1, "Foxes")
                    .State("Idle", null, OnUpdateIdle, null)
                    .State("Walking", OnEnterWalking, OnUpdateWalking, OnExitWalking)
                    .State("Jumping", OnEnterJumping, OnUpdateJumping, OnExitJumping)
                    .State("Fleeing", OnEnterFleeing, OnUpdateFleeing, OnExitFleeing)
                    .State("Mangled", OnEnterMangled, OnUpdateManagled, OnExitMangled)
                    .Transition("Idle", "Walking", HasDestination)
                    .Transition("Walking", "Idle", HasNoDestination)
                    .Transition("Walking", "Jumping", ShouldJump)
                    .Transition("Jumping", "Walking", JumpComplete)
                    .Transition("Walking", "Fleeing", ShouldFlee)
                    .Transition("Fleeing", "Walking", ShouldStopFleeing)
                    .BuildDefinition();
            }
            Status = fsm_API.Create.CreateInstance("AdvancedFoxFSM", this, "Foxes");
        }

        // --- MOVEMENT HELPERS (Logic copied from original file) ---

        private Vector2 CalculateNextStep(AdvancedFoxContext fox)
        {

            if (Vector2.Distance(fox.Position, fox.Destination) < 1.0f) return fox.Destination;
            float dx = Math.Abs(fox.Destination.X - fox.Position.X);
            float dy = Math.Abs(fox.Destination.Y - fox.Position.Y);
            int stepX = 0, stepY = 0;

            if (dx > dy) stepX = fox.Destination.X > fox.Position.X ? 1 : -1;
            else if (dy > dx) stepY = fox.Destination.Y > fox.Position.Y ? 1 : -1;
            else if (fox.random.Next(2) == 0) stepX = fox.Destination.X > fox.Position.X ? 1 : -1;
            else stepY = fox.Destination.Y > fox.Position.Y ? 1 : -1;

            return new Vector2(fox.Position.X + stepX, fox.Position.Y + stepY);
        }

        private (int minX, int maxX, int minY, int maxY) GetExcludedQuadrantBounds(Vector2 currentPos, int width, int height)
        {
            int halfW = width / 2;
            int halfH = height / 2;
            bool isRight = currentPos.X >= halfW;
            bool isTop = currentPos.Y >= halfH;

            if (isRight && isTop) return (halfW, width, halfH, height);
            if (!isRight && isTop) return (0, halfW, halfH, height);
            if (!isRight && !isTop) return (0, halfW, 0, halfH);
            return (halfW, width, 0, halfH);
        }

        public Vector2 FindRandomDestination()
        {
            int width = Environment.EnvironmentWidth;
            int height = Environment.EngironmentHeight;
            (int exMinX, int exMaxX, int exMinY, int exMaxY) = GetExcludedQuadrantBounds(this.Position, width, height);

            Vector2 newDest = Vector2.Zero;
            bool valid = false;

            while (!valid)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);
                newDest = new Vector2(x, y);

                bool isInExcludedX = x >= exMinX && x < exMaxX;
                bool isInExcludedY = y >= exMinY && y < exMaxY;

                if (!(isInExcludedX && isInExcludedY))
                {
                    valid = true;
                    return newDest;
                }
            }
            return newDest;
        }

        // --- FSM STATE LOGIC (Console.WriteLine removed) ---
        private void OnUpdateIdle(IStateContext context)
        {
            if (context is AdvancedFoxContext fox)
            {
                if (Vector2.Distance(fox.Position, fox.Destination) < 0.1f)
                {
                    fox.Destination = fox.FindRandomDestination();
                }
            }
        }
        private void OnEnterWalking(IStateContext context) { /* Console.WriteLine removed */ }
        private void OnUpdateWalking(IStateContext context)
        {
            if (context is AdvancedFoxContext fox)
            {
                Vector2 nextPos = fox.CalculateNextStep(fox);
                if (Vector2.Distance(fox.Position, fox.Destination) < 1.0f)
                {
                    fox.Position = fox.Destination;
                    return;
                }
                fox.Position = nextPos;
            }
        }
        private void OnExitWalking(IStateContext context) { }
        private void OnEnterJumping(IStateContext context)
        {
            if (context is AdvancedFoxContext fox)
            {
                // Instantaneous jump logic from original file
                var dogAtLandingSpot = fox.Environment.GetAgents()
                    .OfType<AdvancedDogContext>()
                    .Any(d => Vector2.Distance(d.Position, fox.JumpEnd) < 0.5f);

                if (dogAtLandingSpot)
                {
                    fox.Status.TransitionTo("Mangled");
                    fox.Position = fox.JumpEnd;
                }
                else
                {
                    fox.Position = fox.JumpEnd;
                }
            }
        }
        private void OnUpdateJumping(IStateContext context) { }
        private void OnExitJumping(IStateContext context) { }
        private void OnEnterFleeing(IStateContext context)
        {
            if (context is AdvancedFoxContext fox) { fox.Speed = 2; }
        }
        private void OnUpdateFleeing(IStateContext context)
        {
            if (context is AdvancedFoxContext fox)
            {
                // Fleeing logic (copied from original file)
                var dogCollision = fox.CollidingAgents
                    .OfType<AdvancedDogContext>()
                    .OrderBy(d => Vector2.Distance(d.Position, fox.Position))
                    .FirstOrDefault();

                if (dogCollision != null)
                {
                    Vector2 fleeVector = fox.Position - dogCollision.Position;
                    int stepX = 0;
                    int stepY = 0;

                    if (Math.Abs(fleeVector.X) > Math.Abs(fleeVector.Y)) stepX = fleeVector.X > 0 ? fox.Speed : -fox.Speed;
                    else if (Math.Abs(fleeVector.Y) > Math.Abs(fleeVector.X)) stepY = fleeVector.Y > 0 ? fox.Speed : -fox.Speed;
                    else if (fox.random.Next(2) == 0) stepX = fleeVector.X > 0 ? fox.Speed : -fox.Speed;
                    else stepY = fleeVector.Y > 0 ? fox.Speed : -fox.Speed;

                    fox.Position = new Vector2(fox.Position.X + stepX, fox.Position.Y + stepY);
                }
                else
                {
                    Vector2 nextPos = fox.CalculateNextStep(fox);
                    fox.Position = nextPos;
                }
            }
        }
        private void OnExitFleeing(IStateContext context) { }
        private void OnEnterMangled(IStateContext context) { }
        private void OnUpdateManagled(IStateContext context) { }
        private void OnExitMangled(IStateContext context) { }

        // --- FSM TRANSITION LOGIC (Copied from original file) ---
        private bool HasDestination(IStateContext context) => context is AdvancedFoxContext fox && Vector2.Distance(fox.Position, fox.Destination) >= 1.0f;
        private bool HasNoDestination(IStateContext context) => context is AdvancedFoxContext fox && Vector2.Distance(fox.Position, fox.Destination) < 1.0f;
        private bool ShouldJump(IStateContext context)
        {
            // Jump condition logic (copied from original file)
            if (context is AdvancedFoxContext fox)
            {
                Vector2 nextStep = fox.CalculateNextStep(fox);
                var dogToJumpOver = fox.Environment.GetAgents().OfType<AdvancedDogContext>().Any(d => Vector2.Distance(d.Position, nextStep) < 0.5f);

                if (dogToJumpOver)
                {
                    int stepX = (int)(nextStep.X - fox.Position.X) * 2;
                    int stepY = (int)(nextStep.Y - fox.Position.Y) * 2;

                    fox.JumpStart = fox.Position;
                    fox.JumpEnd = new Vector2(fox.Position.X + stepX, fox.Position.Y + stepY);

                    if (fox.JumpEnd.X < 0 || fox.JumpEnd.X >= fox.Environment.EnvironmentWidth ||
                        fox.JumpEnd.Y < 0 || fox.JumpEnd.Y >= fox.Environment.EngironmentHeight) return false;
                    return true;
                }
            }
            return false;
        }
        private bool JumpComplete(IStateContext context) => true;
        private bool ShouldFlee(IStateContext context)
        {
            if (context is AdvancedFoxContext fox)
            {
                var activeDog = fox.VisibleAgents.OfType<AdvancedDogContext>().Any(d => d.Status.CurrentState == "Chasing");
                var collision = fox.CollidingAgents.OfType<AdvancedDogContext>().Any();
                return activeDog || collision;
            }
            return false;
        }
        private bool ShouldStopFleeing(IStateContext context)
        {
            if (context is AdvancedFoxContext fox)
            {
                return !fox.VisibleAgents.OfType<AdvancedDogContext>().Any()
                    && !fox.CollidingAgents.OfType<AdvancedDogContext>().Any();
            }
            return false;
        }
    }
}