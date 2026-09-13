using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.SingularityHub;

namespace TheSingularityWorkshop.Workshop.MicroBundles;

/// <summary>
/// The first concrete Workshop experience implemented as a MicroBundle.
/// Pong is intentionally unaware of idle mode; idle is only one host condition
/// in which this experience may be manifested.
///
/// The Pong simulation is itself FSM_API-driven. The Blazor host supplies input
/// and renders state; it does not own the simulation clock or physics.
/// </summary>
public sealed class PongMicroBundle : IDisposable
{
    public const int BundleId = 2001;
    private const string GameStateName = "PLAYING";

    private readonly MicroBundle _lifecycle;
    private readonly PongStateContext _gameContext;
    private readonly FSMHandle _gameHandle;
    private readonly string _gameProcessingGroup = $"PongGame:{Guid.NewGuid():N}";
    private bool _disposed;

    public PongMicroBundle()
    {
        _lifecycle = new MicroBundle(BundleId, "PONG", new WebMicroBundleProvider());
        _gameContext = new PongStateContext();

        FSM_API.FSM_API.Create.CreateProcessingGroup(_gameProcessingGroup);
        FSM_API.FSM_API.Create.CreateFiniteStateMachine(_gameContext.FsmName, -1, _gameProcessingGroup)
            .State(GameStateName, onEnter: null, onUpdate: _ => _gameContext.Advance(), onExit: null)
            .WithInitialState(GameStateName)
            .BuildDefinition();

        _gameHandle = FSM_API.FSM_API.Create.CreateInstance(
            _gameContext.FsmName,
            _gameContext,
            _gameProcessingGroup);
    }

    public ulong Id => (ulong)_lifecycle.Id;
    public MicroBundleManifestation? Manifestation => _lifecycle.Manifestation;
    public string Phase => ((MicroBundleContext)_lifecycle.Context).Phase;
    public string GameState => _gameHandle.CurrentState;
    public double BallX => _gameContext.BallX;
    public double BallY => _gameContext.BallY;
    public double LeftPaddle => _gameContext.LeftPaddle;
    public double RightPaddle => _gameContext.RightPaddle;
    public int LeftScore => _gameContext.LeftScore;
    public int RightScore => _gameContext.RightScore;

    /// <summary>Applies user paddle movement to the authoritative Pong context.</summary>
    public void MoveLeftPaddle(double delta) => _gameContext.MoveLeftPaddle(delta);

    /// <summary>
    /// Advances both the MicroBundle lifecycle and its Pong FSM through FSM_API.
    /// No simulation work is performed by the presentation host.
    /// </summary>
    public void Update()
    {
        if (_disposed) return;

        _lifecycle.Update();
        FSM_API.FSM_API.Interaction.Update(_gameProcessingGroup);
    }

    public void Invalidate() => _lifecycle.Invalidate();

    public void Dispose()
    {
        if (_disposed) return;

        _lifecycle.Dispose();
        FSM_API.FSM_API.Interaction.DestroyFiniteStateMachine(_gameContext.FsmName, _gameProcessingGroup);
        _disposed = true;
    }

    private sealed class PongStateContext : IStateContext
    {
        private const double FieldTop = 5;
        private const double FieldBottom = 95;
        private const double PaddleHalfHeight = 9;
        private const double PaddleHalfWidth = .5;
        private const double BallHalfWidth = 1;
        private const double BallHalfHeight = 1;
        private const double PaddleMinCenter = 9.7;
        private const double PaddleMaxCenter = 90.3;

        private readonly Random _random = new();
        private double _ballVelocityX = .65;
        private double _ballVelocityY = .42;

        public string FsmName { get; } = $"PongGameFSM:{Guid.NewGuid():N}";
        public string Name { get; set; } = "PongGameContext";
        public bool IsValid { get; set; } = true;
        public double BallX { get; private set; } = 50;
        public double BallY { get; private set; } = 50;
        public double LeftPaddle { get; private set; } = 50;
        public double RightPaddle { get; private set; } = 50;
        public int LeftScore { get; private set; }
        public int RightScore { get; private set; }

        public void MoveLeftPaddle(double delta) =>
            LeftPaddle = Math.Clamp(LeftPaddle + delta, PaddleMinCenter, PaddleMaxCenter);

        public void Advance()
        {
            BallX += _ballVelocityX;
            BallY += _ballVelocityY;

            if (BallY - BallHalfHeight <= FieldTop)
            {
                BallY = FieldTop + BallHalfHeight;
                _ballVelocityY = Math.Abs(_ballVelocityY);
            }
            else if (BallY + BallHalfHeight >= FieldBottom)
            {
                BallY = FieldBottom - BallHalfHeight;
                _ballVelocityY = -Math.Abs(_ballVelocityY);
            }

            LeftPaddle += Math.Clamp((BallY - LeftPaddle) * .08, -1.6, 1.6);
            RightPaddle += Math.Clamp((BallY - RightPaddle) * .07, -1.45, 1.45);
            LeftPaddle = Math.Clamp(LeftPaddle, PaddleMinCenter, PaddleMaxCenter);
            RightPaddle = Math.Clamp(RightPaddle, PaddleMinCenter, PaddleMaxCenter);

            if (BallOverlapsPaddle(BallX, BallY, 2, LeftPaddle) && _ballVelocityX < 0)
            {
                BallX = 2 + PaddleHalfWidth + BallHalfWidth;
                _ballVelocityX = Math.Abs(_ballVelocityX) + .015;
            }
            else if (BallOverlapsPaddle(BallX, BallY, 98, RightPaddle) && _ballVelocityX > 0)
            {
                BallX = 98 - PaddleHalfWidth - BallHalfWidth;
                _ballVelocityX = -Math.Abs(_ballVelocityX) - .015;
            }
            else if (BallX < -2)
            {
                RightScore++;
                ResetBall();
            }
            else if (BallX > 102)
            {
                LeftScore++;
                ResetBall();
            }
        }

        private void ResetBall()
        {
            BallX = 50;
            BallY = 50;
            _ballVelocityX = _random.Next(0, 2) == 0 ? .65 : -.65;
            _ballVelocityY = _random.Next(0, 2) == 0 ? .42 : -.42;
        }

        private static bool BallOverlapsPaddle(double ballX, double ballY, double paddleX, double paddleY)
        {
            var ballLeft = ballX - BallHalfWidth;
            var ballRight = ballX + BallHalfWidth;
            var ballTop = ballY - BallHalfHeight;
            var ballBottom = ballY + BallHalfHeight;
            var paddleLeft = paddleX - PaddleHalfWidth;
            var paddleRight = paddleX + PaddleHalfWidth;
            var paddleTop = paddleY - PaddleHalfHeight;
            var paddleBottom = paddleY + PaddleHalfHeight;

            return ballLeft <= paddleRight
                && ballRight >= paddleLeft
                && ballTop <= paddleBottom
                && ballBottom >= paddleTop;
        }
    }
}

/// <summary>
/// Runtime catalog of installable Workshop MicroBundles.
/// This is deliberately separate from the idle catalog: any experience may be
/// loaded here regardless of why the host requested it.
/// </summary>
public static class MicroBundleCatalog
{
    private static readonly IReadOnlyList<ulong> AvailableIds = [PongMicroBundle.BundleId];

    public static IReadOnlyList<ulong> Available => AvailableIds;

    public static PongMicroBundle CreatePong() => new();

    public static bool IsAvailable(ulong id) => AvailableIds.Contains(id);
}
