using SkiaSharp;

namespace Maui.FreakyUXKit.Coachmark.Animations;

public abstract class BaseCoachmarkAnimation : ICoachmarkAnimation
{
    public event Action? AnimationCompleted;

    protected bool _isRunning = false;

    public virtual void Start(CoachmarkAnimationPhase phase)
    {
        _isRunning = true;
    }

    public virtual void Stop()
    {
        _isRunning = false;
    }

    public abstract void UpdateFrame(float deltaTime);

    public abstract void Render(SKCanvas canvas, SKRect bounds);

    public virtual void Dispose()
    {
        Stop();
        AnimationCompleted = null;
        GC.SuppressFinalize(this);
    }

    protected void OnAnimationCompleted()
    {
        AnimationCompleted?.Invoke();
    }
}