namespace TheSingularityWorkshop.Workshop.Experiences;

/// <summary>Runtime state produced by the concrete Pong MicroBundles.</summary>
public sealed class PongExperienceRuntime
{
    public const double FieldTop = 5;
    public const double FieldBottom = 95;
    public const double PaddleHalfHeight = 9;
    public const double PaddleHalfWidth = .5;
    public const double BallHalfWidth = 1;
    public const double BallHalfHeight = 1;
    public const double PaddleMinCenter = 9.7;
    public const double PaddleMaxCenter = 90.3;

    private readonly Random _random;
    private bool _playerInput;

    public PongExperienceRuntime(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
        Reset();
    }

    public double BallX { get; private set; }
    public double BallY { get; private set; }
    public double BallVelocityX { get; private set; }
    public double BallVelocityY { get; private set; }
    public double PlayerPaddle { get; private set; }
    public double OpponentPaddle { get; private set; }
    public int PlayerScore { get; private set; }
    public int OpponentScore { get; private set; }
    public bool UpPressed { get; private set; }
    public bool DownPressed { get; private set; }
    public PongSoundEvent LastSound { get; private set; }

    public event Action<PongSoundEvent>? SoundTriggered;

    public void SetInput(bool up, bool down)
    {
        UpPressed = up;
        DownPressed = down;
        _playerInput = up || down;
    }

    public void MovePlayer(double delta)
    {
        PlayerPaddle = Math.Clamp(PlayerPaddle + delta, PaddleMinCenter, PaddleMaxCenter);
        _playerInput = true;
    }

    /// <summary>One Hub-owned presentation tick. No timer is created here.</summary>
    public void Step(double deltaScale = 1)
    {
        ProcessInput(deltaScale);
        ProcessBall(deltaScale);
        ProcessOpponent(deltaScale);
        LastSound = PongSoundEvent.None;
    }

    public void Reset()
    {
        BallX = 50;
        BallY = 50;
        BallVelocityX = _random.Next(0, 2) == 0 ? .65 : -.65;
        BallVelocityY = _random.Next(0, 2) == 0 ? .42 : -.42;
        PlayerPaddle = 50;
        OpponentPaddle = 50;
        UpPressed = false;
        DownPressed = false;
        _playerInput = false;
    }

    private void ProcessInput(double scale)
    {
        if (UpPressed) MovePlayer(-1.8 * scale);
        if (DownPressed) MovePlayer(1.8 * scale);
        if (!UpPressed && !DownPressed) _playerInput = false;
    }

    private void ProcessBall(double scale)
    {
        BallX += BallVelocityX * scale;
        BallY += BallVelocityY * scale;

        if (BallY - BallHalfHeight <= FieldTop)
        {
            BallY = FieldTop + BallHalfHeight;
            BallVelocityY = Math.Abs(BallVelocityY);
            Emit(PongSoundEvent.Wall);
        }
        else if (BallY + BallHalfHeight >= FieldBottom)
        {
            BallY = FieldBottom - BallHalfHeight;
            BallVelocityY = -Math.Abs(BallVelocityY);
            Emit(PongSoundEvent.Wall);
        }

        if (BallOverlapsPaddle(BallX, BallY, 2, PlayerPaddle) && BallVelocityX < 0)
        {
            BallX = 2 + PaddleHalfWidth + BallHalfWidth;
            BallVelocityX = Math.Abs(BallVelocityX) + .015;
            Emit(PongSoundEvent.Paddle);
        }
        else if (BallOverlapsPaddle(BallX, BallY, 98, OpponentPaddle) && BallVelocityX > 0)
        {
            BallX = 98 - PaddleHalfWidth - BallHalfWidth;
            BallVelocityX = -Math.Abs(BallVelocityX) - .015;
            Emit(PongSoundEvent.Paddle);
        }
        else if (BallX < -2)
        {
            OpponentScore++;
            Emit(PongSoundEvent.Score);
            ResetBall(1);
        }
        else if (BallX > 102)
        {
            PlayerScore++;
            Emit(PongSoundEvent.Score);
            ResetBall(-1);
        }
    }

    private void ProcessOpponent(double scale)
    {
        if (!_playerInput)
            PlayerPaddle += Math.Clamp((BallY - PlayerPaddle) * .08 * scale, -1.6 * scale, 1.6 * scale);

        OpponentPaddle += Math.Clamp((BallY - OpponentPaddle) * .07 * scale, -1.45 * scale, 1.45 * scale);
        PlayerPaddle = Math.Clamp(PlayerPaddle, PaddleMinCenter, PaddleMaxCenter);
        OpponentPaddle = Math.Clamp(OpponentPaddle, PaddleMinCenter, PaddleMaxCenter);
    }

    private void ResetBall(double direction)
    {
        BallX = 50;
        BallY = 50;
        BallVelocityX = .65 * direction;
        BallVelocityY = _random.Next(0, 2) == 0 ? .42 : -.42;
    }

    private void Emit(PongSoundEvent sound)
    {
        LastSound = sound;
        SoundTriggered?.Invoke(sound);
    }

    public static bool BallOverlapsPaddle(double ballX, double ballY, double paddleX, double paddleY)
    {
        var ballLeft = ballX - BallHalfWidth;
        var ballRight = ballX + BallHalfWidth;
        var ballTop = ballY - BallHalfHeight;
        var ballBottom = ballY + BallHalfHeight;
        var paddleLeft = paddleX - PaddleHalfWidth;
        var paddleRight = paddleX + PaddleHalfWidth;
        var paddleTop = paddleY - PaddleHalfHeight;
        var paddleBottom = paddleY + PaddleHalfHeight;

        return ballLeft <= paddleRight && ballRight >= paddleLeft && ballTop <= paddleBottom && ballBottom >= paddleTop;
    }
}

/// <summary>Presentation events emitted by the sound MicroBundle.</summary>
public enum PongSoundEvent
{
    None,
    Wall,
    Paddle,
    Score
}
